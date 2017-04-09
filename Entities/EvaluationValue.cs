namespace Entities
{
	public class EvaluationValue
	{
		public int Id { get; set; }
		public int? Competency { get; set; }
		public int? Indicator { get; set; }
		public double? Value { get; set; }
		public double? ReviewValue { get; set; }
		public int EvaluationId { get; set; }
		public double? ManagerValue { get; set; }

		public virtual Evaluation Evaluation { get; set; }
	}
}