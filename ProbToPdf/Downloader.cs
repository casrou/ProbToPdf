using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace ProbToPdf
{
    class Downloader
    {
        private string _path;
        private Book _book;

        public Downloader(Book book)
        {
            _path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "book");
            _book = book;

            Log.Information("Creating book folder at: " + _path);
            Directory.CreateDirectory(_path);
            File.WriteAllText(Path.Combine(_path, "config.yml"), "plugins:\n- mathjax");
        }

        internal void Download()
        {
            _book.Pages
                .ForEach(p => WritePage(p));
        }

        private void WritePage(Page p)
        {
            string page = p.Url.Split('/').Last();
            string path = Path.Combine(_path, page);
            Log.Information("Writing page to disk: " + path);
            if (Path.GetExtension(p.Url) == ".pdf")
            {
                using (WebClient client = new WebClient())
                {
                    client.DownloadFile(p.Url, path);
                }
            } else
            {
                path = path.Replace(".php", ".html");
                File.WriteAllTextAsync(path, p.Content);
            }            
        }
    }
}