using Fizzler.Systems.HtmlAgilityPack;
using HtmlAgilityPack;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ProbToPdf
{
    class Program
    {
        //private static Logger _logger;
        //private static readonly HttpClient client = new HttpClient();
        //private static readonly Uri main = new Uri("https://www.probabilitycourse.com/");

        static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .CreateLogger();

            //LogLevel logLevel;
            //switch (char.ToLower(Console.ReadKey().KeyChar))
            //{
            //    case 'n':
            //        logLevel = LogLevel.NONE;
            //        break;
            //    case 'l':
            //        logLevel = LogLevel.LOW;
            //        break;
            //    case 'h':
            //        logLevel = LogLevel.HIGH;
            //        break;
            //    default:
            //        logLevel = LogLevel.NONE;
            //        break;
            //}
            //_logger = new Logger(logLevel);

            Log.Information("BEGIN");
            Book book = new Book("book.json");

            //Book book = new Book
            //{
            //    OtherPages = 
            //    Preface = GetPage("preface.php"),
            //    Chapters = GetChapters(),
            //    Bibliography = GetPage("bibliography.php")
            //};

            Downloader.Download(book);

            PDFGenerator.Generate(book);

            //List<string> filePaths = Download(book);
            //GeneratePDFs(filePaths);

            //RemoveFiles(filePaths);

            //watch.Stop();
            //Console.WriteLine("in " + watch.ElapsedMilliseconds/1000 + " seconds");
            Console.ReadLine();
        }
    }
}

//        private static void RemoveFiles(List<string> filePaths)
//        {
//            foreach (var file in filePaths)
//            {
//                //File.Delete(file);
//                File.Delete(file.Replace(".html", "_temp.htm"));
//            }
//        }

//        private static Page GetPage(string url)
//        {
//            Page page = new Page
//            {
//                Url = url
//            };

//            HtmlNode htmlNode = GetHtmlNode(url);
//            HtmlNode node = htmlNode.QuerySelector("#content");

//            if(node == null)
//            {
//                Console.WriteLine("NULL: " + url);
//            };

//            CleanUp(node);

//            page.Content = AddStyling(FixInlineMath(node.InnerHtml)).Trim();

//            if (debug >= Debug.LOW) Console.WriteLine($"Page generated: {page.Url}");
//            return page;
//        }

//        private static void CleanUp(HtmlNode node)
//        {
            
//            List<HtmlNode> shouldBeDeleted = new List<HtmlNode>();
//            shouldBeDeleted.AddRange(node.SelectNodes("//div[contains(@class,'thinblock')]")); // arrows
//            //var test = node.QuerySelectorAll(".hide_print > a > span"); // WIP remove "Video Available"
//            //if (test.Count() != 0)
//            //{
//            //    Console.WriteLine(test.Count());
//            //}
//            shouldBeDeleted.AddRange(node.QuerySelectorAll(".hide_print > a > span"));
//            shouldBeDeleted.AddRange(node.SelectNodes("//script")); // <script> tags
//            shouldBeDeleted.AddRange(node.SelectNodes("//comment()")); // comments
            
//            foreach (HtmlNode deletable in shouldBeDeleted)
//            {
//                // WIP select only deletable nodes from childnodes of node
//                //if (node.ChildNodes.FirstOrDefault(cn => cn == delete) != null)
//                //{
//                //    node.ChildNodes.Remove(delete);
//                //}
//                deletable.Remove();
//            }

//            var hrefs = node.QuerySelectorAll("[href]").ToList();

//            foreach (var href in hrefs)
//            {
//                string newHref = href.Attributes["href"].Value;

//                if (newHref.StartsWith("//"))
//                {
//                    newHref = newHref.Substring(2);
//                    href.Attributes["href"].Value = newHref;
//                }

//                if (newHref.StartsWith("www"))
//                {
//                    newHref = "https://" + newHref;
//                    href.Attributes["href"].Value = newHref;
//                }

//                if (!newHref.StartsWith("http"))
//                {
//                    newHref = newHref.Split('/').Last().Replace(".php", ".html");
//                    //newHref = "https://www.probabilitycourse.com/" + newHref;
//                    href.Attributes["href"].Value = newHref;                                    
//                }
//            }

//            var srcs = node.QuerySelectorAll("[src]").ToList();

//            foreach (var src in srcs)
//            {
//                string oldSrc = src.Attributes["src"].Value;
//                if (!oldSrc.StartsWith("http") || !oldSrc.StartsWith("www"))
//                {
//                    src.Attributes["src"].Value = "https://www.probabilitycourse.com/" + oldSrc;
//                }

//                if(oldSrc == "//icons.iconarchive.com/icons/bokehlicia/captiva/32/web-google-youtube-icon.png")
//                {
//                    src.Attributes["src"].Value = "https://cdn1.iconfinder.com/data/icons/google_jfk_icons_by_carlosjj/512/youtube.png";
//                    string style = src.Attributes["style"].Value;
//                    if (style != null)
//                    {
//                        src.Attributes["style"].Value = style + "height: 50px;";
//                    }
//                }
//            }
//        }

//        private static string FixInlineMath(string content)
//        {
//            int counter = 0;
//            StringBuilder sb = new StringBuilder();
//            // Change '$ *equation* $' to '\\( *equation* \\)'
//            for (int i = 0; i < content.Length; i++)
//            {
//                if (!content[i].Equals('$'))
//                {
//                    sb.Append(content[i]);
//                }
//                else
//                {
//                    if (content[i + 1] == '$')
//                    {
//                        counter += 2;
//                    }
//                    else
//                    {
//                        if (counter % 2 == 0)
//                        {
//                            sb.Append("\\(");
//                        }
//                        else
//                        {
//                            sb.Append("\\)");
//                        }
//                        counter++;
//                    }
//                }                
//            }
//            return sb.ToString();
//        }

//        private static string AddStyling(string content)
//        {
//            string styling = "<link href=\"https://www.probabilitycourse.com/style_sheet.css\" type=\"text/css\" rel=\"stylesheet\" />\n";
//            styling += "<style>\n";
//            styling += File.ReadAllText("style.css");
//            styling += "\n</style>";

//            return content + styling;
//        }

//        private static List<string> Download(Book book)
//        {
//            /*
//            *  book/
//            *      preface.php
//            *      chapter1/
//            *          1_0_0_introduction.php
//            *          1_1_0_what_is_probability.php
//            *          ...
//            *          transmission_color.png
//            *          ...
//            *      chapter2/
//            *          2_1_0_counting.php
//            *          2_1_1_ordered_with_replacement.php
//            *          ...
//            *      ...
//            *      chapter14/
//            *          Chapter_14.pdf
//            *          chapter14.php
//            *      appendix/
//            *          review_fourier_transform.php
//            *          some_important_distributions.php
//            */

//            List<string> filePaths = new List<string>();
//            string filePath;
//            string folderPath;

//            string downloadDir = @"c:\dev\ProbToPdf\book";
//            Directory.CreateDirectory(downloadDir);
//            File.WriteAllText($"{downloadDir}\\config.yml", "plugins:\n- mathjax");

//            // Preface
//            filePath = $"{downloadDir}\\{book.Preface.Url}";
//            File.WriteAllTextAsync(filePath, book.Preface.Content);
//            filePaths.Add(filePath);

//            // Chapters
//            foreach (Chapter chapter in book.Chapters)
//            {
//                folderPath = $"{downloadDir}\\{chapter.Url}";
//                Directory.CreateDirectory(folderPath);
//                File.WriteAllText($"{folderPath}\\config.yml", "plugins:\n- mathjax");
//                foreach (Page page in chapter.Pages)
//                {
//                    filePath = $"{downloadDir}\\{page.Url.Replace('/', '\\')}";
//                    File.WriteAllTextAsync(filePath, page.Content);
//                    filePaths.Add(filePath);
//                }
//            }

//            // Bibliography
//            filePath = $"{downloadDir}\\{book.Bibliography.Url}";
//            File.WriteAllTextAsync(filePath, book.Bibliography.Content);
//            filePaths.Add(filePath);

//            return filePaths;
//        }

//        private static List<Chapter> GetChapters()
//        {
//            List<Chapter> chapters = new List<Chapter>
//            {
//                new Chapter { Url = "chapter1"},
//                //new Chapter { urlPath = "chapter2"},
//                //new Chapter { urlPath = "chapter3"},
//                //new Chapter { urlPath = "chapter4"},
//                //new Chapter { urlPath = "chapter5"},
//                //new Chapter { urlPath = "chapter6"},
//                //new Chapter { urlPath = "chapter7"},
//                //new Chapter { urlPath = "chapter8"},
//                //new Chapter { urlPath = "chapter9"},
//                //new Chapter { urlPath = "chapter10"},
//                //new Chapter { urlPath = "chapter11"},
//                ////new Chapter { urlPath = "chapter12"},
//                ////new Chapter { urlPath = "chapter13"},
//                ////new Chapter { urlPath = "chapter14"},
//                new Chapter { Url = "appendix"}
//            };

//            List<string> urls = new List<string>
//            {
//                "preface.php"
//            };

//            //var result = Parallel.ForEach(chapters, chapter => ProcessChapter(chapter));

//            foreach (var chapter in chapters)
//            {
//                HtmlNode htmlNode = GetHtmlNode(chapter.Url);
//                //  Fizzler.Systems.HtmlAgilityPack does not support ":not"
//                List<HtmlNode> nodes = htmlNode.QuerySelectorAll("a").Except(htmlNode.QuerySelectorAll("li:first-child > a")).ToList();
//                nodes.Remove(htmlNode.FirstChild);
//                nodes = nodes.Except(htmlNode.QuerySelectorAll("li:first-child > a")).ToList();
//                nodes = nodes
//                    .Where(n => !n.InnerHtml.Contains("archive") &&
//                                !n.InnerHtml.Contains("section")).ToList();
//                foreach (HtmlNode node in nodes)
//                {
//                    string url = node.Attributes["href"].Value.Trim();
//                    Page page = GetPage($"{chapter.Url}/{url}");
//                    chapter.Pages.Add(page);
//                }
//            }

//            return chapters;
//        }

//        private static void ProcessChapter(Chapter chapter)
//        {
//            HtmlNode htmlNode = GetHtmlNode(chapter.Url);
//            //  Fizzler.Systems.HtmlAgilityPack does not support ":not"
//            List<HtmlNode> nodes = htmlNode.QuerySelectorAll("a").ToList();
//            nodes.Remove(htmlNode.FirstChild);
//            nodes = nodes.Except(htmlNode.QuerySelectorAll("li:first-child > a")).ToList();
//            nodes = nodes
//                .Where(n => !n.InnerHtml.Contains("archive") &&
//                            !n.InnerHtml.Contains("section")).ToList();
//            foreach (HtmlNode node in nodes)
//            {
//                string url = node.Attributes["href"].Value.Trim();
//                Page page = GetPage($"{chapter.Url}/{url}");
//                chapter.Pages.Add(page);
//            }
//        }

//        private static HtmlNode GetHtmlNode(string urlPath)
//        {
//            Uri relative = new Uri(main, urlPath);
//            var web = new HtmlWeb();
//            var htmlDoc = web.Load(relative.AbsoluteUri);            

//            while(htmlDoc.DocumentNode.InnerLength < 100)
//            {
//                Console.WriteLine("HTML node to short");
//                Thread.Sleep(5000);
//                htmlDoc = web.Load(relative.AbsoluteUri);
//            }

//            HtmlNode htmlNode = htmlDoc.DocumentNode;

//            return htmlNode;
//        }

//        private static void GeneratePDFs(List<string> filePaths)
//        {
//            //var result = Parallel.ForEach(filePaths, fp => Execute($"relaxed '{fp}' --bo"));
//            //if (debug >= Debug.LOW && result.IsCompleted)
//            //{
//            //    Console.WriteLine("ALL PDFs GENERATED SUCCESSFULLY");
//            //}
//            foreach (var fp in filePaths)
//            {
//                Execute($"relaxed '{fp}' --bo");
//            }
//            Console.WriteLine("ALL PDFs GENERATED SUCCESSFULLY");
//        }

//        private static void Execute(string command)
//        {
//            if (debug >= Debug.LOW) Console.WriteLine("Executing: " + command);
//            using (var ps = PowerShell.Create())
//            {
//                var results = ps.AddScript(command.Replace("'", "''")).Invoke();
//                foreach (var result in results)
//                {
//                    if (debug >= Debug.HIGH) Console.WriteLine(result.ToString());
//                }
//            }
//        }
//    }
//}
