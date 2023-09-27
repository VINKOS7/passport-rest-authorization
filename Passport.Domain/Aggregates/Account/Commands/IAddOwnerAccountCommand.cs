namespace Passport.Domain.Aggregates.Account.Commands;

public interface IAddOwnerAccountCommand
{
    string Nickname { get; }
    string Password { get; }
    string Email { get; }
    string PhoneNumber { get; }
}
