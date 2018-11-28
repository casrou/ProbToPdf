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
        public int PageNumber { get; set; }

        internal static List<Bookmark> GetBookmarksFromBook(Book book)
        {
            int pageNumber = 0;
            List<Bookmark> bookmarks = new List<Bookmark>();

            foreach (Chapter chapter in book.Chapters)
            {
                int level = 1;

                if(chapter.Title != null)
                {
                    /*
                     * Chapter has level 1 bookmark
                     * Ex.
                     * Bla bla      <--
                     *  - bla bla
                     *  - bla bla
                    */
                    Bookmark bookmark = new Bookmark();
                    bookmark.Title = chapter.Title;
                    bookmark.Level = level;
                    bookmark.PageNumber = pageNumber;
                    bookmarks.Add(bookmark);
                    level = 2;
                } 

                foreach (Page page in chapter.Pages)
                {
                    Bookmark b = new Bookmark();
                    b.Level = level;
                    b.Title = page.Url;                    
                    b.PageNumber = pageNumber;
                    pageNumber += GetNumberOfPages(page);
                    bookmarks.Add(b);
                }

                //Log.Information("Generating bookmark: " + command);
                
            }
            return bookmarks;
        }

        private static int GetNumberOfPages(Page page)
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\book";

            // Get filepath of page            
            string filepath = path + "\\" + page.Url.Substring(page.Url.LastIndexOf('/')+1).Replace(".php", ".pdf");
            string data = DumpData(filepath);
            return 1;
        }

        private static string DumpData(string pdf)
        {
            using (var ps = PowerShell.Create())
            {
                try
                {
                    var results = ps.AddScript($"pdftk {pdf} dump_data").Invoke();
                    foreach (var result in results)
                    {
                        Log.Debug(result.ToString());
                        return result.ToString();
                    }
                }
                catch (Exception e)
                {
                    Log.Error("An error occured while dumping data for: " + pdf + "\n" +
                        "Error: " + e.Message);
                }
            }
            return "";
        }       
    }
}