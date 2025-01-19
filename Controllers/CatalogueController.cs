using AutoMapper;
using CoreSystem2024.Helpers;
using CoreSystem2024.Models;
using dplo.Domain;
using dplo.Service;
using Dplo.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.Extensions.Logging;
using System.Configuration;
using CoreSystem2024.Controllers.shop;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Web.Common.Controllers;
using Microsoft.Extensions.Configuration;
using Umbraco.Cms.Core.Security;
using CoreSystem2024.CMSModelBuilderModels;
using Catalogue = CoreSystem2024.CMSModelBuilderModels.Catalogue;

namespace diam_planogram.Controllers
{
    ////[MvcAuthorize]
    public class CatalogueController : BaseMvcController
    {

        #region constructor
        private readonly IStandService _standService;
        private readonly IPlanogramService _planogramService;
        private readonly ICatalogueService _catalogueService;
        private readonly ICountryService _countryService;
        private readonly ICategoryService _categoryService;
        private readonly IProductService _productService;
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;
        private readonly IMemberManager _memberManager;

        private string? _domain;
        private int _brandId;

        public CatalogueController(
            ILogger<RenderController> logger,
            ICompositeViewEngine compositeViewEngine,
            IUmbracoContextAccessor umbracoContextAccessor,
            IStandService standService,
            IPlanogramService planogramService,
            ICatalogueService catalogueService,
            ICountryService countryService,
            ICategoryService categoryService,
            IProductService productService, IMapper mapper, IConfiguration config, IMemberManager memberManager) : base(logger, compositeViewEngine, umbracoContextAccessor)
        {
            _standService = standService;
            _planogramService = planogramService;
            _catalogueService = catalogueService;
            _countryService = countryService;
            _categoryService = categoryService;
            _productService = productService;
            _mapper = mapper;
            _config = config;
            _memberManager = memberManager;
            _domain = _config["AppSettings:ApiUrl"];
            _brandId = int.Parse(_config["AppSettings:ClientBrandId"]);

        }

        #endregion


        public async Task<IActionResult> Catalogue(Catalogue catalogueModel)
        {
            var memberIdentity = await _memberManager.GetCurrentMemberAsync();
            var userInfo = AuthHelper.GetUserInfo(memberIdentity);

            //we will create a custom model
            //var catalogueModel = new CatalogueModel();
            catalogueModel.UserFirstName = userInfo.GivenName;
            catalogueModel.UserLastName = userInfo.Surname;
            var country = _countryService.GetCountry(userInfo.DiamCountryId);
            catalogueModel.BrandId = _brandId;
            catalogueModel.StandTypes = _standService.GetStandTypesWithStands(catalogueModel.BrandId, country.CountryId).ToList();
            catalogueModel.ApiUrl = _config["AppSettings:ApiURL"]; 
            catalogueModel.ServerUrl = _config["AppSettings:ServerURL"]; 


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
                    var heroImageUrl = catalogueModel.ServerUrl + "/planogram/products/photo_art/placeholder.jpg";
                    var heroProduct = _productService.GetHeroProduct(cat.CategoryId, catalogueModel.BrandId);
                    if (heroProduct != null)
                    {
                        var catHeroProduct = _productService.GetProduct(heroProduct.ProductId);
                        if (catHeroProduct != null)
                        {
                            heroImageUrl = catalogueModel.ServerUrl + "/planogram/products/photo_art/" +
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

        public async Task<IActionResult> CatalogueStandAlone(Catalogue catalogueModel)
        {
            var memberIdentity = await _memberManager.GetCurrentMemberAsync();
            var userInfo = AuthHelper.GetUserInfo(memberIdentity);

            //we will create a custom model
            //var catalogueModel = new CatalogueModel();
            catalogueModel.UserFirstName = userInfo.GivenName;
            catalogueModel.UserLastName = userInfo.Surname;
            catalogueModel.BrandId = _brandId;
            var country = _countryService.GetCountry(userInfo.DiamCountryId);
            catalogueModel.CountryId = country.CountryId;
            catalogueModel.StandTypes = _standService.GetStandTypesWithStands(catalogueModel.BrandId, country.CountryId).ToList();
            catalogueModel.ApiUrl = _config["AppSettings:ApiURL"];
            catalogueModel.ServerUrl = _config["AppSettings:ServerURL"];



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
                            heroImageUrl = catalogueModel.ServerUrl + "/planogram/products/photo_art/" +
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

            //TODO: assign some values to the custom model...

            //TODO: we need to associate member with brands and with regions
            catalogueModel.BrandId = _brandId;


            IEnumerable<Part> parts = _catalogueService.GetParts(catalogueModel.BrandId, pCatsToDisplay[0].CategoryId, catalogueModel.StandTypes.First().StandTypeId, countries).Where(p => p.PartTypeId != (int)PartTypeEnum.SparePart);
            catalogueModel.Parts = parts.ToList();
            //simply use the protected method CurrentTemplate<T>, this does all of the
            //above for you... must nicer.
            return CurrentTemplate(catalogueModel);

        }
    }
}