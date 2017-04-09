namespace Web.App_Start
{
	using System.Web.Mvc;
	using System.Web.Routing;

	public class RouteConfig
	{
		public static void Configure(RouteCollection routes)
		{
			routes.MapRoute("Default", "{controller}/{action}/{id}",
				new {controller = "Home", action = "Index", id = UrlParameter.Optional}
				);
		}
	}
}