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
using System.Security.Cryptography;

namespace StudentSquads.Controllers.API
{
    public class PositionsController : ApiController
    {
        private ApplicationDbContext _context;
        //Для обращения к методам Limit и GetHeadOfStudentSquads
        MembersController memberscontr = new MembersController();
        public PositionsController()
        {
            _context = new ApplicationDbContext();
        }
        public List<HeadsOfStudentSquads> LimitPositions(List<HeadsOfStudentSquads> allheads, HeadsOfStudentSquads headofsquad)
        {
            List<HeadsOfStudentSquads> heads = new List<HeadsOfStudentSquads>();
            if (User.IsInRole("SquadManager"))
            {
                //Для руководителей отрядов ограничиваем отрядом
                heads = allheads.Where(h => h.SquadId == headofsquad.SquadId).ToList();
            }
            else if (User.IsInRole("UniManager"))
            {
                //Для руководителей штаба отбираем только штаб
                heads = allheads.Where(m => m.UniversityHeadquarterId == headofsquad.UniversityHeadquarterId).ToList();
            }
            else if (User.IsInRole("RegionalManager"))
            {
                //Это будет работать только в пределах одного регионального отделения, т.к. ограничения по рег.отделению не стоит
                //Для руководителей рег. отделения ограничиваем одобрением ком.состава
                heads = allheads.Where(m => m.RegionalHeadquarterId == headofsquad.RegionalHeadquarterId).ToList();
            }
            return heads;
        }
        [HttpGet]
        public List<DesignationViewModel> GetPositions()
        {
            List<DesignationViewModel> listpositions = new List<DesignationViewModel>();
            var headofSquads = memberscontr.GetHeadOfStudentSquads();
            var positions = _context.HeadsOfStudentSquads.Include(p => p.Person).Include(p => p.MainPosition)
                .OrderBy(p => p.DateofEnd).ThenByDescending(p => p.DateofBegin)
                .ToList();
            positions = LimitPositions(positions, headofSquads);
            string hasrole = "";
            foreach (var position in positions)
            {
                if (position.HasRole == true) hasrole = "Eсть";
                else hasrole = "Нет";
                DesignationViewModel head = new DesignationViewModel
                {
                    Id = position.Person.Id,
                    FIO = position.Person.FIO,
                    Position = position.Position,
                    DateofBegin = position.DateofBegin?.ToString("dd.MM.yyyy"),
                    DateofEnd = position.DateofEnd?.ToString("dd.MM.yyyy"),
                    HasRoleString = hasrole
                };
                listpositions.Add(head);

            }
            return listpositions;
        }
        }
}
