﻿namespace Passport.Domain.Aggregates.Account.Commands;

public interface IAddDefaultAccountCommand
{
    string Nickname { get; }
    string Password { get; }
    string Email { get; }
    string PhoneNumber { get; }
}
