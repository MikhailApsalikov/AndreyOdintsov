namespace Repositories
{
	using System.Data.Entity;
	using System.Linq;
	using Entities;
	using Selp.Common.Entities;
	using Selp.Interfaces;
	using Selp.Repository;

	internal class EvaluationValueRepository : SelpRepository<EvaluationValue, int>
	{
		public EvaluationValueRepository(DbContext dbContext, ISelpConfiguration configuration)
			: base(dbContext, configuration)
		{
		}

		public override bool IsRemovingFake => false;
		public override string FakeRemovingPropertyName => null;
		public override IDbSet<EvaluationValue> DbSet => (DbContext as AccountsDbContext)?.EvaluationValues;

		protected override EvaluationValue Merge(EvaluationValue source, EvaluationValue destination)
		{
			return source;
		}

		protected override IQueryable<EvaluationValue> ApplyFilters(IQueryable<EvaluationValue> entities, BaseFilter filter)
		{
			return entities;
		}
	}
}