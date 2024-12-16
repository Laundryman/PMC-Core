using CoreSystem2024.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Security;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Web.Common.Controllers;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace CoreSystem2024.Controllers
{
    public class LandingPageController : RenderController
    {
        private readonly IConfiguration Configuration;
        private IConfiguration _azureSettings;
        private readonly IMemberManager _memberManager;
        //private readonly 
        private readonly IMemberService _memberService;
        // GET: LandingPage
        public LandingPageController(ILogger<RenderController> logger,
            ICompositeViewEngine compositeViewEngine,
            IUmbracoContextAccessor umbracoContextAccessor,
            IConfiguration configuration,
            IMemberManager memberManager,
            //IMemberManager diamMemberManager,
            IMemberService memberService) : base(logger, compositeViewEngine, umbracoContextAccessor)
        {
            Configuration = configuration;
            _memberManager = memberManager;
            _memberService = memberService;
            _azureSettings = configuration.GetSection("AzureB2C");
        }

        [HttpGet]
        public async Task<IActionResult> Index(ContentModel model)
        {
            if (User.Identity.IsAuthenticated)
            {

                var member = _memberService.GetByUsername(User.Identity.Name);
                var memberIdentity = _memberManager.GetCurrentMemberAsync();
                Response.Redirect("/home");
                //try
                //{
                //    var proxySupport = new ProxyApiSupport(Configuration);
                //    //// Retrieve the token with the specified scopes
                //    var result = await proxySupport.AcquireTokenForScopes(new string[]
                //        { _azureSettings["ReadScope"], _azureSettings["WriteScope"]});
                //    Response.Redirect("/home");
                //}
                //catch (MsalUiRequiredException)
                //{
                //}
            }

            //var LPModel = new Home();
            ////if (UserInfo.userViewModel != null)
            ////{
            ////    LPModel.UserFirstName = UserInfo.GivenName;
            ////    LPModel.UserLastName = UserInfo.Surname;
            ////}
            //LPModel.BrandId = int.Parse(Configuration["AppSettings:ClientBrandId"]);
            //LPModel.ApiUrl = Configuration["AppSettings:ApiURL"];

            //simply use the protected method CurrentTemplate<T>, this does all of the
            //above for you... must nicer.
            return CurrentTemplate(model);
        }
    }
}