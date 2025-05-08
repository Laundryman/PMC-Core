using CoreSystem2024.Helpers;
using CoreSystem2024.Models;
using CoreSystem2024.Models.Shop;
using diam_planogram.Models.Shop;
using dplo_shop.Models;
using dplo.Domain.Entities;
using dplo.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Umbraco.Cms.Core.Security;
using ConfigurationManager = System.Configuration.ConfigurationManager;

//using Dplo.ViewModels;

namespace CoreSystem2024.Controllers.shop
{
    public class NavigationApiController : BaseApiController
    {
        private readonly ICategoryService _categoryService;
        private readonly ICountryService _countryService;
        private readonly IMemberManager _memberManager;
        private readonly IOrderService _orderService;
        private readonly IConfiguration _config;
        private readonly ILogger<NavigationApiController> _logger;

        public NavigationApiController(ICategoryService categoryService, ICountryService countryService, IOrderService orderService, IMemberManager memberManager, IConfiguration config, ILogger<NavigationApiController> logger) : base(config)
        {
            _categoryService = categoryService;
            _countryService = countryService;
            _orderService = orderService;
            _memberManager = memberManager;
            _config = config;
            _logger = logger;
        }

        [System.Web.Http.HttpGet]
        [Route("/umbraco/api/navigationapi/get")]
        public async Task<IActionResult> Get()
        {
            try
            {
                AuthHelper.Initialize(_config);
                RolesHelper.Initialize(_config);
                int brandId = int.Parse(_config["AppSettings:ClientBrandId"] ?? "0");
                int countryId = int.Parse(_config["AppSettings:ClientCountryId"] ?? "0");
                var memberIdentity = await _memberManager.GetCurrentMemberAsync();
                var userInfo = AuthHelper.GetUserInfo(memberIdentity);
                var userCountry = _countryService.GetCountry(userInfo.DiamCountryId);

                var categories = _categoryService.GetShopCategories(brandId, userCountry.CountryId);

                var notthese = new List<int>(new int[]
                {
                    /*8,*/ 28, /*30,*/ /*37,*/ 66
                }); //Non product bearing categories
                var pCatsToDisplay = new List<CategoryModel>();

                foreach (ShopCategory shopCategory in categories)
                {
                    //var hasProducts = CatalogueService.GetParts(BrandId, cat.CategoryId, countries).Any();
                    //if (!notthese.Contains(cat.CategoryId))
                    if (!notthese.Contains(shopCategory.CategoryId))
                    {
                        var catModel = new CategoryModel { Id = shopCategory.CategoryId, Name = shopCategory.Name };
                        pCatsToDisplay.Add(catModel);
                    }
                }

                var navModel = new NavModel
                {
                    Categories = pCatsToDisplay,
                    UserName = userInfo.GivenName + " " + userInfo.Surname,
                    ClientUrl = _config["AppSettings:clientUrl"],
                    PlanogramsUrl = _config["AppSettings:planogramsUrl"],
                    IsAdminShopper = RolesHelper.IsAdminShopper(userInfo.Roles)
                };

                var aOrderId = AuthHelper.GetActiveOrderId(HttpContext, User);

                if (aOrderId != 0)
                {
                    var order = _orderService.GetOrder(aOrderId);

                    if (order != null && order.OrderStatus == (int)OrderStatusEnum.Open)
                    {
                        navModel.CurrentOrderTitle = order.OrderTitle;
                    }
                }

                var response = new ApiResponseModel { data = navModel };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in NavigationApiController.Get()");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }
    }
}
