using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using dplo.Service.MSGraphUtils;
using Microsoft.Identity.Client;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Logging;
using CoreSystem.Controllers.Planx;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Logging;
using Umbraco.Cms.Core.Routing;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Infrastructure.Persistence;
using Umbraco.Cms.Web.Common.Controllers;
using Umbraco.Cms.Web.Website.Controllers;
namespace CoreSystem.Controllers
{
    [AllowAnonymous]
    public class AzureAccountController : SurfaceController
    {
        ///https://github.com/AzureAD/microsoft-identity-web/blob/master/src/Microsoft.Identity.Web.UI/Areas/MicrosoftIdentity/Controllers/AccountController.cs

        public AzureAccountController(
            IUmbracoContextAccessor umbracoContextAccessor,
            IUmbracoDatabaseFactory databaseFactory,
            ServiceContext services,
            AppCaches appCaches,
            IProfilingLogger profilingLogger,
            IPublishedUrlProvider publishedUrlProvider,
            ILogger<AzureAccountController> logger)
            : base(umbracoContextAccessor, databaseFactory, services, appCaches, profilingLogger, publishedUrlProvider)
        {
            _logger = logger;
        }

        private readonly ILogger<AzureAccountController> _logger;

        public ActionResult IsLoggedIn()
        {
            var user = HttpContext.User.Identity;
            ViewData["IsLoggedIn"] = user.IsAuthenticated;
            ViewBag.IsLoggedIn = user.IsAuthenticated;

            try
            {
                if (user.IsAuthenticated)
                {
                    Response.StatusCode = (int)HttpStatusCode.OK;
                }
                else
                {
                    Response.StatusCode = (int)HttpStatusCode.Forbidden;
                }
            }
            catch (Exception ex)
            {
                //sdfkj
            }
            return View();
        }


        #region AzureB2C

        /*
         *  Called when requesting to sign up or sign in
         */
        //[Route("~/AzureAccount/SignUpSignIn/")]
        public void SignUpSignIn(string redirectUrl)
        {
            redirectUrl = redirectUrl ?? "/";
            IdentityModelEventSource.ShowPII = true;
            // Use the default policy to process the sign up / sign in flow
            HttpContext.Authentication.Challenge(new AuthenticationProperties { RedirectUri = redirectUrl });
            return;
        }

        /*
         *  Called when requesting to edit a profile
         */
        public void EditProfile()
        {
            //if (Request.IsAuthenticated)
            //{
            //    // Let the middleware know you are trying to use the edit profile policy (see OnRedirectToIdentityProvider in Startup.Auth.cs)
            //    HttpContext.GetOwinContext().Set("Policy", Globals.EditProfilePolicyId);

            //    // Set the page to redirect to after editing the profile
            //    var authenticationProperties = new AuthenticationProperties { RedirectUri = "/" };
            //    HttpContext.GetOwinContext().Authentication.Challenge(authenticationProperties);

            //    return;
            //}

            //Response.Redirect("/");
        }

        /*
         *  Called when requesting to reset a password
         */
        public void ResetPassword()
        {
            // Let the middleware know you are trying to use the reset password policy (see OnRedirectToIdentityProvider in Startup.Auth.cs)
            HttpContext.GetOwinContext().Set("Policy", Globals.ResetPasswordPolicyId);

            // Set the page to redirect to after changing passwords
            var authenticationProperties = new AuthenticationProperties { RedirectUri = "/" };
            HttpContext.GetOwinContext().Authentication.Challenge(authenticationProperties);

            return;
        }

        /*
         *  Called when requesting to sign out
         */
        public async Task SignOut()
        {
            // To sign out the user, you should issue an OpenIDConnect sign out request.
            _logger.LogInformation("Maxfactor Signout");
            try
            {
                if (User.Identity.IsAuthenticated)
                {
                    // Remove all tokens from MSAL's cache
                    var clientApp = MsalAppBuilder.BuildConfidentialClientApplication();
                    string accountId = ClaimsPrincipal.Current.GetB2CMsalAccountIdentifier(Globals.SignInPolicyId);
                    IAccount account = await clientApp.GetAccountAsync(accountId);
                    if (account != null)
                    {
                        await clientApp.RemoveAsync(account);
                        _logger.LogInformation("Maxfactor Signout found account");
                    }

                    // Then sign-out from OWIN
                    IEnumerable<AuthenticationDescription> authTypes =
                        HttpContext.GetOwinContext().Authentication.GetAuthenticationTypes();
                    HttpContext.GetOwinContext().Authentication
                        .SignOut(authTypes.Select(t => t.AuthenticationType).ToArray());
                    Request.GetOwinContext().Authentication.GetAuthenticationTypes();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Error signing out", ex);
            }
    }



        #endregion
        #region Helpers
        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
        }

        internal class ExternalLoginResult : ActionResult
        {
            public ExternalLoginResult(string provider, string returnUrl)
            {
                Provider = provider;
                ReturnUrl = returnUrl;
            }

            public string Provider { get; private set; }
            public string ReturnUrl { get; private set; }

            //public override void ExecuteResult(ControllerContext context)
            //{
            //    //OAuthWebSecurity.RequestAuthentication(Provider, ReturnUrl);
            //}
        }

        //private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        //{
        //    // See http://go.microsoft.com/fwlink/?LinkID=177550 for
        //    // a full list of status codes.
        //    switch (createStatus)
        //    {
        //        case MembershipCreateStatus.DuplicateUserName:
        //            return "User name already exists. Please enter a different user name.";

        //        case MembershipCreateStatus.DuplicateEmail:
        //            return "A user name for that e-mail address already exists. Please enter a different e-mail address.";

        //        case MembershipCreateStatus.InvalidPassword:
        //            return "The password provided is invalid. Please enter a valid password value.";

        //        case MembershipCreateStatus.InvalidEmail:
        //            return "The e-mail address provided is invalid. Please check the value and try again.";

        //        case MembershipCreateStatus.InvalidAnswer:
        //            return "The password retrieval answer provided is invalid. Please check the value and try again.";

        //        case MembershipCreateStatus.InvalidQuestion:
        //            return "The password retrieval question provided is invalid. Please check the value and try again.";

        //        case MembershipCreateStatus.InvalidUserName:
        //            return "The user name provided is invalid. Please check the value and try again.";

        //        case MembershipCreateStatus.ProviderError:
        //            return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

        //        case MembershipCreateStatus.UserRejected:
        //            return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

        //        default:
        //            return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
        //    }
        //}
        #endregion


    }
}
