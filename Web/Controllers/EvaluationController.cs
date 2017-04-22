namespace Odintsov.Accounts.Web.Controllers
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Web.Mvc;
	using BusinessLogic;
	using Common.Enums;
	using Entities;
	using XmlEntities;

	[Authorize]
	public class EvaluationController : Controller
	{
		private readonly AccountsDbContext db = new AccountsDbContext();

		// GET: Evaluation/Details/5

		public ActionResult Details(int id)
		{
			PrepareComtetencyList();
			Account currentAccount = ViewBag.CurrentAccount = db.Accounts.FirstOrDefault(a => a.Login == User.Identity.Name);
			Evaluation evaluation = ViewBag.Evaluation = db.Evaluations.Find(id);

			return View();
		}

		// GET: Evaluation/Pass
		public ActionResult Pass()
		{
			Account currentAccount = ViewBag.CurrentAccount = db.Accounts.FirstOrDefault(a => a.Login == User.Identity.Name);
			Evaluation lastEvaluation = currentAccount.Evaluations.OrderByDescending(e => e.Passed).FirstOrDefault();

			// Нельзя проходить чаще чем раз в день (на всякий случай).
			if (lastEvaluation != null && lastEvaluation.Passed.Date == DateTime.Today)
			{
				TempData["Error"] = "Нельзя проходить самостоятельную оценку больше раза в день.";
				return RedirectToAction("Index", "Home");
			}

			PrepareComtetencyList();
			PrepareIndicatorsForm();

			return View();
		}

		[HttpPost, ActionName("Pass")]
		[ValidateAntiForgeryToken]
		public ActionResult PassPost()
		{
			PrepareComtetencyList();
			PrepareIndicatorsForm();
			ValidateIndicatorsForm();
			Account currentAccount = ViewBag.CurrentAccount = db.Accounts.FirstOrDefault(a => a.Login == User.Identity.Name);

			if (ViewBag.IndicatorErrors.Count > 0)
			{
				return View();
			}

			var evaluation = new Evaluation();
			evaluation.Examinee = currentAccount;
			evaluation.Passed = DateTime.Now;

			foreach (dynamic kv in ViewBag.IndicatorValues)
			{
				var evaluationValue = new EvaluationValue();
				evaluationValue.Evaluation = evaluation;
				evaluationValue.Competency = int.Parse(kv.Key.Split('_')[1]);
				evaluationValue.Indicator = int.Parse(kv.Key.Split('_')[2]);
				evaluationValue.Value = double.Parse(kv.Value.Replace(".", ","));

				evaluation.EvaluationValues.Add(evaluationValue);
			}

			evaluation.IndicatorsCount = evaluation.EvaluationValues.Count;
			currentAccount.Evaluations.Add(evaluation);
			db.SaveChanges();

			return RedirectToAction("Index", "Home");
		}
		
		[Authorize(Roles = "FunctionalManager, AdministrativeManager")]
		public ActionResult Review(int id)
		{
			PrepareComtetencyList();
			PrepareIndicatorsForm();
			Account currentAccount = ViewBag.CurrentAccount = db.Accounts.FirstOrDefault(a => a.Login == User.Identity.Name);
			Evaluation evaluation = ViewBag.Evaluation = db.Evaluations.Find(id);
			Account examinee = ViewBag.Examinee = evaluation.Examinee;

			if (!EvaluationWorkflow.CanBeReviewedBy(currentAccount, examinee))
			{
				throw new UnauthorizedAccessException();
			}

			return View();
		}

		[Authorize(Roles = "FunctionalManager, AdministrativeManager")]
		[HttpPost, ActionName("Review")]
		[ValidateAntiForgeryToken]
		public ActionResult ReviewPost(int id)
		{
			PrepareComtetencyList();
			PrepareIndicatorsForm();
			Account currentAccount = ViewBag.CurrentAccount = db.Accounts.FirstOrDefault(a => a.Login == User.Identity.Name);
			Evaluation evaluation = ViewBag.Evaluation = db.Evaluations.Find(id);
			Account examinee = ViewBag.Examinee = evaluation.Examinee;

			if (!EvaluationWorkflow.CanBeReviewedBy(currentAccount, examinee))
			{
				throw new UnauthorizedAccessException();
			}

			ValidateIndicatorsForm();

			if (ViewBag.IndicatorErrors.Count > 0)
			{
				return View();
			}

			if (EvaluationWorkflow.CanBeReviewedAsFunctionalManager(currentAccount, examinee))
			{
				evaluation.Reviewed = DateTime.Now;
				currentAccount.EvaluationsReviews.Add(evaluation);
				foreach (dynamic kv in ViewBag.IndicatorValues)
				{
					EvaluationValue evaluationValue = evaluation.EvaluationValues.FirstOrDefault(
						ev => ev.Competency == int.Parse(kv.Key.Split('_')[1]) &&
							  ev.Indicator == int.Parse(kv.Key.Split('_')[2]));
					evaluationValue.ReviewValue = double.Parse(kv.Value.Replace(".", ","));
				}
				evaluation.Examinier = currentAccount;
				evaluation.ReviewedResult = evaluation.EvaluationValues.Sum(ev => ev.ReviewValue ?? 0);
			}

			if (EvaluationWorkflow.CanBeReviewedAsAdministrativeManager(currentAccount, examinee))
			{
				evaluation.ManagerReviewed = DateTime.Now;
				currentAccount.EvaluationsManages.Add(evaluation);
				foreach (dynamic kv in ViewBag.IndicatorValues)
				{
					EvaluationValue evaluationValue = evaluation.EvaluationValues.FirstOrDefault(
						ev => ev.Competency == int.Parse(kv.Key.Split('_')[1]) &&
							  ev.Indicator == int.Parse(kv.Key.Split('_')[2]));
					evaluationValue.ManagerValue = double.Parse(kv.Value.Replace(".", ","));
				}
				evaluation.Manager = currentAccount;
				evaluation.ManagerResult = evaluation.EvaluationValues.Sum(ev => ev.ManagerValue ?? 0);
			}

			
			evaluation.Examinee.LastEvaluationPercent = evaluation.GetPercent();

			db.SaveChanges();
			return RedirectToAction("Details", "Accounts", new {id = examinee.Id});
		}

		private void ValidateIndicatorsForm()
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

		private void PrepareIndicatorsForm()
		{
			ViewBag.IndicatorErrors = new List<string>();
			ViewBag.IndicatorValues  = new Dictionary<string, string>();
		}

		private void PrepareComtetencyList()
		{
			ViewBag.CompetencyList = new CompetencyList(Server.MapPath("~/App_Data/CompetencyList.xml"));
		}
	}
}