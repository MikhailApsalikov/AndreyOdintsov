namespace Repositories
{
	using System.Data.Entity;
	using Entities;

	public class AccountsDbContext : DbContext
	{
		public DbSet<Account> Accounts { get; set; }
		public DbSet<Evaluation> Evaluations { get; set; }
		public DbSet<EvaluationValue> EvaluationValues { get; set; }
	}
}