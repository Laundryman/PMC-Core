using System.Net.Http.Headers;
using Umbraco.Cms.Core.Security;

namespace CoreSystem2024.Helpers
{
    public static class Utilities
    {
        //public static string BaseSiteUrl
        //{
        //    get
        //    {
        //        HttpContext context = HttpContext.Current;
        //        string baseUrl = context.Request.Url.Scheme + "://" + context.Request.Url.Authority + context.Request.ApplicationPath.TrimEnd('/') + '/';
        //        return baseUrl;
        //    }
        //}

        /// <summary>
        /// Ensures that local times are converted to UTC times.  Unspecified kinds are recast to UTC with no conversion.
        /// </summary>
        /// <param name="value">The date-time to convert.</param>
        /// <returns>The date-time in UTC time.</returns>
        public static DateTime AsUtc(this DateTime value)
        {
            if (value.Kind == DateTimeKind.Unspecified)
            {
                return new DateTime(value.Ticks, DateTimeKind.Utc);
            }

            return value.ToUniversalTime();
        }

        public static void SetHttpClientHeaders(HttpClient httpClient, MemberIdentityUser memberIdentity)
        {
            var accessToken = memberIdentity.LoginTokens.FirstOrDefault(t => t.Name == "access_token").Value;
            var idToken = memberIdentity.LoginTokens.FirstOrDefault(t => t.Name == "id_token").Value;

            httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", accessToken);
            //httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Id", idToken);
            httpClient.DefaultRequestHeaders.Add("x-user-info", idToken);
        }
    }
}