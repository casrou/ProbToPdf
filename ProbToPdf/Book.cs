using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serilog;

namespace ProbToPdf
{
    class Book
    {
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

        public Book(string filePath)
        {
            string json = File.ReadAllText(filePath);
            Log.Information("Getting book template from json: " + filePath);
            JsonConvert.PopulateObject(json, this);
            Process();
        }

        private void Process()
        {
            Chapters.ForEach(c => c.Pages.ForEach(p => p.Process()));            
        }

        private List<Page> GetPages()
        {
            List<Page> pages = new List<Page>();
            Chapters.ForEach(c => pages.AddRange(c.Pages));
            return pages;
        }

        public override string ToString()
        {
            //return $"{Title} - {Author}";
            return "book";
        }
    }
}
