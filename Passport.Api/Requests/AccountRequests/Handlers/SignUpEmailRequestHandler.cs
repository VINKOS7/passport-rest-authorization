using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

using MediatR;

using Passport.Api.Requests;
using Passport.Api.Responses;
using Passport.Domain.Aggregates.Account;
using Passport.Api.Services;

namespace Passport.Passport.Api.Requests.Handlers;

public class SignUpEmailRequestHandler : IRequestHandler<SignUpEmailRequest, SignUpResponse>
{
    private readonly IEmailService _emailService;
    private readonly IAccountRepo _accountRepo;
    private readonly ILogger<SignUpEmailRequestHandler> _logger;
    private readonly IMediator _mediator;
    public SignUpEmailRequestHandler(
        IMediator mediator,
        IEmailService emailService,
        IAccountRepo accountRepo,
        ILogger<SignUpEmailRequestHandler> logger)
    {
        _mediator = mediator;
        _emailService = emailService;
        _accountRepo = accountRepo;
        _logger = logger;
    }

    public async Task<SignUpResponse> Handle(SignUpEmailRequest request, CancellationToken cancellationToken)
    {
        try
        {
            if (!new EmailAddressAttribute().IsValid(request.Email) && request.Email is not null) throw new BadHttpRequestException("Email not valid");

            if (!new Regex(@"^[\w$&_]{5,24}$").IsMatch(request.Nickname)) throw new BadHttpRequestException("Nick not valid");

            if (!new Regex(@"^\S{4,23}$").IsMatch(request.Password)) throw new BadHttpRequestException("Password not valid");

            if (await _accountRepo.FindByEmailAsync(request.Email) is not null) throw new BadHttpRequestException("Email is busy");

            if (await _accountRepo.FindByNickNameAsync(request.Nickname) is not null) throw new BadHttpRequestException("Nick is busy");

            var account = Account.From(request);

            await _accountRepo.AddAsync(account);

            await _emailService.SendEmailAsync(
                account.Nickname,
                account.Email,
                "SignIn Library",
                $"Activation code: {account.ActivationCode}");

            await _accountRepo.UnitOfWork.SaveEntitiesAsync(cancellationToken);

            return new SignUpResponse(Message: "Code send to you email");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error with sign up. Error: {ex}");

            throw;
        }
    }
}
