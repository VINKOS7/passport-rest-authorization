using Dotseed.Domain;

using Passport.Domain.Aggregates.Account.Enums;
using Passport.Domain.Aggregates.Account.Values;
using Passport.Domain.Aggregates.Account.Commands;

namespace Passport.Domain.Aggregates.Account;

public class Account : Entity, IAggregateRoot
{
    public static Account From(IAddAccountCommand command)
    {
        var now = DateTime.UtcNow;

        var account = new Account()
        {
            AccessStatus = AccessStatus.WaitActivate,
            ActivationCode = $"{new Random().Next(000000, 999999)}",
            Nickname = command.Nickname,
            Password = command.Password,
            Email = command.Email,
            PhoneNumber = command.PhoneNumber,

            Devices = new(),

            PasswordAt = now,
            PhoneNumberAt = now,
            EmailAt = now,
        };

        account.SetCreatedAt(now);
        account.SetUpdateAt(now);

        return account; 
    }

    public AccessStatus AccessStatus { get; set; }
    public string ActivationCode { get; set; }
    public string Nickname { get; set; }
    public string Password { get; set; }
    public DateTime PasswordAt { get; set; }
    public string Email { get; set; }
    public DateTime EmailAt { get; set; }
    public string PhoneNumber { get; set; }
    public List<Device> Devices { get; set; }
    public DateTime PhoneNumberAt { get; set; }
}
