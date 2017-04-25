namespace Entities
{
	using System.Collections.Generic;
	using System.ComponentModel.DataAnnotations;
	using Common.Enums;

	public class AccountMetadata
	{
		[Display(Name = "Активен")] public string Active;

		[Display(Name = "Админ. руководитель")] public string AdministrativeManager;

		[Display(Name = "Таб. номер")] public string Code;

		[Display(Name = "Отдел(ФН)")] public string Department;

		[Display(Name = "Оценки личн. компетенций")] public ICollection<Evaluation> Evaluations;

		[Display(Name = "Ревью личн. компетенций как админ. рук.")] public ICollection<Evaluation> EvaluationsManages;

		[Display(Name = "Ревью личн. компетенций как фукнц. рук.")] public ICollection<Evaluation> EvaluationsReviews;

		[Display(Name = "Оценки проф. компетенций")]
		public ICollection<ProfEvaluation> ProfEvaluations;

		[Display(Name = "Ревью проф. компетенций как админ. рук.")]
		public ICollection<ProfEvaluation> ProfEvaluationsManages;

		[Display(Name = "Ревью проф. компетенций как фукнц. рук.")]
		public ICollection<ProfEvaluation> ProfEvaluationsReviews;

		[Display(Name = "ФИО")] public string FullName;

		[Display(Name = "Логин")] [RegularExpression(@"[a-zA-Z0-9]+")] [Required] public string Login;

		[Display(Name = "Функц. руководитель")] public string Manager;

		[Display(Name = "Макрорегион")] public string MicroRegion;

		[Display(Name = "Должность")] public string Position;

		[Display(Name = "Город")] public string Region;

		[Display(Name = "Роль")] public Role Role;

		[Display(Name = "Пол")] public string Sex;

		[Display(Name = "Функц. область")]
		public string FunctionalArea { get; set; }
	}
}