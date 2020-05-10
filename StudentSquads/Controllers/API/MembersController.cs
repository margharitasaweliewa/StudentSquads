using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using StudentSquads.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.ComponentModel;
using System.Data.Entity;
using System.Dynamic;
using System.Web;

using StudentSquads.ViewModels;
namespace StudentSquads.Controllers.API
{
    public class MembersController : ApiController
    {
        private ApplicationDbContext _context;
        public MembersController()
        {
            _context = new ApplicationDbContext();
        }
        public HeadsOfStudentSquads GetHeadOfStudentSquads()
        {
            string id = User.Identity.GetUserId();
            var person = _context.People.SingleOrDefault(u => u.ApplicationUserId == id);
            //Проверяем 2 условия. В таблице "Руководителей" личность совпадает с текущей, а также должность активна
            var headofsquad = _context.HeadsOfStudentSquads.Include(h => h.MainPosition).Include(h => h.Squad)
                .Include(h => h.UniversityHeadquarter).Include(h => h.RegionalHeadquarter)
                .SingleOrDefault(h => (h.PersonId == person.Id) && (h.DateofEnd == null) && (h.DateofBegin != null));
            //Если активной записи о руководстве не найдено, перенаправляем на главную страницу
            return headofsquad;
        }
        public List<Member> LimitMembers(List<Member> allmembers, HeadsOfStudentSquads headofsquad)
        {
            List<Member> members = new List<Member>();
            if (User.IsInRole("SquadManager"))
            {
                //Для руководителей отрядов ограничиваем отрядом
                members = allmembers.Where(m => m.SquadId == headofsquad.SquadId).ToList();
            }
            else if (User.IsInRole("UniManager"))
            {
                //Для руководителей штаба отбираем только штаб
                members = allmembers.Where(m => m.Squad.UniversityHeadquarterId == headofsquad.UniversityHeadquarterId).ToList();
            }
            else if (User.IsInRole("RegionalManager"))
            {
                //Это будет работать только в пределах одного регионального отделения, т.к. ограничения по рег.отделению не стоит
                //Для руководителей рег. отделения ограничиваем одобрением ком.состава
                members = allmembers.Where(m => m.ApprovedByCommandStaff == true).ToList();
            }
            return members;
        }
        [HttpDelete]
        public IHttpActionResult ApplyForExit(Guid personId)
        {
            //Если пользователь является командиром отряда, необходимо сначала найти себе замену, потом только выйти из состава организации
            var headofsquad = GetHeadOfStudentSquads();
            if (headofsquad != null)
            {
                if (headofsquad.MainPositionId != 1) throw new HttpResponseException(HttpStatusCode.NotFound); ;
            }
                //Если является руководителем на данный момент, проставляем дату окончания 
                if (headofsquad != null) headofsquad.DateofEnd = DateTime.Now;
                //ExitDocument(model);
                var personInDb = _context.People.SingleOrDefault(p => p.Id == personId);
                //Сразу ставим дату выхода из организации
                personInDb.DateOfExit = DateTime.Now;
                personInDb.ExitReason = "По собственному желанию";
                //Если есть роли, убираем роли
                if (User.IsInRole("SquadManager") || User.IsInRole("UniManager") || User.IsInRole("RegionalManager") || User.IsInRole("DesantManager"))
                {
                    string id = User.Identity.GetUserId();
                    var userManager = new ApplicationUserManager(new UserStore<ApplicationUser>(_context));
                    if (User.IsInRole("SquadManager"))
                        userManager.RemoveFromRole(id, "SquadManager");
                    if (User.IsInRole("UniManager"))
                        userManager.RemoveFromRole(id, "UniManager");
                    if (User.IsInRole("RegionalManager"))
                        userManager.RemoveFromRole(id, "RegionalManager");
                    if (User.IsInRole("DesantManager"))
                        userManager.RemoveFromRole(id, "DesantManager");
                }
                _context.SaveChanges();
            return Ok();
        }

    }
}
