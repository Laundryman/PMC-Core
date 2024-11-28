using System.Configuration;
using Dplo.ViewModels;
using dplo.Domain;
using dplo.Service;
using CoreSystem.Models;
using CoreSystem.Helpers;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Web.Common.Controllers;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Umbraco.Cms.Core.Web;
using AutoMapper;

namespace diam_planogram.Controllers
{
    ////[MvcAuthorize]
    public class CatalogueController : RenderController
    {

        #region constructor
        private IStandService _standService;
        private IPlanogramService _planogramService;
        private ICatalogueService _catalogueService;
        private ICountryService _countryService;
        private ICategoryService _categoryService;
        private IProductService _productService;
        private IMapper _mapper;

        public CatalogueController(
            ILogger<RenderController> logger, 
            ICompositeViewEngine compositeViewEngine, 
            IUmbracoContextAccessor umbracoContextAccessor,
            IStandService standService,
            IPlanogramService planogramService,
            ICatalogueService catalogueService,
            ICountryService countryService,
            ICategoryService categoryService,
            IProductService productService, IMapper mapper) : base(logger, compositeViewEngine, umbracoContextAccessor)
        {
            _standService = standService;
            _planogramService = planogramService;
            _catalogueService = catalogueService;
            _countryService = countryService;
            _categoryService = categoryService;
            _productService = productService;
            _mapper = mapper;
        }

        #endregion


        public IActionResult Catalogue()
        {

            //if (Request.UrlReferrer.PathAndQuery.Contains("edit-planogram.aspx") && !Request.Url.PathAndQuery.Contains("edit-planogram.aspx"))
            //{
            //    var querystring = Request.UrlReferrer.Query.Split('?')[1];
            //    var qparams = querystring.Split('&');
            //    var paramsList = new List<Tuple<string, string>>();
            //    var planoIdToUnLock = 0;
            //    foreach (var param in qparams)
            //    {
            //        var paramSplt = param.Split('=');
            //        paramsList.Add(new Tuple<string, string>(paramSplt[0], paramSplt[1]));
            //        if (paramSplt[0].ToLower() == "pid")
            //        {
            //            planoIdToUnLock = int.Parse(paramSplt[1]);
            //            break;
            //        }
            //    }
            //    try
            //    {
            //        PlanogramService.UnLockPlanogram(planoIdToUnLock, UserInfo.userViewModel);
            //    }
            //    catch (Exception Ex)
            //    {
            //    }
            //}

            //we will create a custom model
            var catalogueModel = new CatalogueModel();
            catalogueModel.UserFirstName = UserInfo.GivenName;
            catalogueModel.UserLastName = UserInfo.Surname;
            var country = _countryService.GetCountry(UserInfo.DiamCountryId);
            catalogueModel.BrandId = int.Parse(ConfigurationManager.AppSettings["brand"]);
            catalogueModel.StandTypes = _standService.GetStandTypesWithStands(catalogueModel.BrandId, country.CountryId).ToList();
            catalogueModel.ApiUrl = ConfigurationManager.AppSettings["apiURL"];

            catalogueModel.CountryId = country.CountryId;
            //TODO: we need to associate member with brands and with regions

            List<Country> countries = new List<Country>();
            countries.Add(country); 

            IEnumerable<Category> parentCats = _categoryService.GetParentCategories();
            List<int> notthese = new List<int>(new int[] { 8, 28, 37 }); //Non product bearing categories
            List<CategoryModel> pCatsToDisplay = new List<CategoryModel>();
            foreach (Category cat in parentCats)
            {
                var hasProducts = _catalogueService.GetParts(catalogueModel.BrandId, cat.CategoryId, countries).Any();
                if (hasProducts && !notthese.Contains(cat.CategoryId))
                {
                    var heroImageUrl = catalogueModel.ApiUrl + "/planogram/products/photo_art/placeholder.jpg";
                    var heroProduct = _productService.GetHeroProduct(cat.CategoryId, catalogueModel.BrandId);
                    if (heroProduct != null)
                    {
                        var catHeroProduct = _productService.GetProduct(heroProduct.ProductId);
                        if (catHeroProduct != null)
                        {
                            heroImageUrl = catalogueModel.ApiUrl + "/planogram/products/photo_art/" +
                                           catHeroProduct.ProductImage;
                        }
                    }

                    CategoryModel catModel = new CategoryModel();
                    catModel = _mapper.Map<CategoryModel>(cat);
                    catModel.HeroImageUrl = heroImageUrl;
                    pCatsToDisplay.Add(catModel);
                }
            }

            catalogueModel.ParentCategories = pCatsToDisplay;
            catalogueModel.ApiUrl = ConfigurationManager.AppSettings["apiURL"];

            try
            {
                if (catalogueModel.StandTypes.Count > 0)
                {
                    List<Part> parts = _catalogueService.GetParts(catalogueModel.BrandId, pCatsToDisplay[0].CategoryId,
                            catalogueModel.StandTypes.First().StandTypeId, countries)
                        .Where(p => p.PartTypeId != (int)PartTypeEnum.SparePart).ToList();
                    if (parts.Any())
                        catalogueModel.Parts = parts;
                }
            }
            catch (Exception ex)
            {
                return CurrentTemplate(catalogueModel);
            }
            //simply use the protected method CurrentTemplate<T>, this does all of the
            //above for you... must nicer.
            return CurrentTemplate(catalogueModel);
        }

        public IActionResult CatalogueStandAlone()
        {

            //we will create a custom model
            var catalogueModel = new CatalogueModel();
            catalogueModel.UserFirstName = UserInfo.GivenName;
            catalogueModel.UserLastName = UserInfo.Surname;
            catalogueModel.BrandId = int.Parse(ConfigurationManager.AppSettings["brand"]);
            var country = _countryService.GetCountry(UserInfo.DiamCountryId);
            catalogueModel.CountryId = country.CountryId;
            catalogueModel.StandTypes = _standService.GetStandTypesWithStands(catalogueModel.BrandId, country.CountryId).ToList();
            catalogueModel.ApiUrl = ConfigurationManager.AppSettings["apiURL"];


            List<Country> countries = new List<Country>();
            countries.Add(country);


            IEnumerable<Category> parentCats = _categoryService.GetParentCategories();
            List<int> notthese = new List<int>(new int[] { 8, 28, 37, 56, 58, 60 });
            List<CategoryModel> pCatsToDisplay = new List<CategoryModel>();
            foreach (Category cat in parentCats)
            {
                var hasProducts = _catalogueService.GetParts(catalogueModel.BrandId, cat.CategoryId, countries).Any();
                if (hasProducts && !notthese.Contains(cat.CategoryId))
                {
                    var heroImageUrl = "placeholder.jpg";
                    var heroProduct = _productService.GetHeroProduct(cat.CategoryId, catalogueModel.BrandId);
                    if (heroProduct != null)
                    {
                        var catHeroProduct = _productService.GetProduct(heroProduct.ProductId);
                        if (catHeroProduct != null)
                        {
                            heroImageUrl = catalogueModel.ApiUrl + "/planogram/products/photo_art/" +
                                           catHeroProduct.ProductImage;
                        }
                    }

                    CategoryModel catModel = new CategoryModel();
                    catModel = _mapper.Map<CategoryModel>(cat);
                    catModel.HeroImageUrl = heroImageUrl;
                    pCatsToDisplay.Add(catModel);
                }
            }

            catalogueModel.ParentCategories = pCatsToDisplay;
            catalogueModel.ApiUrl = ConfigurationManager.AppSettings["apiURL"];

            //TODO: assign some values to the custom model...

            //TODO: we need to associate member with brands and with regions
            catalogueModel.BrandId = int.Parse(ConfigurationManager.AppSettings["brand"]);


            IEnumerable<Part> parts = _catalogueService.GetParts(catalogueModel.BrandId, pCatsToDisplay[0].CategoryId, catalogueModel.StandTypes.First().StandTypeId, countries).Where(p => p.PartTypeId != (int)PartTypeEnum.SparePart);
            catalogueModel.Parts = parts.ToList();
            //simply use the protected method CurrentTemplate<T>, this does all of the
            //above for you... must nicer.
            return CurrentTemplate(catalogueModel);

        }
    }
}