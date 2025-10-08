using CoreSystem2024.Models;
//using System.Web.Http.Owin;
using CoreSystemII.Config;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Client;
using System.Diagnostics;
using System.Globalization;
using System.Security.Claims;
using System.Text;
//using CoreSystem2024.Config;
using PMApplication.Dtos;
using Umbraco.Cms.Core.Security;

namespace CoreSystem2024.Helpers
{
    public class AuthHelper
    {
        private static readonly CultureInfo UnitedKingdom = CultureInfo.GetCultureInfo("en-GB");
        private static readonly CultureInfo UnitedStates = CultureInfo.GetCultureInfo("en-US");
        private static string env = System.Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        private static string settingsFile = "appsettings." + env + ".json";
        private static string? sessionGuid { get; set; }
        private static IConfiguration _config = new ConfigurationBuilder().SetBasePath(AppDomain.CurrentDomain.BaseDirectory).AddJsonFile(settingsFile).Build();
        public static void Initialize(IConfiguration Configuration)
        {

        }

        /// <summary>
        /// Removes the query string from the passed uri
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public static string RemoveQueryStringFromUri(string uri)
        {
            int index = uri.IndexOf('?');
            if (index > -1)
            {
                uri = uri.Substring(0, index);
            }
            return uri;
        }

        /// <summary>
        /// The set of characters that are unreserved in RFC 2396 but are NOT unreserved in RFC 3986.
        /// </summary>
        private static readonly string[] UriRfc3986CharsToEscape = new[] { "!", "*", "'", "(", ")" };

        /// <summary>
        /// Escapes a string according to the URI data string rules given in RFC 3986.
        /// </summary>
        /// <param name="value">The value to escape.</param>
        /// <returns>The escaped value.</returns>
        /// <remarks>
        /// The <see cref="Uri.EscapeDataString"/> method is <i>supposed</i> to take on
        /// RFC 3986 behavior if certain elements are present in a .config file.  Even if this
        /// actually worked (which in my experiments it <i>doesn't</i>), we can't rely on every
        /// host actually having this configuration element present.
        /// </remarks>
        public static string EscapeUriDataStringRfc3986(string value)
        {
            //Requires.NotNull(value, "value");

            // Start with RFC 2396 escaping by calling the .NET method to do the work.
            // This MAY sometimes exhibit RFC 3986 behavior (according to the documentation).
            // If it does, the escaping we do that follows it will be a no-op since the
            // characters we search for to replace can't possibly exist in the string.
            StringBuilder escaped = new StringBuilder(Uri.EscapeDataString(value));

            // Upgrade the escaping to RFC 3986, if necessary.
            for (int i = 0; i < UriRfc3986CharsToEscape.Length; i++)
            {
                escaped.Replace(UriRfc3986CharsToEscape[i], Uri.HexEscape(UriRfc3986CharsToEscape[i][0]));
            }

            // Return the fully-RFC3986-escaped string.
            return escaped.ToString();
        }

        public static void ClearSessionCookie(HttpRequest request, HttpResponse response)
        {
            IConfigurationSection appSettings = _config.GetSection("AppSettings");
            var sessionName = appSettings["SessionName"];
            var diamSessionCookie = request.Cookies[sessionName];

            if (diamSessionCookie == null) return;

            var cookieOptions = new CookieOptions();
            cookieOptions.Expires = DateTime.Now.AddMinutes(-20);
            response.Cookies.Append(sessionName, "", cookieOptions);
        }

        public static CurrentUser GetUserInfo(ClaimsPrincipal user)
        {
            IConfigurationSection appSettings = _config.GetSection("AppSettings");
            var sessionName = appSettings["SessionName"];

            try
            {
                //var user = HttpRequest.GetOwinContext().Authentication.User.Claims;
                //Check here if userinfo exists in the session (will be faster to use that)
                var userInfo = new CurrentUser();
                userInfo.GivenName = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.GivenName)?.Value;
                userInfo.Surname = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Surname)?.Value;
                userInfo.Email = user.Claims.FirstOrDefault(c => c.Type == "extension_userEmailAddress")?.Value;
                userInfo.Id = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;;
                userInfo.Roles = user.Claims.FirstOrDefault(c => c.Type == "extension_diamRoles")?.Value;
                userInfo.DisplayName = user.Claims.FirstOrDefault(c => c.Type == "name")?.Value;
                var value = user.Claims
                    .FirstOrDefault(c => c.Type == "extension_diamCountryId")?.Value;
                if (value != null)
                {
                    userInfo.DiamCountryId = int.Parse(value);
                    //if (user.FirstOrDefault(c => c.Type == "extension_diamUserId") != null) 
                    //    userInfo.DiamUserId = int.Parse(user.FirstOrDefault(c => c.Type == "extension_diamUserId").Value);
                    userInfo.Brands = user.Claims.FirstOrDefault(c => c.Type == "extension_brands")?.Value;
                }


                return userInfo;
            }
            catch (Exception ex)
            {
                //If it fails then there is something wrong with the auth - need to login again
            }
            return null;
        }

        public static CurrentUser GetUserInfo(MemberIdentityUser user)
        {
            IConfigurationSection appSettings = _config.GetSection("AppSettings");
            var sessionName = appSettings["SessionName"];
            string readScope = _config["AzureB2C:ReadScope"];
            string writeScope = _config["AzureB2CWriteScope"];
            try
            {
                //var user = HttpRequest.GetOwinContext().Authentication.User.Claims;
                //Check here if userinfo exists in the session (will be faster to use that)
                var userInfo = new CurrentUser();
                userInfo.GivenName = user.Claims.FirstOrDefault(c => c.ClaimType == "given_name").ClaimValue;
                userInfo.Email = user.Claims.FirstOrDefault(c => c.ClaimType == "extension_userEmailAddress").ClaimValue;
                userInfo.Id = user.Claims.FirstOrDefault(c => c.ClaimType == "sub").ClaimValue;
                userInfo.Roles = user.Claims.FirstOrDefault(c => c.ClaimType == "extension_diamRoles").ClaimValue;
                userInfo.DisplayName = user.Claims.FirstOrDefault(c => c.ClaimType == "name").ClaimValue;
                userInfo.DiamCountryId = int.Parse(user.Claims.FirstOrDefault(c => c.ClaimType == "extension_diamCountryId").ClaimValue);
                //if (user.FirstOrDefault(c => c.Type == "extension_diamUserId") != null) 
                //    userInfo.DiamUserId = int.Parse(user.FirstOrDefault(c => c.Type == "extension_diamUserId").Value);
                userInfo.Brands = user.Claims.FirstOrDefault(c => c.ClaimType == "extension_brands").ClaimValue;
                userInfo.UserName = user.Claims.FirstOrDefault(c => c.ClaimType == "name").ClaimValue;
                userInfo.Surname = user.Claims.FirstOrDefault(c => c.ClaimType == "family_name").ClaimValue;


                return userInfo;
            }
            catch (Exception ex)
            {
                //If it fails then there is something wrong with the auth - need to login again
            }
            //}
            //else
            //{
            //    return null;
            //}
            return null;
        }
        public static Guid SetUserSession(ClaimsPrincipal userInfo, HttpContext httpContext)
        {
            //Check User is valid for this client (scope should contain the client Id)
            IConfigurationSection appSettings = _config.GetSection("AppSettings");
            string readScope = _config["AzureB2C:ReadScope"];
            string writeScope = _config["AzureB2CWriteScope"];
            var connString = _config["ConnectionStrings:dploContext"]; //ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            var userContext = new UsersContext(connString);

            try
            {
                var userSession = new UserSession();
                var sessionId = Guid.NewGuid();
                userSession.SessionGuid = sessionId;
                userSession.UserId = userInfo.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier").Value;
                userSession.UserName = userInfo.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name").Value; ;
                userSession.DateCreated = DateTime.Now;
                userContext.UserSession.Add(userSession);
                userContext.SaveChanges();
                string sessionName = _config["AppSettings:SessionName"];
                var diamSessionCookie = httpContext.Request.Cookies[sessionName];

                if (diamSessionCookie != null)
                {
                    //_logger.DebugFormat("LoginAuth session cookie exists");

                    var cookieOptions = new CookieOptions();
                    cookieOptions.Expires = DateTime.Now.AddYears(50);
                    cookieOptions.Secure = true;

                    httpContext.Response.Cookies.Append(sessionName, userSession.SessionGuid.ToString(), cookieOptions);

                    //_logger.DebugFormat("Auth session cookie set " + diamSessionCookie.ToString());
                }

                else
                {
                    //_logger.DebugFormat("Auth session cookie does not exist");
                    var cookieOptions = new CookieOptions();
                    cookieOptions.Domain = appSettings["cookieDomain"];
                    cookieOptions.Expires = DateTime.Now.AddYears(50);
                    cookieOptions.Secure = true;

                    httpContext.Response.Cookies.Append(sessionName, userSession.SessionGuid.ToString(), cookieOptions);
                    
                    //_logger.DebugFormat("Auth session cookie set " + newSessionCookie.ToString());
                }

                return userSession.SessionGuid;
            }
            catch (Exception ex)
            {
                //_logger.ErrorFormat("Error setting user session: " + ex.Message);
                throw;
            }

        }

        public static int SetActiveOrderId(HttpRequest request, int ActiveOrderId)
        {
            IConfigurationSection appSettings = _config.GetSection("AppSettings");
            var sessionName = appSettings["SessionName"];
            var diamSessionCookie = request.Cookies[sessionName];

            var sessionGuid = new Guid(diamSessionCookie);
            var connString = _config["ConnectionStrings:dploContext"]; //ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            var userContext = new UsersContext(connString);

            UserSession userSession = null;

            if (userContext.UserSession != null)
                userSession = userContext.UserSession.SingleOrDefault(s => s.SessionGuid == sessionGuid);

            if (userSession == null)
                userSession = new UserSession();
            userSession.ActiveOrderId = ActiveOrderId;
            userContext.SaveChanges();

            return userSession.ActiveOrderId;
        }

        public static int GetActiveOrderId(HttpContext httpContext, ClaimsPrincipal user)
        {

            //SetUserSession(user, httpContext);
            IConfigurationSection appSettings = _config.GetSection("AppSettings");
            var sessionName = appSettings["SessionName"];
            var diamSessionCookie = httpContext.Request.Cookies[sessionName];
            Guid sessionGuid;
            if (diamSessionCookie == null)
            {
                sessionGuid = SetUserSession(user, httpContext);
                diamSessionCookie = httpContext.Request.Cookies[sessionName];

            }
            else sessionGuid = new Guid(diamSessionCookie);

            var connString = _config["ConnectionStrings:dploContext"];
            var userContext = new UsersContext(connString);
            UserSession userSession = null;

            if (userContext.UserSession != null)
            { userSession = userContext.UserSession.SingleOrDefault(s => s.SessionGuid == sessionGuid); }
            else
            {
                userSession = userContext.UserSession.OrderBy(us => us.DateCreated).Skip(1).First();
            }

            if (userSession != null)
            {
                return userSession.ActiveOrderId;
            }
            else
            {
                return 0;
            }
        }


        //public static async Task<string> GetAccessToken(string[] scopes)
        //{
        //    //we need to re-auth using the reauth process
        //    string accessToken = null;
        //    var proxySupport = new ProxyApiSupport();

        //    try
        //    {
        //        var result = await proxySupport.AcquireTokenForScopes(new string[]
        //            { Globals.ReadTasksScope, Globals.WriteTasksScope });
        //        accessToken = result.AccessToken;
        //    }
        //    catch (MsalUiRequiredException ex)
        //    {
        //        // A MsalUiRequiredException happened on AcquireTokenSilent.
        //        // This indicates you need to call AcquireTokenInteractive to acquire a token
        //        Debug.WriteLine($"MsalUiRequiredException: {ex.Message}");

        //        try
        //        {
        //            var result = await proxySupport.AcquireTokenInteractive(new string[]
        //                { Globals.ReadTasksScope, Globals.WriteTasksScope });
        //            accessToken = result.IdToken;
        //        }
        //        catch (MsalException msalex)
        //        {
        //            throw new Exception($"Error Acquiring Token:{System.Environment.NewLine}{msalex}");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception($"Error Acquiring Token Silently:{System.Environment.NewLine}{ex}");
        //    }



        //    // Retrieve the token with the specified scopes
        //    //AuthenticationResult result = await AcquireTokenForScopes(new string[] { Globals.WriteTasksScope });

        //    return accessToken;

        //}
        //public static async Task<AuthenticationResult> AcquireTokenForScopes(string[] scopes)
        //{
        //    IConfidentialClientApplication cca = MsalAppBuilder.BuildConfidentialClientApplication();
        //    string accountId = ClaimsPrincipal.Current.GetB2CMsalAccountIdentifier(Globals.SignInPolicyId);
        //    var account = await cca.GetAccountAsync(accountId);
        //    return await cca.AcquireTokenSilent(scopes, account).ExecuteAsync();
        //}



    }
}