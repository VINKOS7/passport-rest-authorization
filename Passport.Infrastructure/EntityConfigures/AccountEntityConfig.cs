using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Passport.Domain.Aggregates.Account;

namespace Passport.Infrastructure.EntityConfigures;

public class AccountEntityConfig : IEntityTypeConfiguration<Account>
{
    public void Configure(EntityTypeBuilder<Account> builder) => builder
        .OwnsMany(acc => acc.Devices);
}
