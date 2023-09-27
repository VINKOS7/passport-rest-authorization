using Microsoft.EntityFrameworkCore;

using Dotseed.Domain;

using Passport.Domain.Aggregates.Account;

namespace Passport.Infrastructure;

public class AccountRepo : IAccountRepo
{
    private readonly Context _db;

    public AccountRepo(Context db) => _db = db;

    public IUnitOfWork UnitOfWork => _db;

    public async Task AddAsync(Account account) 
    {
        switch (account)
        {
            case DefaultAccount acc:
                break;
            case OwnerAccount acc:
                break;
            case AdminAccount acc:
                break;
            case ModerAccount acc:
                break;
        }
    }

    public async Task<Account> FindByActivationCodeAsync(string ActivationCode) => await _db.Accounts.FirstOrDefaultAsync(acc => acc.ActivationCode == ActivationCode);

    public async Task<Account> FindByEmailAsync(string Email) => await _db.Accounts.FirstOrDefaultAsync(acc => acc.Email == Email);

    public Task<Account> FindByIdAsync(Guid Id) => _db.Accounts.FirstOrDefaultAsync(acc => acc.Id == Id);

    public Task<Account> FindByNickNameAsync(string Nick) => _db.Accounts.FirstOrDefaultAsync(acc => acc.Nickname == Nick);

    public async Task RemoveById(Guid Id) => _db.Accounts.Remove(await _db.Accounts
        .FirstOrDefaultAsync(acc => acc.Id == Id)
        ?? throw new());
}
