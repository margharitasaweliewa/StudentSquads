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
            //Добавляем показатель
            RaitingSection section = new RaitingSection
            {
                Id = Guid.NewGuid(),
                Name = model.Name,
            MembershipTypeId = Convert.ToInt32(model.MembershipIdString),
            Removed = false
            };
            try { section.Coef = Convert.ToDouble(model.Coef); }
            catch (Exception) { return BadRequest(); }

            _context.RaitingSections.Add(section);
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
        public IHttpActionResult SaveSection()
        {

            return Ok();
        }
        [HttpDelete]
        public IHttpActionResult DeleteSection()
        {

            return Ok();
        }
    }
}
