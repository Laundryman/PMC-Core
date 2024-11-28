using System.Configuration;
using CoreSystem.Helpers;
using diam_planogram.Models.Shop;
using dplo.Domain.Entities;
using dplo_shop.Models;
using dplo.Service;
using Microsoft.AspNetCore.Mvc;

//using Dplo.ViewModels;

namespace CoreSystem.Controllers.shop
{
    public class NavigationApiController : BaseApiController
    {
        private ICategoryService _categoryService;
        public NavigationApiController(ICategoryService categoryService, ICatalogueService catalogueService, ICountryService countryService, IPlanogramService planogramService, IOrderService orderService, IStandService standService) : base(categoryService, catalogueService, countryService, planogramService, orderService, standService)
        {
            _categoryService = categoryService;
        }

        [System.Web.Http.HttpGet]
        [Route("/api/navigationapi/get")]
        public IActionResult Get()
        {
            

            //AuthHelper.ReAuth(Authorization, WebServerClient);

            //var countries = new List<Country> {UserCountry};

            var categories = _categoryService.GetShopCategories(BrandId, UserCountry.CountryId);

            var notthese = new List<int>(new int[] { /*8,*/ 28, /*30,*/ /*37,*/ 66 }); //Non product bearing categories
            var pCatsToDisplay = new List<CategoryModel>();

            foreach (ShopCategory shopCategory in categories)
            {
                //var hasProducts = CatalogueService.GetParts(BrandId, cat.CategoryId, countries).Any();
                //if (!notthese.Contains(cat.CategoryId))
                if (!notthese.Contains(shopCategory.CategoryId))
                {
                    var catModel = new CategoryModel{Id = shopCategory.CategoryId, Name = shopCategory.Name};
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


            var aOrderId = AuthHelper.GetActiveOrderId(HttpContext);

            if (aOrderId != 0)
            {
                var order = _orderService.GetOrder(aOrderId);

                if (order != null && order.OrderStatus == (int) OrderStatusEnum.Open)
                {
                    navModel.CurrentOrderTitle = order.OrderTitle;
                }
            }

            var response = new ApiResponseModel { data = navModel };

            return Ok(response);
        }
    }
}
