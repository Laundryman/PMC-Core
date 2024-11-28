using Dplo.ViewModels;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Web;
using Dplo.ViewModels;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace CoreSystem.Helpers
{
    public class UserInfo
    {
        private static UserViewModel user;
        public UserInfo(ClaimsPrincipal principal)
        {
             user = AuthHelper.GetUserInfo(principal);
        }

        public static UserViewModel userViewModel => user;
        public static string Id => user.Id;
        public static string GivenName => user.GivenName;
        public static string Surname => user.Surname;
        public static string Roles => user.Roles;
        public static int DiamCountryId => user.DiamCountryId;
        public static string DiamCountryName => user.DiamCountryName;
        public static string UserName => user.UserName;

        public static string FullName => $"{GivenName} {Surname}";
    }
}