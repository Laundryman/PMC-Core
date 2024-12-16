using CoreSystem2024.Helpers;
using CoreSystem2024.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.Extensions.Logging;
using System.Configuration;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Web.Common.Controllers;


namespace diam_planogram.Controllers
{
    ////[MvcAuthorize]
    public class MihLandingPageController : RenderController
    {

        public MihLandingPageController(ILogger<RenderController> logger, ICompositeViewEngine compositeViewEngine, IUmbracoContextAccessor umbracoContextAccessor) : base(logger, compositeViewEngine, umbracoContextAccessor)
        {
        }

        public IActionResult MihLandingPage(ContentModel model)
        {

            //we will create a custom model
            //var HomeModel = new HomeModel(model.Content);
            //if (UserInfo.userViewModel != null)
            //{
            //    HomeModel.UserFirstName = UserInfo.GivenName;
            //    HomeModel.UserLastName = UserInfo.Surname;
            //}
            //HomeModel.BrandId = int.Parse(ConfigurationManager.AppSettings["brand"]);
            //HomeModel.ApiUrl = ConfigurationManager.AppSettings["apiURL"];

            //simply use the protected method CurrentTemplate<T>, this does all of the
            //above for you... must nicer.
            //return CurrentTemplate(HomeModel);
            return CurrentTemplate(model.Content);
        }

    }
}