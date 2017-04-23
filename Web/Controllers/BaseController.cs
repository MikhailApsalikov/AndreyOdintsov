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
			ViewBag.CompetencyList = ClWorkflow.GetDefault();
		}
	}
}