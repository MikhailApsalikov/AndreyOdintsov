namespace Odintsov.Accounts.Web.App_Start
{
	using AutoMapper;
	using Entities;
	using Models;

	public static class AutoMapperConfig
	{
		public static void RegisterMappings()
		{
			Mapper.Initialize(cfg =>
			{
				cfg.CreateMap<AccountModel, Account>()
					.ForMember(m => m.Manager, s => s.Ignore())
					.ReverseMap();
				cfg.CreateMap<Account, ShortAccountModel>();
			});
		}
	}
}