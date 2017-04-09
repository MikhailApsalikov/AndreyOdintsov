namespace Repositories
{
	using System.Data.Entity;
	using System.Linq;
	using Entities;
	using Selp.Common.Entities;
	using Selp.Interfaces;
	using Selp.Repository;

	public class AccountRepository : SelpRepository<Account, int>
	{
		public AccountRepository(DbContext dbContext, ISelpConfiguration configuration) : base(dbContext, configuration)
		{
		}

		public override bool IsRemovingFake => false;
		public override string FakeRemovingPropertyName => null;
		public override IDbSet<Account> DbSet => (DbContext as AccountsDbContext)?.Accounts;

		protected override Account Merge(Account source, Account destination)
		{
			return source;
		}

		protected override IQueryable<Account> ApplyFilters(IQueryable<Account> entities, BaseFilter filter)
		{
			return entities;
		}
	}
}