//using Dotseed.EventBus.RabbitMQ.Options;
//using Dotseed.EventBus.RabbitMQ.Setup;
using E.Shop.Passport.Identity.Data;
using E.Shop.Passport.Identity.IntegrationEvents.EventHandlers;
using E.Shop.Passport.Identity.IntegrationEvents.Events;
using E.Shop.Passport.Identity.Models;
using E.Shop.Passport.Identity.Options;
using E.Shop.Passport.Identity.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Quartz;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace E.Shop.Passport.Identity;
public class Startup
{
    public Startup(IConfiguration configuration)
        => Configuration = configuration;

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        //services.Configure<RabbitMqSettings>(Configuration.GetSection("RabbitMqSettings"));
        services.Configure<UserActivationSettings>(Configuration.GetSection("UserActivationSettings"));
        services.Configure<DefaultHostSettings>(Configuration.GetSection("DefaultHostSettings"));

        services.AddControllersWithViews();

        services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
            {
                options.LoginPath = "/account/login";
            });

        //services.ConfigureNonBreakingSameSiteCookies();

        services.AddDbContext<ApplicationDbContext>(options =>
        {
            // Configure the context to use Microsoft SQL Server.
            options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection"));

            // Register the entity sets needed by OpenIddict.
            // Note: use the generic overload if you need
            // to replace the default OpenIddict entities.
            options.UseOpenIddict();
        });

        // Register the Identity services.
        services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequiredLength = 1;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireDigit = false;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        // OpenIddict offers native integration with Quartz.NET to perform scheduled tasks
        // (like pruning orphaned authorizations/tokens from the database) at regular intervals.
        services.AddQuartz(options =>
        {
            options.UseMicrosoftDependencyInjectionJobFactory();
            options.UseSimpleTypeLoader();
            options.UseInMemoryStore();
        });

        // Register the Quartz.NET service and configure it to block shutdown until jobs are complete.
        services.AddQuartzHostedService(options => options.WaitForJobsToComplete = true);

        services.AddCors(options =>
        {
            //TODO Define CORS policy
            options.AddPolicy("allowAll",
                corsBuilder =>
                {
                    corsBuilder.AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
        });

        services.AddOpenIddict()

            // Register the OpenIddict core components.
            .AddCore(options =>
            {
                // Configure OpenIddict to use the Entity Framework Core stores and models.
                // Note: call ReplaceDefaultEntities() to replace the default OpenIddict entities.
                options.UseEntityFrameworkCore()
                       .UseDbContext<ApplicationDbContext>();

                // Developers who prefer using MongoDB can remove the previous lines
                // and configure OpenIddict to use the specified MongoDB database:
                // options.UseMongoDb()
                //        .UseDatabase(new MongoClient().GetDatabase("openiddict"));

                // Enable Quartz.NET integration.
                options.UseQuartz();
            })

            // Register the OpenIddict client components.
            .AddClient(options =>
            {
                
                // Enable the redirection endpoint needed to handle the callback stage.
                //
                // Note: to mitigate mix-up attacks, it's recommended to use a unique redirection endpoint
                // address per provider, unless all the registered providers support returning an "iss"
                // parameter containing their URL as part of authorization responses. For more information,
                // see https://datatracker.ietf.org/doc/html/draft-ietf-oauth-security-topics#section-4.4.
                options.SetRedirectionEndpointUris("/callback/login/github");

                // Register the signing and encryption credentials used to protect
                // sensitive data like the state tokens produced by OpenIddict.
                options.AddDevelopmentEncryptionCertificate()
                       .AddDevelopmentSigningCertificate();

                // Register the ASP.NET Core host and configure the ASP.NET Core-specific options.
                options.UseAspNetCore()
                       .EnableStatusCodePagesIntegration()
                       .EnableRedirectionEndpointPassthrough();

                // Register the System.Net.Http integration and use the identity of the current
                // assembly as a more specific user agent, which can be useful when dealing with
                // providers that use the user agent as a way to throttle requests (e.g Reddit).
                options.UseSystemNetHttp()
                       .SetProductInformation(typeof(Startup).Assembly);

                // Register the Web providers integrations.
                options.UseWebProviders()
                       .UseGitHub(options =>
                       {
                           options.SetClientId("hr.vision")
                                  .SetClientSecret("da6bed851b75e317bf6b2cb67013679d9467c122")
                                  .SetRedirectUri("https://localhost:5000/callback/login/github");
                       });
            })

            // Register the OpenIddict server components.
            .AddServer(options =>
            {
                //options.AcceptAnonymousClients();
                // Enable the authorization, device, introspection,
                // logout, token, userinfo and verification endpoints.
                options.SetAuthorizationEndpointUris("/connect/authorize")
                       .SetDeviceEndpointUris("/connect/device")
                       .SetIntrospectionEndpointUris("/connect/introspect")
                       .SetLogoutEndpointUris("/connect/logout")
                       .SetTokenEndpointUris("/connect/token")
                       .SetUserinfoEndpointUris("/connect/userinfo")
                       .SetVerificationEndpointUris("/connect/verify");

                // Note: this sample uses the code, device code, password and refresh token flows, but you
                // can enable the other flows if you need to support implicit or client credentials.
                options.AllowAuthorizationCodeFlow()
                       .AllowDeviceCodeFlow()
                       .AllowPasswordFlow()
                       .AllowRefreshTokenFlow();

                // Mark the "email", "profile", "roles" and "demo_api" scopes as supported scopes.
                options.RegisterScopes(Scopes.Email, Scopes.Profile, Scopes.Roles, "demo_api");

                // Register the signing and encryption credentials.
                options.AddDevelopmentEncryptionCertificate()
                       .AddDevelopmentSigningCertificate();

                // Force client applications to use Proof Key for Code Exchange (PKCE).
                options.RequireProofKeyForCodeExchange();

                // Register the ASP.NET Core host and configure the ASP.NET Core-specific options.
                options.UseAspNetCore()
                       .EnableStatusCodePagesIntegration()
                       .EnableAuthorizationEndpointPassthrough()
                       .EnableLogoutEndpointPassthrough()
                       .EnableTokenEndpointPassthrough()
                       .EnableUserinfoEndpointPassthrough()
                       .EnableVerificationEndpointPassthrough()
                       .DisableTransportSecurityRequirement()
                       .EnableAuthorizationRequestCaching();// During development, you can disable the HTTPS requirement.

                // Note: if you don't want to specify a client_id when sending
                // a token or revocation request, uncomment the following line:
                //
                // options.AcceptAnonymousClients();

                // Note: if you want to process authorization and token requests
                // that specify non-registered scopes, uncomment the following line:
                //
                // options.DisableScopeValidation();

                // Note: if you don't want to use permissions, you can disable
                // permission enforcement by uncommenting the following lines:
                //
                // options.IgnoreEndpointPermissions()
                //        .IgnoreGrantTypePermissions()
                //        .IgnoreResponseTypePermissions()
                //        .IgnoreScopePermissions();

                // Note: when issuing access tokens used by third-party APIs
                // you don't own, you can disable access token encryption:
                //
                // options.DisableAccessTokenEncryption();
                options
                       .AddEphemeralEncryptionKey()
                       .AddEphemeralSigningKey()
                       .DisableAccessTokenEncryption();
            })

            // Register the OpenIddict validation components.
            .AddValidation(options =>
            {
                // Configure the audience accepted by this resource server.
                // The value MUST match the audience associated with the
                // "demo_api" scope, which is used by ResourceController.
                options.AddAudiences("resource_server");

                // Import the configuration from the local OpenIddict server instance.
                options.UseLocalServer();

                // Instead of validating the token locally by reading it directly,
                // introspection can be used to ask a remote authorization server
                // to validate the token (and its attached database entry).
                //
                // options.UseIntrospection()
                //        .SetIssuer("https://localhost:44395/")
                //        .SetClientId("resource_server")
                //        .SetClientSecret("80B552BB-4CD8-48DA-946E-0815E0147DD2");
                //
                // When introspection is used, System.Net.Http integration must be enabled.
                //
                // options.UseSystemNetHttp();

                // Register the ASP.NET Core host.
                options.UseAspNetCore();

                // For applications that need immediate access token or authorization
                // revocation, the database entry of the received tokens and their
                // associated authorizations can be validated for each API call.
                // Enabling these options may have a negative impact on performance.
                //
                // options.EnableAuthorizationEntryValidation();
                // options.EnableTokenEntryValidation();
            });

        //services.AddTransient<IEmailSender, AuthMessageSender>();
        //services.AddTransient<ISmsSender, AuthMessageSender>();

        // Register the worker responsible for seeding the database with the sample clients.
        // Note: in a real world application, this step should be part of a setup script.
        //services.AddHostedService<Worker>();

        services.AddAuthentication()
            .AddOpenIdConnect("oidc", options =>
            {
                options.RequireHttpsMetadata = false;
                options.Authority = "https://passport.hr.vision";
                options.ClientId = "hr.vision";
                options.UsePkce = true;
                options.ResponseType = "code id_token";
                options.CallbackPath = "/connect/authorize/callback";
                options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
            .AddOpenIdConnect("grabber-oidc", options =>
            {
                options.RequireHttpsMetadata = false;
                options.Authority = "http://localhost:5000";
                options.ClientId = "hr.vision.grabber";
                options.UsePkce = true;
                options.ResponseType = "code id_token";
                options.CallbackPath = "/connect/authorize/callback";
                options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
            .AddOpenIdConnect("workspace-app", options =>
            {
                options.RequireHttpsMetadata = false;
                options.Authority = "http://localhost:5000";
                options.ClientId = "hr.vision.workspace.app";
                options.UsePkce = true;
                options.ResponseType = "code id_token";
                options.CallbackPath = "/signin-oidc";
                options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            });

        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IActivationService, ActivationService>();
        services.AddScoped<IPasswordRestoreService, PasswordRestoreService>();

/*        var eventBusConfigurator = services.ConfigureEventBus();
        eventBusConfigurator
            .WithReceiveEndpoint<UserAddedIntegrationEventHandler, UserAddedIntegrationEvent>()
            .WithReceiveEndpoint<UserRemovedIntegrationEventHandler, UserRemovedIntegrationEvent>()
            .Inject();*/

        services.AddHostedService<Worker>();
    }

    public void Configure(IApplicationBuilder app)
    {
        app.UseCookiePolicy();

        app.UseDeveloperExceptionPage();

        app.UseStaticFiles();

        app.UseRouting();

        app.UseCors("allowAll");

        var forwardOptions = new ForwardedHeadersOptions
        {
            ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto,
            RequireHeaderSymmetry = false
        };

        forwardOptions.KnownNetworks.Clear();
        forwardOptions.KnownProxies.Clear();

        // ref: https://github.com/aspnet/Docs/issues/2384
        app.UseForwardedHeaders(forwardOptions);

        app.UseRequestLocalization(options =>
        {
            options.AddSupportedCultures("en-US", "fr-FR");
            options.AddSupportedUICultures("en-US", "fr-FR");
            options.SetDefaultCulture("en-US");
        });

        app.UseAuthentication();
        app.UseAuthorization();
       
       

        app.UseEndpoints(options =>
        {
            options.MapControllers();
            options.MapDefaultControllerRoute();
        });

        string connectionString = Configuration.GetConnectionString("DefaultConnection");

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseNpgsql(
                connectionString,
                b => b
                    .MigrationsAssembly(typeof(Program).Assembly.FullName)
                    .MigrationsHistoryTable(
                        "__EFMigrationsHistory",
                        "public"))
            .Options;

        using (var appContext = new ApplicationDbContext(options))
        {
            //appContext.Database.Migrate();
        }
    }
}
