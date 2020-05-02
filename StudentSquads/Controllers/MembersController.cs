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
        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
        }
        //Вот эту функцию можно будет удалить/(это для проверки ролей)
        public ViewResult Index()
        {
            var members = _context.Members.Include(m => m.Squad).ToList();
            if (User.IsInRole("SquadManager"))
                return View("SquadManager",members);
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ApplyForEnter(PersonMainFormViewModel model)
        {
            //var personInDb = _context.People.SingleOrDefault(p => p.Id == id);
            //if (personInDb == null) return RedirectToAction("PersonMainForm", "People");
            EnterDocument(model);
            Member newMember = new Member
            {
                Id = Guid.NewGuid(),
                PersonId = model.Person.Id,
                SquadId = model.SquadId
            };
            _context.Members.Add(newMember);
            _context.SaveChanges();
            return RedirectToAction("PersonMainForm","People");
        }
        [HttpGet]
        public ActionResult EnterApplications()
        {
            List<ApplicationsListViewModel> listmodel = new List<ApplicationsListViewModel>();
            //Объявили список для отображения
            List<Member> members = null;
            string id = User.Identity.GetUserId();
            var person = _context.People.SingleOrDefault(u => u.ApplicationUserId == id);
            //Проверяем 2 условия. В таблице "Руководителей" личность совпадает с текущей, а также должность активна
            var headofsquad = _context.HeadsOfStudentSquads.SingleOrDefault(h => (h.PersonId == person.Id) && (h.DateofEnd == null) && (h.DateofBegin != null));
            //Если активной записи о руководстве не найдено, перенаправляем на главную страницу
            if (headofsquad == null)  return RedirectToAction("PersonMainForm", "People"); 
            //Выбираем из таблицы Members без даты вступления, решение ком. состава либо true, либо null, даты выхода нет(если есть, значит, отклонен)
            var allmembers = _context.Members.Include(m => m.Person).Include(m => m.Squad)
                .Where(m => (m.DateOfEnter == null) && (m.ApprovedByCommandStaff != false) && (m.Person.DateOfExit == null)).ToList();
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
            foreach (var member in members)
            {
                string sex = "";
                string status = "";
                string feepayment = "";
                //Если ещё не выставлено решение ком. состава
                if (member.ApprovedByCommandStaff == null) status = "Не рассмотрено";
                //Если ещё не одобрено рег. отделением
                else if (member.Person.DateOfEnter == null) status = "На рассмотрении рег. штабом";
                //Если уже одобрено, но пока не является членом организации (Member.DateOfEnter = null)
                else status = "Ожидается взнос";
                //Определяем пол для отображения
                if (member.Person.Sex == true) sex = "Муж";
                else sex = "Жен";
                var payments = _context.FeePayments.Where(m => m.PersonId == member.PersonId).ToList();
                if (payments == null) feepayment = "Не сдан";
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
                    PlaceOfStudy = member.Person.PhoneNumber,
                    Squad = member.Squad.Name,
                    FeePayment = feepayment,
                    Status = status
                };
                listmodel.Add(newapplication);
            }
            return View(listmodel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ApproveEnterApllications(List<ApplicationsListViewModel> applications)
        {

            foreach (var member in applications)
            {//Если выбрали для одобрения
                if (member.Choosen)
                {

                    //Если ком. составом рассматривается, делаем "Одобрено ком. составом"
                    if (User.IsInRole("SquadManager"))
                    {
                        var memberInDb = _context.Members.Single(m => m.Id == member.Id);
                        memberInDb.ApprovedByCommandStaff = true;
                    }
                    //Если региональным отделением, то ставим дату вступления у личности, пока без номера членского билета
                    else if (User.IsInRole("RegionalManager"))
                    {
                        var personInDb = _context.People.Single(m => m.Id == member.PersonId);
                        personInDb.DateOfEnter = DateTime.Now;
                    }
                }

            }
            _context.SaveChanges();
                return RedirectToAction("EnterApplications","Members");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RejectApplications(List<ApplicationsListViewModel> applications)
        {
            foreach (var member in applications)
            {//Если выбрали для одобрения
                if (member.Choosen)
                {
                    //Если ком. составом рассматривается, делаем "Одобрено ком. составом" = false
                    if (User.IsInRole("SquadManager"))
                    {
                        var memberInDb = _context.Members.Single(m => m.Id == member.Id);
                        memberInDb.ApprovedByCommandStaff = false;
                    }
                    //Если региональным отделением, то ставим дату исклбчения личности, без даты вступления с датой исключения будут считаться непринятые
                    //Они не будут рассматриваться нигде
                    else if (User.IsInRole("RegionalManager"))
                    {
                        var personInDb = _context.People.Single(m => m.Id == member.PersonId);
                        personInDb.DateOfExit = DateTime.Now;
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
            return RedirectToAction("EnterApplications","Members");
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
            foreach(var number in numbers)
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
        public void EnterDocument(PersonMainFormViewModel model) 
        {
            Person person = _context.People.SingleOrDefault(p => p.Id == model.Person.Id);
            string fileName = "C:/Users/Маргарита/source/repos/StudentSquadsGit2/StudentSquads/Files/Заявление_на_вступление_в_РСО.docx";
            var wordDocument = WordprocessingDocument.Open(fileName as string, false);
            string newfileName = "C:/Users/Маргарита/source/repos/StudentSquadsGit2/StudentSquads/Files/Заявление_на_вступление_в_РСО_"+person.LastName+".docx";
            wordDocument.Clone(newfileName, true).Close();
            var newwordDocument = WordprocessingDocument.Open(newfileName as string, true);
            var bookMarks = FindBookmarks(newwordDocument.MainDocumentPart.Document);
            foreach (var end in bookMarks)
            {
                string inn = "";
                if (end.Key == "FIO") 
                { inn = person.FIO; }
                var textElement = new Text(inn);
                var runElement = new Run(textElement);
                end.Value.InsertAfterSelf(runElement);
            }
            newwordDocument.MainDocumentPart.Document.Save();
            //Добавляем путь к файлу
            person.EnterDocumentPath = newfileName;
            _context.SaveChanges();
        }

    }
}