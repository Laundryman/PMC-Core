using CoreSystem2024.Helpers;
using dplo.Domain;
using dplo.Service;
using Microsoft.AspNetCore.Mvc;
using System.Configuration;
using Dplo.ViewModels;
using Microsoft.Extensions.Configuration;
using Umbraco.Cms.Web.Common.Controllers;
using Umbraco.Cms.Core.Security;
using Umbraco.Cms.Web.Common.Security;

namespace CoreSystem2024.Controllers.shop
{
    [ApiController]
    public class BaseApiController : Controller
    {
        protected int BrandId => int.Parse(_config["AppSettings:ClientBrandId"] ?? "0");
        protected int CountryId => int.Parse(_config["AppSettings:ClientCountryId"] ?? "0");


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

