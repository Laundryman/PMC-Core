//using System.Web.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using PMApplication.Dtos;
using PMApplication.Entities.CountriesAggregate;
using PMApplication.Interfaces.ServiceInterfaces;
using PMApplication.Specifications.Filters;
using Umbraco.Cms.Web.Common.Controllers;
using Umbraco.Cms.Web.Common.Filters;
using IMapper = AutoMapper.IMapper;

namespace diam_planogram.Controllers
{
    [UmbracoMemberAuthorize]
    [ApiController]
    public class SettingsApiController : Controller
    {

        #region Services, managers

        public IPartService _partService;
        public ICountryService _countryService;
        public IMapper _mapper;

        public SettingsApiController(IPartService partService, ICountryService countryService, IMapper mapper)
        {
            _partService = partService;
            _countryService = countryService;
            _mapper = mapper;
        }

        #endregion
        #region Countries

        [HttpGet]
        [Route("/Api/settingsapi/getCountryList")]
        public async Task<IEnumerable<SelectListItem>> GetCountryList(int regionId)
        {
            var countryFilter = new CountryFilter
            {
                RegionId = regionId,
            };

        var countries = await _countryService.GetCountries(countryFilter);
             var countrySelectList = countries.Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                });
            //return Json(products, JsonRequestBehavior.AllowGet);
            return countrySelectList;

        }

        [HttpPost]
        [Route("/Api/settingsapi/getPartList")]
        public async Task<IEnumerable<SelectListItem>> GetPartList(int brandId, int categoryId, int standTypeId, [FromQuery] string[] countries)
        {

            List<CountryDto> countryList = new List<CountryDto>();
            for (var i = 0; i < countries.Count(); i++)
            {
                Country country = await _countryService.GetCountry(int.Parse(countries[i]));
                countryList.Add(_mapper.Map<CountryDto>(country));
            }
            //countryList = countryService.GetCountries().ToList();

            if (brandId != 0 && categoryId != 0 && countries != null)
            {
                var partFilter = new PartFilter
                {
                    BrandId = brandId,
                    CategoryId = categoryId,
                    StandTypeId = standTypeId,
                    Countries = countryList
                };
                var parts = await _partService.GetParts(partFilter);
                    var partSelectList = parts.Select(p => new SelectListItem
                    {
                        Value = p.Id.ToString(),
                        Text = p.Name
                    });
                //return Json(products, JsonRequestBehavior.AllowGet);
                return partSelectList;
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
