namespace BusinessLogic
{
	using System.Linq;
	using Common.Enums;
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
			return examinee.Manager != null || examinee.AdministrativeManager != null || examinee.Role == Role.Employee;
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
			if (examinee.Manager != null)
			{
				return examinee.Manager == manager;
			}
			return examinee.Department == manager.Department && manager.Role == Role.FunctionalManager;
		}

		public static bool CanBeReviewedAsAdministrativeManager(Account manager, Account examinee)
		{
			if (manager == null || examinee == null)
			{
				return false;
			}
			return examinee.AdministrativeManager == manager;
		}
	}
}