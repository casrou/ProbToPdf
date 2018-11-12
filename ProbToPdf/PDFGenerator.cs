using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Threading.Tasks;

namespace ProbToPdf
{
    class PDFGenerator
    {
        private string _path;

        public PDFGenerator(Book book)
        {
            _path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\" + book;
        }

        public void Generate(Book book)
        {            
            // Get filename of all pages
            List<string> files = book.GetPages().Select(p => _path + "\\" + p.Url.Split('/').Last().Replace(".php", ".html")).ToList();

            // Generate pdfs
            //files.ForEach(f => Execute($"relaxed \"{f}\" --bo"));
            var result = Parallel.ForEach(files, f => Execute($"relaxed \"{f}\" --bo"));

            // Remove temporary files
            RemoveFiles(files.Select(f => f.Replace(".html", "_temp.htm")));    // *_temp.htm
            RemoveFiles(files);                                                 // *.html

            // Merge all pdfs
            Execute($"pdftk {String.Join(' ', files.Select(f => f.Replace(".html", ".pdf")))} cat output {_path}\\output.pdf");
        }
        
        private void Execute(string command)
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

        private void RemoveFiles(IEnumerable<string> filePaths)
        {
            foreach (var file in filePaths)
            {
                File.Delete(file);
            }
        }
    }
}