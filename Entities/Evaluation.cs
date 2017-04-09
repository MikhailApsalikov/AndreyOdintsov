namespace Entities
{
	using System;
	using System.Collections.Generic;

	public class Evaluation
	{
		public Evaluation()
		{
			EvaluationValues = new HashSet<EvaluationValue>();
		}

		public int Id { get; set; }
		public DateTime Passed { get; set; }
		public DateTime? Reviewed { get; set; }
		public int ExamineeId { get; set; }
		public int? ExaminerId { get; set; }
		public double? ReviewedResult { get; set; }
		public int? IndicatorsCount { get; set; }
		public int? ManagerId { get; set; }
		public double? ManagerResult { get; set; }
		public DateTime? ManagerReviewed { get; set; }

		public virtual Account Examinee { get; set; }
		public virtual Account Examinier { get; set; }

		public virtual ICollection<EvaluationValue> EvaluationValues { get; set; }

		public virtual Account Manager { get; set; }
	}
}