using System.Net;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Options;

using MediatR;

using Passport.Api.Options;
using Passport.Api.Responses;
using Passport.Api.Requests;
using Passport.Domain.Aggregates.Account;
using Passport.Passport.Api.Requests.AccountRequests.Handlers;
using Passport.Domain.Services.UserAgentParser;

namespace Passport.Passport.Api.Requests.Handlers;

public class SignInEmailRequestHandler : IRequestHandler<SignInEmailRequest, SignInResponse>
{
    private readonly JWTOptions _jwtOptions;
    private readonly IAccountRepo _accountRepo;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<SignInEmailRequestHandler> _logger;

    public SignInEmailRequestHandler(
        IAccountRepo accountRepo,
        IHttpContextAccessor httpContextAccessor,
        IOptions<JWTOptions> jwtOptions,
        ILogger<SignInEmailRequestHandler> logger)
    {
        _accountRepo = accountRepo;
        _httpContextAccessor = httpContextAccessor;
        _jwtOptions = jwtOptions.Value;
        _logger = logger;
    }

    public async Task<SignInResponse> Handle(SignInEmailRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var account = await _accountRepo.FindByEmailAsync(request.Email);

            if(account is null) throw new BadHttpRequestException("Account not found");

            if(account.Password != request.Password) throw new BadHttpRequestException("Bad password");

            var jwtToken = new JwtToken(account.Id, _jwtOptions.SecretKey, _jwtOptions.Issuer, _jwtOptions.ExpiresHours).Value;

            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwtToken);

            var userAgent = _httpContextAccessor.HttpContext.Request.Headers.UserAgent.ToString();

            var operationSystem = UserAgentParser.GetOperatingSystem(userAgent);

            if (account.Devices is null) account.Devices = new();
            else
            {
                var deviceForRemove = account.Devices.Where(d => 
                    DateTime.UnixEpoch 
                    + TimeSpan.FromSeconds(long.Parse(new JwtSecurityTokenHandler().ReadJwtToken(d.Token).Claims.First(claim => claim.Type.Equals("exp")).Value)) 
                    <= DateTime.UtcNow)
                    .ToList();

                if (deviceForRemove.Count() > 0) foreach (var device in deviceForRemove) account.Devices.Remove(device);
            }

            account.Devices.Add(new()
            {
                Id = Guid.NewGuid(),
                Name = operationSystem,
                Version = UserAgentParser.GetOsVersion(userAgent, userAgent),
                Token = encodedJwt,
                OnlineAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            });

            await _accountRepo.UnitOfWork.SaveChangesAsync(cancellationToken);

            return new SignInResponse(Token: encodedJwt, Message: "Access true");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error with signin. Error: {ex}");

            throw new HttpRequestException("Error with attemping signin.", ex, HttpStatusCode.InternalServerError);
        }
    }
}
