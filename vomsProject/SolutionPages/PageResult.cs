using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vomsProject.SolutionPages
{
    public class PageResult : IActionResult
    {
        private PageModel PageModel;
        public PageResult(PageModel pageModel)
        {
            PageModel = pageModel;
        }
        public async Task ExecuteResultAsync(ActionContext context)
        {
            context.HttpContext.Response.StatusCode = StatusCodes.Status200OK;
            context.HttpContext.Response.ContentType = "text/html";

            var body = new StreamWriter(context.HttpContext.Response.Body, Encoding.UTF8);
            try
            {
                await body.WriteAsync(SolutionRenderers.Instance.PageRenderer(PageModel));
            }
            finally
            {
                await body.DisposeAsync();
            }
        }
    }
}
