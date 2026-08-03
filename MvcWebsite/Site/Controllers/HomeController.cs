using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Site.Controllers;

[Route("")]
public class HomeController : Controller
{
    [HttpGet, Route("")]
    public async Task<IActionResult> Index()
    {
        return View();
    }

    [HttpGet, Route("sign-out"), AllowAnonymous]
    public IActionResult LogOutOfSystem()
    {
        return SignOut(
            new AuthenticationProperties
            {
                RedirectUri = Url.Action(nameof(Index), "Home")
            },
            CookieAuthenticationDefaults.AuthenticationScheme,
            OpenIdConnectDefaults.AuthenticationScheme);
    }
}
