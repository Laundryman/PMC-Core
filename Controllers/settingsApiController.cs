using dplo.Domain;
using dplo.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Umbraco.Cms.Web.Common.Controllers;

namespace diam_planogram.Controllers
{
    public class SettingsApiController : UmbracoApiController
    {

        #region Services, managers

        public ICatalogueService _catalogueService;
        public ICountryService _countryService;

        #endregion
        #region Countries

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
