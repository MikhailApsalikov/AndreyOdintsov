namespace Repositories
{
	using System.Collections.Generic;
	using System.Data.Entity;
	using System.Linq;
	using Common.Enums;
	using Common.Filters;
	using Entities;
	using Interfaces;
	using Selp.Common.Entities;
	using Selp.Interfaces;
	using Selp.Repository;

	public class AccountRepository : SelpRepository<Account, int>, IAccountRepository
	{
		public AccountRepository(DbContext dbContext, ISelpConfiguration configuration) : base(dbContext, configuration)
		{
		}

		public override bool IsRemovingFake => false;
		public override string FakeRemovingPropertyName => null;
		public override IDbSet<Account> DbSet => (DbContext as AccountsDbContext)?.Accounts;

		public Account GetByLogin(string login)
		{
			return GetByCustomExpression(a => a.Login == login).FirstOrDefault();
		}

		public List<Account> GetByFilter(AccountsFilter filter, out int total)
		{
			return base.GetByFilter(filter, out total);
		}

		protected override Account Merge(Account source, Account destination)
		{
			return source;
		}

		protected override IQueryable<Account> ApplyFilters(IQueryable<Account> entities, BaseFilter filter)
		{
			var accountFilter = filter as AccountsFilter;
			var currentAccount = GetByLogin(accountFilter.CurrentAccount);
			if (!string.IsNullOrEmpty(accountFilter.Region))
			{
				entities = entities.Where(a => a.Region == accountFilter.Region);
			}
			if (!string.IsNullOrEmpty(accountFilter.MicroRegion))
			{
				entities = entities.Where(a => a.MicroRegion == accountFilter.MicroRegion);
			}
			if (!string.IsNullOrEmpty(accountFilter.Department))
			{
				entities = entities.Where(a => a.Department == accountFilter.Department);
			}
			if (!string.IsNullOrEmpty(accountFilter.Position))
			{
				entities = entities.Where(a => a.Position == accountFilter.Position);
			}

			if (currentAccount.Team.Count > 0)
			{
				IEnumerable<int> teamIds = currentAccount.Team.Select(m => m.Id);
				entities = entities.Where(a => teamIds.Contains(a.Id));
			}

			if (currentAccount.Role == Role.DepCheef)
			{
				if (currentAccount.Team.Count > 0)
				{
					entities =
						entities.Union(
							GetByCustomExpression(
								a => a.Department == currentAccount.Department && a.Login != accountFilter.CurrentAccount));
				}
				else
				{
					entities = entities.Where(a => a.Department == currentAccount.Department);
					entities = entities.Where(a => a.Login != accountFilter.CurrentAccount);
				}
			}

			return entities;
		}
	}
}