namespace BusinessLogic
{
	using Common.Enums;
	using Entities;

	public static class EvaluationWorkflow
	{
		public static bool CanBeReviewed(Account currentAccount, Account examinee)
		{
			return CanBeReviewedAsFunctionalManager(currentAccount, examinee) || CanBeReviewedAsAdministrativeManager(currentAccount, examinee);
		}

		public static bool CanBeReviewedAsFunctionalManager(Account currentAccount, Account examinee)
		{
			if (currentAccount == null || examinee == null)
			{
				return false;
			}
			return (examinee.Department == currentAccount.Department || examinee.Manager == currentAccount) && currentAccount.Role == Role.FunctionalManager;
		}

		public static bool CanBeReviewedAsAdministrativeManager(Account currentAccount, Account examinee)
		{
			if (currentAccount == null || examinee == null)
			{
				return false;
			}
			return examinee.AdministrativeManager == currentAccount && currentAccount.Role == Role.AdministrativeManager;
		}
	}
}