namespace Odintsov.Accounts.Web.Controllers
{
	using System;
	using System.Web.Mvc;

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
			ViewBag.CompetencyLists = ClWorkflow.GetProfCompetencyLists();
			return View();
		}
	}
}