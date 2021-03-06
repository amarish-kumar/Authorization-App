﻿using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AuthorizationLab.Controllers
{
    [AllowAnonymous]
    public class AccountController : Controller
    {
        public async Task<ActionResult> Login(string returnUrl = null)
        {
            const string issuer = "https://contoso.com";
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, "Elvis", ClaimValueTypes.String, issuer),
                new Claim(ClaimTypes.Role, "Administrator", ClaimValueTypes.String, issuer),
                new Claim("EmployeeId", "123", ClaimValueTypes.String, issuer),
                new Claim(ClaimTypes.DateOfBirth, "1970-06-08", ClaimValueTypes.Date),
                new Claim("BadgeNumber", "123456", ClaimValueTypes.String, issuer),
            };

            var userIdentity = new ClaimsIdentity("SuperSecureLogin");
            userIdentity.AddClaims(claims);
            var userPrincipal = new ClaimsPrincipal(userIdentity);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                userPrincipal,
                new AuthenticationProperties
                {
                    ExpiresUtc = DateTime.UtcNow.AddMinutes(20),
                    IsPersistent = false,
                    AllowRefresh = false
                });

            return RedirectToLocal(returnUrl);
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            return Url.IsLocalUrl(returnUrl)
                ? Redirect(returnUrl)
                : RedirectToAction("Index", "Home") as ActionResult;
        }

        public ActionResult Forbidden()
        {
            return View();
        }
    }
}
