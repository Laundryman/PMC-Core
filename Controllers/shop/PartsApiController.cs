using System.Configuration;
using CoreSystem.Helpers;
using CoreSystem.Models.Shop;
using diam_planogram.Helpers;
using diam_planogram.Models.Shop;
using dplo.Domain;
using dplo.Domain.Entities;
using dplo_shop.Models;
using dplo.Service;
using Microsoft.AspNetCore.Mvc;
using HttpGetAttribute = System.Web.Http.HttpGetAttribute;


namespace CoreSystem.Controllers.shop
{
    public class PartsApiController : BaseApiController
    {
        private const int PageSize = 24;
        private const int DefaultPartTypeId = 1;
        private IStandService _standService;
        private ICategoryService _categoryService;
        

        public PartsApiController(ICategoryService categoryService, ICatalogueService catalogueService, ICountryService countryService, IPlanogramService planogramService, IOrderService orderService, IStandService standService) : base(categoryService, catalogueService, countryService, planogramService, orderService, standService)
        {
            _categoryService = categoryService;
            _standService = standService;
        }

        [Route("/api/partsapi/GetStandTypes")]
        public IActionResult GetStandTypes(int? parentCategoryId, int? partId)
        {
            //var accessToken = AuthHelper.ReAuth(Authorization, WebServerClient);
            IEnumerable<StandType> standTypes;

            if (RolesHelper.IsAdminUser(Helpers.UserInfo.Roles)) // admin - get all
            {
                standTypes = _standService.GetFilteredStandTypes(
                    BrandId, null, null, null, parentCategoryId, null, true);
            }
            else if (RolesHelper.IsClientValidator(Helpers.UserInfo.Roles)) // regional manager
            {
                var region = UserCountry.Regions.FirstOrDefault(x => x.BrandId == BrandId);

                if (region == null)
                    throw new Exception("Failed to look up region for this user's country/brandId: " +
                                        UserInfo.DiamCountryId + " / " + BrandId);

                standTypes = _standService.GetFilteredStandTypes(
                    BrandId, region.RegionId, UserCountry.CountryId, null, parentCategoryId, partId, true);
            }
            else // normal user
            {
                standTypes = _standService.GetFilteredStandTypes(
                    BrandId, null, UserCountry.CountryId, null, parentCategoryId, partId, true);
            }

            var model = new StandTypesModel
            {
                StandTypes = standTypes.Select(x => new StandTypeModel {Id = x.StandTypeId, Name = x.Name}).ToList()
            };


            var response = new ApiResponseModel { data = model };

            return Ok(response);
        }



        //[Route("api/parts/{parentCategoryId:int?}")]
        [Route("/api/partsapi/GetParts/{parentCategoryId}/{page}/{pageSize}/{standTypeId}")]

        public IActionResult GetParts(int? parentCategoryId = null, int? page = 1, int? pageSize = PageSize, int? standTypeId = null)
        {
            
            
            //var accessToken = AuthHelper.ReAuth(Authorization, WebServerClient);

            if (parentCategoryId == 0) parentCategoryId = null;

            if (standTypeId == null)
            {
                standTypeId = _standService.GetStandTypes(BrandId).FirstOrDefault()?.StandTypeId;
            }

            var userId = Helpers.UserInfo.Id;;
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
                UserCountry.CountryId, null, standTypeId);

                





            var partModels = BuildPartModels(parts);

            var model = new PartsListModel
            {
                categoryId = parentCategoryId,
                pageTitle = pageTitle,
                parts = partModels,
                standTypeId = standTypeId
            };

            var response = new ApiResponseModel {data = model};
            
            return Ok(response);

        }



        //[[Route("api/parts/search")]
        [HttpGet]
        [Route("/api/partsapi/Search")]
        public IActionResult Search(string q = null, int? standTypeId = null)
        {
            

            //var accessToken = AuthHelper.ReAuth(Authorization, WebServerClient);

            if (q == null)
            {
                return Ok(new ApiResponseModel { data = null });
            }

            
            IEnumerable<PartInfo> parts;

            //if (RolesHelper.IsAdminUser(Helpers.UserInfo.Roles)) // admin - get all
            //{
            //    parts = CatalogueService.GetFilteredShopParts(
            //        BrandId, 1, null, "Name", null, q,
            //        Helpers.UserInfo.Id; DefaultPartTypeId, null, null,
            //        null, null, standTypeId);
            //}
            //else if (RolesHelper.IsClientValidator(Helpers.UserInfo.Roles)) // regional manager
            //{
            //    var region = UserCountry.Regions.FirstOrDefault(x => x.BrandId == BrandId);

            //    if (region == null)
            //        throw new Exception("Failed to look up region for this user's country/brandId: " +
            //                            UserInfo.DiamCountryId; + " / " + BrandId);

            //    parts = CatalogueService.GetFilteredShopParts(
            //        BrandId, 1, 200, "Name", null, q,
            //        Helpers.UserInfo.Id; DefaultPartTypeId, null, null,
            //        null, region.RegionId, standTypeId);
            //}
            //else // normal user
            //{
            //    parts = CatalogueService.GetFilteredShopParts(
            //        BrandId, 1, 200, "Name", null, q,
            //        Helpers.UserInfo.Id; DefaultPartTypeId, null, null,
            //        UserCountry.CountryId, null, standTypeId);
            //}

            parts = _catalogueService.GetFilteredShopParts(
                BrandId, 1, 200, "Name", null, q,
                 null, null, null,
                UserCountry.CountryId, null, standTypeId);


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


        [Route("/api/partsapi/BuildPartModels")]

        private List<PartModel> BuildPartModels(IEnumerable<PartInfo> parts)
        {
            var imageDomain = ConfigurationManager.AppSettings["cassette-photo-url"];

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