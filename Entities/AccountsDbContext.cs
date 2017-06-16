namespace Entities
{
	using System.Data.Entity;

	public class AccountsDbContext : DbContext
	{
		public DbSet<Account> Accounts { get; set; }
		public DbSet<Evaluation> Evaluations { get; set; }
		public DbSet<EvaluationValue> EvaluationValues { get; set; }
		public DbSet<ProfEvaluation> ProfEvaluations { get; set; }
		public DbSet<ProfEvaluationValue> ProfEvaluationValues { get; set; }

		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Account>().HasOptional(a => a.Manager).WithMany(a => a.Team);
			modelBuilder.Entity<Evaluation>().HasRequired(a => a.Examinee).WithMany(a => a.Evaluations);
			modelBuilder.Entity<Evaluation>().HasOptional(a => a.Examinier).WithMany(a => a.EvaluationsReviews);
			modelBuilder.Entity<Evaluation>().HasOptional(a => a.Manager).WithMany(a => a.EvaluationsManages);
			modelBuilder.Entity<ProfEvaluation>().HasRequired(a => a.Examinee).WithMany(a => a.ProfEvaluations);
			modelBuilder.Entity<ProfEvaluation>().HasOptional(a => a.Examinier).WithMany(a => a.ProfEvaluationsReviews);
			modelBuilder.Entity<ProfEvaluation>().HasOptional(a => a.Manager).WithMany(a => a.ProfEvaluationsManages);
			base.OnModelCreating(modelBuilder);
		}
	}
}