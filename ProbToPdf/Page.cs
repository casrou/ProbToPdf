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

        public void Process()
        {
            if (Path.GetExtension(Url) == ".pdf")
            {
                return;
            };            

            Content = GetPage(Url);

            if (Url.Contains("'"))
            {
                Url = Url.Replace("'", string.Empty);
            }
        }

        private string GetPage(string url)
        {
            Log.Information("Parsing: " + url);
            HtmlDocument html = DownloadHtml(url);
            return ParseHtml(html);
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

        private string ParseHtml(HtmlDocument html)
        {
            HtmlNode node = html.DocumentNode.SelectSingleNode("//*[@id=\"content\"]");
            HtmlNode numberingStyling = html.DocumentNode.SelectSingleNode("(//style)[1]"); // Include problem/lemma/theorem/definition numbering (#8)
            if(numberingStyling != null)
            {
                node.ChildNodes.Add(numberingStyling);
            }             

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

            return 
                AddStyling(
                    FixInlineMath(
                        node.InnerHtml));
        }

        private static void FixHrefs(HtmlNode node)
        {
            var hrefs = node.SelectNodes("//*[@href]");
            if (hrefs == null || hrefs.Count < 1) return;

            foreach (var href in hrefs)
            {
                /*
                    Fix various href formats

                    Examples:
                        //en.wikipedia.org/wiki/Boy_or_Girl_paradox

                    Correct format:
                        http(s)://(www.)test.com

                */
                string newHref = href.Attributes["href"].Value;
                newHref = newHref.StartsWith("//") ? newHref.Substring(2) : newHref;
                //newHref = newHref.StartsWith("www") ? "http://" + newHref : newHref;
                if (newHref.StartsWith("chapter")) {
                    newHref = newHref.Split('/')[1]; // local href, eg. chapter11/11_1_2_basic_concepts_of_the_poisson_process.php#equation11_1 (without chapter11)
                } else
                {
                    newHref = !newHref.StartsWith("http") ? "http://" + newHref : newHref;
                }                
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

                // If src is Youtube icon, replace it
                if (newSrc == "//icons.iconarchive.com/icons/bokehlicia/captiva/32/web-google-youtube-icon.png")
                {
                    newSrc = "https://cdn1.iconfinder.com/data/icons/google_jfk_icons_by_carlosjj/512/youtube.png";
                    src.Attributes["src"].Value = newSrc;
                    string style = src.Attributes["style"].Value;
                    src.Attributes["style"].Value = style + "height: 50px;";
                    continue;
                }

                /*
                    Fix various src formats

                    Examples:
                        //www.probabilitycourse.com/images/chapter6/Convex_b.png
                        images/chapter1/transmission_color.png

                    Correct format:
                        http(s)://(www.)test.com/test.png

                */
                newSrc = newSrc.StartsWith("//") ? newSrc.Substring(2) : newSrc;
                newSrc = newSrc.StartsWith("www") ? newSrc.Substring(4) : newSrc;
                newSrc = !newSrc.StartsWith("probabilitycourse") ? "probabilitycourse.com/" + newSrc : newSrc;
                newSrc = !newSrc.StartsWith("http") ? "https://" + newSrc : newSrc;
                src.Attributes["src"].Value = newSrc;
            }
        }

        /*
            Corrects inline math surrounded by dollar signs ($), since
            the MathJax plugin for ReLaXed needs inline equations
            to be surrounded by \\( and \\). 
            (Display math surrounded by double dollar sign ($$),
             needs to stay the same)

            Example:
                Here is an equation: $ 1+1=2 $
            becomes
                Here is an equation: \\( 1+1=2 \\)
        */
        private static string FixInlineMath(string content)
        {
            int counter = 0;
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < content.Length; i++)
            {
                if (content[i] != '$')
                {
                    sb.Append(content[i]);
                    continue;
                }

                // $

                if(content[i+1] != '$')
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
                    continue;
                }

                // $$

                if (counter % 2 == 0)
                {
                    sb.Append("$$");                    
                }
                i++;
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
