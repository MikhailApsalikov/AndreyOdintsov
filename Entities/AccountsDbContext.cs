namespace Entities
{
	using System.Data.Entity;

	public class AccountsDbContext : DbContext
	{
		public DbSet<Account> Accounts { get; set; }
		public DbSet<Evaluation> Evaluations { get; set; }
		public DbSet<EvaluationValue> EvaluationValues { get; set; }
	}
}