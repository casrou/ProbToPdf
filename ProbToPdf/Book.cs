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
        public List<Page> Other { get; set; }

        public Book(string filePath)
        {
            string json = File.ReadAllText(filePath);
            Log.Information("Getting book template from json: " + filePath);
            JsonConvert.PopulateObject(json, this);
            Process();
        }

        private void Process()
        {
            Other.ForEach(o => o.Process());
            Chapters.ForEach(c => c.Pages.ForEach(p => p.Process()));            
        }

        public override string ToString()
        {
            //return $"{Title} - {Author}";
            return "book";
        }
    }
}
