// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

using E.Shop.Passport.Identity.Models;
using E.Shop.Passport.Identity.Services;
using E.Shop.Passport.Identity.Controllers.Account.Models;
using E.Shop.Passport.Identity.Data;
using E.Shop.Passport.Identity.Options;
using E.Shop.Passport.Identity.Services.Models;


namespace E.Shop.Passport.Identity.Controllers.Account
{
    /// <summary>
    /// This sample controller implements a typical login/logout/provision workflow for local and external accounts.
    /// The login service encapsulates the interactions with the user data store. This data store is in-memory only and cannot be used for production!
    /// The interaction service provides a way for the UI to communicate with identityserver for validation and context retrieval
    /// </summary>
    [SecurityHeaders]
    [AllowAnonymous]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IAuthenticationSchemeProvider _schemeProvider;
        private readonly IPasswordRestoreService _passwordRestoreService;
        private static bool _databaseChecked;
        private readonly string _defaultFrontEndHost;
        private readonly string _thisHost;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ApplicationDbContext applicationDbContext,
            IOptions<DefaultHostSettings> options,
            IAuthenticationSchemeProvider schemeProvider,
            IPasswordRestoreService passwordRestoreService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _schemeProvider = schemeProvider;
            _defaultFrontEndHost = options.Value.MainFrontEndHost;
            _thisHost = options.Value.ThisHost;
            _applicationDbContext = applicationDbContext;
            _passwordRestoreService = passwordRestoreService;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string ReturnUrl)
        {
            ViewBag.ReturnUrl = ReturnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string button, string returnUrl)
        {
            EnsureDatabaseCreated(_applicationDbContext);
            switch (button)
            {
                case "forgot_password":
                    model.RestorePassword = true;
                    model.RestoreLinkSent = false;
                    return WhoForgotPassword();

                case "back":
                    model.RestorePassword = false;
                    model.RestoreLinkSent = false;
                    break;
            }

            if (ModelState.IsValid)
            {
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var result = await _signInManager.PasswordSignInAsync(model.Username, model.Password, model.RememberMe, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    return RedirectToLocal(model.ReturnUrl);
                }
                if (result.IsLockedOut)
                {
                    return View("Lockout");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return View(model);
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ChangeForgotPassword(string code = "", string email = "")
        {      

            if (code.Length + email.Length is 0) return View();

            var model = new ChangeForgotPasswordModel();

            model.Token = code;
            model.Email = email;

            return View("ChangeForgotPassword", model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeForgotPassword(ChangeForgotPasswordModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (model.Token != await _userManager.GetSecurityStampAsync(user))
                throw new Exception("Temp");
                //View(new CustomErrorViewModel( "Some error", "Some description"));

            var result = await _userManager.ResetPasswordAsync(user, await _userManager.GeneratePasswordResetTokenAsync(user), model.NewPassword);

            _applicationDbContext.Users.Update(user);

            await _applicationDbContext.SaveChangesAsync();

            return Redirect(_defaultFrontEndHost);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user is null)
                return View(ChangePasswordModel.GetThisWithError(ErrorChangePassword.errorEmail));
            
            if (model.Password != model.NewPassword)
                return View(ChangePasswordModel.GetThisWithError(ErrorChangePassword.errorPassword));

            await _userManager.ResetPasswordAsync(
                user,
                await _userManager.GeneratePasswordResetTokenAsync(user),
                model.NewPassword);

            return View(_defaultFrontEndHost);
        }

        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordModel model)
        {
            if (model.Email is null)
                return View("WhoForgotPassword", 
                        new WhoForgotPasswordModel 
                        { 
                            Email = String.Empty,
                            Message = "Email empty"
                        }); 

            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user is null)
                return View("WhoForgotPassword",
                        new WhoForgotPasswordModel
                        {
                            Email = String.Empty,
                            Message = "Email not found"
                        });

            await _userManager.UpdateSecurityStampAsync(user);

            var code = await _userManager.GetSecurityStampAsync(user);

            await SendCodeToEmail(user.Name, user.Email, code, $"{_thisHost}/Account/ChangeForgotPassword?code={code}&email={user.Email}");

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            return Redirect(_defaultFrontEndHost);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout(LogoutInputModel model)
        {
            await _signInManager.SignOutAsync();

            return Redirect(_defaultFrontEndHost);
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Activation(string email, string code)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null) return Redirect(_defaultFrontEndHost);

            var model = new ActivationModel {Email = email, PasswordToken = code, NewPassword = String.Empty };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Activation(ActivationModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);

            await _userManager.ResetPasswordAsync(user, model.PasswordToken, model.NewPassword);

            return Redirect(_defaultFrontEndHost);
        }

        [HttpGet]
        public IActionResult GoFromActivationToMainHost()
        {
            return Redirect(_defaultFrontEndHost);
        }

        /*****************************************/
        /* helper APIs for the AccountController */
        /*****************************************/
        

        private IActionResult WhoForgotPassword()
        {        
            return View("WhoForgotPassword");
        }

        private static void EnsureDatabaseCreated(ApplicationDbContext context)
        {
            if (!_databaseChecked)
            {
                _databaseChecked = true;
                context.Database.EnsureCreated();
            }
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);         
            else
                return Redirect(_defaultFrontEndHost);          
        }

        private async Task SendCodeToEmail(string name, string email, string code, string data)
        {
            var _emailService = new EmailService();

            //await _userManager.Ad

            var mail = new Email { Name = name, Address = email };

            var mailData = new EmailData 
            {
                Subject = "Fixing access", 
                Data = data, 
                HtmlContent = "" 
            };

            await _emailService.Send(mail, mailData);
        }
    }
}
