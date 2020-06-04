using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Dynamic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using StudentSquads.Models;
using StudentSquads.ViewModels;
using System.IO;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Wordprocessing;
using DocumentFormat.OpenXml.Packaging;
using System.Xml;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;

namespace StudentSquads.Controllers
{
    public class MembersController : Controller
    {
        // GET: Members
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
            var headofsquad = _context.HeadsOfStudentSquads.Include(h => h.MainPosition)
                .SingleOrDefault(h => (h.PersonId == person.Id) && (h.DateofEnd == null) && (h.DateofBegin != null));
            //Если активной записи о руководстве не найдено, перенаправляем на главную страницу
            return headofsquad;
        }
        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
        }
        //Вот эту функцию можно будет удалить/(это для проверки ролей)
        public ViewResult Index()
        {
            var members = _context.Members.Include(m => m.Squad).ToList();
            if (User.IsInRole("SquadManager"))
                return View("SquadManager", members);
            else
                return View("ShowAll", members);
        }
        public ActionResult ShowAll(int? pageIndex, string sortBy)
        {
            if (!pageIndex.HasValue)
                pageIndex = 1;
            if (String.IsNullOrWhiteSpace(sortBy))
                sortBy = "Squad";
            var members = _context.Members.Include(m => m.Squad).Include(m => m.Status).ToList();
            return View(members);

            //return Content(String.Format("pageIndex={0}&sortBy={1}", pageIndex, sortBy));
        }
        //Функция для ограничения списка заявок согласно с ролью пользователя
        public List<Member> LimitMembers(List<Member> allmembers, HeadsOfStudentSquads headofsquad)
        {
            List<Member> members = new List<Member>();
            if (User.IsInRole("SquadManager"))
            {
                //Для руководителей отрядов ограничиваем отрядом
                members = allmembers.Where(m => m.SquadId == headofsquad.SquadId).ToList();
            }
            if (User.IsInRole("UniManager"))
            {
                //Для руководителей штаба отбираем только штаб
                members = allmembers.Where(m => m.Squad.UniversityHeadquarterId == headofsquad.UniversityHeadquarterId).ToList();
            }
            if (User.IsInRole("RegionalManager"))
            {
                //Это будет работать только в пределах одного регионального отделения, т.к. ограничения по рег.отделению не стоит
                //Для руководителей рег. отделения ограничиваем одобрением ком.состава
                members = allmembers.Where(m => m.ApprovedByCommandStaff == true).ToList();
            }
            return members;
        }
        //Следующие функций для вступления в организацию
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ApplyForEnter(PersonMainFormViewModel model)
        {
            //var personInDb = _context.People.SingleOrDefault(p => p.Id == id);
            //if (personInDb == null) return RedirectToAction("PersonMainForm", "People");
            //Создаем заявление на вступление
            CreateDocument(model, 1);
            Member newMember = new Member
            {
                Id = Guid.NewGuid(),
                PersonId = model.Person.Id,
                SquadId = model.SquadId,
                ApplicationStatus = "Не рассмотрено"
            };
            _context.Members.Add(newMember);
            _context.SaveChanges();
            return RedirectToAction("PersonMainForm", "People");
        }
        [HttpGet]
        public ActionResult EnterApplications()
        {
            List<ApplicationsListViewModel> listmodel = new List<ApplicationsListViewModel>();
            var headofsquad = GetHeadOfStudentSquads();
            if (headofsquad == null) return RedirectToAction("PersonMainForm", "People");
            //Выбираем из таблицы Members без даты вступления, решение ком. состава либо true, либо null, даты выхода нет(если есть, значит, отклонен)
            //Также откидываем записи уже состоящих в отрядах
            var allmembers = _context.Members.Include(m => m.Person).Include(m => m.Squad)
                .Where(m => (m.Person.MembershipNumber == null) && (m.ApprovedByCommandStaff != false) && (m.Person.DateOfExit == null)).ToList();
            //Получаем ограниченный по роли список
            List<Member> members = LimitMembers(allmembers, headofsquad);
            foreach (var member in members)
            {
                string sex = "";
                string feepayment = "";
                //Определяем пол для отображения
                if (member.Person.Sex == true) sex = "Муж";
                else sex = "Жен";
                var payments = _context.FeePayments.Where(m => m.PersonId == member.PersonId).ToList();
                if (payments.Count==0) feepayment = "Не сдан";
                else feepayment = "Cдан";
                ApplicationsListViewModel newapplication = new ApplicationsListViewModel
                {
                    Choosen = false,
                    Id = member.Id,
                    PersonId = member.PersonId,
                    FIO = member.Person.FIO,
                    Sex = sex,
                    DateOfBirth = member.Person.DateofBirth.ToString("dd.MM.yyyy"),
                    PhoneNumber = member.Person.PhoneNumber,
                    PlaceOfStudy = member.Person.PlaceofStudy,
                    Squad = member.Squad.Name,
                    FeePayment = feepayment,
                    Status = member.ApplicationStatus
                };
                listmodel.Add(newapplication);
            }
            return View(listmodel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ApproveEnterApllications(List<ApplicationsListViewModel> applications)
        {
            if (applications == null) return RedirectToAction("EnterApplications", "Members");
            foreach (var member in applications)
            {//Если выбрали для одобрения
                if (member.Choosen)
                {
                    var memberInDb = _context.Members.Single(m => m.Id == member.Id);
                    //Если ком. составом рассматривается, делаем "Одобрено ком. составом"
                    if (User.IsInRole("SquadManager"))
                    {
                        memberInDb.ApprovedByCommandStaff = true;
                        memberInDb.ApplicationStatus = "На рассмотрении рег.штабом";
                    }
                    //Если региональным отделением, то ставим дату вступления у личности, пока без номера членского билета
                    else if (User.IsInRole("RegionalManager"))
                    {
                        var personInDb = _context.People.Single(m => m.Id == member.PersonId);
                        personInDb.DateOfEnter = DateTime.Now;
                        memberInDb.ApplicationStatus = "Ожидается членский взнос";
                    }
                }

            }
            _context.SaveChanges();
            return RedirectToAction("EnterApplications", "Members");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RejectEnterApplications(List<ApplicationsListViewModel> applications)
        {
            foreach (var member in applications)
            {//Если выбрали для одобрения
                if (member.Choosen)
                {
                    var memberInDb = _context.Members.Single(m => m.Id == member.Id);
                    //Если ком. составом рассматривается, делаем "Одобрено ком. составом" = false
                    if (User.IsInRole("SquadManager"))
                    {
                        memberInDb.ApprovedByCommandStaff = false;
                        memberInDb.ApplicationStatus = "Отклонено ком. составом ЛСО";
                    }
                    //Если региональным отделением, то ставим дату исклбчения личности, без даты вступления с датой исключения будут считаться непринятые
                    //Они не будут рассматриваться нигде
                    else if (User.IsInRole("RegionalManager"))
                    {
                        var personInDb = _context.People.Single(m => m.Id == member.PersonId);
                        personInDb.DateOfExit = DateTime.Now;
                        memberInDb.ApplicationStatus = "Отклонено рег. отделением";
                    }
                }

            }
            _context.SaveChanges();
            return RedirectToAction("EnterApplications", "Members");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateEnterProtocol(List<ApplicationsListViewModel> applications)
        {//Проверяем (на всякий случай) роль
            if (User.IsInRole("RegionalManager"))
            { //Идем по всем в списке
                foreach (var member in applications)
                {//Если выбран
                    if (member.Choosen)
                    {
                        var memberInDb = _context.Members.Single(m => m.Id == member.Id);
                        //Добавляем статус
                        memberInDb.ApplicationStatus = "Член отряда";
                        //Добавляем дату вступелния члену отряда
                        memberInDb.DateOfEnter = DateTime.Now;
                        var personInDb = _context.People.Single(p => p.Id == member.PersonId);
                        //Если после одобрения уже составляется протокол, то время изменяется, так как время принятися = время составление протокола
                        personInDb.DateOfEnter = DateTime.Now;
                        //Фукция для вычисления рег. номера
                        string regnumber = NewRegNumber(memberInDb);
                        personInDb.MembershipNumber = regnumber;
                    }
                }
                _context.SaveChanges();
                //Функция для заполнения протокола (т.е. списком должно выводиться все)
            }
            return RedirectToAction("EnterApplications", "Members");
        }
        public string NewRegNumber(Member member)
        {
            string regnumber = "";
            //Находим номер штаба
            UniversityHeadquarter uni = _context.Squads.Include(u => u.UniversityHeadquarter).Single(u => u.Id == member.SquadId).UniversityHeadquarter;
            string uninumber = uni.UniversityNumber;
            //Находим номер рег отделения
            string regionnumber = _context.RegionalHeadquarters.Single(r => r.Id == uni.RegionalHeadquarterId).RegionNumber;
            regnumber = regionnumber + uninumber;
            //Находим самый большой номер с таким началом
            var numbers = _context.People.Where(p => p.MembershipNumber != null).Select(p => p.MembershipNumber).ToList();
            List<int> allnumbers = new List<int>();
            foreach (var number in numbers)
            {
                //Если номера с таким же началом, удаляем подстроку-начало, конвертируем в int, добавляем в список значений
                if (number.Contains(regnumber))
                {
                    allnumbers.Add(Convert.ToInt32(number.Replace(regnumber, "")));
                }
            }
            //Находим самое большое из найденных значений
            int max = allnumbers.Max();
            //Тут ещё надо спросить, что происходит при 999
            //Формируем рег. номер
            string newregnumber = regnumber + Convert.ToString(max + 1);
            return newregnumber;
        }
        //Функции для перехода в другой отряд
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ApplyForTransition(PersonMainFormViewModel model)
        {
            //Находим запись, в котором отряде в настоящее время находится член организации (совпадает личность, он активен)
            var memberInDb = _context.Members.SingleOrDefault(m => (m.PersonId == model.Person.Id) && (m.DateOfExit == null) && (m.DateOfEnter != null));
            //Создаем заявление на переход
            //TransitionDocument(model, memberInDb.SquadId);
            Member newMember = new Member
            {
                Id = Guid.NewGuid(),
                PersonId = model.Person.Id,
                SquadId = model.SquadId,
                FromSquadId = memberInDb.SquadId,
                ApplicationStatus = "Не рассмотрено"
            };
            //Добавляем параметр "Переходит в отряд" в текущую активную запись члена организации в отряде
            memberInDb.ToSquadId = model.SquadId;
            _context.Members.Add(newMember);
            _context.SaveChanges();
            //Создаем заявление о переходе
            CreateDocument(model, 2);
            return RedirectToAction("PersonMainForm", "People");
        }
        [HttpGet]
        public ActionResult TransitionApplications()
        {
            List<ApplicationsListViewModel> listmodel = new List<ApplicationsListViewModel>();
            //Объявили список для отображения
            var headofsquad = GetHeadOfStudentSquads();
            if (headofsquad == null) return RedirectToAction("PersonMainForm", "People");
            //Выбираем из таблицы Members без даты вступления, решение ком. состава либо true, либо null, "из отряда" заполнено
            //Также смотрим, чтобы DateOfExit был null (в этом случае отклоняется руководителем рег. отделением)
            var allmembers = _context.Members.Include(m => m.Person).Include(m => m.Squad).Include(m => m.FromSquad)
                .Where(m => (m.DateOfEnter == null) && (m.DateOfExit == null) && (m.ApprovedByCommandStaff != false) && (m.FromSquadId != null)).ToList();
            //Получаем ограниченный по роли список
            List<Member> members = LimitMembers(allmembers, headofsquad);
            foreach (var member in members)
            {
                //Ищем текущего члена отряда по личности
                var oldmember = _context.Members.Single(o => (o.PersonId == member.PersonId) && (o.DateOfEnter != null) && (o.DateOfExit == null));
                ApplicationsListViewModel newapplication = new ApplicationsListViewModel
                {
                    Choosen = false,
                    Id = member.Id,
                    OldId = oldmember.Id,
                    PersonId = member.PersonId,
                    FIO = member.Person.FIO,
                    MembershipNumber = member.Person.MembershipNumber,
                    DateOfBirth = member.Person.DateofBirth.ToString("dd.MM.yyyy"),
                    PhoneNumber = member.Person.PhoneNumber,
                    Squad = member.Squad.Name,
                    OldSquad = member.FromSquad.Name,
                    Status = member.ApplicationStatus
                };
                listmodel.Add(newapplication);
            }
            return View(listmodel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ApproveTransitionApllications(List<ApplicationsListViewModel> applications)
        {
            foreach (var member in applications)
            {//Если выбрали для одобрения
                if (member.Choosen)
                {
                    var memberInDb = _context.Members.Single(m => m.Id == member.Id);
                    //Если ком. составом рассматривается, делаем "Одобрено ком. составом"
                    if (User.IsInRole("SquadManager"))
                    {
                        memberInDb.ApprovedByCommandStaff = true;
                        memberInDb.ApplicationStatus = "На рассмотрении рег. штабом";
                    }
                    //Если региональным отделением, то ставим дату вступления новому члену отряда, дату выхода старому
                    else if (User.IsInRole("RegionalManager"))
                    {
                        memberInDb.DateOfEnter = DateTime.Now;
                        memberInDb.ApplicationStatus = "Член отряда";
                        var oldmemberInDb = _context.Members.Single(m => m.Id == member.OldId);
                        oldmemberInDb.DateOfExit = DateTime.Now;
                        oldmemberInDb.ApplicationStatus = "Исключен";
                    }
                }

            }
            _context.SaveChanges();
            return RedirectToAction("TransitionApplications", "Members");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RejectTransitionApplications(List<ApplicationsListViewModel> applications)
        {
            foreach (var member in applications)
            {//Если выбрали для одобрения
                if (member.Choosen)
                {
                    var memberInDb = _context.Members.Single(m => m.Id == member.Id);
                    //Если ком. составом рассматривается, делаем "Одобрено ком. составом" = false
                    if (User.IsInRole("SquadManager"))
                    {
                        memberInDb = _context.Members.Single(m => m.Id == member.Id);
                        memberInDb.ApprovedByCommandStaff = false;
                        memberInDb.ApplicationStatus = "Отклонено ком. составом ЛСО";
                    }
                    //Если региональным отделением, то очищаем поле "Переходит в отряд" в старом члене отрядов
                    //В таблице Member ставим Дату выхода. Если есть дата выхода без даты вступления, значит, отменено рег. отделением
                    else if (User.IsInRole("RegionalManager"))
                    {
                        memberInDb.DateOfExit = DateTime.Now;
                        memberInDb.ApplicationStatus = "Отклонено рег. отделением";
                        var oldmemberInDb = _context.Members.Single(m => m.Id == member.OldId);
                        oldmemberInDb.ToSquad = null;
                    }
                }

            }
            _context.SaveChanges();
            return RedirectToAction("TransitionApplications", "Members");
        }
        //Следующие функции для исключения из организации
        [HttpDelete]
        [ValidateAntiForgeryToken]
        public void ApplyForExit(Guid personId)
        {   
            //Если пользователь является командиром отряда, необходимо сначала найти себе замену, потом только выйти из состава организации
            bool ok = true;
            var headofsquad = GetHeadOfStudentSquads();
            if (headofsquad != null)
            {
                if (headofsquad.MainPositionId != 1) ok=false;
            }
            if (ok)
            {
                //Если является руководителем на данный момент, проставляем дату окончания 
                if(headofsquad!=null)headofsquad.DateofEnd = DateTime.Now;
                //ExitDocument(model);
                var personInDb = _context.People.SingleOrDefault(p => p.Id == personId);
                //Сразу ставим дату выхода из организации
                personInDb.DateOfExit = DateTime.Now;
                //Если есть в каком-то отряде на данный момент, из него тоже исключаем
                var memberInDb = _context.Members.SingleOrDefault(p => (p.PersonId == personId) && (p.DateOfEnter != null) && (p.DateOfExit == null));
                if (memberInDb != null) memberInDb.DateOfExit = DateTime.Now;
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
            }
        }
        [HttpGet]
        public ActionResult ExitApplications()
        {
            List<ApplicationsListViewModel> listmodel = new List<ApplicationsListViewModel>();
            var headofsquad = GetHeadOfStudentSquads();
            if (headofsquad == null) return RedirectToAction("PersonMainForm", "People");
            List<Member> members = new List<Member>();
            if (User.IsInRole("SquadManager")||(User.IsInRole("UniManager")))
            {
                //Для выходящих из организации выбираем активных членов отряда, где DateofExit у личности не null
                List<Member> outorganisation = _context.Members.Include(o => o.Person).Include(o => o.Squad)
                  .Where(o => (o.DateOfEnter != null) && (o.DateOfExit == null) && (o.Person.DateOfExit != null)).ToList();
                //Для переходящих в друго отряд выбираем активных, которые подали заявку на переход
                List<Member> toothersquad = _context.Members.Include(t => t.Person).Include(o => o.Squad).Include(t => t.ToSquad)
                    .Where(o => (o.DateOfEnter != null) && (o.DateOfExit == null) && (o.ToSquadId != null)).ToList();
                //Если показываем руководителю штаба, то только те, кто переходит в другой штаб
                if ((User.IsInRole("UniManager")))
                {
                    toothersquad = toothersquad.Where(a => a.ToSquad.UniversityHeadquarterId != headofsquad.UniversityHeadquarterId).ToList();
                }
                List<Member> allmemberssquad = outorganisation.Union(toothersquad).ToList();
                members = LimitMembers(allmemberssquad, headofsquad);
                foreach (var member in members)
                {
                    string status = "";
                    if (member.ToSquad != null) status = "Переход в другой отряд";
                    else status = "Выход из организации";
                    ApplicationsListViewModel newapplication = new ApplicationsListViewModel
                    {
                        Id = member.Id,
                        Squad = member.Squad.Name,
                        PersonId = member.PersonId,
                        FIO = member.Person.FIO,
                        DateOfBirth = member.Person.DateofBirth.ToString("dd.MM.yyyy"),
                        PhoneNumber = member.Person.PhoneNumber,
                        MembershipNumber = member.Person.MembershipNumber,
                        Status = status
                    };
                    listmodel.Add(newapplication);
                }
            }
             else if (User.IsInRole("RegionalManager"))
            {
                //Для рег. отделения получаем всех личностей, у кого ещё нет протокола об исключении
                var people = _context.People.Where(p => (p.DateOfEnter != null) && (p.DateOfExit != null) && (p.ExitDocumentPath == null)).ToList();
                foreach (var person in people)
                {
                    Guid memberid = new Guid();
                    string squad = "";
                    //Исправить!!!
                    var memberInDb = _context.Members.Include(m => m.Squad).SingleOrDefault(m => (m.PersonId ==person.Id)&&(m.DateOfEnter!=null)&&(m.DateOfExit==null));
                    if (memberInDb != null) { memberid = memberInDb.Id; squad = memberInDb.Squad.Name; }
                    ApplicationsListViewModel newapplication = new ApplicationsListViewModel
                        {
                            Id = memberid,
                            Squad = squad,
                            PersonId = person.Id,
                            FIO = person.FIO,
                            DateOfBirth = person.DateofBirth.ToString("dd.MM.yyyy"),
                            PhoneNumber = person.PhoneNumber,
                            MembershipNumber = person.MembershipNumber,
                        };
                        listmodel.Add(newapplication);
       
                }
            }
            return View(listmodel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateExitProtocol(List<ApplicationsListViewModel> applications)
        {
            foreach (var member in applications)
            {//Если выбран для протокола об исключении
                if (member.Choosen)
                {
                    var memberInDb = _context.Members.SingleOrDefault(m => m.Id == member.Id);
                    memberInDb.ApplicationStatus = "Исключен";
                }

            }
            _context.SaveChanges();
            return RedirectToAction("ExitApplications", "Members");
        }
        private static Dictionary<string, BookmarkEnd> FindBookmarks(OpenXmlElement documentPart, Dictionary<string, BookmarkEnd> results = null, Dictionary<string, string> unmatched = null)
        {
            results = results ?? new Dictionary<string, BookmarkEnd>();
            unmatched = unmatched ?? new Dictionary<string, string>();

            foreach (var child in documentPart.Elements())
            {
                if (child is BookmarkStart)
                {
                    var bStart = child as BookmarkStart;
                    unmatched.Add(bStart.Id, bStart.Name);
                }

                if (child is BookmarkEnd)
                {
                    var bEnd = child as BookmarkEnd;
                    foreach (var orphanName in unmatched)
                    {
                        if (bEnd.Id == orphanName.Key)
                            results.Add(orphanName.Value, bEnd);
                    }
                }
                FindBookmarks(child, results, unmatched);
            }
            return results;
        }
        public void CreateDocument(PersonMainFormViewModel model, int type) 
        {
            Squad squad = new Squad();
            Squad tosquad = new Squad();
            Member member = new Member();
            Person person = _context.People.SingleOrDefault(p => p.Id == model.Person.Id);
            if (type==1)
                //Если на вступление, то Squad = это туда, куда вступают
                squad = _context.Squads.Include(s => s.Direction).SingleOrDefault(s => s.Id == model.SquadId);
            else
            {
                //Если уже есть член организации, то Squad = это то, где этот член уже состоит
                member = _context.Members.SingleOrDefault(m => (m.PersonId == model.Person.Id) && (m.DateOfEnter != null) && (m.DateOfExit == null));
                squad = _context.Squads.SingleOrDefault(s => s.Id == member.SquadId);
                //Если переход, то новый отряд также находим
                if (member.ToSquadId != null) tosquad = _context.Squads.Include(t => t.UniversityHeadquarter).SingleOrDefault(t => t.Id == member.ToSquadId);
            }
            string fileName = "";
            string newfileName = "";
            switch (type)
            {
                //Заявление на вступление
                case 1:
                    fileName = "C:/Users/Маргарита/source/repos/StudentSquadsGit2/StudentSquads/Files/Заявление_на_вступление_в_РСО.docx";
                    break;
                //Заявление на переход
                case 2:
                    fileName = "C:/Users/Маргарита/source/repos/StudentSquadsGit2/StudentSquads/Files/Заявление_о_переходе.docx";
                    break;
                //Заявление на выход
                case 3:
                    fileName = "C:/Users/Маргарита/source/repos/StudentSquadsGit2/StudentSquads/Files/Заявление_о_выходе_из_РСО.docx";
                    break;
            };
            //Находим штаб, к которому член организации привязан
            UniversityHeadquarter uni = _context.UniversityHeadquarters.Include(u => u.RegionalHeadquarter).SingleOrDefault(u => u.Id == squad.UniversityHeadquarterId);
            var wordDocument= WordprocessingDocument.Open(fileName as string, false);
            newfileName = fileName +"_"+ person.FIOinGenetiv + "_" + DateTime.Now.ToString("dd.MM.yyyy") + ".docx";
            wordDocument.Clone(newfileName, true).Close();
            var newwordDocument = WordprocessingDocument.Open(newfileName as string, true);
            var bookMarks = FindBookmarks(newwordDocument.MainDocumentPart.Document);
            foreach (var end in bookMarks)
            {
                //Переменная для заполнения
                string inn = "";
                switch (end.Key)
                {
                    case "FIO":
                        inn = person.FIOinGenetiv;
                        break;
                    case "DateofBirth":
                        inn = person.DateofBirth.ToString("dd.MM.yyyy");
                        break;
                    case "PlaceofStudy":
                        if(person.PlaceofStudy!=null)
                        inn = person.PlaceofStudy;
                        break;
                    case "RegistrationPlace":
                        inn = person.RegistrationPlace;
                        break;
                    case "PhoneNumber":
                        inn = person.PhoneNumber;
                        break;
                    case "Email":
                        inn = person.Email;
                        break;
                    case "PassportSerie":
                        inn = person.PassportSerie;
                        break;
                    case "PassportNumber":
                        inn = person.PassportNumber;
                        break;
                    case "PassportGiven":
                        inn = person.PassportGiven;
                        break;
                    case "DateofIssue":
                        inn = person.DateofIssue.ToString("dd.MM.yyyy");
                        break;
                    case "DepartmentCode":
                        inn = person.DepartmentCode;
                        break;
                    case "INN":
                        inn = person.INN;
                        break;
                    case "Snils":
                        inn = person.Snils;
                        break;
                    case "Squad":
                        inn = squad.Name;
                        break;
                    case "Direction":
                        inn = squad.Direction.Name;
                        break;
                    case "Uni":
                        inn = uni.University;
                        break;
                    case "Region":
                        inn = uni.RegionalHeadquarter.Region;
                        break;
                    case "Date":
                        inn = DateTime.Now.ToString("dd.MM.yyyy");
                        break;
                    case "MembershipNumber":
                        inn = person.MembershipNumber;
                        break;
                    case "FIOInfinitiv":
                        inn = person.FIO;
                        break;
                    case "ToSquad":
                        inn = tosquad.Name;
                        break;
                    case "ToUni":
                        inn = tosquad.UniversityHeadquarter.University;
                        break;
                };
                //Настраиваем размер шрифта
                RunProperties runProp = new RunProperties();
                FontSize size = new FontSize();
                size.Val = new StringValue("24");
                runProp.Append(size);
                //Создаем элемент для прогона
                var textElement = new Text(inn);
                var runElement = new Run(textElement);
                //Подставляем значение
                runElement.PrependChild<RunProperties>(runProp);
                end.Value.InsertAfterSelf(runElement);
            }
            newwordDocument.MainDocumentPart.Document.Save();
            //Добавляем путь к файлу
            switch (type)
            {
                case 1:
                    person.EnterDocumentPath = newfileName;
                    break;
                case 2:
                    member.TransitionDocumentPath = newfileName;
                    break;
                case 3:
                    person.ExitDocumentPath = newfileName;
                    break;
            }
            _context.SaveChanges();
        }
        public ActionResult ChangeSquadManagerApplications()
        {
            List<ApplicationsListViewModel> listofapplications = new List<ApplicationsListViewModel>();
            //Находим все неутвержденные должности
            var managers = _context.HeadsOfStudentSquads.Include(h => h.Person).Include(h => h.Squad).Include(h => h.MainPosition)
                .Where(h => (h.DateofBegin==null)&&(h.DateofEnd==null)).ToList();
            foreach(var manager in managers)
            {
                string oldmanagerstring = "Отсутсвует";
                //Находим того, кто сейчас на этой должности
                var oldmanager = _context.HeadsOfStudentSquads.Include(h => h.Person)
                    .SingleOrDefault(h => (h.DateofBegin!=null)&&(h.DateofEnd==null)
                    &&(h.MainPositionId==manager.MainPositionId)&&(h.SquadId==manager.SquadId));
                if (oldmanager != null) oldmanagerstring = oldmanager.Person.FIO;
                ApplicationsListViewModel newapplication = new ApplicationsListViewModel
                {
                    FIO = manager.Person.FIO,
                    Squad = manager.Squad.Name,
                    Uni = _context.Squads
                    .Include(u => u.UniversityHeadquarter)
                    .Single(u => u.Id == manager.SquadId).UniversityHeadquarter.ShortContent,
                    Id = manager.Id,
                    OldFIO = oldmanagerstring,
                    Position = manager.MainPosition.Name
                };
                listofapplications.Add(newapplication);
            }
            return View(listofapplications);
        }
        public ActionResult AddFeeApplications()
        {
            List<ApplicationsListViewModel> listapplications = new List<ApplicationsListViewModel>();
            var fees = _context.FeePayments.Include(f => f.Person)
                .Where(f => f.Approved == null).ToList();
            foreach (var fee in fees)
            {
                var member = _context.Members.Include(m => m.Squad)
                    .SingleOrDefault(m => (m.PersonId ==fee.PersonId)&&(m.DateOfEnter!=null)&&(m.DateOfExit==null));
                if (member != null) 
                {
                    string uni = _context.Squads.Include(s => s.UniversityHeadquarter)
                        .Single(s => s.Id == member.SquadId).UniversityHeadquarter.ShortContent;
                    ApplicationsListViewModel newFee = new ApplicationsListViewModel
                    {
                        FIO = fee.Person.FIO,
                        Squad = member.Squad.Name,
                        Id = fee.Id,
                        FeePayment = fee.DateofPayment.ToString("dd.MM.yyyy"),
                        SumOfPayment = 300,
                        Uni = uni,
                        Choosen = false
                    };
                    listapplications.Add(newFee);
                }
               
            }
            return View(listapplications);
        }
        public ActionResult ApproveFee(List<ApplicationsListViewModel> applications)
        {
            foreach(var application in applications)
            {
                if (application.Choosen)
                {
                    var feeInDb = _context.FeePayments.Single(f => f.Id == application.Id);
                    feeInDb.Approved = true;
                }
            }
            _context.SaveChanges();
            return RedirectToAction("AddFeeApplications","Members");
        }
        public ActionResult RejectFee(List<ApplicationsListViewModel> applications)
        {
            foreach (var application in applications)
            {
                if (application.Choosen)
                {
                    var feeInDb = _context.FeePayments.Single(f => f.Id == application.Id);
                    feeInDb.Approved = false;
                }
            }
            _context.SaveChanges();
            return RedirectToAction("AddFeeApplications", "Members");
        }
        public ActionResult ApproveChange (List<ApplicationsListViewModel> applications)
        {
            var userManager = new ApplicationUserManager(new UserStore<ApplicationUser>(_context));
            foreach (var application in applications)
            {
                if (application.Choosen)
                {
                    var managerInDb = _context.HeadsOfStudentSquads.SingleOrDefault(m => m.Id==application.Id);
                    //Находим того, у кого была старая должность
                    var oldmanager = _context.HeadsOfStudentSquads
                        .SingleOrDefault(m => (m.DateofBegin != null) && (m.DateofEnd == null)
                        && (m.SquadId == managerInDb.SquadId) && (m.MainPositionId == managerInDb.MainPositionId));
                   //Если есть старый, то снимаем его с должности
                    if (oldmanager != null)
                    {
                        oldmanager.DateofEnd = DateTime.Now;
                        //Убираем роль у пользователя
                        //Находим id пользователя, который связан с личностью
                        var newid = _context.People.SingleOrDefault(i => i.Id == oldmanager.PersonId).ApplicationUserId;
                        userManager.RemoveFromRole(newid, "SquadManager");
                    }
                    //Нового назначаем
                    managerInDb.DateofBegin = DateTime.Now;
                    //Добавляем роль
                    //Находим id пользователя, который связан с личностью
                    var oldid = _context.People.SingleOrDefault(i => i.Id == managerInDb.PersonId).ApplicationUserId;
                    userManager.AddToRole(oldid, "SquadManager");
                }
            }
            _context.SaveChanges();
            return RedirectToAction("ChangeSquadManagerApplications", "Members");
        }
        public ActionResult RejectChange(List<ApplicationsListViewModel> applications)
        {
            foreach (var application in applications)
            {
                if (application.Choosen)
                {
                    var managerInDb = _context.HeadsOfStudentSquads.SingleOrDefault(m => m.Id == application.Id);
                    //Нового отклоняем, ставя ему дату окончания
                    managerInDb.DateofEnd = DateTime.Now;
                }
            }
            _context.SaveChanges();
            return RedirectToAction("ChangeSquadManagerApplications", "Members");
        }

    }
}