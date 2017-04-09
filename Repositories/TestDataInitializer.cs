using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
	using System.Data.Entity;
	using Common.Enums;
	using Entities;

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
					Password = "4bf04605086345a609b4bccdc20affb8b51fcb72",
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
