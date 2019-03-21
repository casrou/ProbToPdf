using Serilog;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Runtime.InteropServices;
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
            String path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), _book.ToString());
            
            // Get filename of all pages
            List<string> files = _book.Pages
                .Select(p => Path.Combine(path, p.Url.Split('/').Last().Replace(".php", ".html")))
                .ToList();

            
            bool isWindows = System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
            if(isWindows){
                // Generate pdfs
                files.ForEach(f => Execute($"relaxed \"{f}\" --bo")); // Windows
                // Merge all pdfs
                Execute($"pdftk {String.Join(' ', files.Select(f => f.Replace(".html", ".pdf")))} cat output {Path.Combine(path, "output.pdf")}"); // Windows
            } else {
            // Generate pdfs
                files.ForEach(f => $"relaxed \"{f}\" --bo".Bash()); // Linux
                // Merge all pdfs
                $"pdftk {String.Join(' ', files.Select(f => f.Replace(".html", ".pdf")))} cat output {Path.Combine(path, "output.pdf")}".Bash(); // Linux
            }

            //ConcurrentBag<String> concurrentBag = new ConcurrentBag<string>(files); // thread-safe
            //var result = Parallel.ForEach(concurrentBag, f => Execute($"relaxed \"{f}\" --bo")); 
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