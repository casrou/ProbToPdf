using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serilog;

namespace ProbToPdf
{
    class Book
    {
        private ServiceProvider _services;

        public string Title { get; set; }
        public string Author { get; set; }
        public List<Chapter> Chapters { get; set; }
        public List<Page> Pages
        {
            get
            {
                return GetPages();
            }
        }
        //public List<Page> Other { get; set; }

        public Book(string filePath, ServiceProvider services)
        {
            _services = services;
            string json = File.ReadAllText(filePath);
            Log.Information("Getting book template from json: " + filePath);
            JsonConvert.PopulateObject(json, this);
        }

        public void Process()
        {
            //GetPages().ForEach(p => p.Process());
            Parallel.ForEach(GetPages().Where(page => Path.GetExtension(page.Url) != ".pdf"), p => p.Process(_services.GetService<HtmlWeb>()));            
        }

        public override string ToString()
        {
            //return $"{Title} - {Author}";
            return "book";
        }

        public List<Page> GetPages()
        {
            List<Page> pages = new List<Page>();
            Chapters.ForEach(c => pages.AddRange(c.Pages));
            return pages;
        }
    }
}
