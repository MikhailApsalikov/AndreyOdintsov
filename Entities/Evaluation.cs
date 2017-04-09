namespace Entities
{
	using System;
	using System.Collections.Generic;
	using Selp.Interfaces;

	public sealed class Evaluation : ISelpEntity<int>
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

		public Account Examinee { get; set; }
		public Account Examinier { get; set; }

		public ICollection<EvaluationValue> EvaluationValues { get; set; }

		public Account Manager { get; set; }
	}
}