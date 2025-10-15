using Microsoft.AspNetCore.Mvc;
using CoreSystem2024.Helpers;
using Umbraco.Cms.Core.Security;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Web;
using CoreSystem2024.ProxyServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Configuration;
using PMApplication.Interfaces.ServiceInterfaces;
using Umbraco.Cms.Core.Security;
using Microsoft.AspNetCore.Identity;
using PMApplication.Helpers;

namespace CoreSystem2024.Controllers
{
    [ApiController]
    [Authorize]
    public class AuthApiController : Controller
    {


        #region Services, managers

        private IStandService _standService;
        private IPlanogramService _planogramService;
        private ICountryService _countryService;
        private readonly IConfiguration _configuration;
        private readonly IMemberManager _memberManager;
        //private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ILogger<AuthApiController> _logger;


        public AuthApiController(IStandService standService, IPlanogramService planogramService, ICountryService countryService, IMemberManager memberManager, IConfiguration configuration,ILogger<AuthApiController> logger)
        {
            _standService = standService;
            _planogramService = planogramService;
            _countryService = countryService;
            _memberManager = memberManager;
            _configuration = configuration;
            //_signInManager = signInManager;
            _logger = logger;
        }

        #endregion


        #region LocalApiCalls

        [HttpGet]
        [Route("/Api/AuthApi/GetUserRole")]
        public async Task<IActionResult> GetUserRole()
        {
            //we need to re-auth using the reauth process
            try
            {
                var memberIdentity = await _memberManager.GetCurrentMemberAsync();
                if (memberIdentity != null)
                {
                    //var claimsPrincipal = await _signInManager.CreateUserPrincipalAsync(memberIdentity);
                    var userInfo = AuthHelper.GetUserInfo(memberIdentity);
                    bool IsShopper = RolesHelper.IsShopper(userInfo.Roles);
                    bool IsArchiver = RolesHelper.IsArchiver(userInfo.Roles);
                    bool IsCreator = RolesHelper.IsCreator(userInfo.Roles);
                    if (!IsShopper)
                    {
                        IsShopper = RolesHelper.IsAdminShopper(userInfo.Roles);
                    }

                    //only show shop settings if there is a shop
                    var hasShop = _configuration["AppSettings:HasShop"];
                    if (hasShop != "true")
                    {
                        IsShopper = false;
                    }

                    var myRole = new
                        { Role = "clientEditor", Shopper = IsShopper, Creator = IsCreator, Archiver = IsArchiver };
                    if (RolesHelper.IsAdministrator(userInfo.Roles))
                    {
                        myRole = new
                            { Role = "administrator", Shopper = IsShopper, Creator = IsCreator, Archiver = IsArchiver };
                        return Ok(myRole);
                    }

                    if (RolesHelper.IsValidator(userInfo.Roles))
                    {
                        myRole = new
                            { Role = "validator", Shopper = IsShopper, Creator = IsCreator, Archiver = IsArchiver };
                        return Ok(myRole);
                    }

                    if (RolesHelper.IsApprover(userInfo.Roles))
                    {
                        myRole = new
                            { Role = "approver", Shopper = IsShopper, Creator = IsCreator, Archiver = IsArchiver };
                        return Ok(myRole);
                    }

                    if (RolesHelper.IsClientEditor(userInfo.Roles))
                    {
                        myRole = new
                            { Role = "clientEditor", Shopper = IsShopper, Creator = IsCreator, Archiver = IsArchiver };
                        return Ok(myRole);
                    }

                    return Ok(myRole);
                }
                else
                {
                    var myRole = new { Role = "clientEditor" };
                    return BadRequest(myRole);
                }
            }
            catch (Exception ex)
            {
                //SystemLog.ErrorFormat("GetUserRole " + ex.ToString());
                _logger.LogError("Error getting Role " + " --- " + ex.ToString());
                var myRole = new { Role = "clientEditor" };
                return BadRequest(myRole);

            }

        }

        //[System.Web.Http.HttpGet]
        //public HttpResponseMessage GetUserName()
        //{
        //    //we need to re-auth using the reauth process
        //    client.RefreshAuthorization(Authorization);
        //    RefreshUserSession(HttpContext.Current.Request, Authorization);

        //    var userFirstName = UserInfo["firstname"];
        //    var userLastName = UserInfo["lastname"];
        //    var userName = userFirstName + ' ' + userLastName;

        //    return Request.CreateResponse(HttpStatusCode.OK, userName);

        //}

        #endregion


        #region remote api calls



        #endregion

        #region helper functions

        #endregion
    }
}
