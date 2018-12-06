using Serilog;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Threading.Tasks;

namespace ProbToPdf
{
    class PDFGenerator
    {
        private Book _book;

        public PDFGenerator(Book book)
        {
            _book = book;
        }

        internal void Generate()
        {
            String path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\" + _book;
            
            // Get filename of all pages
            List<string> files = _book.Pages
                .Select(p => path + "\\" + p.Url.Split('/').Last().Replace(".php", ".html"))
                .ToList();

            // Generate pdfs
            files.ForEach(f => Execute($"relaxed \"{f}\" --bo"));
            //ConcurrentBag<String> concurrentBag = new ConcurrentBag<string>(files); // thread-safe
            //var result = Parallel.ForEach(concurrentBag, f => Execute($"relaxed \"{f}\" --bo")); 

            // Merge all pdfs
            Execute($"pdftk {String.Join(' ', files.Select(f => f.Replace(".html", ".pdf")))} cat output {path}\\output.pdf");
        }

        public static void Execute(string command)
        {
            Log.Information("Executing: " + command);
            using (var ps = PowerShell.Create())
            {
                try
                {
                    var results = ps.AddScript(command).Invoke();
                    foreach (var result in results)
                    {
                        Log.Debug(result.ToString());
                    }
                }
                catch (Exception e)
                {
                    Log.Error("An error occured while executing command: " + command + "\n" +
                        "Error: " + e.Message);
                }
            }
        }

        private static void RemoveFiles(IEnumerable<string> filePaths)
        {
            foreach (var file in filePaths)
            {
                File.Delete(file);
            }
        }
    }
}