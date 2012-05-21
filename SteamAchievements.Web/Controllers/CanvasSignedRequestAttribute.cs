using System;
using System.Web;
using System.Web.Mvc;
using SteamAchievements.Services;
using SteamAchievements.Services.Models;

namespace SteamAchievements.Web.Controllers
{
    /// <summary>
    /// Parses the signed_request parameter and sets the user session value.
    /// </summary>
    public class CanvasSignedRequestAttribute : ActionFilterAttribute
    {
        private const string _userSettingsKey = "UserSettings";
        private readonly IFacebookClientService _facebookClient;
        private readonly IUserService _userService;

        public CanvasSignedRequestAttribute(IFacebookClientService facebookClient, IUserService userService)
        {
            _facebookClient = facebookClient;
            _userService = userService;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            HttpContextBase context = filterContext.HttpContext;
            string signedRequestValue = context.Request["signed_request"];
            HttpSessionStateBase session = context.Session;
            if (session[_userSettingsKey] == null && !String.IsNullOrEmpty(signedRequestValue))
            {
                SignedRequest signedRequest = _facebookClient.ParseSignedRequest(signedRequestValue);

                User user = _userService.GetUser(signedRequest.UserId);
                if (user == null)
                {
                    user = new User {FacebookUserId = signedRequest.UserId};
                }

                user.AccessToken = signedRequest.AccessToken;
                user.SignedRequest = signedRequestValue;

                session[_userSettingsKey] = user;
            }
        }
    }
}