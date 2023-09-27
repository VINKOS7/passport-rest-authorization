using Passport.Api.Services;

namespace Passport.Api.Extensions;

public static class ApplicationExtensions
{
    public static IServiceCollection ConfigureApplicationServices(this IServiceCollection services, IConfiguration configuration) => services
        .AddScoped<IEmailService, EmailService>();
}
