﻿namespace Entities
{
	public class ProfEvaluationValue
	{
		public int Id { get; set; }
		public int? Competency { get; set; }
		public int? Indicator { get; set; }
		public double? Value { get; set; }
		/// <summary>
		/// Оценка функционального
		/// </summary>
		public double? ReviewValue { get; set; }
		public int EvaluationId { get; set; }
		/// <summary>
		/// Оценка административного
		/// </summary>
		public double? ManagerValue { get; set; }
		public virtual ProfEvaluation Evaluation { get; set; }

		public double GetAvg()
		{

			if (!ReviewValue.HasValue && !ManagerValue.HasValue)
			{
				return 0;
			}

			if (ReviewValue.HasValue && !ManagerValue.HasValue)
			{
				return ReviewValue.Value;
			}

			if (!ReviewValue.HasValue)
			{
				return ManagerValue.Value;
			}

			return (ReviewValue.Value + ManagerValue.Value) / 2;
		}
	}
}