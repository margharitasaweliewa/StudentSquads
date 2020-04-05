using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using StudentSquads.Models;

namespace StudentSquads.Controllers
{
    public class MembersController : Controller
    {
        // GET: Members
        private ApplicationDbContext _context;
        public MembersController()
        {
            _context = new ApplicationDbContext();
        }
        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
        }
        public ActionResult ShowAll(int? pageIndex, string sortBy)
        {
            if (!pageIndex.HasValue)
                pageIndex = 1;
            if (String.IsNullOrWhiteSpace(sortBy))
                sortBy = "Squad";

            var members = _context.Members.Include(m => m.Squad).ToList();
            return View(members);
            //return Content(String.Format("pageIndex={0}&sortBy={1}", pageIndex, sortBy));
        }
        
    }
}