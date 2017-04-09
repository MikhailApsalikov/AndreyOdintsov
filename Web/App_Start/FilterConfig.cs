namespace Web.App_Start
{
	using Odintsov.Accounts.Web.Infrastructure;

	public class FilterConfig
	{
		public static void Configure(System.Web.Mvc.GlobalFilterCollection filters)
		{
			filters.Add(new HandleErrorAttribute());
		}
	}
}
