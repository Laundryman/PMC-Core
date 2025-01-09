//using System.Web.Http;
using dplo.Domain;
using dplo.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Umbraco.Cms.Web.Common.Controllers;
using Umbraco.Cms.Web.Common.Filters;

namespace diam_planogram.Controllers
{
    [UmbracoMemberAuthorize]
    [ApiController]
    public class SettingsApiController : Controller
    {

        #region Services, managers

        public ICatalogueService _catalogueService;
        public ICountryService _countryService;

        public SettingsApiController(ICatalogueService catalogueService, ICountryService countryService)
        {
            _catalogueService = catalogueService;
            _countryService = countryService;
        }

        #endregion
        #region Countries
        [HttpGet]
        [Route("/Api/settingsapi/getCountryList")]
        public IEnumerable<SelectListItem> GetCountryList(int regionId)
        {

            IEnumerable<SelectListItem> countries = _countryService.GetCountriesByRegion(regionId)
                .Select(c => new SelectListItem
                {
                    Value = c.CountryId.ToString(),
                    Text = c.Name
                });
            //return Json(products, JsonRequestBehavior.AllowGet);
            return countries;

        }

        [HttpPost]
        [Route("/Api/settingsapi/getPartList")]
        public IEnumerable<SelectListItem> GetPartList(int brandId, int categoryId, int standTypeId, [FromQuery] string[] countries)
        {

            List<Country> countryList = new List<Country>();
            for (var i = 0; i < countries.Count(); i++)
            {
                Country country = _countryService.GetCountry(int.Parse(countries[i]));
                countryList.Add(country);
            }
            //countryList = countryService.GetCountries().ToList();

            if (brandId != 0 && categoryId != 0 && countries != null)
            {
                var parts = _catalogueService.GetParts(brandId, categoryId, standTypeId, countryList)
                    .Select(p => new SelectListItem
                    {
                        Value = p.CatPartId.ToString(),
                        Text = p.Name
                    });
                //return Json(products, JsonRequestBehavior.AllowGet);
                return parts;
            }
            else
            {
                return null;
                //return Json("", JsonRequestBehavior.AllowGet);
            }

        }



        #endregion


    }
}
