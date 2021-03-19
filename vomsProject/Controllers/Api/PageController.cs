using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using vomsProject.Data;

namespace vomsProject.Controllers.Api
{
    public class PublishDTO
    {
        public object content { get; set; }
        public string html { get; set; }
    }
    [Route("api/[controller]")]
    [ApiController]
    public class PageController : ControllerBase
    {
        ApplicationDbContext Context;
        public PageController(ApplicationDbContext context)
        {
            Context = context;
        }
        [Route("{id}/update")]
        [HttpPost]
        public async Task Update(int id, [FromBody] object body)
        {
            var page = Context.Pages.Find(id);
            page.Content = body.ToString();
            await Context.SaveChangesAsync();
        }
        [Route("{id}/publish")]
        [HttpPost]
        public async Task Publish(int id, [FromBody] PublishDTO body)
        {
            var page = Context.Pages.Find(id);
            page.Content = body.content.ToString();
            page.HtmlRenderContent = body.html;
            await Context.SaveChangesAsync();
        }
    }
}
