﻿using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using SteamAchievements.Data;

namespace SteamAchievements.Web.Controllers
{
    [Authorize]
    public class AccountController : UserController
    {
        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider,
                                       Url.Action("ExternalLoginCallback", "Account", new {ReturnUrl = returnUrl}));
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            var user = await UserManager.FindByNameAsync(loginInfo.Login.ProviderKey);
            if (user == null)
            {
                long facebookId = Convert.ToInt64(loginInfo.Login.ProviderKey);
                user = new steam_User {UserName = loginInfo.Login.ProviderKey, FacebookUserId = facebookId};
                var createResult = await UserManager.CreateAsync(user);
                if (!createResult.Succeeded)
                {
                    AddErrors(createResult);
                    return View("ExternalLoginFailure");
                }
            }

            var logins = await UserManager.GetLoginsAsync(user.Id);
            if (!logins.Any())
            {
                var addResult = await UserManager.AddLoginAsync(user.Id, loginInfo.Login);
                if (!addResult.Succeeded)
                {
                    AddErrors(addResult);
                    return View("ExternalLoginFailure");
                }
            }

            //var userClaims = await UserManager.GetClaimsAsync(user.Id);
            //foreach (var claim in loginInfo.ExternalIdentity.Claims)
            //{
            //    if (userClaims.Where(e => e.Type == claim.Type).FirstOrDefault() == null)
            //    {
            //        await UserManager.AddClaimAsync(user.Id, claim);
            //    }
            //}

            //if (String.IsNullOrEmpty(loginInfo.Email))
            //{
            //    loginInfo.Email = "none@example.com";
            //}

            //TODO:

            /* [ArgumentNullException: Value cannot be null.
Parameter name: value]
   System.Security.Claims.Claim..ctor(String type, String value, String valueType, String issuer, String originalIssuer, ClaimsIdentity subject, String propertyKey, String propertyValue) +14015857
   System.Security.Claims.Claim..ctor(String type, String value) +73
   Microsoft.AspNet.Identity.<CreateAsync>d__0.MoveNext() +977
   System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task task) +13908500
   System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task task) +61
   Microsoft.AspNet.Identity.Owin.<SignInAsync>d__2.MoveNext() +266
            */
            await SignInManager.ExternalSignInAsync(loginInfo, true);
            return RedirectToLocal("~/");
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new steam_User
                {
                    UserName = info.DefaultUserName,
                    Email = info.Email,
                    AccessToken = info.ExternalIdentity.FindFirstValue("AccessToken"),
                    FacebookUserId = Convert.ToInt64(info.ExternalIdentity.GetUserId())
                };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, false, false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut();
            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        #region Helpers

        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get { return HttpContext.GetOwinContext().Authentication; }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties {RedirectUri = RedirectUri};
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }

        #endregion
    }
}