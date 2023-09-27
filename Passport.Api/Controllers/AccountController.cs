using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

using MediatR;

using Passport.Api.Requests;
using Passport.Api.Responses;

namespace Passport.Api.Controllers;

[Route("passport/account")]
public class AccountController : Controller
{
    private readonly IMediator _mediator;

    public AccountController(IMediator mediator) => _mediator = mediator;

    [AllowAnonymous, HttpPost("signin")]
    public async Task<SignInResponse> SignIn([FromBody] SignInEmailRequest request) => await _mediator.Send(request);
    

    [AllowAnonymous, HttpPost("signup")]
    public async Task<SignUpResponse> SignUp([FromBody] SignUpEmailRequest request) => await _mediator.Send(request);
    

    [AllowAnonymous, HttpPost("activation")]
    public async Task<ActivationResponse> Activation([FromBody] ActivationRequest request) => await _mediator.Send(request);
    

    [AllowAnonymous, HttpPost("forgot")] 
    public async Task<ForgotPasswordResponse> Forgot([FromBody] ForgotPasswordRequest request) => await _mediator.Send(request);


    [AllowAnonymous, HttpPost("activate")] 
    public async Task<ActivateNewPasswordResponse> PasswordActivate([FromBody] ActivateNewPasswordRequest request) => await _mediator.Send(request);

    [Authorize, HttpDelete("signout")]
    public async new Task<SignOutResponse> SignOut()
    {//тут по идее надо изменить куки клиента
        return await _mediator.Send(new SignOutRequest(Request.Headers.Authorization));
    }
}
