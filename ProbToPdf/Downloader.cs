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
        private Book _book;
        private string _path;

        public Downloader(Book book)
        {
            _book = book;
            _path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\" + _book;

            Log.Information("Creating book folder at: " + _path);
            Directory.CreateDirectory(_path);
            File.WriteAllText(_path + "\\config.yml", "plugins:\n- mathjax");
        }

        internal void Download(Book book)
        {
            //book.Other
            //    .ForEach(p => WritePage(p, path));

            book.GetPages()
                .ForEach(p => WritePage(p, _path));
        }

        internal void Download(Page page)
        {
            WritePage(page, _path);
        }

        private static void WritePage(Page p, string path)
        {
            string page = p.Url.Split('/').Last();
            Log.Information("Writing page to disk: " + p.Url);
            if (Path.GetExtension(p.Url) == ".pdf")
            {
                using (WebClient client = new WebClient())
                {
                    client.DownloadFile(p.Url, path + "\\" + page);
                }
            } else
            {
                page = page.Replace(".php", ".html");
                File.WriteAllTextAsync(path + "\\" + page, p.Content);
            }            
        }
    }
}