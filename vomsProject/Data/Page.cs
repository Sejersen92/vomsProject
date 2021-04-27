﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace vomsProject.Data
{
    public class Page
    {
        public int Id { get; set; }
        public string PageName { get; set; }
        public string Content { get; set; }
        public string HtmlRenderContent { get; set; }
        public bool IsPublished { get; set; }
        public Solution Solution { get; set; }
    }
}
