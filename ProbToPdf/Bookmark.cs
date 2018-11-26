using Serilog;
using System;
using System.Collections.Generic;
using System.Management.Automation;

namespace ProbToPdf
{
    internal class Bookmark
    {
        public string Title { get; set; }
        public int Level { get; set; }
        public int NumberOfPages { get; set; }

        static void Main(string[] args)
        {
            Console.WriteLine("test");
        }

        internal static List<Bookmark> GetBookmarksFromBook(Book book)
        {
            List<Bookmark> bookmarks = new List<Bookmark>();

            foreach (Chapter chapter in book.Chapters)
            {
                String command = "";
                bookmarks.Add(new Bookmark() { Title = chapter.});
                Log.Information("Generating bookmark: " + command);
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

            

            throw new NotImplementedException();
        }
       
    }
}