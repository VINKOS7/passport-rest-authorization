using Microsoft.EntityFrameworkCore;

using MediatR;
using Dotseed.Context;

using Passport.Domain.Aggregates.Account;
using Passport.Infrastructure.EntityConfigures;

namespace Passport.Infrastructure;

public class Context : UnitOfWorkContext
{
    public DbSet<DefaultAccount> DefaultAccounts { get; set; }
    public DbSet<OwnerAccount> OwnerAccounts { get; set; }
    public DbSet<AdminAccount> AdminAccounts { get; set; }
    public DbSet<ModerAccount> ModerAccounts { get; set; }

    public Context(DbContextOptions options, IMediator mediator) : base(options, mediator) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new DefaultEntityConfig());
        modelBuilder.ApplyConfiguration(new OwnerAccountEntityConfig());
        modelBuilder.ApplyConfiguration(new AdminAccountEntityConfig());
        modelBuilder.ApplyConfiguration(new ModerAccountEntityConfig());
    }
}