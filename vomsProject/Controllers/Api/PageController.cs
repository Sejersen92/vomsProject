using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using vomsProject.Data;

namespace vomsProject.Controllers.Api
{
    public class PublishDto
    {
        public PublishDto(string html, object content)
        {
            Html = html;
            Content = content;
        }

        public object Content { get; private set; }
        public string Html { get; private set; }
    }

    [Route("api/[controller]")]
    [ApiController]
    public class PageController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public PageController(ApplicationDbContext context)
        {
            _context = context;
        }
        [Route("{id}/update")]
        [HttpPost]
        public async Task Update(int id, [FromBody] object body)
        {
            var page = _context.Pages.Find(id);
            page.Content = body.ToString();
            await _context.SaveChangesAsync();
        }
        [Route("{id}/publish")]
        [HttpPost]
        public async Task Publish(int id, [FromBody] PublishDto body)
        {
            var page = _context.Pages.Find(id);
            page.IsPublished = true;
            page.Content = body.Content.ToString();
            page.HtmlRenderContent = body.Html;
            await _context.SaveChangesAsync();
        }
    }
}
