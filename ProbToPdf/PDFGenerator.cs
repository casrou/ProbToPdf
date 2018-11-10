using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;

namespace ProbToPdf
{
    class PDFGenerator
    {
        internal static void Generate(Book book)
        {
            String path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\" + book;

            List<Page> pages = new List<Page>();
            book.Chapters.ForEach(c => pages.AddRange(c.Pages));
            pages.AddRange(book.Other);

            List<string> files = pages.Select(p => path + "\\" + p.Url.Split('/').Last().Replace(".php", ".html")).ToList();

            files.ForEach(f => Execute($"relaxed '{f}' --bo"));

            Execute($"pdfunite {String.Join(" ", files.Select(f => f.Replace(".html", ".pdf")))} output.pdf");
        }

        private static void Execute(string command)
        {
            Log.Information("Executing: " + command);
            using (var ps = PowerShell.Create())
            {
                var results = ps.AddScript(command.Replace("'", "''")).Invoke();
                foreach (var result in results)
                {
                    Log.Information(result.ToString());
                }
            }
        }
    }
}