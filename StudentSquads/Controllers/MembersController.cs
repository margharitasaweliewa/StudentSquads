using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using StudentSquads.Models;

namespace StudentSquads.Controllers
{
    public class MembersController : Controller
    {
        // GET: Members

        public ActionResult ShowAll(int? pageIndex, string sortBy)
        {
            if (!pageIndex.HasValue)
                pageIndex = 1;
            if (String.IsNullOrWhiteSpace(sortBy))
                sortBy = "Squad";
            var member = new Member() { ExitReason = "Не выдержал" };
            return View(member);
            //return Content(String.Format("pageIndex={0}&sortBy={1}", pageIndex, sortBy));
        }
    }
}