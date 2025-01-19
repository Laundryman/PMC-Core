using CoreSystem2024.Helpers;
using dplo.Domain;
using dplo.Service;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.Extensions.Logging;
using System.Configuration;
using Dplo.ViewModels;
using Umbraco.Cms.Core.Security;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Web.Common.Controllers;

namespace CoreSystem2024.Controllers.shop
{
    public class BaseMvcController : RenderController
    {
        //private readonly JsonMediaTypeFormatter _jsonMediaTypeFormatter;



        #region Services, managers

        #endregion
        public BaseMvcController(
            ILogger<RenderController> logger,
            ICompositeViewEngine compositeViewEngine,
            IUmbracoContextAccessor umbracoContextAccessor)
            : base(logger, compositeViewEngine, umbracoContextAccessor)
        {
            //AuthHelper.SetUserSession(User, HttpContext);
        }





        //protected async Task<string> GetAccessToken(string[] scopes)
        //{
        //    //we need to re-auth using the reauth process
        //    string accessToken = null;

        //    // Retrieve the token with the specified scopes
        //    AuthenticationResult result = await AcquireTokenForScopes(new string[] { Globals.WriteTasksScope });
        //    accessToken = result.AccessToken;

        //    return accessToken;

        //}
        //protected async Task<AuthenticationResult> AcquireTokenForScopes(string[] scopes)
        //{
        //    IConfidentialClientApplication cca = MsalAppBuilder.BuildConfidentialClientApplication();
        //    string accountId = ClaimsPrincipal.Current.GetB2CMsalAccountIdentifier(Globals.SignInPolicyId);
        //    var account = await cca.GetAccountAsync(accountId);
        //    return await cca.AcquireTokenSilent(scopes, account).ExecuteAsync();
        //}
    }
}

