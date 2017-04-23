namespace Odintsov.Accounts.Web.Controllers
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Web.Mvc;
	using BusinessLogic;
	using Entities;

	[Authorize]
	public class EvaluationController : BaseController
	{
		// GET: Evaluation/Details/5

		public ActionResult Details(int id)
		{
			PrepareCompetencyList();
			Account currentAccount = ViewBag.CurrentAccount = GetCurrentAccount();
			Evaluation evaluation = ViewBag.Evaluation = Db.Evaluations.Find(id);

			return View();
		}

		// GET: Evaluation/Pass
		public ActionResult Pass()
		{
			Account currentAccount = ViewBag.CurrentAccount = GetCurrentAccount();
			Evaluation lastEvaluation = currentAccount.Evaluations.OrderByDescending(e => e.Passed).FirstOrDefault();

			// Нельзя проходить чаще чем раз в день (на всякий случай).
			if (lastEvaluation != null && lastEvaluation.Passed.Date == DateTime.Today)
			{
				TempData["Error"] = "Нельзя проходить самостоятельную оценку больше раза в день.";
				return RedirectToAction("Index", "Home");
			}

			PrepareCompetencyList();
			PrepareIndicatorsForm();

			return View();
		}

		[HttpPost, ActionName("Pass")]
		[ValidateAntiForgeryToken]
		public ActionResult PassPost()
		{
			PrepareCompetencyList();
			PrepareIndicatorsForm();
			ValidateIndicatorsForm();
			Account currentAccount = ViewBag.CurrentAccount = GetCurrentAccount();

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
			Db.SaveChanges();

			return RedirectToAction("Index", "Home");
		}

		[Authorize(Roles = "FunctionalManager, AdministrativeManager")]
		public ActionResult Review(int id)
		{
			PrepareCompetencyList();
			PrepareIndicatorsForm();
			Account currentAccount = ViewBag.CurrentAccount = GetCurrentAccount();
			Evaluation evaluation = ViewBag.Evaluation = Db.Evaluations.Find(id);
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
			PrepareCompetencyList();
			PrepareIndicatorsForm();
			Account currentAccount = ViewBag.CurrentAccount = GetCurrentAccount();
			Evaluation evaluation = ViewBag.Evaluation = Db.Evaluations.Find(id);
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

			Db.SaveChanges();
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
	}
}