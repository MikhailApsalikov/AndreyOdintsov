namespace Common.Filters
{
	using System.Web;
	using System.Web.Script.Serialization;

	public class AccountsFilter
	{
		public string Region { get; set; }
		public string MicroRegion { get; set; }
		public string Department { get; set; }
		public string Position { get; set; }

		public string ToJson(bool urlEncode = true)
		{
			JavaScriptSerializer js = new JavaScriptSerializer();
			return urlEncode ? HttpUtility.UrlEncode(js.Serialize(this)) : js.Serialize(this);
		}
	}
}