using dplo.Domain.Entities;
using dplo.Domain;
using dplo.Service;
using System.Configuration;
using System.Security.Claims;
using Dplo.ViewModels;
using dplo.Service.MSGraph;
using dplo.Service.MSGraphUtils;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using User = Microsoft.Graph.User;
using ValidationResult = dplo.Service.Common.ValidationResult;

namespace CoreSystem.Helpers
{

    public static class ControllerExtensions
    {
        public static Brand CurrentBrand(this Controller controller, IBrandService brandService)
        {
            var brandSelection = controller.Request.Cookies["diamBrandCookie"];

            if (brandSelection == null)
            {
                controller.Response.Redirect("~/");
                return null;
            }
            return brandService.GetBrand(int.Parse(brandSelection));
        }


        public static string CurrentBrandLogo(this Controller controller, IBrandService brandService)
        {
            var currentBrand = CurrentBrand(controller, brandService);
            Brand brand = brandService.GetBrand(currentBrand.BrandId);
            var storageRoot = ConfigurationManager.AppSettings["snapShotUploadPath"] + "/Brands";
            storageRoot = storageRoot.Replace("~", "");
            return storageRoot + "/" + brand.BrandLogo;
        }

        public static async Task<UserViewModel> MappedUser(this Controller controller, IAIdentityService identityService)
        {
            // we can retrieve the userId from the request
            var currentUser = controller.User;
            //var me = _identityService.GetMe(Globals.B2cExtensionAppId);
            var identity = ((System.Security.Claims.ClaimsPrincipal)currentUser);
            var userOID = identity.Claims.FirstOrDefault((x => x.Type == ClaimTypes.NameIdentifier)).Value;// 'http://schemas.microsoft.com/identity/claims/objectidentifier']
            var user = await identityService.GetUser(userOID, Globals.B2cExtensionAppId);
            //var mapper = MapperConfig.InitializeAutomapper();

            //var graphUsers = new List<User>();
            //graphUsers = userCollectionResponse.Value;
            var mappedUser = Mapper.Map<User, UserViewModel>(user);
            return mappedUser;
        }

        public static async Task<UserViewModel> MappedUser(this ControllerBase controller, IAIdentityService identityService)
        {
            // we can retrieve the userId from the request
            var currentUser = controller.User;
            //var me = _identityService.GetMe(Globals.B2cExtensionAppId);
            var identity = ((System.Security.Claims.ClaimsPrincipal)currentUser);
            var userOID = identity.Claims.FirstOrDefault((x => x.Type == ClaimTypes.NameIdentifier)).Value;// 'http://schemas.microsoft.com/identity/claims/objectidentifier']
            var user = await identityService.GetUser(userOID, Globals.B2cExtensionAppId);
            //var mapper = MapperConfig.InitializeAutomapper();

            //var graphUsers = new List<User>();
            //graphUsers = userCollectionResponse.Value;
            var mappedUser = Mapper.Map<User, UserViewModel>(user);
            return mappedUser;
        }

        public static List<Brand> MappedBrands(this Controller controller, UserViewModel user, IBrandService brandService)
        {
            var brandIds = user.Brands.Split(',');
            var brands = new List<Brand>();
            for (int i = 0; i < brandIds.Length; i++)
            {
                var brand = brandService.GetBrand(int.Parse(brandIds[i]));
                brands.Add(brand);
            }
            return brands;
        }

        public static List<Brand> MappedBrands(this ControllerBase controller, UserViewModel user, IBrandService brandService)
        {
            var brandIds = user.Brands.Split(',');
            var brands = new List<Brand>();
            for (int i = 0; i < brandIds.Length; i++)
            {
                var brand = brandService.GetBrand(int.Parse(brandIds[i]));
                brands.Add(brand);
            }
            return brands;
        }

        /// <summary>
        /// Adds a model errors for each validation result from the business service.
        /// </summary>
        /// <param name="validationResults">The validation results from a business service.</param>
        /// <param name="controller">The model state dictionary used to add errors.</param>
        /// <param name="defaultErrorKey">The default key to use if a field is not specified in a business service valdiation result.</param>
        public static void AddModelErrors(this Controller controller, IEnumerable<ValidationResult> validationResults, string defaultErrorKey = null)
        {
            if (validationResults != null)
            {
                foreach (var validationResult in validationResults)
                {
                    if (!string.IsNullOrEmpty(validationResult.MemberName))
                    {
                        controller.ModelState.AddModelError(validationResult.MemberName, validationResult.Message);
                    }
                    else if (defaultErrorKey != null)
                    {
                        controller.ModelState.AddModelError(defaultErrorKey, validationResult.Message);
                    }
                    else
                    {
                        controller.ModelState.AddModelError(string.Empty, validationResult.Message);
                    }
                }
            }
        }

        /// <summary>
        /// Adds a model errors for each validation result from the business service.
        /// </summary>
        /// <param name="validationResults">The validation results from a business service.</param>
        /// <param name="modelState">The model state dictionary used to add errors.</param>
        /// <param name="defaultErrorKey">The default key to use if a field is not specified in a business service valdiation result.</param>
        public static void AddModelErrors(this ModelStateDictionary modelState, IEnumerable<ValidationResult> validationResults, string defaultErrorKey = null)
        {
            if (validationResults == null) return;

            foreach (var validationResult in validationResults)
            {
                string key = validationResult.MemberName ?? defaultErrorKey ?? string.Empty;
                modelState.AddModelError(key, validationResult.Message);
            }
        }
        public static List<AssignedCountryData> GetViewModelCountries(IEnumerable<Country> allCountries)
        {
            var viewModel = new List<AssignedCountryData>();
            foreach (var country in allCountries)
            {
                viewModel.Add(new AssignedCountryData
                {
                    CountryId = country.CountryId,
                    Name = country.Name,
                    Assigned = false
                });
            }
            return viewModel;
        }

    }
}