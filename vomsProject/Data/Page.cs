using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace vomsProject.Data
{
    public class Page
    {
        public int Id { get; set; }
        /// <summary>
        /// The url path used to look up a page.
        /// </summary>
        public string PageName { get; set; }
        /// <summary>
        /// The title of the page.
        /// </summary>
        public string Title { get; set; }

        public int? LastSavedVersionId { get; set; }
        /// <summary>
        /// The version loaded by default.
        /// </summary>
        public PageContent LastSavedVersion { get; set; }

        /// <summary>
        /// The Rendered published version. Used when displayed as a normal page.
        /// </summary>
        public string HtmlRenderContent { get; set; }
        public int? PublishedVersionId { get; set; }
        public PageContent PublishedVersion { get; set; }

        public bool IsPublished { get; set; }

        public Solution Solution { get; set; }
        public ICollection<PageContent> Versions { get; set; }
        public Layout Layout { get; set; }

        [NotMapped]
        public string DestinationUrl { get; set; }
    }
}
