namespace Entities
{
	using System.Data.Entity;

	public class TestDataInitializer : CreateDatabaseIfNotExists<AccountsDbContext>
	{
		protected override void Seed(AccountsDbContext context)
		{
		}
	}
}