using System.Security.Claims;
using PMApplication.Dtos;
using Umbraco.Cms.Core.Security;

namespace CoreSystem2024.Helpers
{
    public class UserInfo
    {
        private static CurrentUser user;
        public UserInfo(ClaimsPrincipal mUser)
        {
            user = AuthHelper.GetUserInfo(mUser);
        }

        public static CurrentUser userViewModel => user;
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