using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;

namespace ProbToPdf
{
    internal class Bookmark
    {
        public string Title { get; set; }
        public int Level { get; set; }
        public int PageNumber { get; set; }        
    }
}