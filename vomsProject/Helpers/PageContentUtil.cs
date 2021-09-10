using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using vomsProject.Data;

namespace vomsProject.Helpers
{
    public class PageContentUtil
    {
        /// <summary>
        /// Construct the full page content
        /// </summary>
        /// <param name="page">The page. Layout and LastSavedVersion needs to be included on the page object</param>
        /// <returns>The full page content</returns>
        public static IEnumerable<Editor.TransferBlock> ConstructPageContent(Page page)
        {
            var header = JsonConvert.DeserializeObject<IEnumerable<Editor.TransferBlock>>(
                page.Layout != null ? page.Layout.HeaderEditableContent : "[]");

            var mainContent = JsonConvert.DeserializeObject<IEnumerable<Editor.TransferBlock>>(
                page.LastSavedVersion != null ? page.LastSavedVersion.Content : "[]");

            var footer = JsonConvert.DeserializeObject<IEnumerable<Editor.TransferBlock>>(
                page.Layout != null ? page.Layout.FooterEditableContent : "[]");

            return new Editor.TransferBlock[]
            {
                new Editor.TransferBlock()
                {
                    type = Editor.BlockType.Container,
                    tagType = "header",
                    properties = new Dictionary<string, string>(),
                    blocks = header,
                },
                new Editor.TransferBlock()
                {
                    type = Editor.BlockType.Container,
                    tagType = "main",
                    properties = new Dictionary<string, string>(),
                    blocks = mainContent,
                },
                new Editor.TransferBlock()
                {
                    type = Editor.BlockType.Container,
                    tagType = "footer",
                    properties = new Dictionary<string, string>(),
                    blocks = footer,
                }
            };
        }
    }
}
