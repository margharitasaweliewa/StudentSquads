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
using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Office2010.Excel;

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
        [HttpPut]
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
            //var headofSquads = memberscontr.GetHeadOfStudentSquads();
            string id = User.Identity.GetUserId();
            var person = _context.People.SingleOrDefault(u => u.ApplicationUserId == id);
            //Проверяем 2 условия. В таблице "Руководителей" личность совпадает с текущей, а также должность активна
            var headofsquad = _context.HeadsOfStudentSquads.Include(h => h.MainPosition).Include(h => h.Squad)
                .Include(h => h.UniversityHeadquarter).Include(h => h.RegionalHeadquarter)
                .SingleOrDefault(h => (h.PersonId == person.Id) && (h.DateofEnd == null) && (h.DateofBegin != null));
            //Если активной записи о руководстве не найдено, перенаправляем на главную страницу
            var positions = _context.HeadsOfStudentSquads.Include(p => p.Person).Include(p => p.MainPosition)
                .OrderBy(p => p.DateofEnd).ThenByDescending(p => p.DateofBegin)
                .ToList();
            positions = LimitPositions(positions, headofsquad);
            string hasrole = "";
            foreach (var position in positions)
            {
                if (position.HasRole == true) hasrole = "Eсть";
                else hasrole = "Нет";
                DesignationViewModel head = new DesignationViewModel
                {
                    HeadofStudentSquadsId = position.Id,
                    PersonId = position.Person.Id,
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
        [HttpPost]
        public IHttpActionResult CreateNewHead(DesignationViewModel head)
        {
            //Определяем текущего пользоватея
            //var headofsquads = memberscontr.GetHeadOfStudentSquads();
            string Id = User.Identity.GetUserId();
            var person = _context.People.SingleOrDefault(u => u.ApplicationUserId == Id);
            //Проверяем 2 условия. В таблице "Руководителей" личность совпадает с текущей, а также должность активна
            var headofsquad = _context.HeadsOfStudentSquads.Include(h => h.MainPosition).Include(h => h.Squad)
                .Include(h => h.UniversityHeadquarter).Include(h => h.RegionalHeadquarter)
                .SingleOrDefault(h => (h.PersonId == person.Id) && (h.DateofEnd == null) && (h.DateofBegin != null));
            //Если активной записи о руководстве не найдено, перенаправляем на главную страницу
            bool changerole = false;
            //Объект "роль"
            HeadsOfStudentSquads positionInDb = new HeadsOfStudentSquads();
            //При редактировании находим в БД и редактируем данные
            if (head.HeadofStudentSquadsId != Guid.Empty)
            {
                positionInDb = _context.HeadsOfStudentSquads.Single(p => p.Id == head.HeadofStudentSquadsId);
                //Изменяю поля
                positionInDb.PersonId = head.PersonId;
                positionInDb.Position = head.Position;
                //Если понадобится редактировать даты, то необходимо сделать их типом дата
                //Если изменили ролевую доступность, тогда добавляем/удаляем роль
                if (head.HasRole != positionInDb.HasRole) changerole = true;
            }
            //При создании нового подставляем полученные параметры
            else 
            {
                //Определяем, задана ли главная должность
                int? mainposition = null;
                if (head.MainPositionId != 0) { mainposition = head.MainPositionId; }
                //Надо добавить валидацию
                positionInDb.Id = Guid.NewGuid();
                positionInDb.PersonId = head.PersonId;
                positionInDb.Position = head.Position;
                positionInDb.MainPositionId = mainposition;
                //Если не позволять самим подставлять дату начала работы
                positionInDb.DateofBegin = DateTime.Now;
                positionInDb.SquadId = headofsquad.SquadId;
                positionInDb.UniversityHeadquarterId = headofsquad.UniversityHeadquarterId;
                positionInDb.RegionalHeadquarterId = headofsquad.RegionalHeadquarterId;
            }
            //При создании или если изменилась роль, работаем с ролями
            if (changerole || head.HeadofStudentSquadsId == Guid.Empty)
            {
                var userManager = new ApplicationUserManager(new UserStore<ApplicationUser>(_context));
                //Находим id пользователя, который связан с личностью
                var id = _context.People.SingleOrDefault(i => i.Id == head.PersonId).ApplicationUserId;
                string role = "";
                if (User.IsInRole("SquadManager")) role = "SquadManager";
                else if (User.IsInRole("UniManager")) role = "UniManager";
                else if (User.IsInRole("RegionalManager")) role = "RegionalManager";
                else if (User.IsInRole("DesantManager")) role = "DesantManager";
                if (head.HasRole)
                {
                    positionInDb.HasRole = true;
                    userManager.AddToRole(id, role);
                }
                else 
                {
                    positionInDb.HasRole = false;
                    userManager.RemoveFromRole(id, role);
                }
            }
            //Добавляем только, если это был новый пользователя
            if(head.HeadofStudentSquadsId == Guid.Empty)
            _context.HeadsOfStudentSquads.Add(positionInDb);
            _context.SaveChanges();
            return Ok();
        }
        }
}
