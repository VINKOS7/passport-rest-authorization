using Dotseed.Domain;

namespace Passport.Domain.Aggregates.Account;

public interface IAccountRepo : IRepository<Account>
{
    public Task AddAsync(Account account);

    public Task RemoveById(Guid Id);

    public Task<Account> FindByEmailAsync(string Email);

    public Task<Account> FindByNickNameAsync(string Nick);

    public Task<Account> FindByIdAsync(Guid Id);

    public Task<Account> FindByActivationCodeAsync(string ActivationCode);
}
