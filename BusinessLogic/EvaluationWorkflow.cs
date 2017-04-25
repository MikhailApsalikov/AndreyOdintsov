namespace BusinessLogic
{
	using System.Linq;
	using Entities;

	public static class EvaluationWorkflow
	{
		public static bool CanPass(string examineeName)
		{
			Account examinee = new AccountsDbContext().Accounts.FirstOrDefault(a => a.Login == examineeName);
			if (examinee == null)
			{
				return false;
			}
			return examinee.Manager != null || examinee.AdministrativeManager != null;
		}

		public static bool CanBeReviewedBy(Account manager, Account examinee)
		{
			return CanBeReviewedAsFunctionalManager(manager, examinee) || CanBeReviewedAsAdministrativeManager(manager, examinee);
		}

		public static bool CanBeReviewedAsFunctionalManager(Account manager, Account examinee)
		{
			if (manager == null || examinee == null)
			{
				return false;
			}
			return examinee.Manager == manager;
		}

		public static bool CanBeReviewedAsAdministrativeManager(Account manager, Account examinee)
		{
			if (manager == null || examinee == null)
			{
				return false;
			}
			return examinee.AdministrativeManager == manager;
		}

		public static bool CanPassProf(string name)
		{
			Account examinee = new AccountsDbContext().Accounts.FirstOrDefault(a => a.Login == name);
			if (examinee == null)
			{
				return false;
			}
			return (examinee.Manager != null || examinee.AdministrativeManager != null) && !string.IsNullOrEmpty(examinee.FunctionalArea) && CompetencyListWorkflow.IsProfCompetencyExist(examinee.FunctionalArea); 
		}
	}
}