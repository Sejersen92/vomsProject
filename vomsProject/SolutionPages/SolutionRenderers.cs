using Stubble.Compilation.Builders;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace vomsProject.SolutionPages
{
    public class EditablePageModel
    {
        public int id;
        public string content;
        public string title;
        public string header;
        public string footer;
        public bool isPublished;
    }
    public class PageModel
    {
        public string content;
        public string title;
        public string header;
        public string footer;
    }
    public class Page404Model
    {
        public string solutionName;
    }

    public class SolutionRenderers
    {
        public static readonly SolutionRenderers Instance = new SolutionRenderers();
        public readonly Func<EditablePageModel, string> EditablePageRenderer;
        public readonly Func<PageModel, string> PageRenderer;
        public readonly Func<Page404Model, string> Status404Renderer;

        private SolutionRenderers()
        {
            var Stubble = new StubbleCompilationBuilder().Build();
            var templateDir = Path.GetDirectoryName(
                Uri.UnescapeDataString(
                    new Uri(Assembly.GetExecutingAssembly().Location).AbsolutePath)) + "/SolutionPages";
            using (var editablePage = new StreamReader(templateDir + "/EditablePageTemplate.html"))
            using (var page = new StreamReader(templateDir + "/PageTemplate.html"))
            using (var page404 = new StreamReader(templateDir + "/404.html"))
            {
                EditablePageRenderer = Stubble.Compile<EditablePageModel>(editablePage.ReadToEnd());
                PageRenderer = Stubble.Compile<PageModel>(page.ReadToEnd());
                Status404Renderer = Stubble.Compile<Page404Model>(page404.ReadToEnd());
            }
        }
    }
}