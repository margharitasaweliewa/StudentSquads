using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using StudentSquads.Models;
using StudentSquads.ViewModels;
using System.Data.Entity;
using System.Web;
using Microsoft.AspNet.Identity;
using System.Dynamic;
using System.ComponentModel;
using Microsoft.AspNet.Identity.EntityFramework;
using StudentSquads.Controllers;
using System.Web.Services.Protocols;
using System.Data;
using DocumentFormat.OpenXml.Office2010.PowerPoint;

namespace StudentSquads.Controllers.API
{
    public class RaitingSectionsController : ApiController
    {
        private ApplicationDbContext _context;
        //Для обращения к методам Limit и GetHeadOfStudentSquads
        public RaitingSectionsController()
        {
            _context = new ApplicationDbContext();
        }
        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
        }
        [HttpPost]
        public IHttpActionResult NewSection(RaitingSectionViewModel model)
        {
            RaitingSection section = new RaitingSection();
            if (model.Id == Guid.Empty) 
            {
                //Добавляем показатель
                    section.Id = Guid.NewGuid();
                    section.Name = model.Name;
                    section.MembershipTypeId = Convert.ToInt32(model.MembershipIdString);
                    section.Removed = false;
                try { section.Coef = Convert.ToDouble(model.Coef); }
                catch (Exception) { return BadRequest(); }
                _context.RaitingSections.Add(section);
            }
            else
            {
                section = _context.RaitingSections.Single(s => s.Id == model.Id);
                section.Name = model.Name;
                section.MembershipTypeId = Convert.ToInt32(model.MembershipIdString);
            }
            //Добавлеяем уровень показателя
            foreach (var levelId in model.LevelIds)
            {
                RaitingSectionLevel newlevel = new RaitingSectionLevel
                {
                    Id = Guid.NewGuid(),
                    EventLevelId = levelId,
                    RaitingSectionId = section.Id
                };
                _context.RaitingSectionLevels.Add(newlevel);
            }

            _context.SaveChanges();
            return Ok();
        }
        [HttpPut]
        public IHttpActionResult DeleteSectionLevel(Guid id)
        {
            var raitinglevel = _context.RaitingSectionLevels.SingleOrDefault(r => r.Id == id);
            if (raitinglevel != null) 
            { 
            _context.RaitingSectionLevels.Remove(raitinglevel);
            _context.SaveChanges();
            }
            return Ok();
        }
        [HttpDelete]
        public IHttpActionResult DeleteSection(Guid id)
        {
            var sectionInDb = _context.RaitingSections.Single(s => s.Id == id);
            sectionInDb.Removed = true;
            _context.SaveChanges();
            return Ok();
        }
    }
}
