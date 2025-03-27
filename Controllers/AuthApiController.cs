using Microsoft.AspNetCore.Mvc;
using CoreSystem2024.Helpers;
using dplo.Service;
using Umbraco.Cms.Core.Security;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Web;
using CoreSystem2024.ProxyServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Configuration;
using Umbraco.Cms.Core.Security;

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
        private readonly IMemberManager _memberManager;
        private readonly ILogger<AuthApiController> _logger;


        public AuthApiController(IStandService standService, IPlanogramService planogramService, ICountryService countryService, IMemberManager memberManager)
        {
            _standService = standService;
            _planogramService = planogramService;
            _countryService = countryService;
            _memberManager = memberManager;
        }

        #endregion


        #region LocalApiCalls

        [System.Web.Mvc.HttpGet]
        [Route("/Api/AuthApi/GetUserRole")]
        public async Task<IActionResult> GetUserRole()
        {
            //we need to re-auth using the reauth process
            try
            {
                var memberIdentity = await _memberManager.GetCurrentMemberAsync();
                var userInfo = AuthHelper.GetUserInfo(memberIdentity);
                bool IsShopper = RolesHelper.IsShopper(userInfo.Roles);
                if (!IsShopper)
                {
                    IsShopper = RolesHelper.IsAdminShopper(userInfo.Roles);
                }
                var myRole = new { Role = "clientEditor", Shopper = IsShopper };
                if (RolesHelper.IsAdministrator(userInfo.Roles))
                {
                    myRole = new { Role = "administrator", Shopper = IsShopper };
                    return Ok(myRole);
                }
                if (RolesHelper.IsValidator(userInfo.Roles))
                {
                    myRole = new { Role = "validator", Shopper = IsShopper };
                    return Ok(myRole);
                }
                if (RolesHelper.IsApprover(userInfo.Roles))
                {
                    myRole = new { Role = "approver", Shopper = IsShopper };
                    return Ok(myRole);
                }
                if (RolesHelper.IsClientEditor(userInfo.Roles))
                {
                    myRole = new { Role = "clientEditor", Shopper = IsShopper };
                    return Ok(myRole);
                }
                return Ok(myRole);
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
