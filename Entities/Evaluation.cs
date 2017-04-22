namespace Entities
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics.CodeAnalysis;

	public class Evaluation
	{
		public Evaluation()
		{
			EvaluationValues = new HashSet<EvaluationValue>();
		}

		public int Id { get; set; }
		public DateTime Passed { get; set; }

		/// <summary>
		///     Дата ревью функционального
		/// </summary>
		public DateTime? Reviewed { get; set; }

		public int ExamineeId { get; set; }
		/// <summary>
		/// Функциональный
		/// </summary>
		public int? ExaminerId { get; set; }
		public double? ReviewedResult { get; set; }
		public int? IndicatorsCount { get; set; }
		/// <summary>
		/// Административный
		/// </summary>
		public int? ManagerId { get; set; }
		public double? ManagerResult { get; set; }

		/// <summary>
		///     Дата ревью административного
		/// </summary>
		public DateTime? ManagerReviewed { get; set; }

		public virtual Account Manager { get; set; }

		public virtual Account Examinee { get; set; }
		public virtual Account Examinier { get; set; }

		[SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
		public virtual ICollection<EvaluationValue> EvaluationValues { get; set; }


		public double? GetPercent()
		{
			// Формула подсчета результата в процентах: (X - Xmin) * 100 / (Xmax - Xmin). 
			// X - это (ReviewedResult + ManagerResult) / 2, Xmin - IndicatorsCount, Xmax - IndicatorsCount * 3
			// Умножаем на 120 т.к. по ТЗ максимальный балл - это 120%.

			if (!ReviewedResult.HasValue && !ManagerResult.HasValue)
			{
				return null;
			}

			if (ReviewedResult.HasValue && !ManagerResult.HasValue)
			{
				return (ReviewedResult- IndicatorsCount) * 120 / (IndicatorsCount * 2);
			}

			if (!ReviewedResult.HasValue)
			{
				return (ManagerResult - IndicatorsCount) * 120 / (IndicatorsCount * 2);
			}

			return ((ReviewedResult + ManagerResult) / 2 - IndicatorsCount) * 120 / (IndicatorsCount * 2);
		}
	}
}