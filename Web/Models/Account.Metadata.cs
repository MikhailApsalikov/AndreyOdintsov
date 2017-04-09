using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace Odintsov.Accounts.Web.Models
{
	using global::Models;

	[MetadataType(typeof(AccountModel))]
	public partial class Account : IUser
    {
        [Display(Name = "Роль")]
        public string RoleName => RoleMap.Default.GetNameByRole(Role);

        public string RoleDisplayName => RoleMap.Default.GetDisplayName(Role);

        string IUser<string>.Id
        {
            get { return Guid; }
        }

        [Display(Name = "Функц. руководитель")]
        public Account Principal { get; set; }

        public string UserName
        {
            get { return Login; }
            set { Login = value; }
        }
        
        public void SetPassword(string password)
        {
            Salt = Hash(new Random().Next().ToString() + Login ?? "" + FullName ?? "" +
                ":SOINpw98chn4p9UBEPW(*BNECpub");

            Password = Hash(password + Salt);
        }

        public string GetPasswordHash(string password)
        {
            return Hash(password + Salt);
        }

        public Evaluation GetLastReviewedEvaluation()
        {
           var evaluation = Evaluations.Where(e => e.Examinier != null && e.Manager != null)
               .OrderByDescending(e => e.Reviewed ).FirstOrDefault();

            return evaluation;
       }

        public static string Hash(string input)
        {
            var hash = (new SHA1Managed()).ComputeHash(Encoding.UTF8.GetBytes(input));
            return string.Join("", hash.Select(b => b.ToString("x2")).ToArray());
        }
    }
}