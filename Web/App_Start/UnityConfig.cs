namespace Odintsov.Accounts.Web.App_Start
{
	using System;
	using System.Collections.Generic;
	using System.Web.Http;
	using System.Web.Http.Dependencies;
	using System.Web.Mvc;
	using Interfaces;
	using Microsoft.Practices.ServiceLocation;
	using Microsoft.Practices.Unity;
	using Repositories;
	using Selp.Configuration;
	using Selp.Interfaces;
	using Unity.Mvc5;
	using IDependencyResolver = System.Web.Http.Dependencies.IDependencyResolver;

	public static class UnityConfig
	{
		public static void Register(HttpConfiguration config)
		{
			var dbContext = new AccountsDbContext();
			var container = new UnityContainer();

			container.RegisterType<ISelpConfiguration, InMemoryConfiguration>();
			var efConstructorParameter = new InjectionConstructor(dbContext, container.Resolve<ISelpConfiguration>());

			container.RegisterType<IAccountRepository, AccountRepository>(efConstructorParameter);
			var locator = new UnityServiceLocator(container);
			ServiceLocator.SetLocatorProvider(() => locator);
			config.DependencyResolver = new UnityResolver(container);
			DependencyResolver.SetResolver(new UnityDependencyResolver(container));
		}
	}

	public class UnityResolver : IDependencyResolver
	{
		protected IUnityContainer container;

		public UnityResolver(IUnityContainer container)
		{
			if (container == null)
			{
				throw new ArgumentNullException("container");
			}
			this.container = container;
		}

		public object GetService(Type serviceType)
		{
			try
			{
				return container.Resolve(serviceType);
			}
			catch (ResolutionFailedException)
			{
				return null;
			}
		}

		public IEnumerable<object> GetServices(Type serviceType)
		{
			try
			{
				return container.ResolveAll(serviceType);
			}
			catch (ResolutionFailedException)
			{
				return new List<object>();
			}
		}

		public IDependencyScope BeginScope()
		{
			IUnityContainer child = container.CreateChildContainer();
			return new UnityResolver(child);
		}

		public void Dispose()
		{
			container.Dispose();
		}
	}
}