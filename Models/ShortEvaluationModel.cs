namespace Models
{
	using System;

	public class ShortEvaluationModel
	{
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

		// Формула подсчета результата в процентах: (X - Xmin) * 100 / (Xmax - Xmin). 
		// X - это (ReviewedResult + ManagerResult) / 2, Xmin - IndicatorsCount, Xmax - IndicatorsCount * 3
		// Умножаем на 120 т.к. по ТЗ максимальный балл - это 120%.
		public double? Percent => ((ReviewedResult + ManagerResult) / 2 - IndicatorsCount) * 120 / (IndicatorsCount * 2);
	}
}