namespace Odintsov.Accounts.Web.Controllers
{
	using System;
	using System.Collections.Generic;
	using System.Data.Entity;
	using System.IO;
	using System.Linq;
	using System.Net;
	using System.Reflection;
	using System.Text;
	using System.Text.RegularExpressions;
	using System.Web;
	using System.Web.Mvc;
	using System.Web.Script.Serialization;
	using System.Web.Security;
	using BusinessLogic;
	using Common.Enums;
	using Common.Filters;
	using Entities;
	using Models;
	using PagedList;
	using XmlEntities;

	[Authorize]
	public class AccountsController : BaseController
	{
		[AllowAnonymous]
		public ActionResult LogOn()
		{
			return View();
		}

		[HttpPost]
		[AllowAnonymous]
		public ActionResult LogOn(LogOnModel model)
		{
			Account account = Db.Accounts.FirstOrDefault(x => x.Login.ToLower() == model.Login.ToLower());

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
			Account account = ViewBag.Account = Db.Accounts.Find(id);
			if (account == null)
			{
				return HttpNotFound();
			}
			if (account.Login != User.Identity.Name)
			{
				if (!User.IsInRole("Admin"))
				{
					return HttpNotFound();
				}
			}

			return View();
		}

		[HttpPost, ActionName("ResetPassword")]
		[ValidateAntiForgeryToken]
		public ActionResult ResetPasswordPost(int id, ResetPassword resetPassword)
		{
			Account account = ViewBag.Account = Db.Accounts.Find(id);
			if (account == null)
			{
				return HttpNotFound();
			}
			if (account.Login != User.Identity.Name)
			{
				if (!User.IsInRole("Admin"))
				{
					return HttpNotFound();
				}
			}

			if (ModelState.IsValid)
			{
				account.SetPassword(resetPassword.Password);
				Db.SaveChanges();
				if (account.Login == User.Identity.Name)
				{
					return RedirectToAction("Index", "Home");
				}
				return RedirectToAction("Details", new {id = account.Id});
			}

			return View();
		}

		public ActionResult LogOff()
		{
			ViewBag.CurrentAccount = GetCurrentAccount();
			FormsAuthentication.SignOut();
			return RedirectToAction("LogOn", "Accounts");
		}

		// GET: Accounts
		public ActionResult Index(string sortOrder = "+FullName", string filter = "", int page = 1, int pageSize = 20)
		{
			Account currentAccount = ViewBag.CurrentAccount = GetCurrentAccount();

			IQueryable<Account> accounts = from a in Db.Accounts select a;

			if (currentAccount.Team.Count > 0)
			{
				IEnumerable<int> teamIds = currentAccount.Team.Select(m => m.Id);
				accounts = accounts.Where(a => teamIds.Contains(a.Id));
			}

			if (currentAccount.Role == Role.FunctionalManager)
			{
				if (currentAccount.Team.Count > 0)
				{
					accounts = accounts.Union(Db.Accounts
						.Where(a => a.Department == currentAccount.Department && a.Login != User.Identity.Name));
				}
				else
				{
					accounts = accounts.Where(a => a.Department == currentAccount.Department);
					accounts = accounts.Where(a => a.Login != User.Identity.Name);
				}
			}

			if (currentAccount.Role == Role.AdministrativeManager)
			{
				accounts = accounts.Where(a => a.AdministrativeManager.Id == currentAccount.Id);
			}

			// =======================     Сортировки      =============================

			ViewBag.CodeSortParm = sortOrder != "+Code" ? "+Code" : "-Code";
			ViewBag.MicroRegionSortParm = sortOrder != "+MicroRegion" ? "+MicroRegion" : "-MicroRegion";
			ViewBag.RegionSortParm = sortOrder != "+Region" ? "+Region" : "-Region";
			ViewBag.FullNameSortParm = sortOrder != "+FullName" ? "+FullName" : "-FullName";
			ViewBag.SexSortParm = sortOrder != "+Sex" ? "+Sex" : "-Sex";
			ViewBag.PositionSortParm = sortOrder != "+Position" ? "+Position" : "-Position";
			ViewBag.DepartmentSortParm = sortOrder != "+Department" ? "+Department" : "-Department";
			ViewBag.LoginSortParm = sortOrder != "+Login" ? "+Login" : "-Login";
			ViewBag.RoleSortParm = sortOrder != "+Role" ? "+Role" : "-Role";
			ViewBag.ManagerSortParm = sortOrder != "+Manager" ? "+Manager" : "-Manager";
			ViewBag.AdministrativeManagerSortParm = sortOrder != "+AdministrativeManager" ? "+AdministrativeManager" : "-AdministrativeManager";
			ViewBag.LastEvaluationPercentSortParm = sortOrder != "+LastEvaluationPercent"
				? "+LastEvaluationPercent"
				: "-LastEvaluationPercent";
			ViewBag.LastProfEvaluationPercentSortParm = sortOrder != "+LastProfEvaluationPercent"
				? "+LastProfEvaluationPercent"
				: "-LastProfEvaluationPercent";
			ViewBag.CurrentSort = sortOrder;

			PropertyInfo propInfo = typeof (Account).GetProperty(sortOrder.Substring(1));
			if (propInfo == null)
			{
				return View(accounts.ToPagedList(page, pageSize));
			}
			bool ascending = sortOrder[0] == '+';

			switch (propInfo.Name)
			{
				case "Code":
					accounts = @ascending
						? accounts.OrderBy(s => string.IsNullOrEmpty(s.Code)).ThenBy(s => s.Code)
						: accounts.OrderByDescending(s => s.Code);
					break;
				case "MicroRegion":
					accounts = @ascending
						? accounts.OrderBy(s => string.IsNullOrEmpty(s.MicroRegion)).ThenBy(s => s.MicroRegion)
						: accounts.OrderByDescending(s => s.MicroRegion);
					break;
				case "Region":
					accounts = @ascending
						? accounts.OrderBy(s => string.IsNullOrEmpty(s.Region)).ThenBy(s => s.Region)
						: accounts.OrderByDescending(s => s.Region);
					break;
				case "Sex":
					accounts = @ascending ? accounts.OrderBy(s => s.Sex) : accounts.OrderByDescending(s => s.Sex);
					break;
				case "Position":
					accounts = @ascending
						? accounts.OrderBy(s => string.IsNullOrEmpty(s.Department)).ThenBy(s => s.Position)
						: accounts.OrderByDescending(s => s.Position);
					break;
				case "Department":
					accounts = @ascending
						? accounts.OrderBy(s => string.IsNullOrEmpty(s.Department)).ThenBy(s => s.Department)
						: accounts.OrderByDescending(s => s.Department);
					break;
				case "Login":
					accounts = @ascending ? accounts.OrderBy(s => s.Login) : accounts.OrderByDescending(s => s.Login);
					break;
				case "Role":
					accounts = @ascending ? accounts.OrderBy(s => s.Role) : accounts.OrderByDescending(s => s.Role);
					break;
				case "FullName":
				default:
					accounts = @ascending
						? accounts.OrderBy(s => string.IsNullOrEmpty(s.FullName)).ThenBy(s => s.FullName)
						: accounts.OrderByDescending(s => s.FullName);
					break;
				case "Manager":
					accounts = @ascending
						? accounts.OrderBy(s => s.Manager == null).ThenBy(s => s.Manager.FullName)
						: accounts.OrderByDescending(s => s.Manager.FullName);
					break;
				case "AdministrativeManager":
					accounts = @ascending
						? accounts.OrderBy(s => s.AdministrativeManager == null).ThenBy(s => s.AdministrativeManager.FullName)
						: accounts.OrderByDescending(s => s.AdministrativeManager.FullName);
					break;
				case "LastEvaluationPercent":
					accounts = @ascending
						? accounts.OrderBy(s => s.LastEvaluationPercent == null).ThenBy(s => s.LastEvaluationPercent)
						: accounts.OrderByDescending(s => s.LastEvaluationPercent);
					break;
				case "LastProfEvaluationPercent":
					accounts = @ascending
						? accounts.OrderBy(s => s.LastProfEvaluationPercent == null).ThenBy(s => s.LastProfEvaluationPercent)
						: accounts.OrderByDescending(s => s.LastProfEvaluationPercent);
					break;
			}


			// =======================     Фильтры      =============================

			if (!string.IsNullOrEmpty(filter))
			{
				var js = new JavaScriptSerializer();
				AccountsFilter filters = ViewBag.Filter = js.Deserialize<AccountsFilter>(HttpUtility.UrlDecode(filter));

				if (!string.IsNullOrEmpty(filters.Region))
				{
					accounts = accounts.Where(a => a.Region == filters.Region);
				}
				if (!string.IsNullOrEmpty(filters.MicroRegion))
				{
					accounts = accounts.Where(a => a.MicroRegion == filters.MicroRegion);
				}
				if (!string.IsNullOrEmpty(filters.Department))
				{
					accounts = accounts.Where(a => a.Department == filters.Department);
				}
				if (!string.IsNullOrEmpty(filters.Position))
				{
					accounts = accounts.Where(a => a.Position == filters.Position);
				}
			}
			else
			{
				ViewBag.Filter = new AccountsFilter();
			}


			// =======================     Данные для фильтров      =============================

			ViewBag.Regions = Db.Accounts.GroupBy(a => a.Region)
				.Select(grp => grp.FirstOrDefault().Region).Where(r => !string.IsNullOrEmpty(r)).ToList();
			ViewBag.MicroRegions = Db.Accounts.GroupBy(a => a.MicroRegion)
				.Select(grp => grp.FirstOrDefault().MicroRegion).Where(m => !string.IsNullOrEmpty(m)).ToList();
			ViewBag.Departments = Db.Accounts.GroupBy(a => a.Department)
				.Select(grp => grp.FirstOrDefault().Department).Where(d => !string.IsNullOrEmpty(d)).ToList();
			ViewBag.Positions = Db.Accounts.GroupBy(a => a.Position)
				.Select(grp => grp.FirstOrDefault().Position).Where(p => !string.IsNullOrEmpty(p)).ToList();


			// =======================     График      =============================
			ViewBag.Percents = accounts.Select(a => a.LastEvaluationPercent).Where(p => p.HasValue).ToList();
			ViewBag.ProfPercents = accounts.Select(a => a.LastProfEvaluationPercent).Where(p => p.HasValue).ToList();

			return View(accounts.ToPagedList(page, pageSize));
		}

		public ActionResult Details(int? id)
		{
			ViewBag.CurrentAccount = GetCurrentAccount();
			Account account = id.HasValue ? Db.Accounts.Find(id) : ViewBag.CurrentAccount;
			if (account == null)
			{
				return HttpNotFound();
			}
			if (!id.HasValue && account.Role == Role.Employee && !EvaluationWorkflow.CanPass(account.Login))
			{
				SetError("Внимание! Вы не можете проходить оценку компетенций, потому что для вас в системе не указано ни функционального, ни административного руководителя!");
			}
			ViewBag.CompetencyList = ClWorkflow.GetCompetencyList().Competencies;
			ViewBag.ProfCompetencyList = CompetencyListWorkflow.IsProfCompetencyExist(account.FunctionalArea) ? ClWorkflow.GetProfCompetencyList(account.FunctionalArea).Competencies : null;
			return View(account);
		}

		// GET: Accounts/Create
		[Authorize(Roles = "Admin")]
		public ActionResult Create()
		{
			ViewBag.Accounts = new SelectList(Db.Accounts.OrderBy(a => a.FullName), "Id", "FullName");
			return View();
		}

		// POST: Accounts/Create
		// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
		// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize(Roles = "Admin,FunctionalManager")]
		public ActionResult Create(
			[Bind(Include = "Id,Code,Region,MicroRegion,FullName,Sex,Role,Department,Position,Login,Active,ManagerId,AdministrativeManagerId,FunctionalArea")] Account
				account)
		{
			Account dbAccount = Db.Accounts.FirstOrDefault(a => a.Login == account.Login);
			Account currentAccount = GetCurrentAccount();
			ViewBag.Accounts = new SelectList(Db.Accounts.OrderBy(a => a.FullName), "Id", "FullName", Request.Form["ManagerId"]);

			if (dbAccount != null)
			{
				ModelState.AddModelError("Login", "Пользователь с таким логином уже существует");
				return View(account);
			}

			if (ModelState.IsValid)
			{
				account.SetPassword("123456Qq");
				Db.Accounts.Add(account);
				Db.SaveChanges();
				return RedirectToAction("Index");
			}


			// Можно редактировать админу, и нач. своего отдела.
			if (currentAccount.Role == Role.Admin ||
			    (currentAccount.Role == Role.FunctionalManager && account.Department == currentAccount.Department &&
			     account != currentAccount))
			{
				return View(account);
			}
			return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
		}

		// GET: Accounts/Edit/5
		[Authorize(Roles = "Admin,AdministrativeManager,FunctionalManager,DirectManager")]
		public ActionResult Edit(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			Account account = Db.Accounts.Find(id);
			if (account == null)
			{
				return HttpNotFound();
			}
			Account currentAccount = GetCurrentAccount();

			ViewBag.Accounts = new SelectList(Db.Accounts.OrderBy(a => a.FullName), "Id", "FullName", account.ManagerId);

			// Можно редактировать админу, и нач. своего отдела.
			if (currentAccount.Role == Role.Admin ||
			    (currentAccount.Role == Role.FunctionalManager && account.Department == currentAccount.Department &&
			     account != currentAccount))
			{
				return View(account);
			}
			return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
		}

		// POST: Accounts/Edit/5
		// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
		// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize(Roles = "Admin,AdministrativeManager,FunctionalManager,DirectManager")]
		public ActionResult Edit(
			[Bind(Include = "Id,Code,Region,MicroRegion,FullName,Sex,Role,Department,Position,Login,Active,ManagerId,AdministrativeManagerId,FunctionalArea")] Account
				account)
		{
			if (ModelState.IsValid)
			{
				Db.Entry(account).State = EntityState.Modified;
				Db.Entry(account).Property(uco => uco.Password).IsModified = false;
				Db.Entry(account).Property(uco => uco.Salt).IsModified = false;
				Db.Entry(account).Property(uco => uco.Guid).IsModified = false;
				Db.Entry(account).Property(uco => uco.LastEvaluationPercent).IsModified = false;
				Db.Entry(account).Property(uco => uco.LastProfEvaluationPercent).IsModified = false;
				Db.SaveChanges();
				return RedirectToAction("Index");
			}
			ViewBag.Accounts = new SelectList(Db.Accounts.OrderBy(a => a.FullName), "Id", "FullName", account.ManagerId);
			return View(account);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				Db.Dispose();
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
			HttpPostedFileBase rawFile = Request.Files.Count > 0 ? Request.Files[0] : null;

			if (rawFile == null)
			{
				SetError("Не выбран файл");
				return View();
			}

			if (rawFile.ContentType != "application/vnd.ms-excel" &&
			    rawFile.ContentType != "application/csv" &&
			    rawFile.ContentType != "text/csv")
			{
				SetError("Пожалуйста, выберете файл CSV");
				return View();
			}

			Encoding windows1251 = Encoding.GetEncoding(1251), utf8 = Encoding.UTF8;
			List<Tuple<Account, string, string>> accounts = new List<Tuple<Account, string, string>>();

			using (var csvReader = new StreamReader(rawFile.InputStream, windows1251, true))
			{
				string inputLine;
				/* 
				 * 0 - № п/п;
				 * 1 - МР
				 * 2 - Город
				 * 3 - ФИО
				 * 4 - Пол
				 * 5 - Должность/категория
				 * 6 - Отдел
				 * 7 - Административный руководитель
				 * 8 - Функциональный руководитель;
				 * 9 - Учетная запись;
				 * 10 - Функциональное направление
				*/

				csvReader.ReadLine().Split(';');

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
					if (values.Length < 4 || values[3].ToLower() == "вакансия")
					{
						continue;
					}

					// Без логина не импортируем
					if (values.Length < 10)
					{
						continue;
					}

					// Корявые и пустые логины тоже не нужны
					if (!Regex.IsMatch(values[9], @"^[a-zA-Z0-9]+$"))
					{
						continue;
					}
					string login = values[9];

					Account account = null;
					var creation = false;

					// Ищем по логину
					account = Db.Accounts.FirstOrDefault(a => a.Login == login);

					// Если не нашли - создаем
					if (account == null)
					{
						creation = true;
						account = new Account
						{
							Active = true,
							Role = Role.Employee,
							Guid = Guid.NewGuid().ToString()
						};
					}

					if (values.Length > 0)
					{
						account.Code = values[0];
					}
					if (values.Length > 1)
					{
						account.MicroRegion = values[1];
					}
					if (values.Length > 2)
					{
						account.Region = values[2];
					}
					if (values.Length > 3)
					{
						account.FullName = values[3];
					}
					if (values.Length > 4)
					{
						account.Sex = values[4];
					}
					if (values.Length > 5)
					{
						account.Position = values[5];
					}
					if (values.Length > 6)
					{
						account.Department = values[6];
					}
					if (values.Length > 8)
					{
						accounts.Add(new Tuple<Account, string, string>(account, values[7], values[8]));
					}
					if (values.Length > 9)
					{
						account.Login = values[9];
					}
					if (values.Length > 10)
					{
						account.FunctionalArea = values[10];
					}

					// TODO: Заполнение ролей общего начальника, главного по ФН
					if ((account.Position.StartsWith("Начальник отдела") ||
					     account.Position.StartsWith("Нач. отдела") || account.Position.StartsWith("Нач.отдела"))
					    && account.Region.StartsWith("Москва"))
					{
						account.Role = Role.FunctionalManager;
					}

					if (creation)
					{
						account.SetPassword("123456Qq");
						Db.Accounts.Add(account);
					}
					
				}

				Db.SaveChanges();
			}

			// После импорта обновляем руководителей
			foreach (var acc in accounts)
			{
				if (!string.IsNullOrEmpty(acc.Item2))
				{
					acc.Item1.AdministrativeManager = Db.Accounts.FirstOrDefault(a => a.FullName == acc.Item2);
					if (acc.Item1.AdministrativeManager != null)
					{
						acc.Item1.AdministrativeManager.Role = Role.AdministrativeManager;
					}
				}

				if (!string.IsNullOrEmpty(acc.Item3))
				{
					acc.Item1.Manager = Db.Accounts.FirstOrDefault(a => a.FullName == acc.Item3);
					if (acc.Item1.Manager != null)
					{
						acc.Item1.Manager.Role = Role.AdministrativeManager;
					}
				}
			}

			Db.SaveChanges();
			return RedirectToAction("Index");
		}


		
	}
}