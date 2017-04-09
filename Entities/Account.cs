namespace Entities
{
	using System.Collections.Generic;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Linq;
	using Common.Enums;
	using Selp.Interfaces;

	public class Account : ISelpEntity<int>
	{
		public Account()
		{
			Evaluations = new HashSet<Evaluation>();
			EvaluationsReviews = new HashSet<Evaluation>();
			Team = new HashSet<Account>();
			EvaluationsManages = new HashSet<Evaluation>();
		}

		public int Id { get; set; }
		public string Code { get; set; }
		public string Login { get; set; }
		public string Password { get; set; }
		public bool Active { get; set; }
		public string Salt { get; set; }
		public string FullName { get; set; }
		public string Sex { get; set; }
		public string Region { get; set; }
		public string MicroRegion { get; set; }
		public string Department { get; set; }
		public string Position { get; set; }
		public Role? Role { get; set; }
		public string Guid { get; set; }
		public int? ManagerId { get; set; }

		public double? LastEvaluationPercent { get; set; }

		[NotMapped]
		public Evaluation LastReviewedEvaluation => Evaluations.Where(e => e.Examinier != null && e.Manager != null)
					.OrderByDescending(e => e.Reviewed)
					.FirstOrDefault();

		public virtual ICollection<Evaluation> Evaluations { get; set; }
		public virtual ICollection<Evaluation> EvaluationsReviews { get; set; }
		public virtual ICollection<Account> Team { get; set; }
		public virtual Account Manager { get; set; }
		public virtual ICollection<Evaluation> EvaluationsManages { get; set; }
	}
}