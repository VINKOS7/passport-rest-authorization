using Microsoft.EntityFrameworkCore;

using MediatR;
using Dotseed.Context;

using Passport.Domain.Aggregates.Account;
using Passport.Infrastructure.EntityConfigures;

namespace Passport.Infrastructure;

public class Context : UnitOfWorkContext
{
    public DbSet<Account> Accounts { get; set; }

    public Context(DbContextOptions options, IMediator mediator) : base(options, mediator) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new AccountEntityConfig());
    }
}