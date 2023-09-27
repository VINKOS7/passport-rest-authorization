using Passport.Domain.Aggregates.Account;
using Passport.Domain.Aggregates.Book;
using Passport.Infrastructure;

namespace Passport.Api.Extensions;

public static class InfrastructureExtensions
{
    public static IServiceCollection ConfigureInfrastructureServices(this IServiceCollection services) => services
        .AddScoped<IBookRepo, BookRepo>()
        .AddScoped<IAccountRepo, AccountRepo>();
    
}
