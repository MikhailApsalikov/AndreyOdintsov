namespace Odintsov.Accounts.Web.Controllers
{
	using System;
	using System.IO;
	using System.Web.Mvc;
	using System.Xml.Serialization;
	using XmlEntities;

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
			ViewBag.CompetencyList =  ClWorkflow.GetDefaultAsText();
			return View();
		}

		[Authorize(Roles = "Admin")]
		[HttpPost, ValidateInput(false), ActionName("CompetencyList")]
		public ActionResult CompetencyListPost()
		{
			string text = Request.Form["xml"];

			try
			{
				ClWorkflow.SetDefaultAsText(text);
			}
			catch (Exception e)
			{
				ViewBag.Error = "Ошибка: " + e.Message;
				ViewBag.CompetencyList = text;
				return View();
			}
			
			return RedirectToAction("Index", "Home");
		}
	}
}