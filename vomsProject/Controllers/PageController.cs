using Microsoft.AspNetCore.Authorization;
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
        private readonly ApplicationDbContext _context;
        public PageController(ApplicationDbContext context)
        {
            _context = context;
        }

        [Authorize]
        public IActionResult Index(int? id)
        {
            var page = id == null ? _context.Pages.FirstOrDefault() : _context.Pages.Find(id);

            if (page != null) return View(page);

            page = new Page {Solution = _context.Solutions.FirstOrDefault()};

            _context.Pages.Add(page);
            _context.SaveChanges();

            return View(page);
        }
    }
}
