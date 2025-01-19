using dplo.Service.MSGraphUtils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Client;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Web.Common.Controllers;

namespace CoreSystem2024.Controllers.shop
{
    public class ShopController : BaseMvcController
    {

        private readonly IConfiguration _configuration;
        private string? _domain;
        private string? _brandId;
        private string? _readScope;
        private string? _writeScope;

        // GET: Shop
        public ShopController(ILogger<RenderController> logger, ICompositeViewEngine compositeViewEngine, IUmbracoContextAccessor umbracoContextAccessor, IConfiguration configuration) : base(logger, compositeViewEngine, umbracoContextAccessor)
        {
            _configuration = configuration;
            _domain = _configuration["AppSettings:ApiUrl"];
            _brandId = _configuration["AppSettings:ClientBrandId"];
            _readScope = _configuration["AzureB2C:ReadScope"];
            _writeScope = _configuration["AzureB2CWriteScope"];
        }

        public async Task<IActionResult> Shop()
        {

            return CurrentTemplate(CurrentPage);
        }
    }
}