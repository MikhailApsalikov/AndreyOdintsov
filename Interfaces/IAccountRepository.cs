namespace Interfaces
{
	using Entities;
	using Selp.Interfaces;

	public interface IAccountRepository : ISelpRepository<Account, int>
	{
		Account GetByLogin(string login);
	}
}