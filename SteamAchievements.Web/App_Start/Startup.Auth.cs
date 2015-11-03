using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security.Facebook;
using Owin;
using SteamAchievements.Data;
using SteamAchievements.Web.Properties;

namespace SteamAchievements.Web
{
    public partial class Startup
    {
        // For more information on configuring authentication, please visit http://go.microsoft.com/fwlink/?LinkId=301864
        public void ConfigureAuth(IAppBuilder app)
        {
            // Configure the db context, user manager and signin manager to use a single instance per request

            app.CreatePerOwinContext(() => new SteamContext());
            app.CreatePerOwinContext<UserManager<steam_User, int>>((options, context) =>
            {
                var userStore = new UserStore<steam_User, Role, int, UserLogin, UserRole, UserClaim>(context.Get<SteamContext>());
                var manager = new UserManager<steam_User, int>(userStore);

                manager.UserValidator = new UserValidator<steam_User, int>(manager) {RequireUniqueEmail = false, AllowOnlyAlphanumericUserNames = false};
                manager.PasswordValidator = new PasswordValidator();

                manager.UserLockoutEnabledByDefault = false;

                var dataProtectionProvider = options.DataProtectionProvider;
                if (dataProtectionProvider != null)
                {
                    manager.UserTokenProvider =
                        new DataProtectorTokenProvider<steam_User, int>(dataProtectionProvider.Create("ASP.NET Identity"));
                }

                return manager;
            });

            app.CreatePerOwinContext<SignInManager<steam_User, int>>((options, context) =>
            {
                var manager = new SignInManager<steam_User, int>(context.Get<UserManager<steam_User, int>>(), context.Authentication);

                return manager;
            });


            // Enable the application to use a cookie to store information for the signed in user
            // and to use a cookie to temporarily store information about a user logging in with a third party login provider
            // Configure the sign in cookie
            //TODO: app.UseCookieAuthentication(new CookieAuthenticationOptions
            //                                {
            //                                    AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
            //                                    LoginPath = new PathString("/Account/Login"),
            //                                    Provider = new CookieAuthenticationProvider
            //                                                   {
            //                                                       // Enables the application to validate the security stamp when the user logs in.
            //                                                       // This is a security feature which is used when you change a password or add an external login to your account.  
            //                                                       OnValidateIdentity =
            //                                                           SecurityStampValidator
            //                                                           .OnValidateIdentity
            //                                                           <ApplicationUserManager, ApplicationUser>(
            //                                                                                                     TimeSpan
            //                                                                                                         .FromMinutes
            //                                                                                                         (30),
            //                                                                                                     (
            //                                                                                                         manager,
            //                                                                                                         user)
            //                                                                                                     =>
            //                                                                                                     user
            //                                                                                                         .GenerateUserIdentityAsync
            //                                                                                                         (manager))
            //                                                   }
            //                                });
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            // Enables the application to temporarily store user information when they are verifying the second factor in the two-factor authentication process.
            //app.UseTwoFactorSignInCookie(DefaultAuthenticationTypes.TwoFactorCookie, TimeSpan.FromMinutes(5));

            // Enables the application to remember the second login verification factor such as phone or email.
            // Once you check this option, your second step of verification during the login process will be remembered on the device where you logged in from.
            // This is similar to the RememberMe option when you log in.
            //app.UseTwoFactorRememberBrowserCookie(DefaultAuthenticationTypes.TwoFactorRememberBrowserCookie);

            // Uncomment the following lines to enable logging in with third party login providers

            //app.UseTwitterAuthentication(
            //   consumerKey: "",
            //   consumerSecret: "");

            //app.UseFacebookAuthentication(
            //   appId: "",
            //   appSecret: "");

            //app.UseGoogleAuthentication(new GoogleOAuth2AuthenticationOptions()
            //{
            //    ClientId = "",
            //    ClientSecret = ""
            //});

            if (!string.IsNullOrEmpty(Settings.Default.FacebookAppId))
            {
                var options = new FacebookAuthenticationOptions
                {
                    Provider = new FacebookAuthenticationProvider
                    {
                        OnAuthenticated = context =>
                        {
                            context.Identity.AddClaim(new Claim("AccessToken", context.AccessToken));
                            
                            return Task.FromResult(0);
                        }
                    },
                    AppId = Settings.Default.FacebookAppId,
                    AppSecret = Settings.Default.FacebookAppSecret
                };
                app.UseFacebookAuthentication(options);
            }
        }
    }
}