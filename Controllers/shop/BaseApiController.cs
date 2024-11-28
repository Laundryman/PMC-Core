using System.Configuration;
using System.Diagnostics;
using System.Net;
using System.Security.Claims;
using CoreSystem.Helpers;
using dplo.Domain;
using dplo.Service;
using dplo.Service.MSGraphUtils;
using Dplo.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Umbraco.Cms.Web.Common.Controllers;
using ApiResponseModel = dplo_shop.Models.ApiResponseModel;

namespace CoreSystem.Controllers.shop
{
    [ApiController]
    public class BaseApiController : UmbracoApiController
    {
        //ClaimsIdentity claimsIdentity = (User as ClaimsPrincipal)?.Identities.FirstOrDefault();
        //protected UserViewModel UserInfo => AuthHelper.GetUserInfo();

        protected UserViewModel UserInfo => AuthHelper.GetUserInfo(User);
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

        private ICategoryService _categoryService;

        internal ICatalogueService _catalogueService;

        private ICountryService _countryService;

        internal IPlanogramService _planogramService;

        internal IOrderService _orderService;

        private IStandService _standService;

        
        #endregion
        public BaseApiController(
                ICategoryService categoryService,
                ICatalogueService catalogueService,
                ICountryService countryService,
                IPlanogramService planogramService,
                IOrderService orderService,
                IStandService standService
            )
        {
            _categoryService = categoryService;
            _catalogueService = catalogueService;
            _countryService = countryService;
            _planogramService = planogramService;
            _orderService = orderService;
            _standService = standService;
        }



        //protected async Task<string> GetAccessToken(string[] scopes)
        //{
        //    //we need to re-auth using the reauth process
        //    string accessToken = null;
        //    var proxySupport = new ProxyApiSupport();

        //    try
        //    {
        //        var result = await proxySupport.AcquireTokenForScopes(new string[]
        //            { Globals.ReadTasksScope, Globals.WriteTasksScope });
        //        accessToken = result.AccessToken;
        //    }
        //    catch (MsalUiRequiredException ex)
        //    {
        //        // A MsalUiRequiredException happened on AcquireTokenSilent.
        //        // This indicates you need to call AcquireTokenInteractive to acquire a token
        //        Debug.WriteLine($"MsalUiRequiredException: {ex.Message}");

        //        try
        //        {
        //            var result = await proxySupport.AcquireTokenInteractive(new string[]
        //                { Globals.ReadTasksScope, Globals.WriteTasksScope });
        //            accessToken = result.IdToken;
        //        }
        //        catch (MsalException msalex)
        //        {
        //            throw new Exception($"Error Acquiring Token:{System.Environment.NewLine}{msalex}");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception($"Error Acquiring Token Silently:{System.Environment.NewLine}{ex}");
        //    }



        //    // Retrieve the token with the specified scopes
        //    //AuthenticationResult result = await AcquireTokenForScopes(new string[] { Globals.WriteTasksScope });

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

