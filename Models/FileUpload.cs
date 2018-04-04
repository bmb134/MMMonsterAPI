using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace ImageHost.Models
{
    public class FileUpload
    {
        [Required]
        [Display(Name = "Picture")]
        public IFormFile Picture { get; set; }
    }
}