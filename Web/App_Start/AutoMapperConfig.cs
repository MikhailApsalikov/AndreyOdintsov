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
					.ForMember(m => m.Manager, d => d.Ignore())
					.ReverseMap()
					.ForMember(m=>m.ManagerId, d=>d.MapFrom(s=>s.ManagerId))
					.ForMember(m => m.Manager, d => d.MapFrom(s => s.Manager.FullName));
				cfg.CreateMap<Account, ShortAccountModel>();

				cfg.CreateMap<Evaluation, EvaluationModel>()
					.ReverseMap();
				cfg.CreateMap<Evaluation, ShortEvaluationModel>();
			});
		}
	}
}