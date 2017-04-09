namespace Interfaces
{
	using System.Collections.Generic;
	using Common.Filters;
	using Entities;
	using Selp.Interfaces;

	public interface IAccountRepository : ISelpRepository<Account, int>
	{
		Account GetByLogin(string login);
		List<Account> GetByFilter(AccountsFilter filter, out int total);
	}
}