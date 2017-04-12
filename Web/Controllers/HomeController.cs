namespace Web.Controllers
{
	using System;
	using System.IO;
	using System.Linq;
	using System.Web.Mvc;
	using System.Xml.Serialization;
	using Common.Enums;
	using Entities;
	using XmlEntities;

	[Authorize]
	public class HomeController : Controller
	{
		private readonly AccountsDbContext db = new AccountsDbContext();

		// GET: Home
		public ActionResult Index()
		{
			object error;
			TempData.TryGetValue("Error", out error);
			if (error != null)
			{
				ViewBag.Error = error.ToString();
			}
			ViewBag.CompetencyList = new CompetencyList(Server.MapPath("~/App_Data/CompetencyList.xml")).Competencies;
			Account currentAccount = ViewBag.CurrentAccount = db.Accounts.FirstOrDefault(a => a.Login == User.Identity.Name);

			ViewBag.Principal =
				db.Accounts.FirstOrDefault(a => a.Department == currentAccount.Department && a.Role == Role.DepCheef);

			return View(db.Accounts.FirstOrDefault(a => a.Login == User.Identity.Name));
		}

		[Authorize(Roles = "Admin")]
		public ActionResult CompetencyList()
		{
			ViewBag.CompetencyList = System.IO.File.ReadAllText(Server.MapPath("~/App_Data/CompetencyList.xml"));
			return View();
		}

		[Authorize(Roles = "Admin")]
		[HttpPost, ValidateInput(false), ActionName("CompetencyList")]
		public ActionResult CompetencyListPost()
		{
			string text = Request.Form["xml"];

			try
			{
				new XmlSerializer(typeof (CompetencyList)).Deserialize(new StringReader(text));
			}
			catch (Exception e)
			{
				ViewBag.Error = "Ошибка: " + e.Message;
				ViewBag.CompetencyList = text;
				return View();
			}

			System.IO.File.WriteAllText(Server.MapPath("~/App_Data/CompetencyList.xml"), text);
			return RedirectToAction("Index", "Home");
		}
	}
}