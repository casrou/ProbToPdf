using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ProbToPdf
{
    class Downloader
    {
        internal static void Download(Book book)
        {
            String path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\" + book;
            
            Directory.CreateDirectory(path);
            File.WriteAllText(path + "\\config.yml", "plugins:\n- mathjax");

            book.Other
                .Where(p => p.Order < Order.LAST).ToList()
                .ForEach(p => WritePage(p, path));

            book.Chapters
                .ForEach(c => c.Pages
                .ForEach(p => WritePage(p, path)));

            book.Other
                .Where(p => p.Order == Order.LAST).ToList()
                .ForEach(p => WritePage(p, path));
        }

        private static void WritePage(Page p, string path)
        {
            string page = p.Url.Split('/').Last().Replace(".php", ".html");
            Log.Information("Writing to disk: " + page);
            File.WriteAllTextAsync(path + "\\" + page, p.Content);
        }
    }
}