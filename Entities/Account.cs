namespace Entities
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Linq;
	using System.Security.Cryptography;
	using System.Text;
	using Common;
	using Common.Enums;
	using Microsoft.AspNet.Identity;

	[MetadataType(typeof(AccountMetadata))]
	public class Account : IUser
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
		public string ManagerFullName { get; set; }
		public virtual Account Manager { get; set; }

		public int? AdministrativeManagerId { get; set; }
		public virtual Account AdministrativeManager { get; set; }
		public double? LastEvaluationPercent { get; set; }

		public virtual ICollection<Evaluation> Evaluations { get; set; }
		public virtual ICollection<Evaluation> EvaluationsReviews { get; set; }
		public virtual ICollection<Account> Team { get; set; }

		public virtual ICollection<Evaluation> EvaluationsManages { get; set; }

		[Display(Name = "Роль")]
		[NotMapped]
		public string RoleName => RoleMap.Default.GetNameByRole(Role);

		[NotMapped]
		public string RoleDisplayName => RoleMap.Default.GetDisplayName(Role);

		[Display(Name = "Функц. руководитель")]
		[NotMapped]
		public Account Principal { get; set; }

		[NotMapped]
		string IUser<string>.Id => Guid;

		[NotMapped]
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

		public Evaluation GetLastReviewedEvaluation()
		{
			Evaluation evaluation = Evaluations.Where(e => e.Examinier != null && e.Manager != null)
				.OrderByDescending(e => e.Reviewed).FirstOrDefault();

			return evaluation;
		}

		public static string Hash(string input)
		{
			byte[] hash = new SHA1Managed().ComputeHash(Encoding.UTF8.GetBytes(input));
			return string.Join("", hash.Select(b => b.ToString("x2")).ToArray());
		}
	}
}