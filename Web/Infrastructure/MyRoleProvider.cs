namespace Odintsov.Accounts.Web.Infrastructure
{
	using System;
	using System.Linq;
	using System.Web.Security;
	using Common;
	using Common.Enums;
	using Entities;

	public class MyRoleProvider : RoleProvider
	{
		private readonly AccountsDbContext db = new AccountsDbContext();

		public override string[] GetAllRoles()
		{
			return RoleMap.Default.GetAllRolesNames();
		}

		public override string[] GetRolesForUser(string username)
		{
			string role = RoleMap.Default.GetNameByRole(Role.None);

			var account = db.Accounts.FirstOrDefault(a => a.Login == username);
			if (account != null)
			{
				role = RoleMap.Default.GetNameByRole(account.Role);
			}

			return new[] {role};
		}

		#region Not implemented section

		public override void AddUsersToRoles(string[] usernames, string[] roleNames)
		{
			throw new NotImplementedException();
		}

		public override string ApplicationName
		{
			get { throw new NotImplementedException(); }
			set { throw new NotImplementedException(); }
		}

		public override void CreateRole(string roleName)
		{
			throw new NotImplementedException();
		}

		public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
		{
			throw new NotImplementedException();
		}

		public override string[] FindUsersInRole(string roleName, string usernameToMatch)
		{
			throw new NotImplementedException();
		}

		public override string[] GetUsersInRole(string roleName)
		{
			throw new NotImplementedException();
		}

		public override bool IsUserInRole(string username, string roleName)
		{
			throw new NotImplementedException();
		}

		public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
		{
			throw new NotImplementedException();
		}

		public override bool RoleExists(string roleName)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}