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
using DocumentFormat.OpenXml.Wordprocessing;

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
        public HeadsOfStudentSquads GetHeadOfStudentSquads()
        {
            string id = User.Identity.GetUserId();
            var person = _context.People.SingleOrDefault(u => u.ApplicationUserId == id);
            //Проверяем 2 условия. В таблице "Руководителей" личность совпадает с текущей, а также должность активна
            var headofsquad = _context.HeadsOfStudentSquads.Include(h => h.MainPosition)
                .SingleOrDefault(h => (h.PersonId == person.Id) && (h.DateofEnd == null) && (h.DateofBegin != null));
            //Если активной записи о руководстве не найдено, перенаправляем на главную страницу
            return headofsquad;
        }
        // GET: People
        //Нам не нужна эта функция, так как мы используем api для отображения личностей
        public ActionResult AllPeople(int? pageIndex, string sortBy)
        {
            List<NewPersonViewModel> listofpeople = new List<NewPersonViewModel>();
            //Находим текущего пользователя
            var headofsquad = GetHeadOfStudentSquads();
            if (User.IsInRole("RegionalManager"))
            {
                //Находим всех активных членов
                var allpeople = _context.People
                    .Where(p => (p.DateOfEnter != null) && (p.DateOfExit == null)).ToList();
                foreach (var person in allpeople)
                {
                    //Находим последнего активного члена отряда для личности (который не перемещался в другой отряд)
                    var member = _context.Members.Include(m => m.Person).Include(m => m.Squad).Include(m => m.Status)
                        .SingleOrDefault(m => (m.PersonId == person.Id) && (m.DateOfEnter != null) && (m.ToSquadId == null));
                    string uni = "";
                    string lastfee = "";
                    if (member != null)
                    {
                        uni = _context.Squads.Include(u => u.UniversityHeadquarter)
                              .Single(u => u.Id == member.SquadId).UniversityHeadquarter.ShortContent;
                        //Находим все принятые взносы
                        var fees = _context.FeePayments.Where(f => (f.PersonId == person.Id)&&(f.Approved==true)).Select(f => f.DateofPayment).ToList();
                        if(fees.Count!=0)
                        lastfee = fees.Max().ToString("dd.MM.yyyy");
                    NewPersonViewModel newPerson = new NewPersonViewModel
                    {
                        
                        LastFee = lastfee,
                        Id = person.Id,
                        FIO = person.FIO,
                        DateofBirth = person.DateofBirth.ToString("dd.MM.yyyy"),
                        PhoneNumber = person.PhoneNumber,
                        MembershipNumber = person.MembershipNumber,
                        SquadName = member?.Squad.Name,
                        Uni = uni,
                        StatusName = (member.StatusId == null ? String.Empty : member.Status.Name),
                        Choosen = false
                    };
                    listofpeople.Add(newPerson);
                    }

                }
            }
            //Иначе находим членов
            else
            {
                //Находим выбывших
                var members = _context.Members.Include(m => m.Squad).Include(m => m.Person).Include(m => m.Status)
                    .Where(m => (m.DateOfEnter != null) && (m.DateOfExit == null) &&
                    ((m.SquadId == headofsquad.SquadId) || (m.Squad.UniversityHeadquarterId == headofsquad.UniversityHeadquarterId))).ToList();
                foreach (var member in members)
                {
                    string uni = _context.Squads.Include(u => u.UniversityHeadquarter).Single(u => u.Id == member.SquadId).UniversityHeadquarter.ShortContent;
                    NewPersonViewModel newPerson = new NewPersonViewModel
                    {
                        Id = member.Person.Id,
                        FIO = member.Person.FIO,
                        DateofBirth = member.Person.DateofBirth.ToString("dd.MM.yyyy"),
                        PhoneNumber = member.Person.PhoneNumber,
                        MembershipNumber = member.Person.MembershipNumber,
                        SquadName = member.Squad.Name,
                        Uni = uni,
                        StatusName = (member.StatusId == null ? String.Empty : member.Status.Name),
                        Choosen = false
                    };
                    listofpeople.Add(newPerson);
                }
            }
            return View(listofpeople);
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
            string str = "";
            if (person != null)
            {
                if (person.EnterDocumentPath != null)
                {
                    enterfile = File(person.EnterDocumentPath, "application/docx", "Заявление на вступление в РСО");
                    str = enterfile.FileDownloadName;
                }
            }
            //var fileContents = System.IO.File.ReadAllText(Server.MapPath(@person.EnterDocumentPath));
            PersonMainFormViewModel newmember = new PersonMainFormViewModel
            {

                file = person.EnterDocumentPath,
                Person =person,
                Squads = squads,
                UniversityHeadquarters = unis,
            };
            if (person == null) return View(newmember);
            var personid = person.Id;
            //Тут нужно прописать зависимость
            //Находим все связи с отрядами
            var allsquads = _context.Members.Include(m => m.Squad).Include(m => m.Status)
                .Where(s => (s.PersonId == personid)).ToList();
            //Переводим в модели
            List<ApplicationsListViewModel> listsquads = new List<ApplicationsListViewModel>();
            if (allsquads != null)
            {
                foreach (var member in allsquads)
                {
                    string status = "";
                    if (member.StatusId != null && member.DateOfExit == null) status = member.Status.Name;
                    else status = member.ApplicationStatus;
                    ApplicationsListViewModel newMember = new ApplicationsListViewModel
                    {
                        Squad = member.Squad.Name,
                        DateOfEnter = member.DateOfEnter?.ToString("dd.MM.yyyy"),
                        DateOfExit = member.DateOfExit?.ToString("dd.MM.yyyy"),
                        Status = status,

                    };
                    listsquads.Add(newMember);
                }
            }
            //Если не отказано, не вышел из отряда и нет одобренной заявки ком. составом, 
            bool ismember = false;
            var squadsforbotton = allsquads.Where(s => s.ApprovedByCommandStaff != false);
            if (squadsforbotton.Count() != 0) ismember = true;
            //Если подал заявку в другой отряд, и она ещё не одобрена, тогда неьзя ещё одну заявку подавать
            bool inothersquad = false;
            var othersquad = allsquads.Where(o => (o.FromSquadId!=null)&&(o.DateOfEnter==null)&&(o.DateOfExit==null)&&(o.ApprovedByCommandStaff!=false)).ToList();
            if (othersquad.Count != 0) inothersquad = true;
            //Если вышел из организации, то не будут кнопки отображаться
            //Находим все связи с должностями
            var allpositions = _context.HeadsOfStudentSquads.Include(p => p.Squad).Include(p => p.MainPosition)
                .Include(p => p.UniversityHeadquarter).Include(p => p.RegionalHeadquarter)
                .Where(p => p.PersonId == personid).ToList();
            List<DesignationViewModel> listpositions= new List<DesignationViewModel>();
            if (allpositions != null)
            {
                foreach (var position in allpositions)
                {
                    string place = "";
                    if (position.SquadId != null) place = position.Squad.Name;
                    else if (position.UniversityHeadquarterId != null) place = position.UniversityHeadquarter.University;
                    else if (position.RegionalHeadquarterId != null) place = position.RegionalHeadquarter.Name;
                    DesignationViewModel newPosition = new DesignationViewModel
                    {
                        Position = position.Position,
                        DateofBegin = position.DateofBegin?.ToString("dd.MM.yyyy"),
                        DateofEnd = position.DateofEnd?.ToString("dd.MM.yyyy"),
                        Place = place
                    };
                    listpositions.Add(newPosition);
                }
            }
            //Находим трудовую деятельность
            var allworks = _context.Works.Include(w => w.Member)
                .Where(w => (w.Member.PersonId == personid)&&(w.Approved==true)&&(w.Removed==null));
            //Группируем по первоначальной записи
            var groups = allworks.GroupBy(g => g.OriginalWorkId).ToList();
            //Очищаем works
            List<Work>works = new List<Work>();
            //Добавляем к работам на целине записи с последними неотклоненными изменениями
            if (groups.Count != 0) 
            {
                foreach (var group in groups)
                {
                    //Находим максимум по дате создания в группе
                    var time = group.Max(n => n.CreateTime);
                    List<Work> groupworks = _context.Works.Include(w => w.Employer).Include(w => w.WorkProject)
                        .Where(w => (w.OriginalWorkId == group.Key)).ToList();
                    Work work = groupworks.Single(w => w.CreateTime == time);
                    //Добавляем только записи с последним изменением
                    works.Add(work);
                }
            }
            List<WorkViewModel> listworks = new List<WorkViewModel>();
            if (works.Count != 0) 
            {
                foreach (var work in works)
                {
                    string affirmed = "Нет решения";
                    if (work.Affirmed == true) affirmed = "Засчитана";
                    else if (work.Affirmed == false) affirmed = "Выговор";
                    WorkViewModel newWork = new WorkViewModel
                    {
                        Employer = work.Employer?.Name,
                        WorkProject = work.WorkProject?.Name,
                        DateofBeginString = work.DateofBegin.ToString("dd.MM.yyyy"),
                        DateofEndString = work.DateofEnd.ToString("dd.MM.yyyy"),
                        Affirmed = affirmed,
                        Season = work.Season.ToString()
                    };
                    listworks.Add(newWork);
                }
            }
            //Добавляем в модель
            newmember.AllPersonSquads = listsquads;
            newmember.AllPersonPositions = listpositions;
            newmember.AllPersonWorks = listworks;
            newmember.IsMember = ismember;
            newmember.InOtherSquad = inothersquad;
            return View(newmember);
        }
        public ActionResult PersonForm()
        {//Определяем вид для отображения
            var squads = _context.Squads.ToList();
            var mainpositions = _context.MainPositions.ToList();
            var universities = _context.UniversityHeadquarters.ToList();
            var viewModel = new NewPersonViewModel
            {
                Member = new Member(),
                Squads = squads,
                MainPositions = mainpositions,
                UniversityHeadquarters = universities
            };
            string id = User.Identity.GetUserId();
            var person = _context.People.SingleOrDefault(u => u.ApplicationUserId == id);
            //Если User ещё не привязан к личности, возвращаем пустую форму
            if (person == null) return View(viewModel);
            //Иначе возвращаем заполненную
            else
            {
                var modelperson = _context.People.SingleOrDefault(p => p.Id == person.Id);
                viewModel.Id = modelperson.Id;
                viewModel.Person = modelperson;
                return View(viewModel);
            }

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Save(NewPersonViewModel newModel)
        {
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
                if (newModel.AlreadyMember) 
                {
                    //Если является членом организации, сразу проставляем дату вступления
                    newModel.Person.DateOfEnter = DateTime.Now;
                    //Если является ком. составом отряда,создаем запись в таблице "HeadsofStudentSquads"
                    if (newModel.HeadsOfStudentSquads.MainPositionId != null)
                    {
                        //И добавляем пользователю Роль "Руководитель отряда"
                        var userManager = new ApplicationUserManager(new UserStore<ApplicationUser>(_context));
                        newModel.HeadsOfStudentSquads.Id = Guid.NewGuid();
                        newModel.HeadsOfStudentSquads.PersonId = personId;
                        //Если заполнен отряд, тогда делаем его руководителем отряда
                        if (newModel.Member.SquadId != null)
                        {
                            newModel.HeadsOfStudentSquads.SquadId = newModel.Member.SquadId;
                            userManager.AddToRole(id, "SquadManager");
                        }
                        //Если выбран штаб и руководящая роль
                        else if (newModel.UniverityId != null)
                        {
                            newModel.HeadsOfStudentSquads.UniversityHeadquarterId = newModel.UniverityId;
                            userManager.AddToRole(id, "UniManager");
                        }
                        newModel.HeadsOfStudentSquads.DateofBegin = DateTime.Now;
                        _context.HeadsOfStudentSquads.Add(newModel.HeadsOfStudentSquads);
                        //Добавлеяем статус "Член ком. состава"
                        newModel.Member.StatusId = 8;
                    }
                    //Если является членом отряда, создаем запись в таблице "Member"
                    if (newModel.Member.SquadId != null)
                    {
                        newModel.Member.Id = Guid.NewGuid();
                        newModel.Member.PersonId = personId;
                        newModel.Member.DateOfEnter = DateTime.Now;
                        newModel.Member.ApprovedByCommandStaff = true;
                        newModel.Member.ApplicationStatus = "Член отряда";
                        _context.Members.Add(newModel.Member);
                    }
                    else if (newModel.UniverityId != null)
                    {
                        newModel.Member.Id = Guid.NewGuid();
                        newModel.Member.PersonId = personId;
                        newModel.Member.DateOfEnter = DateTime.Now;
                        newModel.Member.ApprovedByCommandStaff = true;
                        newModel.Member.ApplicationStatus = "Член штаба";
                        //Найдем отряд "Без отрядв" для данного штаба
                        var withoutsquad = _context.Squads
                            .Single(s => (s.UniversityHeadquarterId == newModel.UniverityId) && (s.Name == "Без отряда"));
                        //Заносим его в найденный отряд
                        newModel.Member.SquadId = withoutsquad.Id;
                        _context.Members.Add(newModel.Member);
                    }
                }
                
               
                _context.People.Add(newModel.Person);
               
            }
            else
            {
                var personInDb = _context.People.Single(p => p.Id == newModel.Id);
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
                personInDb.PassportSerie = newModel.Person.PassportSerie;
                personInDb.PassportNumber = newModel.Person.PassportNumber;
                personInDb.PassportGiven = newModel.Person.PassportGiven;
                personInDb.RegistrationPlace = newModel.Person.RegistrationPlace;
                personInDb.Sex = newModel.Person.Sex;
                personInDb.Snils = newModel.Person.Snils;
                personInDb.FIO = Convert.ToString(newModel.Person.LastName + ' ' + newModel.Person.FirstName + ' ' + newModel.Person.PatronymicName);
                personInDb.FIOinGenetiv = newModel.Person.FIOinGenetiv;
            }
            _context.SaveChanges();
            
            return RedirectToAction("PersonMainForm","People");
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
        public ActionResult AllOldPeople()
        {
            return View();
        }
        public ActionResult AllHeadsofStudentSquads()
        {
            return View();
        }
        public ActionResult AddFeeInfo(List<NewPersonViewModel> people)
        {
            foreach (var person in people)
            {//Если выбрали для одобрения
                if (person.Choosen)
                {
                    FeePayment newPayment = new FeePayment
                    {
                        DateofPayment = DateTime.Now,
                        Id = Guid.NewGuid(),
                        PersonId = person.Id,
                        SumofPayment = 300
                    };
                    _context.FeePayments.Add(newPayment);
                }

            }
            _context.SaveChanges();
            return RedirectToAction("AllPeople", "People");
        }


    }
}