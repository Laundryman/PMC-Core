using CoreSystem2024.Helpers;
using diam_planogram.Models.Shop;
using dplo.Domain.Entities;
using dplo.Service;
using dplo_shop.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
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
        public NavigationApiController(ICategoryService categoryService, ICountryService countryService, IOrderService orderService, IMemberManager memberManager, IConfiguration config) : base(config)
        {
            _categoryService = categoryService;
            _countryService = countryService;
            _orderService = orderService;
            _memberManager = memberManager;
            _config = config;
        }

        [System.Web.Http.HttpGet]
        [Route("/api/navigationapi/get")]
        public async Task<IActionResult> Get()
        {

            int brandId = int.Parse(_config["AppSettings:ClientBrandId"] ?? "0");
            int countryId = int.Parse(_config["AppSettings:ClientCountryId"] ?? "0");
            var userCountry = _countryService.GetCountry(countryId);
            //AuthHelper.ReAuth(Authorization, WebServerClient);

            //var countries = new List<Country> {UserCountry};

            var categories = _categoryService.GetShopCategories(brandId, userCountry.CountryId);

            var notthese = new List<int>(new int[] { /*8,*/ 28, /*30,*/ /*37,*/ 66 }); //Non product bearing categories
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
                UserName = Helpers.UserInfo.FullName,
                ClientUrl = ConfigurationManager.AppSettings["clientUrl"],
                PlanogramsUrl = ConfigurationManager.AppSettings["planogramsUrl"],
                IsAdminShopper = RolesHelper.IsAdminShopper(Helpers.UserInfo.Roles)
            };

            var memberIdentity = await _memberManager.GetCurrentMemberAsync();
            var aOrderId = AuthHelper.GetActiveOrderId(HttpContext, memberIdentity);

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
    }
}
