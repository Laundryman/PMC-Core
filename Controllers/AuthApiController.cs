using Microsoft.AspNetCore.Mvc;
using CoreSystem2024.Helpers;
using dplo.Service;
using Umbraco.Cms.Core.Security;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Web;
using CoreSystem2024.ProxyServices;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Configuration;
using Umbraco.Cms.Core.Security;

namespace CoreSystem2024.Controllers
{
    [ApiController]
    public class AuthApiController : Controller
    {


        #region Services, managers

        private IStandService _standService;
        private IPlanogramService _planogramService;
        private ICountryService _countryService;
        private readonly IMemberManager _memberManager;


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

                var myRole = new { Role = "clientEditor" };
                if (RolesHelper.IsAdministrator(userInfo.Roles))
                {
                    myRole = new { Role = "administrator" };
                    return Ok(myRole);
                }
                if (RolesHelper.IsValidator(userInfo.Roles))
                {
                    myRole = new { Role = "validator" };
                    return Ok(myRole);
                }
                if (RolesHelper.IsApprover(userInfo.Roles))
                {
                    myRole = new { Role = "approver" };
                    return Ok(myRole);
                }
                if (RolesHelper.IsClientEditor(userInfo.Roles))
                {
                    myRole = new { Role = "clientEditor" };
                    return Ok(myRole);
                }
                return Ok(myRole);
            }
            catch (Exception ex)
            {
                //SystemLog.ErrorFormat("GetUserRole " + ex.ToString());

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
