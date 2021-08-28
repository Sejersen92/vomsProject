using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace vomsProject.SolutionPages
{
    public class Status404PageResult : IActionResult
    {
        private Page404Model Page404Model;
        public Status404PageResult(Page404Model page404Model)
        {
            Page404Model = page404Model;
        }
        public async Task ExecuteResultAsync(ActionContext context)
        {
            context.HttpContext.Response.StatusCode = StatusCodes.Status404NotFound;
            context.HttpContext.Response.ContentType = "text/html";
            
            var body = new StreamWriter(context.HttpContext.Response.Body, Encoding.UTF8);
            try
            {
                await body.WriteAsync(SolutionRenderers.Instance.Status404Renderer(Page404Model));
            }
            finally
            {
                await body.DisposeAsync();
            }
        }
    }
}
