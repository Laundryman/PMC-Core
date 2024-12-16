using CoreSystem2024.Helpers;
using dplo.Domain;
using dplo.Service;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.Extensions.Logging;
using System.Configuration;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Web.Common.Controllers;

namespace CoreSystem2024.Controllers.shop
{
    public class BaseMvcController : RenderController
    {
        //private readonly JsonMediaTypeFormatter _jsonMediaTypeFormatter;

        //protected static WebServerClient WebServerClient = AuthHelper.CreateClient();
        //protected UserViewModel UserInfo => AuthHelper.GetUserInfo(HttpContext.User);

        //protected static IAuthorizationState Authorization => (AuthorizationState)AuthHelper.GetAuth(HttpContext.Current.Request);

        protected int BrandId => int.Parse(ConfigurationManager.AppSettings["brand"]);

        protected Country UserCountry
        {
            get
            {
                try
                {
                    var country = _countryService.GetCountry(UserInfo.DiamCountryId);

                    if (country == null) throw new Exception();

                    return country;
                }
                catch (Exception ex)
                {
                    throw new Exception("This user's country was not recognised: " + UserInfo.DiamCountryId);
                }
            }
        }

        #region Services, managers

        //private ICategoryService _categoryService;

        //private ICatalogueService _catalogueService;

        private ICountryService _countryService;

        //private IPlanogramService _planogramService;

        //private IOrderService _orderService;

        //private IStandService _standService;


        #endregion
        public BaseMvcController(
            ILogger<RenderController> logger,
            ICompositeViewEngine compositeViewEngine,
            IUmbracoContextAccessor umbracoContextAccessor,
            ICountryService countryService)
            : base(logger, compositeViewEngine, umbracoContextAccessor)
        {
            _countryService = countryService;
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

