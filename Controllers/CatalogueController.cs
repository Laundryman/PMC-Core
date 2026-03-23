using System.Configuration;
//using Dplo.ViewModels;
//using dplo.Domain;
//using dplo.Service;
using CoreSystem2024.Models;
using CoreSystem2024.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.Extensions.Logging;
using System.Configuration;
using CoreSystem2024.Controllers.shop;
using Umbraco.Cms.Core.Web;
using AutoMapper;
using diam_planogram.Models.Shop;
using Microsoft.Extensions.Configuration;
using PMApplication.Entities;
using PMApplication.Dtos.Categories;
using PMApplication.Entities.CountriesAggregate;
using PMApplication.Entities.PartAggregate;
using PMApplication.Enums;
using PMApplication.Interfaces.ServiceInterfaces;
using Umbraco.Cms.Core.Security;
using Umbraco.Cms.Web.Common.Controllers;
using Microsoft.AspNetCore.Identity;
using PMApplication.Dtos;
using PMApplication.Helpers;
using PMApplication.Specifications.Filters;
using Catalogue = CoreSystem2024.CMSModelBuilderModels.Catalogue;

namespace diam_planogram.Controllers
{
    ////[MvcAuthorize]
    public class CatalogueController : BaseMvcController
    {

        #region constructor
        private IStandService _standService;
        private IPlanogramService _planogramService;
        private IPartService _partService;
        private ICountryService _countryService;
        private ICategoryService _categoryService;
        private IProductService _productService;
        private IMapper _mapper;
        private readonly IConfiguration _config;
        private readonly IMemberManager _memberManager;
        //private readonly SignInManager<IdentityUser> _signInManager;

        private string? _domain;
        private int _brandId;
        public CatalogueController(
            ILogger<RenderController> logger,
            ICompositeViewEngine compositeViewEngine,
            IUmbracoContextAccessor umbracoContextAccessor,
            IStandService standService,
            IPlanogramService planogramService,
            IPartService partService,
            ICountryService countryService,
            ICategoryService categoryService,
            IProductService productService, IMapper mapper, IConfiguration config, IMemberManager memberManager) : base(logger, compositeViewEngine, umbracoContextAccessor)
        {
            _standService = standService;
            _planogramService = planogramService;
            _partService = partService;
            _countryService = countryService;
            _categoryService = categoryService;
            _productService = productService;
            _mapper = mapper;
            _config = config;
            _memberManager = memberManager;
            //_signInManager = signInManager;
            _domain = _config["AppSettings:ApiUrl"];
            _brandId = int.Parse(_config["AppSettings:ClientBrandId"]);

        }

        #endregion


        public async Task<IActionResult> Catalogue(Catalogue catalogueModel)
        {
            var memberIdentity = await _memberManager.GetCurrentMemberAsync();
            if (memberIdentity != null)
            {
                //var claimsPrincipal = await _signInManager.CreateUserPrincipalAsync(memberIdentity);
                var userInfo = AuthHelper.GetUserInfo(memberIdentity);

                //we will create a custom model
                //var catalogueModel = new CatalogueModel();
                catalogueModel.UserFirstName = userInfo.GivenName;
                catalogueModel.UserLastName = userInfo.Surname;
                var country = await _countryService.GetCountry(userInfo.DiamCountryId);
                catalogueModel.BrandId = _brandId;
                var standTypeFilter = new StandTypeFilter
                {
                    BrandId = catalogueModel.BrandId,
                    CountryId = country.Id 
                };
                   var standTypes = await _standService.GetStandTypes(standTypeFilter);

                   catalogueModel.StandTypes = standTypes.Where(st => st.Stands.Count != 0).ToList();

                catalogueModel.ApiUrl = _config["AppSettings:PMCApiBaseUrl"];
                catalogueModel.ServerUrl = _config["AzureBlob:AzureBlobBaseUrl"] + _config["AzureBlob:ProductStoreContainer"];
                catalogueModel.ProductBlobUrl = _config["AzureBlob:AzureBlobBaseUrl"] + _config["AzureBlob:ProductStoreContainer"];
                catalogueModel.CassetteRenderBlobUrl = _config["AzureBlob:AzureBlobBaseUrl"] + _config["AzureBlob:CassetteRenderContainer"];
                catalogueModel.CassettePhotoBlobUrl = _config["AzureBlob:AzureBlobBaseUrl"] + _config["AzureBlob:CassettePhotoContainer"];
                catalogueModel.CassetteTemplateBlobUrl = _config["AzureBlob:AzureBlobBaseUrl"] + _config["AzureBlob:CassetteTemplateContainer"];


                catalogueModel.CountryId = country.Id;
                //TODO: we need to associate member with brands and with regions
                var systemRole = RolesHelper.GetUserRole(userInfo.Roles, _config);
                ICollection<CountryDto> countries = new List<CountryDto>();
                countries.Add(_mapper.Map<CountryDto>(country));

                var pcatFilter = new CategoryFilter
                {
                    GetParents = true
                };
                var parentCats = await _categoryService.GetCategories(pcatFilter);
                List<int> notthese = new List<int>(new int[] { 8, 28, 37 }); //Non product bearing categories
                List<CategoryDto> pCatsToDisplay = new List<CategoryDto>();
                foreach (Category cat in parentCats)
                {
                    var filter = new PartFilter
                    {
                        BrandId = catalogueModel.BrandId,
                        ParentCategoryId = cat.Id,
                        Countries = countries
                    };
                    var hasProducts = await _partService.GetParts(filter);
                    if (hasProducts.Count > 0 && !notthese.Contains(cat.Id))
                    {
                        var heroImageUrl = catalogueModel.ServerUrl + "/placeholder.jpeg";
                        var heroProduct = await _productService.GetHeroProduct(cat.Id, catalogueModel.BrandId);
                        if (heroProduct != null)
                        {
                            var catHeroProduct = await _productService.GetProduct(heroProduct.Id);
                            if (catHeroProduct != null)
                            {
                                heroImageUrl = catalogueModel.ServerUrl + "/" +
                                               catHeroProduct.ProductImage;
                            }
                        }

                        CategoryDto catModel = new CategoryDto();
                        catModel = _mapper.Map<CategoryDto>(cat);
                        catModel.HeroImageUrl = heroImageUrl;
                        pCatsToDisplay.Add(catModel);
                    }
                }

                catalogueModel.ParentCategories = pCatsToDisplay;

                try
                {
                    if (catalogueModel.StandTypes.Count > 0)
                    {
                        var filter = new PartFilter
                        {
                            BrandId = catalogueModel.BrandId,
                            CategoryId = pCatsToDisplay[0].Id,
                            StandTypeId = catalogueModel.StandTypes.First().Id,
                            Countries = countries
                        };

                        IReadOnlyList<Part> partList = await _partService.GetParts(filter);
                        var parts = partList.Where(p => p.PartTypeId != (int)PartTypeEnum.SparePart).ToList();
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
            else
            {
                return Unauthorized("not logged in");
            }
        }

        public async Task<IActionResult> CatalogueStandAlone(Catalogue catalogueModel)
        {
            var memberIdentity = await _memberManager.GetCurrentMemberAsync();
            if (memberIdentity != null)
            {
                //var claimsPrincipal = await _signInManager.CreateUserPrincipalAsync(memberIdentity);
                var userInfo = AuthHelper.GetUserInfo(memberIdentity);

                //we will create a custom model
                //var catalogueModel = new CatalogueModel();
                catalogueModel.UserFirstName = userInfo.GivenName;
                catalogueModel.UserLastName = userInfo.Surname;
                catalogueModel.BrandId = _brandId;
                var country = await _countryService.GetCountry(userInfo.DiamCountryId);
                catalogueModel.CountryId = country.Id;
                var stFilter = new StandTypeFilter
                {
                    BrandId = catalogueModel.BrandId,
                    CountryId = country.Id,
                    HasStands = true
                };
                var standTypes = await _standService.GetStandTypes(stFilter);
                catalogueModel.StandTypes = standTypes.ToList();
                catalogueModel.ApiUrl = _config["AppSettings:ApiURL"];
                catalogueModel.ServerUrl = _config["AzureBlob:AzureBlobBaseUrl"] + _config["AzureBlob:ProductStoreContainer"];



                List<CountryDto> countries = new List<CountryDto>();
                countries.Add(_mapper.Map<CountryDto>(country));


                var pcatFilter = new CategoryFilter
                {
                    ParentCatId = 0
                };
                var parentCats = await _categoryService.GetCategories(pcatFilter);
                List<int> notthese = new List<int>(new int[] { 8, 28, 37, 56, 58, 60 });
                List<CategoryDto> pCatsToDisplay = new List<CategoryDto>();
                foreach (Category cat in parentCats)
                {
                    var filter = new PartFilter
                    {
                        BrandId = catalogueModel.BrandId,
                        CategoryId = cat.Id,
                        Countries = countries
                    };
                    var partsList = await _partService.GetParts(filter);
                    var hasProducts = partsList.Any();
                    if (hasProducts && !notthese.Contains(cat.Id))
                    {
                        var heroImageUrl = catalogueModel.ServerUrl + "/" + "placeholder.jpeg";
                        var heroProduct = await _productService.GetHeroProduct(cat.Id, catalogueModel.BrandId);
                        if (heroProduct != null)
                        {
                            var catHeroProduct = await _productService.GetProduct(heroProduct.ProductId);
                            if (catHeroProduct != null)
                            {
                                heroImageUrl = catalogueModel.ServerUrl + "/" +
                                               catHeroProduct.ProductImage;
                            }
                        }

                        CategoryDto catModel = new CategoryDto();
                        catModel = _mapper.Map<CategoryDto>(cat);
                        catModel.HeroImageUrl = heroImageUrl;
                        pCatsToDisplay.Add(catModel);
                    }
                }

                catalogueModel.ParentCategories = pCatsToDisplay;

                //TODO: assign some values to the custom model...

                //TODO: we need to associate member with brands and with regions
                catalogueModel.BrandId = _brandId;

                var filter2 = new PartFilter
                {
                    BrandId = catalogueModel.BrandId,
                    CategoryId = pCatsToDisplay[0].Id,
                    StandTypeId = catalogueModel.StandTypes.First().Id,
                    Countries = countries,
                    excludeSpareParts = true
                };
                IEnumerable<Part> parts = await _partService.GetParts(filter2);
                catalogueModel.Parts = parts.ToList();
                //simply use the protected method CurrentTemplate<T>, this does all of the
                //above for you... must nicer.
                return CurrentTemplate(catalogueModel);
            }
            else
            {
                return Unauthorized("not logged in");
            }

        }
    }
}