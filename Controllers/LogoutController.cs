using System.Security.Claims;
using dplo.Service;
using dplo.Service.MSGraphUtils;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using Microsoft.Identity.Web;
using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Logging;
using Umbraco.Cms.Core.Routing;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Infrastructure.Persistence;
using Umbraco.Cms.Web.Website.Controllers;

namespace diam_planogram.Controllers
{
    public class LogoutController : SurfaceController
    {


        #region Services, managers


        public IPlanogramService _planogramService;
        public ICountryService _countryService;

        #endregion
        ////[MvcAuthorize]
        public LogoutController(IUmbracoContextAccessor umbracoContextAccessor, IUmbracoDatabaseFactory databaseFactory, ServiceContext services, AppCaches appCaches, IProfilingLogger profilingLogger, IPublishedUrlProvider publishedUrlProvider, IPlanogramService planogramService, ICountryService countryService) : base(umbracoContextAccessor, databaseFactory, services, appCaches, profilingLogger, publishedUrlProvider)
        {
            _planogramService = planogramService;
            _countryService = countryService;
        }

        public async Task<IActionResult> Logout()
        {
            // To sign out the user, you should issue an OpenIDConnect sign out request.
            //if (Request.IsAuthenticated)
            //{
            // Remove all tokens from MSAL's cache
            var clientApp = MsalAppBuilder.BuildConfidentialClientApplication();
            string accountId = ClaimsPrincipal.Current.GetB2CMsalAccountIdentifier(Globals.SignInPolicyId);
            IAccount account = await clientApp.GetAccountAsync(accountId);
            if (account != null)
            {
                await clientApp.RemoveAsync(account);
            }

            // Then sign-out from OWIN
            IEnumerable<AuthenticationDescription> authTypes = HttpContext.GetOwinContext().Authentication.GetAuthenticationTypes();
            HttpContext.GetOwinContext().Authentication.SignOut(authTypes.Select(t => t.AuthenticationType).ToArray());
            Request.GetOwinContext().Authentication.GetAuthenticationTypes();
            //}

            if (AppServicesAuthenticationInformation.IsAppServicesAadAuthenticationEnabled)
            {
                if (AppServicesAuthenticationInformation.LogoutUrl != null)
                {
                    return LocalRedirect(AppServicesAuthenticationInformation.LogoutUrl);
                }
                return Ok();
            }
            else
            {
                scheme ??= OpenIdConnectDefaults.AuthenticationScheme;
                var callbackUrl = Url.Page("/Account/SignedOut", pageHandler: null, values: null, protocol: Request.Scheme);
                return SignOut(
                    new AuthenticationProperties
                    {
                        RedirectUri = callbackUrl,
                    },
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    scheme);
            }



            Response.Redirect("/welcome");
            return null;
        }

    }
}