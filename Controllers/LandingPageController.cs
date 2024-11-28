using System.Configuration;
using CoreSystem.Models;
using dplo.Service.MSGraphUtils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Client;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Web.Common.Controllers;

namespace CoreSystem.Controllers
{
    public class LandingPageController : RenderController
    {
        private readonly IConfiguration Configuration;
        // GET: LandingPage
        public LandingPageController(ILogger<RenderController> logger, ICompositeViewEngine compositeViewEngine, IUmbracoContextAccessor umbracoContextAccessor, IConfiguration configuration) : base(logger, compositeViewEngine, umbracoContextAccessor)
        {
            Configuration = configuration;
        }
        [HttpGet]
        public async Task<IActionResult> Index(ContentModel model)
        {
            if (User.Identity.IsAuthenticated)
            {
                try
                {
                    var proxySupport = new ProxyApiSupport();
                    //// Retrieve the token with the specified scopes
                    var result = await proxySupport.AcquireTokenForScopes(new string[]
                        { Globals.ReadTasksScope, Globals.WriteTasksScope });
                    Response.Redirect("/home");
                }
                catch (MsalUiRequiredException)
                {
                }
            }

            var LPModel = new HomeModel(model.Content);
            //if (UserInfo.userViewModel != null)
            //{
            //    LPModel.UserFirstName = UserInfo.GivenName;
            //    LPModel.UserLastName = UserInfo.Surname;
            //}
            LPModel.BrandId = int.Parse(Configuration["ClientSettings:brand"]);
            LPModel.ApiUrl = Configuration["ClientSettings:apiURL"];

            //simply use the protected method CurrentTemplate<T>, this does all of the
            //above for you... must nicer.
            return CurrentTemplate(LPModel);
        }
    }
}