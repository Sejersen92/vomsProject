using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using vomsProject.Data;

namespace vomsProject.Controllers
{
    public class PageController : Controller
    {
        ApplicationDbContext Context;
        public PageController(ApplicationDbContext context)
        {
            Context = context;
        }
        public IActionResult Index(int? id)
        {
            Page page;
            if (id == null)
            {
                page = Context.Pages.FirstOrDefault();
            }
            else
                page = Context.Pages.Find(id);
            if (page == null)
            {
                page = new Page();
                Context.Pages.Add(page);
                Context.SaveChanges();
            }
            return View(page);
        }
    }
}
