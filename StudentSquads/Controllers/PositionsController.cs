using StudentSquads.Models;
using StudentSquads.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace StudentSquads.Controllers
{
    public class PositionsController : Controller
    {
        // GET: Positions
        private ApplicationDbContext _context;
        MembersController memberscontr = new MembersController();
        public PositionsController()
        {
            _context = new ApplicationDbContext();

        }
        public ActionResult AllPositions()
        {
            return View();
        }
        public ActionResult PositionForm()
        {
            DesignationViewModel viewmodel = new DesignationViewModel
            {
                HasRole = false
            };
            return View(viewmodel);
        }
        public ActionResult Edit(Guid id)
        {
            var position = _context.HeadsOfStudentSquads.Include(c => c.Person).Include(c => c.MainPosition)
                .SingleOrDefault(c => c.Id == id);
            if (position == null)
                return HttpNotFound();
            var viewModel = new DesignationViewModel
            {
                HeadofStudentSquadsId = position.Id,
                PersonId = position.Person.Id,
                FIO = position.Person.FIO,
                Position = position.Position,
                DateofBegin = position.DateofBegin?.ToString("dd.MM.yyyy"),
                DateofEnd = position.DateofEnd?.ToString("dd.MM.yyyy"),
                HasRole = position.HasRole
            };
            return View("PositionEditForm", viewModel);
        }
        public ActionResult Dismiss(DesignationViewModel model)
        {
            var positionInDb = _context.HeadsOfStudentSquads.Single(p => p.Id == model.HeadofStudentSquadsId);
            positionInDb.DateofEnd = DateTime.Now;
            _context.SaveChanges();
            return RedirectToAction("AllPositions", "Positions");
        }
        public ActionResult Delete(DesignationViewModel model)
        {
            var positionInDb = _context.HeadsOfStudentSquads.Single(p => p.Id == model.HeadofStudentSquadsId);
            _context.HeadsOfStudentSquads.Remove(positionInDb);
            _context.SaveChanges();
            return RedirectToAction("AllPositions", "Positions");
        }
        public ActionResult ChangeSquadManager()
        {
           var mainPositions = _context.MainPositions.ToList();
            DesignationViewModel newModel = new DesignationViewModel
            {
                MainPositions = mainPositions
            };
            return View(newModel);
        }
    }
}