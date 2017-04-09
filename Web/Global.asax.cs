namespace Web
{
	using System;
	using System.Data.Entity;
	using System.Diagnostics;
	using System.Web;
	using System.Web.Http;
	using System.Web.Mvc;
	using System.Web.Routing;
	using App_Start;
	using Odintsov.Accounts.Web.App_Start;
	using Repositories;

	public class Global : HttpApplication
	{
		protected void Application_Start(object sender, EventArgs e)
		{
			FilterConfig.Configure(GlobalFilters.Filters);
			RouteConfig.Configure(RouteTable.Routes);
			AutoMapperConfig.RegisterMappings();
			UnityConfig.Register(GlobalConfiguration.Configuration);
			Database.SetInitializer(new TestDataInitializer());
		}

		protected void Session_Start(object sender, EventArgs e)
		{
		}

		protected void Application_BeginRequest(object sender, EventArgs e)
		{
		}

		protected void Application_AuthenticateRequest(object sender, EventArgs e)
		{
		}
		/*
		protected void Application_Error(object sender, EventArgs e)
		{
			Exception ex = Server.GetLastError();
			if (ex != null)
			{
				Trace.TraceError(ex.ToString());
			}
		}
		*/

		protected void Session_End(object sender, EventArgs e)
		{
		}

		protected void Application_End(object sender, EventArgs e)
		{
		}
	}
}