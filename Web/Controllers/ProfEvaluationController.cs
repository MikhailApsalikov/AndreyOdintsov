namespace Odintsov.Accounts.Web.Controllers
{
	using System;
	using System.Linq;
	using System.Web.Mvc;
	using BusinessLogic;
	using Entities;

	[Authorize]
	public class ProfEvaluationController : BaseController
	{
		public ActionResult Details(int id)
		{
			Account currentAccount = ViewBag.CurrentAccount = GetCurrentAccount();
			ProfEvaluation evaluation = ViewBag.Evaluation = Db.ProfEvaluations.Find(id);
			PrepareCompetencyList(evaluation.Examinee.FunctionalArea);
			return View();
		}

		public ActionResult Pass()
		{
			Account currentAccount = ViewBag.CurrentAccount = GetCurrentAccount();
			ProfEvaluation lastEvaluation = currentAccount.ProfEvaluations.OrderByDescending(e => e.Passed).FirstOrDefault();

			// Нельзя проходить чаще чем раз в день (на всякий случай).
			if (lastEvaluation != null && lastEvaluation.Passed.Date == DateTime.Today)
			{
				TempData["Error"] = "Нельзя проходить самостоятельную оценку больше раза в день.";
				return RedirectToAction("Index", "Home");
			}

			PrepareCompetencyList(currentAccount.FunctionalArea);
			PrepareIndicatorsForm();

			return View();
		}

		[HttpPost, ActionName("Pass")]
		[ValidateAntiForgeryToken]
		public ActionResult PassPost()
		{
			Account currentAccount = ViewBag.CurrentAccount = GetCurrentAccount();
			PrepareCompetencyList(currentAccount.FunctionalArea);
			PrepareIndicatorsForm();
			ValidateIndicatorsForm();

			if (ViewBag.IndicatorErrors.Count > 0)
			{
				return View();
			}

			var evaluation = new ProfEvaluation
			{
				Examinee = currentAccount,
				Passed = DateTime.Now,
				Position = currentAccount.Position
			};

			foreach (dynamic kv in ViewBag.IndicatorValues)
			{
				var evaluationValue = new ProfEvaluationValue
				{
					Evaluation = evaluation,
					Competency = int.Parse(kv.Key.Split('_')[1]),
					Indicator = int.Parse(kv.Key.Split('_')[2]),
					Value = double.Parse(kv.Value.Replace(".", ","))
				};

				evaluation.EvaluationValues.Add(evaluationValue);
			}

			evaluation.IndicatorsCount = evaluation.EvaluationValues.Count;
			currentAccount.ProfEvaluations.Add(evaluation);
			Db.SaveChanges();

			return RedirectToAction("Index", "Home");
		}

		[Authorize(Roles = "FunctionalManager, AdministrativeManager")]
		public ActionResult Review(int id)
		{
			Account currentAccount = ViewBag.CurrentAccount = GetCurrentAccount();
			ProfEvaluation evaluation = ViewBag.Evaluation = Db.ProfEvaluations.Find(id);
			Account examinee = ViewBag.Examinee = evaluation.Examinee;
			PrepareCompetencyList(examinee.FunctionalArea);
			PrepareIndicatorsForm();

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
			Account currentAccount = ViewBag.CurrentAccount = GetCurrentAccount();
			ProfEvaluation evaluation = ViewBag.Evaluation = Db.ProfEvaluations.Find(id);
			Account examinee = ViewBag.Examinee = evaluation.Examinee;
			PrepareCompetencyList(examinee.FunctionalArea);
			PrepareIndicatorsForm();

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
				currentAccount.ProfEvaluationsReviews.Add(evaluation);
				foreach (dynamic kv in ViewBag.IndicatorValues)
				{
					ProfEvaluationValue evaluationValue = evaluation.EvaluationValues.FirstOrDefault(
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
				currentAccount.ProfEvaluationsManages.Add(evaluation);
				foreach (dynamic kv in ViewBag.IndicatorValues)
				{
					ProfEvaluationValue evaluationValue = evaluation.EvaluationValues.FirstOrDefault(
						ev => ev.Competency == int.Parse(kv.Key.Split('_')[1]) &&
							  ev.Indicator == int.Parse(kv.Key.Split('_')[2]));
					evaluationValue.ManagerValue = double.Parse(kv.Value.Replace(".", ","));
				}
				evaluation.Manager = currentAccount;
				evaluation.ManagerResult = evaluation.EvaluationValues.Sum(ev => ev.ManagerValue ?? 0);
			}


			evaluation.Examinee.LastProfEvaluationPercent = evaluation.GetPercent();

			Db.SaveChanges();
			return RedirectToAction("Details", "Accounts", new { id = examinee.Id });
		}
	}
}