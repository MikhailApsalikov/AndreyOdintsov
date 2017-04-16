namespace Entities
{
	using System.Collections.Generic;
	using System.ComponentModel.DataAnnotations;
	using Common.Enums;

	public class AccountMetadata
	{
		[Display(Name = "Активен")] public string Active;

		[Display(Name = "Таб. номер")] public string Code;

		[Display(Name = "Отдел(ФН)")] public string Department;

		[Display(Name = "Оценки")] public ICollection<Evaluation> Evaluations;

		[Display(Name = "Ревью как админ. рук.")] public ICollection<Evaluation> EvaluationsManages;

		[Display(Name = "Ревью как фукнц. рук.")] public ICollection<Evaluation> EvaluationsReviews;

		[Display(Name = "ФИО")] public string FullName;

		[Display(Name = "Логин")] [RegularExpression(@"[a-zA-Z0-9]+")] [Required] public string Login;

		[Display(Name = "Админ. руководитель")] public string AdministrativeManager;

		[Display(Name = "Функц. руководитель")] public string Manager;

		[Display(Name = "Макрорегион")] public string MicroRegion;

		[Display(Name = "Должность")] public string Position;

		[Display(Name = "Город")] public string Region;

		[Display(Name = "Роль")] public Role Role;

		[Display(Name = "Пол")] public string Sex;
	}
}