﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Odintsov.Accounts.Web.Models;
using System.IO;
using System.Text;
using PagedList;

using System.Web.Security;
using System.Security.Permissions;
using Newtonsoft.Json;
using System.Web.Script.Serialization;
using System.Text.RegularExpressions;

namespace Odintsov.Accounts.Web.Controllers
{
	using global::Models;
	using XmlEntities;

	[Authorize]
    public class AccountsController : Controller
    {
        private AccountsContainer db = new AccountsContainer();

        [AllowAnonymous]
        public ActionResult LogOn()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult LogOn(LogOnModel model)
        {
            Account account = db.Accounts.FirstOrDefault(x => x.Login.ToLower() == model.Login.ToLower());
			
            if (account == null)
            {
				ModelState.AddModelError("", "Логин не найден.");
				return View(model);
            }
            
            if (!account.Active)
            {
				ModelState.AddModelError("", "Аккаунт не активен.");
				return View(model);
            }

            if (account.GetPasswordHash(model.Password) != account.Password)
            {
				ModelState.AddModelError("", "Неправильный пароль.");
				return View(model);
            }
			
            FormsAuthentication.RedirectFromLoginPage(account.Login, true);
            return RedirectToAction("Index", "Home");
        }

        public ActionResult ResetPassword(int id)
        {
            Account account = ViewBag.Account = db.Accounts.Find(id);
            if (account == null) return HttpNotFound();
            if (account.Login != User.Identity.Name) if (!User.IsInRole("Admin")) return HttpNotFound();

            return View();
        }

        [HttpPost, ActionName("ResetPassword")]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPasswordPost(int id, ResetPassword resetPassword)
        {
            Account account = ViewBag.Account = db.Accounts.Find(id);
            if (account == null) return HttpNotFound();
            if (account.Login != User.Identity.Name) if (!User.IsInRole("Admin")) return HttpNotFound();

            if (ModelState.IsValid)
            {
                account.SetPassword(resetPassword.Password);
                db.SaveChanges();
                if (account.Login == User.Identity.Name)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    return RedirectToAction("Details", new { id = account.Id });
                }
            }
            
            return View();
        }

        public ActionResult LogOff()
        {
            Account currentAccount = ViewBag.CurrentAccount = db.Accounts.FirstOrDefault(a => a.Login == User.Identity.Name);
            
            FormsAuthentication.SignOut();
            return RedirectToAction("LogOn", "Accounts");
        }
               
        // GET: Accounts
        public ActionResult Index(string sortOrder = "+FullName", string filter = "", int page = 1, int pageSize = 20)
        {
            Account currentAccount = ViewBag.CurrentAccount = db.Accounts.FirstOrDefault(a => a.Login == User.Identity.Name);

            var accounts = from a in db.Accounts select a;

            if (currentAccount.Team.Count > 0)
            {
                var teamIds = currentAccount.Team.Select(m => m.Id);
                accounts = accounts.Where(a => teamIds.Contains(a.Id));
            }

            if (currentAccount.Role == Role.DepCheef)
            {
                if (currentAccount.Team.Count > 0)
                {
                    accounts = accounts.Union(db.Accounts
                        .Where(a => a.Department == currentAccount.Department && a.Login != User.Identity.Name));
                }
                else
                {
                    accounts = accounts.Where(a => a.Department == currentAccount.Department);
                    accounts = accounts.Where(a => a.Login != User.Identity.Name);
                }
            }

            // =======================     Сортировки      =============================

            ViewBag.CodeSortParm        = (sortOrder != "+Code") ?          "+Code" :           "-Code";
            ViewBag.MicroRegionSortParm = (sortOrder != "+MicroRegion") ?   "+MicroRegion" :    "-MicroRegion";
            ViewBag.RegionSortParm      = (sortOrder != "+Region") ?        "+Region" :         "-Region";
            ViewBag.FullNameSortParm    = (sortOrder != "+FullName") ?      "+FullName" :       "-FullName";
            ViewBag.SexSortParm         = (sortOrder != "+Sex") ?           "+Sex" :            "-Sex";
            ViewBag.PositionSortParm    = (sortOrder != "+Position") ?      "+Position" :       "-Position";
            ViewBag.DepartmentSortParm  = (sortOrder != "+Department") ?    "+Department" :     "-Department";
            ViewBag.LoginSortParm       = (sortOrder != "+Login") ?         "+Login" :          "-Login";
            ViewBag.RoleSortParm        = (sortOrder != "+Role") ?          "+Role" :           "-Role";
            ViewBag.ManagerSortParm     = (sortOrder != "+Manager") ?       "+Manager" :        "-Manager";
            ViewBag.LastEvaluationPercentSortParm = (sortOrder != "+LastEvaluationPercent") ? "+LastEvaluationPercent" : "-LastEvaluationPercent";
			ViewBag.CurrentSort = sortOrder;

            var propInfo = typeof(Account).GetProperty(sortOrder.Substring(1));
            if (propInfo == null) return View(accounts.ToPagedList(page, pageSize));
            bool ascending = sortOrder[0] == '+';

            switch (propInfo.Name)
            {
                case "Code":
                    accounts = (ascending) ? accounts.OrderBy(s => string.IsNullOrEmpty(s.Code)).ThenBy(s => s.Code) : accounts.OrderByDescending(s => s.Code);
                    break;
                case "MicroRegion":
                    accounts = (ascending) ? accounts.OrderBy(s => string.IsNullOrEmpty(s.MicroRegion)).ThenBy(s => s.MicroRegion) : accounts.OrderByDescending(s => s.MicroRegion);
                    break;
                case "Region":
                    accounts = (ascending) ? accounts.OrderBy(s => string.IsNullOrEmpty(s.Region)).ThenBy(s => s.Region) : accounts.OrderByDescending(s => s.Region);
                    break;
                case "Sex":
                    accounts = (ascending) ? accounts.OrderBy(s => s.Sex) : accounts.OrderByDescending(s => s.Sex);
                    break;
                case "Position":
                    accounts = (ascending) ? accounts.OrderBy(s => string.IsNullOrEmpty(s.Department)).ThenBy(s => s.Position) : accounts.OrderByDescending(s => s.Position);
                    break;
                case "Department":
                    accounts = (ascending) ? accounts.OrderBy(s => string.IsNullOrEmpty(s.Department)).ThenBy(s => s.Department) : accounts.OrderByDescending(s => s.Department);
                    break;
                case "Login":
                    accounts = (ascending) ? accounts.OrderBy(s => s.Login) : accounts.OrderByDescending(s => s.Login);
                    break;
                case "Role":
                    accounts = (ascending) ? accounts.OrderBy(s => s.Role) : accounts.OrderByDescending(s => s.Role);
                    break;
                case "FullName":
                default:
                    accounts = (ascending) ? accounts.OrderBy(s => string.IsNullOrEmpty(s.FullName)).ThenBy(s => s.FullName) : accounts.OrderByDescending(s => s.FullName);
                    break;
				case "Manager":
					accounts = (ascending) ? accounts.OrderBy(s => s.Manager == null).ThenBy(s => s.Manager.FullName) : accounts.OrderByDescending(s => s.Manager.FullName);
					break;
				case "LastEvaluationPercent":
					accounts = (ascending) ? accounts.OrderBy(s => s.LastEvaluationPercent == null).ThenBy(s => s.LastEvaluationPercent) : accounts.OrderByDescending(s => s.LastEvaluationPercent);
					break;
			}


            // =======================     Фильтры      =============================

            if (!string.IsNullOrEmpty(filter))
            {
                JavaScriptSerializer js = new JavaScriptSerializer();
                AccountsFilter filters = ViewBag.Filter = js.Deserialize<AccountsFilter>(HttpUtility.UrlDecode(filter));

                if (!string.IsNullOrEmpty(filters.Region)) accounts = accounts.Where(a => a.Region == filters.Region);
                if (!string.IsNullOrEmpty(filters.MicroRegion)) accounts = accounts.Where(a => a.MicroRegion == filters.MicroRegion);
                if (!string.IsNullOrEmpty(filters.Department)) accounts = accounts.Where(a => a.Department == filters.Department);
                if (!string.IsNullOrEmpty(filters.Position)) accounts = accounts.Where(a => a.Position == filters.Position);
            }
            else
            {
                ViewBag.Filter = new AccountsFilter();
            }


            // =======================     Данные для фильтров      =============================

            ViewBag.Regions = db.Accounts.GroupBy(a => a.Region)
                .Select(grp => grp.FirstOrDefault().Region).Where(r => !string.IsNullOrEmpty(r)).ToList();
            ViewBag.MicroRegions = db.Accounts.GroupBy(a => a.MicroRegion)
                .Select(grp => grp.FirstOrDefault().MicroRegion).Where(m => !string.IsNullOrEmpty(m)).ToList();
            ViewBag.Departments = db.Accounts.GroupBy(a => a.Department)
                .Select(grp => grp.FirstOrDefault().Department).Where(d => !string.IsNullOrEmpty(d)).ToList();
            ViewBag.Positions = db.Accounts.GroupBy(a => a.Position)
                .Select(grp => grp.FirstOrDefault().Position).Where(p => !string.IsNullOrEmpty(p)).ToList();


            // =======================     График      =============================
            ViewBag.Percents = accounts.Select(a => a.LastEvaluationPercent).Where(p => p.HasValue).ToList();

            return View(accounts.ToPagedList(page, pageSize));
        }

        // GET: Accounts/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Account account = db.Accounts.Find(id);
            if (account == null)
            {
                return HttpNotFound();
            }
            ViewBag.CompetencyList = new CompetencyList(Server.MapPath("~/App_Data/CompetencyList.xml")).Competencies;
			ViewBag.CurrentAccount = db.Accounts.FirstOrDefault(a => a.Login == User.Identity.Name);
            account.Principal = db.Accounts.FirstOrDefault(a => a.Department == account.Department && a.Role == Role.DepCheef);

            return View(account);
        }

        // GET: Accounts/Create
        [Authorize(Roles = "Admin")]
        public ActionResult Create()
		{
			ViewBag.Accounts = new SelectList(db.Accounts.OrderBy(a => a.FullName), "Id", "FullName");
			return View();
        }

        // POST: Accounts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,DepCheef")]
        public ActionResult Create([Bind(Include = "Id,Code,Region,MicroRegion,FullName,Sex,Department,Position,Login,Active,ManagerId")] Account account)
        {
            var dbAccount = db.Accounts.FirstOrDefault(a => a.Login == account.Login);
            var currentAccount = db.Accounts.FirstOrDefault(a => a.Login == User.Identity.Name);
			ViewBag.Accounts = new SelectList(db.Accounts.OrderBy(a => a.FullName), "Id", "FullName", Request.Form["ManagerId"]);

			if (dbAccount != null)
            {
                ModelState.AddModelError("Login", "Пользователь с таким логином уже существует");
                return View(account);
            }

            if (ModelState.IsValid)
            {
                account.SetPassword("123456Qq");
                db.Accounts.Add(account);
                db.SaveChanges();
                return RedirectToAction("Index");
            }


            // Можно редактировать админу, и нач. своего отдела.
            if (currentAccount.Role == Role.Admin ||
                (currentAccount.Role == Role.DepCheef && account.Department == currentAccount.Department && account != currentAccount))
            {
                return View(account);
            }
            else
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }
        }

        // GET: Accounts/Edit/5
        [Authorize(Roles = "Admin,Cheef,DepCheef,FuncManager")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Account account = db.Accounts.Find(id);
            if (account == null)
            {
                return HttpNotFound();
            }
            Account currentAccount = db.Accounts.FirstOrDefault(a => a.Login == User.Identity.Name);
			
			ViewBag.Accounts = new SelectList(db.Accounts.OrderBy(a => a.FullName), "Id", "FullName", account.ManagerId);

			// Можно редактировать админу, и нач. своего отдела.
			if (currentAccount.Role == Role.Admin ||
                (currentAccount.Role == Role.DepCheef && account.Department == currentAccount.Department && account != currentAccount))
            {
                return View(account);
            }
            else
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }
        }

        // POST: Accounts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
		[Authorize(Roles = "Admin,Cheef,DepCheef,FuncManager")]
		public ActionResult Edit([Bind(Include = "Id,Code,Region,MicroRegion,FullName,Sex,Role,Department,Position,Login,Active,ManagerId")] Account account)
		{
			if (ModelState.IsValid)
            {
                db.Entry(account).State = EntityState.Modified;
                db.Entry(account).Property(uco => uco.Password).IsModified = false;
                db.Entry(account).Property(uco => uco.Salt).IsModified = false;
                db.Entry(account).Property(uco => uco.Guid).IsModified = false;
                db.Entry(account).Property(uco => uco.ManagerFullName).IsModified = false;
                db.Entry(account).Property(uco => uco.LastEvaluationPercent).IsModified = false;
                db.SaveChanges();
                return RedirectToAction("Index");
			}
			ViewBag.Accounts = new SelectList(db.Accounts.OrderBy(a => a.FullName), "Id", "FullName", account.ManagerId);
			return View(account);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        // GET: Accounts/Import
        [Authorize(Roles = "Admin")]
        public ActionResult Import()
		{
			return View();
		}

        // POST: Accounts/Import
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Import")]
		public ActionResult ImportPost()
		{
			var rawFile = Request.Files.Count > 0 ? Request.Files[0] : null;

			if (rawFile == null)
			{
				ViewBag.Error = "Не выбран файл";
				return View();
			}

			if (rawFile.ContentType != "application/vnd.ms-excel" &&
                rawFile.ContentType != "application/csv" &&
                rawFile.ContentType != "text/csv")
            {
				ViewBag.Error = "Пожалуйста, выберете файл CSV";
                return View();
            }

            Encoding windows1251 = Encoding.GetEncoding(1251), utf8 = Encoding.UTF8;

            using (StreamReader csvReader = new StreamReader(rawFile.InputStream, windows1251, true))
            {
                string inputLine = "";
                // 0    ;1 ;2     ;3  ;4  ;5        ;6 ;7   ;8         
                // № п/п;МР;Город;ФИО;Пол;Должность;ФН;Функц.руководитель;Логин
                string[] headers = csvReader.ReadLine().Split(';');

                while ((inputLine = csvReader.ReadLine()) != null)
                {
                    string utf8Line = utf8.GetString(Encoding.Convert(windows1251, utf8, windows1251.GetBytes(inputLine)));
                    utf8Line.Replace('ё', 'е');
                    utf8Line.Replace('Ё', 'Е');

                    string[] values = (
                        from v in utf8Line.Trim().Split(';')
                        where v.Trim().Trim('"').Trim() != "#Н/Д"
                        select v.Trim().Trim('"').Trim()
                    ).ToArray();

                    // Забиваем на вакансии
                    if (values.Length < 4 || values[3].ToLower() == "вакансия") continue;

                    // Без логина не импортируем
                    if (values.Length < 9) continue;

                    // Корявые и пустые логины тоже не нужны
                    if (!Regex.IsMatch(values[8], @"^[a-zA-Z0-9]+$")) continue;
                    var login = values[8];

                    Account account = null;
                    bool creation = false;
                    
                    // Ищем по логину
                    account = db.Accounts.FirstOrDefault(a => a.Login == login);

                    // Если не нашли - создаем
                    if (account == null)
                    {
                        creation = true;
                        account = new Account();
                        account.Active = true;
                        account.Role = Role.Employee;
                        account.Guid = Guid.NewGuid().ToString();
                    }

                    if (values.Length > 0) account.Code             = values[0];
                    if (values.Length > 1) account.MicroRegion      = values[1];
                    if (values.Length > 2) account.Region           = values[2];
                    if (values.Length > 3) account.FullName         = values[3];
                    if (values.Length > 4) account.Sex              = values[4];
                    if (values.Length > 5) account.Position         = values[5];
                    if (values.Length > 6) account.Department       = values[6];
                    if (values.Length > 7) account.ManagerFullName  = values[7];
                    if (values.Length > 8) account.Login            = values[8];

                    // TODO: Заполнение ролей общего начальника, главного по ФН
                    if ((account.Position.StartsWith("Начальник отдела") ||
                    account.Position.StartsWith("Нач. отдела") || account.Position.StartsWith("Нач.отдела")) 
                    && account.Region.StartsWith("Москва"))
                    {
                        account.Role = Role.DepCheef;
                    }
                    
                    if (creation)
                    {
                        account.SetPassword("123456Qq");
                        db.Accounts.Add(account);
                    }
                }

                db.SaveChanges();
            }

            // После импорта обновляем функц. руководителя
            foreach (var account in db.Accounts)
            {
                account.Manager = db.Accounts.FirstOrDefault(a => a.FullName == account.ManagerFullName);
                if (account.Manager != null) account.Manager.Role = Role.FuncManager;
            }
            /*
             // После импорта обновляем админ. руководителя
             foreach (var account in db.Accounts)
             {
                 account.Manager = db.Accounts.FirstOrDefault(a => a.FullName == account.ManagerFullName);
                 if (account.Manager != null) account.Manager.Role = Role.DepCheef;
             }*/

            db.SaveChanges();
            return RedirectToAction("Index");
		}
	}
}