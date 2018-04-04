using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImageHost.Models
{
    public class Image
    {
        public Image(string name, string uri)
        {
            Name = name;
            Uri = new Uri(uri);
        }

        public int? Id { get; set; }
        public string Name { get; set; }
        public Uri Uri { get; set; }
    }
}