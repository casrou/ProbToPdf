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
        internal static void Download(Book book)
        {
            String path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\" + book;

            Log.Information("Creating book folder at: " + path);
            Directory.CreateDirectory(path);
            File.WriteAllText(path + "\\config.yml", "plugins:\n- mathjax");

            book.Other
                .ForEach(p => WritePage(p, path));

            book.Chapters
                .ForEach(c => c.Pages
                .ForEach(p => WritePage(p, path)));
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