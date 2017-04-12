namespace Entities
{
	using System;
	using System.Collections.Generic;
	using System.Data.Entity;
	using System.Linq;
	using Common.Enums;

	public class TestDataInitializer : DropCreateDatabaseIfModelChanges<AccountsDbContext>
	{
		private readonly Random random = new Random();

		protected override void Seed(AccountsDbContext context)
		{
			InitializeTestAccounts(context);
		}

		private void InitializeTestAccounts(AccountsDbContext context)
		{
			if (context.Accounts.Any())
			{
				return;
			}

			var accounts = new List<Account>
			{
				new Account
				{
					Login = "admin",
					Password = "b5398670ef184be522f9c86d67d03ec38d04e705",
					Salt = "0b07da3c5afa7ff09fb2c2ff3514a5f1ef6888c2",
					Active = true,
					Code = "1",
					FullName = "Админов Админ Админович",
					Sex = "м",
					Region = "Саратов",
					Position = "administrator",
					Role = Role.Admin,
					Guid = Guid.NewGuid().ToString()
				}
			};

			context.Accounts.AddRange(accounts);
		}
	}
}