namespace Odintsov.Accounts.Web.Controllers
{
	using System;
	using System.Web.Mvc;
	using BusinessLogic;

	[Authorize]
	public class HomeController : BaseController
	{
		// GET: Home
		public ActionResult Index()
		{
			return RedirectToAction("Details", "Accounts");
		}

		[Authorize(Roles = "Admin")]
		public ActionResult CompetencyList()
		{
			ViewBag.CompetencyList = ClWorkflow.GetCompetencyListAsText();
			return View();
		}

		[Authorize(Roles = "Admin")]
		[HttpPost, ValidateInput(false), ActionName("CompetencyList")]
		public ActionResult CompetencyListPost()
		{
			string text = Request.Form["xml"];

			try
			{
				ClWorkflow.SetCompetencyListAsText(text);
			}
			catch (Exception e)
			{
				SetError("Ошибка: " + e.Message);
				ViewBag.CompetencyList = text;
				return View();
			}

			return RedirectToAction("Index", "Home");
		}

		[Authorize(Roles = "Admin")]
		public ActionResult ProfCompetencyLists()
		{
			ViewBag.CompetencyLists = CompetencyListWorkflow.GetProfCompetencyLists();
			return View();
		}

		[Authorize(Roles = "Admin")]
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult CreateProfCompetencyList(string name)
		{
			if (string.IsNullOrEmpty(name))
			{
				SetError("Имя новой компетенции не может быть пустым");
				return RedirectToAction("ProfCompetencyLists");
			}

			if (CompetencyListWorkflow.IsProfCompetencyExist(name))
			{
				SetError("Такая компетенция уже существует");
			}

			ClWorkflow.CreateNewProfCompetency(name);
			return RedirectToAction("ProfCompetencyList", new { name });
		}

		[Authorize(Roles = "Admin")]
		public ActionResult ProfCompetencyList(string name)
		{
			ViewBag.FunctionalArea = name;
			ViewBag.CompetencyList = ClWorkflow.GetProfCompetencyListAsText(name);
			return View();
		}

		[Authorize(Roles = "Admin")]
		[HttpPost, ValidateInput(false), ActionName("ProfCompetencyList")]
		public ActionResult ProfCompetencyListPost(string name)
		{
			string text = Request.Form["xml"];

			try
			{
				ClWorkflow.SetProfCompetencyListAsText(name, text);
			}
			catch (Exception e)
			{
				SetError("Ошибка: " + e.Message);
				ViewBag.CompetencyList = text;
				return View();
			}

			return RedirectToAction("Index", "Home");
		}
	}
}