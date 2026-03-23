using CoreSystem2024.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using PMApplication.Dtos;
using PMApplication.Entities.CountriesAggregate;
using PMApplication.Interfaces.ServiceInterfaces;
using Umbraco.Cms.Web.Common.Controllers;
using Umbraco.Cms.Core.Security;
using Umbraco.Cms.Web.Common.Security;
using ConfigurationManager = System.Configuration.ConfigurationManager;

namespace CoreSystem2024.Controllers.shop
{
    [ApiController]
    public class BaseApiController : Controller
    {
        protected int BrandId => int.Parse(_config["AppSettings:ClientBrandId"] ?? "0");
        protected int CountryId => int.Parse(_config["AppSettings:ClientCountryId"] ?? "0");

        //protected CurrentUser UserInfo => AuthHelper.GetUserInfo(memberIdentity);
        //protected int BrandId => int.Parse(ConfigurationManager.AppSettings["brand"]);
        //protected Country UserCountry
        //{
        //    get
        //    {
        //        try
        //        {
        //            var country = _countryService.GetCountry(UserInfo.DiamCountryId);

        #region Services, managers

        private IConfiguration _config;


        #endregion
        public BaseApiController(
                IConfiguration config)
        {
            _config = config;
        }



    }
}

