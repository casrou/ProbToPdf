using HtmlAgilityPack;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace ProbToPdf
{
    class Page
    {
        public string Url { get; set; }        
        public string Content { get; set; }
        public Order Order { get; set; } = Order.DEFAULT;

        public void Process()
        {
            Content = GetPage(Url);
        }

        private string GetPage(string url)
        {
            Log.Information("Parsing: " + url);
            HtmlDocument html = DownloadHtml(url);

            string result = ParseHtml(html).InnerHtml;

            result = FixInlineMath(result);
            result = AddStyling(result);

            //return AddStyling(FixInlineMath(html.OuterHtml)).Trim();
            return result;
        }

        private static HtmlDocument DownloadHtml(string url)
        {
            var web = new HtmlWeb();
            var html = web.Load(url);

            while (web.StatusCode != System.Net.HttpStatusCode.OK || html.DocumentNode.InnerLength < 100)
            {
                Log.Warning("Url: " + url + ", Statuscode: " + web.StatusCode);
                Thread.Sleep(1000);
                html = web.Load(url);
            }

            return html;
        }

        private HtmlNode ParseHtml(HtmlDocument html)
        {
            // TODO: Make parsing dynamic, fx. from a json file
            HtmlNode node = html.DocumentNode.SelectSingleNode("//*[@id=\"content\"]");

            // Remove unwanted html elements
            List<string> xpaths = new List<string>
            {
                "//div[contains(@class,'thinblock')]",          // next- and previuos page arrows
                "//div[contains(@class,'hide_print')]/a/span",  // "Video available"
                "//script",                                     // <script> tags
                "//comment()"                                   // comments
            };
            xpaths.ForEach(x => node.SelectNodes(x)?.ToList().ForEach(n => n.Remove()));
            
            // Fix links
            FixHrefs(node);
            FixSrcs(node);

            return node;
        }

        private static void FixHrefs(HtmlNode node)
        {
            var hrefs = node.SelectNodes("//*[@href]");
            if (hrefs == null || hrefs.Count < 1) return;

            foreach (var href in hrefs)
            {
                string newHref = href.Attributes["href"].Value;
                newHref = newHref.StartsWith("//") ? newHref.Substring(2) : newHref;
                newHref = newHref.StartsWith("www") ? "https://" + newHref : newHref;
                href.Attributes["href"].Value = newHref;
            }
        }

        private static void FixSrcs(HtmlNode node)
        {
            var srcs = node.SelectNodes("//*[@src]");
            if (srcs == null || srcs.Count < 1) return;

            foreach (var src in srcs)
            {
                string newSrc = src.Attributes["src"].Value;
                if (newSrc == "//icons.iconarchive.com/icons/bokehlicia/captiva/32/web-google-youtube-icon.png")
                {
                    newSrc = "https://cdn1.iconfinder.com/data/icons/google_jfk_icons_by_carlosjj/512/youtube.png";
                    string style = src.Attributes["style"].Value;
                    src.Attributes["style"].Value = style + "height: 50px;";
                }
                newSrc = (!newSrc.StartsWith("http") && !newSrc.StartsWith("www")) ? "https://www.probabilitycourse.com/" + newSrc : newSrc;
                src.Attributes["src"].Value = newSrc;
            }
        }        

        // Change '$ *equation* $' to '\\( *equation* \\)'
        private static string FixInlineMath(string content)
        {
            int counter = 0;
            StringBuilder sb = new StringBuilder();            
            for (int i = 0; i < content.Length; i++)
            {
                if (!content[i].Equals('$'))
                {
                    sb.Append(content[i]);
                }
                else
                {
                    if (content[i + 1] == '$')
                    {
                        counter += 2;
                    }
                    else
                    {
                        if (counter % 2 == 0)
                        {
                            sb.Append("\\(");
                        }
                        else
                        {
                            sb.Append("\\)");
                        }
                        counter++;
                    }
                }
            }
            return sb.ToString();
        }

        private static string AddStyling(string content)
        {
            string styling = "<link href=\"https://www.probabilitycourse.com/style_sheet.css\" type=\"text/css\" rel=\"stylesheet\" />\n";
            styling += "<style>\n";
            styling += File.ReadAllText("style.css");
            styling += "\n</style>";

            return content + styling;
        }
    }

    enum Order
    {
        DEFAULT, FIRST, LAST
    }
}
