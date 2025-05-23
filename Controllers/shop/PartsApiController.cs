using CoreSystem2024.Helpers;
using CoreSystem2024.Models.Shop;
using diam_planogram.Helpers;
using diam_planogram.Models.Shop;
using dplo.Domain;
using dplo.Domain.Entities;
using dplo.Service;
using dplo_shop.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Umbraco.Cms.Core.Security;
using ConfigurationManager = System.Configuration.ConfigurationManager;
using HttpGetAttribute = System.Web.Http.HttpGetAttribute;


namespace CoreSystem2024.Controllers.shop
{
    public class PartsApiController : BaseApiController
    {
        private const int PageSize = 24;
        private const int DefaultPartTypeId = 1;
        private readonly IStandService _standService;
        private readonly ICategoryService _categoryService;
        private readonly IConfiguration _config;
        private readonly ICountryService _countryService;
        private readonly ICatalogueService _catalogueService;
        private readonly IMemberManager _memberManager;


        public PartsApiController(ICategoryService categoryService, ICountryService countryService,
            IStandService standService, IConfiguration config, ICatalogueService catalogueService, IMemberManager memberManager) : base(config)
        {
            _categoryService = categoryService;
            _countryService = countryService;
            _standService = standService;
            _config = config;
            _catalogueService = catalogueService;
            _memberManager = memberManager;
        }

        [Route("/umbraco/api/partsapi/GetStandTypes")]
        public async Task<IActionResult> GetStandTypes(int? parentCategoryId, int? partId)
        {
            var memberIdentity = await _memberManager.GetCurrentMemberAsync();
            var userInfo = AuthHelper.GetUserInfo(memberIdentity);

            IEnumerable<StandType> standTypes;
            var userCountry = _countryService.GetCountry(CountryId);
            if (RolesHelper.IsAdminUser(userInfo.Roles)) // admin - get all
            {
                standTypes = _standService.GetFilteredStandTypes(
                    BrandId, null, null, null, parentCategoryId, null, true);
            }
            else if (RolesHelper.IsClientValidator(userInfo.Roles)) // regional manager
            {
                var region = userCountry.Regions.FirstOrDefault(x => x.BrandId == BrandId);

                if (region == null)
                    throw new Exception("Failed to look up region for this user's country/brandId: " +
                                        userInfo.DiamCountryId + " / " + BrandId);

                standTypes = _standService.GetFilteredStandTypes(
                    BrandId, region.RegionId, userCountry.CountryId, null, parentCategoryId, partId, true);
            }
            else // normal user
            {
                standTypes = _standService.GetFilteredStandTypes(
                    BrandId, null, userCountry.CountryId, null, parentCategoryId, partId, true);
            }

            var model = new StandTypesModel
            {
                StandTypes = standTypes.Select(x => new StandTypeModel { Id = x.StandTypeId, Name = x.Name }).ToList()
            };


            var response = new ApiResponseModel { data = model };

            return Ok(response);
        }



        //[Route("api/parts/{parentCategoryId:int?}")]
        [Route("/umbraco/api/partsapi/GetParts")]

        public async Task<IActionResult> GetParts(int? parentCategoryId = null, int? page = 1, int? pageSize = PageSize, int? standTypeId = null)
        {
            var memberIdentity = await _memberManager.GetCurrentMemberAsync();
            var userInfo = AuthHelper.GetUserInfo(memberIdentity);

            var userCountry = _countryService.GetCountry(userInfo.DiamCountryId);

            if (parentCategoryId == 0) parentCategoryId = null;

            if (standTypeId == null)
            {
                standTypeId = _standService.GetStandTypes(BrandId).FirstOrDefault()?.StandTypeId;
            }

            var userId = userInfo.Id; ;
            //var country = _countryService.GetCountry(defaultCountryId);
            var pageTitle = "ALL PARTS";

            if (parentCategoryId != null)
            {
                var category = _categoryService.GetCategory(parentCategoryId.Value);
                pageTitle = category.Name;
            }

            IEnumerable<PartInfo> parts;

            parts = _catalogueService.GetFilteredShopParts(
                BrandId, page, pageSize, "Name", null, null,
                null, parentCategoryId, null,
                userCountry.CountryId, null, standTypeId);







            var partModels = BuildPartModels(parts);

            var model = new PartsListModel
            {
                categoryId = parentCategoryId,
                pageTitle = pageTitle,
                parts = partModels,
                standTypeId = standTypeId
            };

            var response = new ApiResponseModel { data = model };

            return Ok(response);

        }



        //[[Route("api/parts/search")]
        [HttpGet]
        [Route("/umbraco/api/partsapi/Search")]
        public IActionResult Search(string q = null, int? standTypeId = null)
        {
            var userCountry = _countryService.GetCountry(CountryId);


            if (q == null)
            {
                return Ok(new ApiResponseModel { data = null });
            }


            IEnumerable<PartInfo> parts;

            parts = _catalogueService.GetFilteredShopParts(
                BrandId, 1, 200, "Name", null, q,
                 null, null, null,
                userCountry.CountryId, null, standTypeId);


            var partModels = BuildPartModels(parts);

            var model = new PartsListModel
            {
                // categoryId = parentCategoryId,
                // pageTitle = pageTitle,
                parts = partModels
            };

            var response = new ApiResponseModel { data = model };

            return Ok(response);
        }


        [Route("/umbraco/api/partsapi/BuildPartModels")]

        private List<PartModel> BuildPartModels(IEnumerable<PartInfo> parts)
        {
            var imageDomain = _config["AppSettings:cassette-photo-url"] ?? string.Empty;

            var partModels =
                parts.Select(x => new PartModel()
                {
                    Id = x.CatPartId,
                    Name = x.Name,
                    PartTypeId = x.PartTypeId,
                    Facings = x.Facings,
                    PartNumber = x.PartNumber,
                    AltPartNumber = x.AltPartNumber,
                    CustomerRefNo = x.CustomerRefNo,
                    ManufacturingProcess = x.ManufacturingProcess,
                    Presentation = x.Presentation,
                    TestingType = x.TestingType,
                    Stock = x.Stock,
                    Dimensions = x.Width.ToString() + "(W) X " + x.Height.ToString() + "(H)",
                    Description = x.Description,
                    UnitPrice = x.UnitCost,
                    PackShotImageSrc = imageDomain + x.PackShotImageSrc,
                    LaunchDate = x.LaunchDate,
                    LaunchPrice = x.LaunchPrice,
                    UnitCost = x.UnitCost,
                    CassetteBio = x.CassetteBio,
                    ParentCategoryId = x.ParentCategoryId,
                    CurrentPrice = x.GetPartItemCost(),
                    StandTypeId = x.StandTypeId,
                    DmiReco = x.DmiReco,
                    HidePrices = x.HidePrices

                }).ToList();

            return partModels;
        }

    }
}