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
        public string content;
        public string html;
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
        public async Task Update([FromRoute] int id, string body)
        {
            var page = Context.Pages.Find(id);
            page.Content = body;
            await Context.SaveChangesAsync();
        }
        public async Task Publish(int id, PublishDTO body)
        {
            var page = Context.Pages.Find(id);
            page.Content = body.content;
            page.HtmlRenderContent = body.html;
            await Context.SaveChangesAsync();
        }
    }
}
