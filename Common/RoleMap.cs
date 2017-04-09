namespace Common
{
	using System.Collections.Generic;
	using System.Linq;
	using Enums;

	public class RoleMap
	{
		private readonly Dictionary<Role, string> displayNameMap = new Dictionary<Role, string>
		{
			{Role.Admin, "Администратор"},
			{Role.Cheef, "Начальник"},
			{Role.FuncManager, "Административный руководитель"},
			{Role.DepCheef, "Функциональный руководитель"},
			{Role.Employee, "Сотрудник"},
			{Role.None, "-"}
		};

		private readonly Dictionary<Role, string> map = new Dictionary<Role, string>
		{
			{Role.Admin, "Admin"},
			{Role.Cheef, "Cheef"},
			{Role.FuncManager, "FuncManager"},
			{Role.DepCheef, "DepCheef"},
			{Role.Employee, "Employee"},
			{Role.None, "None"}
		};

		private readonly Dictionary<string, Role> reverseMap;

		private RoleMap()
		{
			reverseMap = map.ToDictionary(el => el.Value, el => el.Key);
		}

		public static RoleMap Default { get; } = new RoleMap();

		public Role GetRoleByName(string name)
		{
			return reverseMap[name];
		}

		public string GetNameByRole(Role? role)
		{
			return map[role ?? Role.None];
		}

		public string GetDisplayName(Role? role)
		{
			return displayNameMap[role ?? Role.None];
		}

		public string GetDisplayName(string roleName)
		{
			return displayNameMap[GetRoleByName(roleName)];
		}

		public string[] GetAllRolesNames()
		{
			return map.Values.ToArray();
		}

		public Dictionary<Role, string> GetDisplayRoleMap()
		{
			return displayNameMap;
		}
	}
}