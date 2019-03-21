using Fizzler.Systems.HtmlAgilityPack;
using HtmlAgilityPack;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .CreateLogger();

            Book book = new Book("book.json");

            Downloader downloader = new Downloader(book);
            downloader.Download();

            PDFGenerator pdfGenerator = new PDFGenerator(book);
            pdfGenerator.Generate();

            BookmarkGenerator bookmarkGenerator = new BookmarkGenerator(book);
            bookmarkGenerator.Generate();

            // Remove temporary files / Clean up
            //RemoveFiles(files.Select(f => f.Replace(".html", "_temp.htm")));
            //RemoveFiles(files);

            Console.WriteLine("Done.");
            Console.ReadLine();
        }
    }
}