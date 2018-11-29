using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Text;

namespace ProbToPdf
{
    class BookmarkGenerator
    {
        private Book _book;
        private static string _path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\book";

        public BookmarkGenerator(Book book)
        {
            _book = book;
        }

        internal void Generate()
        {
            List<Bookmark> bookmarks = GetBookmarksFromBook(_book);
            // Generate a bookmarks file that can be imported to the merged pdf with pdftk 'update-info'
            GenerateBookmarksFile(bookmarks);

            // Import the bookmarks file to the merged pdf
            ImportBookmarksFile();            
        }

        private void ImportBookmarksFile()
        {
            Log.Information("Importing bookmarks");
            string pdf = _path + "\\output.pdf";
            string bookmarks = _path + "\\bookmarks.txt";
            string final = _path + "\\final.pdf";
            using (var ps = PowerShell.Create())
            {
                try
                {
                    var results = ps.AddScript($"pdftk {pdf} update_info {bookmarks} output {final}").Invoke();                    
                    foreach (var result in results)
                    {
                        Log.Debug(result.ToString());
                    }
                }
                catch (Exception e)
                {
                    Log.Error("An error occured while dumping data for: " + pdf + "\n" +
                        "Error: " + e.Message);
                }
            }
        }

        private void GenerateBookmarksFile(List<Bookmark> bookmarks)
        {
            Log.Information("Generating bookmarks");
            string filepath = _path + "\\bookmarks.txt";
            string result = "";
            foreach (Bookmark bookmark in bookmarks)
            {
                result += "BookmarkBegin\n";
                result += $"BookmarkTitle: {bookmark.Title}\n";
                result += $"BookmarkLevel: {bookmark.Level}\n";
                result += $"BookmarkPageNumber: {bookmark.PageNumber}\n";
            }
            File.WriteAllText(filepath, result);
        }

        internal static List<Bookmark> GetBookmarksFromBook(Book book)
        {
            int pageNumber = 1;
            List<Bookmark> bookmarks = new List<Bookmark>();

            foreach (Chapter chapter in book.Chapters)
            {
                int level = 1;

                if (chapter.Title != null)
                {
                    /*
                     * Chapter has a title/is a level 1 bookmark
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
                    Log.Information("Generating bookmark: " + page.Url);
                    Bookmark b = new Bookmark();
                    b.Level = level;
                    b.Title = page.Title;
                    b.PageNumber = pageNumber;
                    pageNumber += GetNumberOfPages(page);
                    bookmarks.Add(b);
                }
            }
            return bookmarks;
        }        

        private static int GetNumberOfPages(Page page)
        {           
            // Get filepath of page            
            string filepath = _path + "\\" + page.Url.Substring(page.Url.LastIndexOf('/') + 1).Replace(".php", ".pdf");
            IEnumerable<PSObject> data = DumpData(filepath);
            return int.Parse(data.First(pso => pso.ToString().Contains("NumberOfPages")).ToString().Split(':').Last());
        }

        private static IEnumerable<PSObject> DumpData(string pdf)
        {
            Dictionary<string, string> dumpedData = new Dictionary<string, string>();
            using (var ps = PowerShell.Create())
            {
                try
                {
                    return ps.AddScript($"pdftk {pdf} dump_data").Invoke();
                }
                catch (Exception e)
                {
                    Log.Error("An error occured while dumping data for: " + pdf + "\n" +
                        "Error: " + e.Message);
                }
            }
            return null;
        }
    }
}
