using System.Globalization;
using OpenIddict.Abstractions;
using static OpenIddict.Abstractions.OpenIddictConstants;
using System;
using Microsoft.Extensions.Hosting;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using E.Shop.Passport.Identity.Data;
using E.Shop.Passport.Identity.Models;
using Microsoft.AspNetCore.Identity;

namespace E.Shop.Passport.Identity;

public class Worker : IHostedService
{
    private readonly IServiceProvider _serviceProvider;

    public Worker(IServiceProvider serviceProvider)
        => _serviceProvider = serviceProvider;

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await using var scope = _serviceProvider.CreateAsyncScope();

        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await context.Database.EnsureCreatedAsync(cancellationToken);

        await RegisterApplicationsAsync(scope.ServiceProvider);
        await RegisterScopesAsync(scope.ServiceProvider);

        static async Task RegisterApplicationsAsync(IServiceProvider provider)
        {
            var manager = provider.GetRequiredService<IOpenIddictApplicationManager>();

            //await manager.DeleteAsync(await manager.FindByClientIdAsync("E.Shop"));

            var userManager = provider.GetRequiredService<UserManager<ApplicationUser>>();
            if (await userManager.FindByEmailAsync("andrey@hr.vision") is null)
            {
                await userManager.CreateAsync(new ApplicationUser
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "Андрей",
                    Surname = "Матвеев",
                    Email = "andrey@hr.vision",
                    UserName = "andrey@hr.vision"
                });
                var user = await userManager.FindByEmailAsync("andrey@hr.vision");
                await userManager.AddPasswordAsync(user, "123");
            }

            if (await userManager.FindByEmailAsync("kravter7@gmail.com") is null)
            {
                await userManager.CreateAsync(new ApplicationUser
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "Костя",
                    Surname = "Науман",
                    Email = "kravter7@gmail.com",
                    UserName = "kravter7@gmail.com"
                });
                var user = await userManager.FindByEmailAsync("kravter7@gmail.com");
                await userManager.AddPasswordAsync(user, "321");
            }

            if (await manager.FindByClientIdAsync("E.Shop") is null)
            {
                await manager.CreateAsync(new OpenIddictApplicationDescriptor
                {
                    ClientId = "E.Shop",
                    ClientSecret = "901564A5-E7FE-42CB-B10D-61EF6A8F3654",
                    ConsentType = ConsentTypes.Explicit,
                    DisplayName = "E.Shop",
                    RedirectUris =
                    {
                        new Uri("http://localhost:3000/signed_in")
                    },
                    PostLogoutRedirectUris =
                    {
                        new Uri("http://localhost:3000/")
                    },
                    Permissions =
                    {
                        Permissions.Endpoints.Authorization,
                        Permissions.Endpoints.Logout,
                        Permissions.Endpoints.Token,
                        Permissions.GrantTypes.AuthorizationCode,
                        Permissions.GrantTypes.RefreshToken,
                        Permissions.ResponseTypes.Code,
                        Permissions.Scopes.Email,
                        Permissions.Scopes.Profile,
                        Permissions.Scopes.Roles,
                        Permissions.Prefixes.Scope + "demo_api"
                    },
                    Requirements =
                    {
                        Requirements.Features.ProofKeyForCodeExchange
                    }
                });
            }

            // Note: when using introspection instead of local token validation,
            // an application entry MUST be created to allow the resource server
            // to communicate with OpenIddict's introspection endpoint.
            if (await manager.FindByClientIdAsync("resource_server") is null)
            {
                await manager.CreateAsync(new OpenIddictApplicationDescriptor
                {
                    ClientId = "resource_server",
                    ClientSecret = "80B552BB-4CD8-48DA-946E-0815E0147DD2",
                    Permissions =
                    {
                        Permissions.Endpoints.Introspection
                    }
                });
            }

            // To test this sample with Postman, use the following settings:
            //
            // * Authorization URL: https://localhost:44395/connect/authorize
            // * Access token URL: https://localhost:44395/connect/token
            // * Client ID: postman
            // * Client secret: [blank] (not used with public clients)
            // * Scope: openid email profile roles
            // * Grant type: authorization code
            // * Request access token locally: yes
            if (await manager.FindByClientIdAsync("postman") is null)
            {
                await manager.CreateAsync(new OpenIddictApplicationDescriptor
                {
                    ClientId = "postman",
                    ConsentType = ConsentTypes.Systematic,
                    DisplayName = "Postman",
                    RedirectUris =
                    {
                        new Uri("urn:postman")
                    },
                    Permissions =
                    {
                        Permissions.Endpoints.Authorization,
                        Permissions.Endpoints.Device,
                        Permissions.Endpoints.Token,
                        Permissions.GrantTypes.AuthorizationCode,
                        Permissions.GrantTypes.DeviceCode,
                        Permissions.GrantTypes.Password,
                        Permissions.GrantTypes.RefreshToken,
                        Permissions.ResponseTypes.Code,
                        Permissions.Scopes.Email,
                        Permissions.Scopes.Profile,
                        Permissions.Scopes.Roles
                    }
                });
            }
        }

        static async Task RegisterScopesAsync(IServiceProvider provider)
        {
            var manager = provider.GetRequiredService<IOpenIddictScopeManager>();

            if (await manager.FindByNameAsync("demo_api") is null)
            {
                await manager.CreateAsync(new OpenIddictScopeDescriptor
                {
                    DisplayName = "Demo API access",
                    Name = "demo_api",
                    Resources =
                    {
                        "resource_server"
                    }
                });
            }
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
