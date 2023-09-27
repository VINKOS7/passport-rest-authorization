using Microsoft.AspNetCore.Mvc;

namespace E.Shop.Passport.Identity.Controllers.Account;

public class RestorePasswordController : Controller
{
    [HttpGet]
    public ActionResult RestorePassword()
    {
        return View("~/Views/Account/RestorePassword.cshtml");
    }
}