using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Passport.Domain.Aggregates.Account;

namespace Passport.Infrastructure.EntityConfigures;

public class DefaultEntityConfig : IEntityTypeConfiguration<DefaultAccount>
{
    public void Configure(EntityTypeBuilder<DefaultAccount> builder) => builder
        .OwnsMany(acc => acc.Devices);
}

public class OwnerAccountEntityConfig : IEntityTypeConfiguration<OwnerAccount>
{
    public void Configure(EntityTypeBuilder<OwnerAccount> builder) => builder
        .OwnsMany(acc => acc.Devices);
}

public class AdminAccountEntityConfig : IEntityTypeConfiguration<AdminAccount>
{
    public void Configure(EntityTypeBuilder<AdminAccount> builder) => builder
        .OwnsMany(acc => acc.Devices);
}

public class ModerAccountEntityConfig : IEntityTypeConfiguration<ModerAccount>
{
    public void Configure(EntityTypeBuilder<ModerAccount> builder) => builder
        .OwnsMany(acc => acc.Devices);
}
