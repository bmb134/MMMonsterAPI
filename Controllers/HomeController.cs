using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ImageHost.Models;
using Imgur.API;
using Imgur.API.Authentication.Impl;
using Imgur.API.Endpoints;
using Imgur.API.Endpoints.Impl;
using Imgur.API.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ImageHost.Controllers
{
    [Route("")]
    public class HomeController : Controller
    {
        private readonly IHostingEnvironment _appEnvironment;

        public Random StreamReader { get; private set; }

        public HomeController(IHostingEnvironment appEnvironment)
        {
            _appEnvironment = appEnvironment;
        }

        [HttpGet("")]
        public IActionResult Upload()
        {
            return View();
        }

        [HttpPost("")]
        public async Task<IActionResult> Upload(List<IFormFile> files)
        {
            long size = files.Sum(f => f.Length);

            // full path to file in temp location
            var filePath = Path.GetTempFileName();

            List<Image> newImages = new List<Image>();

            foreach (var imageFile in files)
            {
                if (imageFile != null && imageFile.Length > 0)
                {
                    try
                    {
                        var file = imageFile;
                        var client = new ImgurClient("aa6b9c5aa286ac1", "1e585413d574918e61c1a2577962c7e67b70cf0a");
                        var endpoint = new ImageEndpoint(client);

                        IImage image;

                        // string uploads = Path.Combine(_appEnvironment.WebRootPath, "uploads\\img");
                        // string fileName = Guid.NewGuid().ToString().Replace("-", "") + Path.GetExtension(file.FileName);
                        var fs = file.OpenReadStream();
                        using (fs)
                        {
                            image = await endpoint.UploadImageStreamAsync(fs);
                            Console.WriteLine(image);
                        }

                        newImages.Add(new Image(file.FileName, image.Link));

                        Debug.Write("Image uploaded. Image Url: " + image.Link);
                    }
                    catch (ImgurException imgurEx)
                    {
                        Debug.Write("An error occurred uploading an image to Imgur.");
                        Debug.Write(imgurEx.Message);
                    }
                }
            }

            UpateJsonStorage(newImages);

            return new JsonResult(newImages);
        }

        [Route("{id:int}")]
        public IActionResult GetImageUrl(int id)
        {
            List<Image> currentImages = new List<Image>();

            using (StreamReader r = new StreamReader("images.json"))
            {
                string json = r.ReadToEnd();
                currentImages = JsonConvert.DeserializeObject<List<Image>>(json);
            }

            Image targetImage = currentImages.Where(x => x.Id == id).FirstOrDefault();

            return new JsonResult(targetImage);
        }

        [Route("GetAll")]
        public IActionResult GetAllImages()
        {
            string uploadedFilesFolder = Path.Combine(_appEnvironment.WebRootPath, "uploads\\img");
            var existingFiles = Directory.GetFiles(uploadedFilesFolder);

            return new JsonResult(existingFiles);
        }

        private void UpateJsonStorage(List<Image> newImages)
        {
            List<Image> currentImages = new List<Image>();
            using (StreamReader r = new StreamReader("images.json"))
            {
                string json = r.ReadToEnd();
                currentImages = JsonConvert.DeserializeObject<List<Image>>(json);
            }

            int currentImageCount = currentImages.Count;

            foreach (Image i in newImages)
            {
                i.Id = currentImageCount + 1;
                currentImages.Add(i);
            }

            using (StreamWriter w = new StreamWriter("images.json", false))
            {
                w.WriteLine(JsonConvert.SerializeObject(currentImages));
            }
        }
    }
}