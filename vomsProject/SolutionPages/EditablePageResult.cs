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
    public class EditablePageResult : IActionResult
    {
        private EditablePageModel EditablePageModel;
        public EditablePageResult(EditablePageModel editablePageModel)
        {
            EditablePageModel = editablePageModel;
        }
        public async Task ExecuteResultAsync(ActionContext context)
        {
            context.HttpContext.Response.StatusCode = StatusCodes.Status200OK;
            context.HttpContext.Response.ContentType = "text/html";

            var body = new StreamWriter(context.HttpContext.Response.Body, Encoding.UTF8);
            try
            {
                await body.WriteAsync(SolutionRenderers.Instance.EditablePageRenderer(EditablePageModel));
            }
            finally
            {
                await body.DisposeAsync();
            }
        }
    }
}
