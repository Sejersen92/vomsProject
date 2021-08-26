using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace vomsProject.Data
{
    public class Page
    {
        public int Id { get; set; }

        public string PageName { get; set; }

        // This is the content that we load into the editor. (Editable content)
        public string Content { get; set; }

        // This is the content to be displayed when the page is published.  (Renderable content)
        public string HtmlRenderContent { get; set; }

        public bool IsPublished { get; set; }

        public Solution Solution { get; set; }
    }
}
