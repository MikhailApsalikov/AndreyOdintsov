namespace Models
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel.DataAnnotations;
	using System.Linq;
	using System.Security.Cryptography;
	using System.Text;
	using Common;
	using Entities.Enums;
	using Microsoft.AspNet.Identity;

	public class AccountModel
	{
		public class AccountMetadata : IUser<string>
		{
			[Display(Name = "Активен")] public string Active;

			[Display(Name = "Таб. номер")] public string Code;

			[Display(Name = "Отдел(ФН)")] public string Department;

			[Display(Name = "Оценки")] public ICollection<EvaluationModel> Evaluations;

			[Display(Name = "Ревью как админ. рук.")] public ICollection<EvaluationModel> EvaluationsManages;

			[Display(Name = "Ревью как фукнц. рук.")] public ICollection<EvaluationModel> EvaluationsReviews;

			[Display(Name = "ФИО")] public string FullName;

			[Display(Name = "Логин")] [RegularExpression(@"[a-zA-Z0-9]+")] [Required] public string Login;

			[Display(Name = "Админ. руководитель")] public string Manager;

			[Display(Name = "Макрорегион")] public string MicroRegion;

			[Display(Name = "Должность")] public string Position;

			[Display(Name = "Город")] public string Region;

			[Display(Name = "Роль")] public Role Role;

			[Display(Name = "Пол")] public string Sex;

			public string Salt { get; set; }

			public string Password { get; set; }

			[Display(Name = "Роль")]
			public string RoleName => RoleMap.Default.GetNameByRole(Role);

			public string RoleDisplayName => RoleMap.Default.GetDisplayName(Role);

			public string Guid { get; set; }

			[Display(Name = "Функц. руководитель")]
			public AccountModel Principal { get; set; }

			string IUser<string>.Id => Guid;

			public string UserName
			{
				get { return Login; }
				set { Login = value; }
			}

			public void SetPassword(string password)
			{
				Salt = Hash(new Random().Next() + Login ?? "" + FullName ?? "" +
				            ":SOINpw98chn4p9UBEPW(*BNECpub");

				Password = Hash(password + Salt);
			}

			public string GetPasswordHash(string password)
			{
				return Hash(password + Salt);
			}

			/*
			public EvaluationModel GetLastReviewedEvaluation()
			{
				EvaluationModel evaluation = Evaluations.Where(e => e.Examinier != null && e.Manager != null)
					.OrderByDescending(e => e.Reviewed).FirstOrDefault();

				return evaluation;
			}
			*/

			public static string Hash(string input)
			{
				byte[] hash = new SHA1Managed().ComputeHash(Encoding.UTF8.GetBytes(input));
				return string.Join("", hash.Select(b => b.ToString("x2")).ToArray());
			}
		}
	}
}