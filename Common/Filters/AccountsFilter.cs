namespace Common.Filters
{
	using System.ComponentModel;
	using System.Web;
	using System.Web.Script.Serialization;
	using Selp.Common.Entities;

	public class AccountsFilter : BaseFilter
	{
		public string Region { get; set; }
		public string MicroRegion { get; set; }
		public string Department { get; set; }
		public string Position { get; set; }

		public string CurrentAccount { get; set; }

		public AccountsFilter(string filter, string sortOrder, string currentAccount)
		{
			if (!string.IsNullOrEmpty(filter))
			{
				var deserialized = new JavaScriptSerializer().Deserialize<AccountsFilter>(HttpUtility.UrlDecode(filter));
				Department = deserialized.Department;
				MicroRegion = deserialized.MicroRegion;
				Position = deserialized.Position;
				Region = deserialized.Region;
			}
			Page = 0;
			PageSize = int.MaxValue;
			SortDirection = sortOrder[0] == '+' ? ListSortDirection.Ascending : ListSortDirection.Descending;
			SortField = sortOrder.Substring(1);
			CurrentAccount = currentAccount;
		}

		public string ToJson(bool urlEncode = true)
		{
			JavaScriptSerializer js = new JavaScriptSerializer();
			return urlEncode ? HttpUtility.UrlEncode(js.Serialize(this)) : js.Serialize(this);
		}
	}
}