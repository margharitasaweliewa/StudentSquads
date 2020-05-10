using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace StudentSquads.Controllers
{
    public class PositionsController : Controller
    {
        // GET: Positions
        public ActionResult AllPositions()
        {
            return View();
        }
    }
}