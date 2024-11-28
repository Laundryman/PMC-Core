using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Http;

namespace CoreSystem.Helpers
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
    }
}