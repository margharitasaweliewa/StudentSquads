using System;
using System.IO;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using StudentSquads.Models;
using StudentSquads.ViewModels;
using Microsoft.AspNet.Identity;
using System.Dynamic;
using System.ComponentModel;
using Microsoft.AspNet.Identity.EntityFramework;

namespace StudentSquads.Controllers
{
    public class PeopleController : Controller
    {

            
        private ApplicationDbContext _context;

        public PeopleController()
        {
            _context = new ApplicationDbContext();
      
        }
        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
        }
        // GET: People
        //Нам не нужна эта функция, так как мы используем api для отображения личностей
        public ActionResult AllPeople(int? pageIndex, string sortBy)
        {
            return View();
        }
        //public FileResult GetFile()
        //{
        //    // Путь к файлу
        //    string file_path = Server.MapPath("~/Files/Заявление на вступление в РСО_Волкова.docx");
        //    // Тип файла - content-type
        //    string file_type = "application/docx";
        //    // Имя файла - необязательно
        //    string file_name = "Заявление на вступление в РСО";
        //    var file = File(file_path, file_type, file_name);
        //    return
        //}

        public ActionResult PersonMainForm()
        {
            string id = User.Identity.GetUserId();
            var unis = _context.UniversityHeadquarters.ToList();
            var squads = _context.Squads.ToList();
            var person = _context.People.SingleOrDefault(u => u.ApplicationUserId == id);
            //Объявляем файл вступления в РСО
            FilePathResult enterfile = null;
            if (person.EnterDocumentPath != null)
            {
                enterfile = File(person.EnterDocumentPath, "application/docx", "Заявление на вступление в РСО");
                string str = enterfile.FileDownloadName;
            }
            //var fileContents = System.IO.File.ReadAllText(Server.MapPath(@person.EnterDocumentPath));
            PersonMainFormViewModel newmember = new PersonMainFormViewModel
            {

                file = "https://vk.com/doc9058999_499015105?hash=fbeb2e184c6b7d11b3&dl=3d0d5c4de610f3032e",
                Person =person,
                Squads = squads,
                UniversityHeadquarters = unis,
            };
        
            if (person == null) return View(newmember);
            var personid = person.Id;
            //Тут нужно прописать зависимость
            //Находим все связи с отрядами
            var allsquads = _context.Members.Include(m => m.Squad).Include(m => m.Status)
                .Where(s => s.PersonId == personid).ToList();
            //Находим все связи с должностями
            var allpositions = _context.HeadsOfStudentSquads.Include(p => p.Squad).Include(p => p.MainPosition)
                .Where(p => p.PersonId == personid).ToList();
            //Добавляем в модель
            newmember.AllPersonSquads = allsquads;
            newmember.AllPersonPositions = allpositions;
            return View(newmember);
        }
        public ActionResult PersonForm()
        {//Определяем вид для отображения
            var squads = _context.Squads.ToList();
            var mainpositions = _context.MainPositions.ToList();
            var viewModel = new NewPersonViewModel
            {
                Squads = squads,
                MainPositions = mainpositions
            };
            string id = User.Identity.GetUserId();
            var person = _context.People.SingleOrDefault(u => u.ApplicationUserId == id);
            //Если User ещё не привязан к личности, возвращаем пустую форму
            if (person == null) return View(viewModel);
            //Иначе возвращаем заполненную
            else
            {
                var modelperson = _context.People.SingleOrDefault(p => p.Id == person.Id);
                viewModel.Person = modelperson;
                return View(viewModel);
            }

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Save(NewPersonViewModel newModel)
        {//Это надо будет перенести в другое место
            foreach (string file in Request.Files)
            {
                HttpPostedFileBase File = Request.Files[file] as HttpPostedFileBase;
                // получаем имя файла
                string fileName = System.IO.Path.GetFileName(File.FileName);
                // сохраняем файл в папку Files в проекте
                File.SaveAs(Server.MapPath("~/Files/" + fileName));
            }
            if (!ModelState.IsValid)
            {
                return View("PersonForm", newModel);
            }
            //Проверяем, есть ли личность у пользователя. Если нет, добавляем
            if (Convert.ToString(newModel.Id) == "00000000-0000-0000-0000-000000000000")
            {
                var personId = Guid.NewGuid();
                newModel.Person.Id = personId;
                //Добавляем новую личность с идентификатором
                newModel.Person.FIO = Convert.ToString(newModel.Person.LastName + ' ' + newModel.Person.FirstName + ' ' + newModel.Person.PatronymicName);
                //Получаем объект User
                string id = User.Identity.GetUserId();
                //Присваиваем Person
                newModel.Person.ApplicationUserId = id;
                //Если является членом организации, сразу проставляем дату вступления
                if (newModel.Person.MembershipNumber != null) newModel.Person.DateOfEnter = DateTime.Now;
                //Если является членом отряда, создаем запись в таблице "Member"
                if (newModel.Member.SquadId != null)
                {
                    newModel.Member.Id = Guid.NewGuid();
                    newModel.Member.PersonId = personId;
                    newModel.Member.DateOfEnter = DateTime.Now;
                    newModel.Member.ApprovedByCommandStaff = true;
                    _context.Members.Add(newModel.Member);
                    //Если является ком. составом отряда,создаем запись в таблице "HeadsofStudentSquads"
                    if (newModel.HeadsOfStudentSquads.MainPositionId != null)
                    {
                        newModel.HeadsOfStudentSquads.Id = Guid.NewGuid();
                        newModel.HeadsOfStudentSquads.PersonId = personId;
                        newModel.HeadsOfStudentSquads.SquadId = newModel.Member.SquadId;
                        newModel.HeadsOfStudentSquads.DateofBegin = DateTime.Now;
                        _context.HeadsOfStudentSquads.Add(newModel.HeadsOfStudentSquads);
                        //Добавлеяем статус "Член ком. состава"
                        newModel.Member.StatusId = 8;
                    }
                }
                _context.People.Add(newModel.Person);
                if (newModel.HeadsOfStudentSquads.MainPositionId != null)
                {
                    //И добавляем пользователю Роль "Руководитель отряда"
                    var userManager = new ApplicationUserManager(new UserStore<ApplicationUser>(_context));
                    userManager.AddToRole(id, "SquadManager");
                }
            }
            else
            {
                var personInDb = _context.People.Single(p => p.Id == newModel.Person.Id);
                //Изменяю поля персональных данных
                personInDb.LastName = newModel.Person.LastName;
                personInDb.FirstName = newModel.Person.FirstName;
                personInDb.PatronymicName = newModel.Person.PatronymicName;
                personInDb.PlaceofStudy = newModel.Person.PlaceofStudy;
                personInDb.FormofStudy = newModel.Person.FormofStudy;
                personInDb.PhoneNumber = newModel.Person.PhoneNumber;
                personInDb.CityofBirth = newModel.Person.CityofBirth;
                personInDb.DateofBirth = newModel.Person.DateofBirth;
                personInDb.DateofIssue = newModel.Person.DateofIssue;
                personInDb.DepartmentCode = newModel.Person.DepartmentCode;
                personInDb.Email = newModel.Person.Email;
                personInDb.INN = newModel.Person.INN;
                personInDb.PasportSerie = newModel.Person.PasportSerie;
                personInDb.PassportNumber = newModel.Person.PassportNumber;
                personInDb.RegistrationPlace = newModel.Person.RegistrationPlace;
                personInDb.Sex = newModel.Person.Sex;
                personInDb.Snils = newModel.Person.Snils;
                personInDb.FIO = Convert.ToString(newModel.Person.LastName + ' ' + newModel.Person.FirstName + ' ' + newModel.Person.PatronymicName);
            }
            _context.SaveChanges();
            
            return RedirectToAction("AllPeople","People");
        }
        public ActionResult Edit(Guid id)
        {
            var person = _context.People.SingleOrDefault(c => c.Id == id);
            var status = _context.Status.ToList();
            if (person == null)
                return HttpNotFound();
            //Может же Member не быть - надо поправить
            var member = _context.Members.SingleOrDefault(m => (m.PersonId == id)&&(m.DateOfEnter!=null) &&(m.DateOfExit == null));
            var viewModel = new NewPersonViewModel
            {
                Person = person,
                Status = status,
                Member = member

            };
            return View("PersonEditForm", viewModel);
        }
        //Думаю насчет перемещения в Member, но тогда надо будет поменять название действия в View
        [HttpPost]
        public ActionResult ChangeStatus(NewPersonViewModel newModel)
        {
            if (!ModelState.IsValid)
            {
                return View("PersonEditForm", newModel);
            }
            var memberInDb = _context.Members.Single(m => m.Id == newModel.Member.Id);
            //Изменяю поля персональных данных
            memberInDb.StatusId = newModel.Member.Status.Id;
            _context.SaveChanges();
            return RedirectToAction("AllPeople", "People");
        }
        

    }
}