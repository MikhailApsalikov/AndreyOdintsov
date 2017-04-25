namespace Odintsov.Accounts.Web.Controllers
{
	using System.Collections.Generic;
	using System.Linq;
	using System.Web.Mvc;
	using BusinessLogic;
	using Entities;

	public abstract class BaseController : Controller
	{
		protected readonly CompetencyListWorkflow ClWorkflow;
		protected readonly AccountsDbContext Db = new AccountsDbContext();

		protected BaseController()
		{
			ClWorkflow = new CompetencyListWorkflow();
		}

		[NonAction]
		protected Account GetCurrentAccount()
		{
			return Db.Accounts.FirstOrDefault(a => a.Login == User.Identity.Name);
		}

		[NonAction]
		protected void PrepareIndicatorsForm()
		{
			ViewBag.IndicatorErrors = new List<string>();
			ViewBag.IndicatorValues = new Dictionary<string, string>();
		}

		[NonAction]
		protected void PrepareCompetencyList()
		{
			ViewBag.CompetencyList = ClWorkflow.GetCompetencyList();
		}

		[NonAction]
		protected void PrepareCompetencyList(string name)
		{
			ViewBag.CompetencyList = ClWorkflow.GetProfCompetencyList(name);
		}

		[NonAction]
		protected void SetError(string text)
		{
			TempData["Error"] = text;
		}

		[NonAction]
		protected void ValidateIndicatorsForm()
		{
			foreach (string indicatorKey in Request.Form.AllKeys.Where(i => i.StartsWith("indicator_")))
			{
				double val;
				bool parsed = double.TryParse(Request.Form[indicatorKey].Replace(".", ","), out val);
				ViewBag.IndicatorValues.Add(indicatorKey, Request.Form[indicatorKey]);

				if (!parsed || val < 1 || val > 3)
				{
					ViewBag.IndicatorErrors.Add(indicatorKey);
				}
			}
		}
	}
}