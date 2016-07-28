using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Facebook;
using Owin;
using SteamAchievements.Data;
using SteamAchievements.Web.Properties;

namespace SteamAchievements.Web
{
    public partial class Startup
    {
        private class ApplicationIdentityFactory : ClaimsIdentityFactory<steam_User, int>
        {
            public async override Task<ClaimsIdentity> CreateAsync(UserManager<steam_User, int> manager, steam_User user, string authenticationType)
            {
                var claims = new[]
                             {
                                 new Claim(UserIdClaimType, user.Id.ToString()),
                                 new Claim(UserNameClaimType, user.UserName)
                             };

                var identity = new ClaimsIdentity(claims, authenticationType, UserNameClaimType, RoleClaimType);

                if (manager.SupportsUserRole)
                {
                    var roles = await manager.GetRolesAsync(user.Id);
                    identity.AddClaims(roles.Select(r => new Claim(RoleClaimType, r)));
                }

                if (manager.SupportsUserClaim)
                {
                    identity.AddClaims(await manager.GetClaimsAsync(user.Id));
                }

                return identity;
            }
        }

        // For more information on configuring authentication, please visit http://go.microsoft.com/fwlink/?LinkId=301864
        public void ConfigureAuth(IAppBuilder app)
        {
            // Configure the db context, user manager and signin manager to use a single instance per request

            app.CreatePerOwinContext(() => new SteamContext());
            app.CreatePerOwinContext<UserManager<steam_User, int>>((options, context) =>
            {
                var userStore =
                    new UserStore<steam_User, Role, int, UserLogin, UserRole, UserClaim>(context.Get<SteamContext>());
                var manager = new UserManager<steam_User, int>(userStore);

                manager.UserValidator = new UserValidator<steam_User, int>(manager)
                {
                    RequireUniqueEmail = false,
                    AllowOnlyAlphanumericUserNames = false
                };
                manager.PasswordValidator = new PasswordValidator();

                manager.UserLockoutEnabledByDefault = false;

                var dataProtectionProvider = options.DataProtectionProvider;
                if (dataProtectionProvider != null)
                {
                    manager.UserTokenProvider =
                        new DataProtectorTokenProvider<steam_User, int>(dataProtectionProvider.Create("ASP.NET Identity"));
                }

                manager.ClaimsIdentityFactory = new ApplicationIdentityFactory();

                return manager;
            });

            app.CreatePerOwinContext<SignInManager<steam_User, int>>((options, context) =>
            {
                var manager = new SignInManager<steam_User, int>(context.Get<UserManager<steam_User, int>>(),
                    context.Authentication);

                return manager;
            });


            // Enable the application to use a cookie to store information for the signed in user
            // and to use a cookie to temporarily store information about a user logging in with a third party login provider
            // Configure the sign in cookie

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Login"),
                CookieName = ".fbsa",
                Provider = new CookieAuthenticationProvider
                {
                // Enables the application to validate the security stamp when the user logs in.
                // This is a security feature which is used when you change a password or add an external login to your account.  
                OnValidateIdentity =
                        SecurityStampValidator.OnValidateIdentity<UserManager<steam_User, int>, steam_User, int>(
                            TimeSpan.FromMinutes(30),
                            CreateIdentityAsync, identity => identity.GetUserId<int>())
                }
            });
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            app.SetDefaultSignInAsAuthenticationType(DefaultAuthenticationTypes.ExternalCookie);

            if (!string.IsNullOrEmpty(Settings.Default.FacebookAppId))
            {
                var options = new FacebookAuthenticationOptions
                {
                    Provider = new FacebookAuthenticationProvider
                    {
                        OnAuthenticated = context =>
                        {
                            context.Identity.AddClaim(new Claim("AccessToken", context.AccessToken));
                            context.Identity.AddClaim(new Claim("FacebookUserId", context.Id));

                            return Task.FromResult(0);
                        }
                    },
                    AppId = Settings.Default.FacebookAppId,
                    AppSecret = Settings.Default.FacebookAppSecret
                };
                app.UseFacebookAuthentication(options);
            }
        }

        private async Task<ClaimsIdentity> CreateIdentityAsync(UserManager<steam_User, int> manager, steam_User user)
        {
            return await manager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
        }
    }
}