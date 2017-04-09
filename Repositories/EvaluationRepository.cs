namespace Repositories
{
	using System.Data.Entity;
	using System.Linq;
	using Entities;
	using Selp.Common.Entities;
	using Selp.Interfaces;
	using Selp.Repository;

	internal class EvaluationRepository : SelpRepository<Evaluation, int>
	{
		public EvaluationRepository(DbContext dbContext, ISelpConfiguration configuration) : base(dbContext, configuration)
		{
		}

		public override bool IsRemovingFake => false;
		public override string FakeRemovingPropertyName => null;
		public override IDbSet<Evaluation> DbSet => (DbContext as AccountsDbContext)?.Evaluations;

		protected override Evaluation Merge(Evaluation source, Evaluation destination)
		{
			return source;
		}

		protected override IQueryable<Evaluation> ApplyFilters(IQueryable<Evaluation> entities, BaseFilter filter)
		{
			return entities;
		}
	}
}