namespace Entities
{
	using System;
	using System.Collections.Generic;
	using System.Data.Entity;
	using System.Linq;
	using Common.Enums;

	public class TestDataInitializer : DropCreateDatabaseIfModelChanges<AccountsDbContext>
	{
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
					MicroRegion = "Заводской",
					Department = "Отдел блекджека и шлюх",
					Position = "administrator",
					Role = Role.Admin,
					Guid = Guid.NewGuid().ToString()
				},
				new Account
				{
					Login = "adm",
					Password = "b5398670ef184be522f9c86d67d03ec38d04e705",
					Salt = "0b07da3c5afa7ff09fb2c2ff3514a5f1ef6888c2",
					Active = true,
					Code = "2",
					FullName = "Административный руководитель",
					Sex = "м",
					Region = "Саратов",
					MicroRegion = "Заводской",
					Department = "Отдел блекджека и шлюх",
					Position = "Административный руководитель",
					Role = Role.AdministrativeManager,
					Guid = Guid.NewGuid().ToString()
				},
				new Account
				{
					Login = "func",
					Password = "b5398670ef184be522f9c86d67d03ec38d04e705",
					Salt = "0b07da3c5afa7ff09fb2c2ff3514a5f1ef6888c2",
					Active = true,
					Code = "3",
					FullName = "Функциональный руководитель",
					Sex = "м",
					Region = "Саратов",
					MicroRegion = "Заводской",
					Department = "Отдел блекджека и шлюх",
					Position = "Функциональный руководитель",
					Role = Role.FunctionalManager,
					Guid = Guid.NewGuid().ToString()
				},
				new Account
				{
					Login = "boss",
					Password = "b5398670ef184be522f9c86d67d03ec38d04e705",
					Salt = "0b07da3c5afa7ff09fb2c2ff3514a5f1ef6888c2",
					Active = true,
					Code = "3",
					FullName = "Босс",
					Sex = "м",
					Region = "Саратов",
					MicroRegion = "Заводской",
					Department = "Отдел блекджека и шлюх",
					Position = "Босс",
					Role = Role.DirectManager,
					Guid = Guid.NewGuid().ToString()
				},
				new Account
				{
					Login = "bro1",
					Password = "b5398670ef184be522f9c86d67d03ec38d04e705",
					Salt = "0b07da3c5afa7ff09fb2c2ff3514a5f1ef6888c2",
					Active = true,
					Code = "4",
					FullName = "Обычный чувак 1",
					Sex = "м",
					Region = "Саратов",
					MicroRegion = "Заводской",
					Department = "Отдел блекджека и шлюх",
					Position = "Обычный чувак 1",
					Role = Role.Employee,
					Guid = Guid.NewGuid().ToString(),
					ManagerId = 3,
					AdministrativeManagerId = 2
				},
				new Account
				{
					Login = "bro2",
					Password = "b5398670ef184be522f9c86d67d03ec38d04e705",
					Salt = "0b07da3c5afa7ff09fb2c2ff3514a5f1ef6888c2",
					Active = true,
					Code = "5",
					FullName = "Обычный чувак 2",
					Sex = "м",
					Region = "Саратов",
					MicroRegion = "Заводской",
					Department = "Отдел блекджека и шлюх",
					Position = "Обычный чувак 2",
					Role = Role.Employee,
					Guid = Guid.NewGuid().ToString(),
					ManagerId = 3,
					AdministrativeManagerId = 2
				},
				new Account
				{
					Login = "bro3",
					Password = "b5398670ef184be522f9c86d67d03ec38d04e705",
					Salt = "0b07da3c5afa7ff09fb2c2ff3514a5f1ef6888c2",
					Active = true,
					Code = "6",
					FullName = "Обычный чувак 3",
					Sex = "м",
					Region = "Саратов",
					MicroRegion = "Заводской",
					Department = "Отдел блекджека и шлюх",
					Position = "Обычный чувак 3",
					Role = Role.Employee,
					Guid = Guid.NewGuid().ToString(),
					ManagerId = 3,
					AdministrativeManagerId = 2
				}
			};

			context.Accounts.AddRange(accounts);
		}
	}
}