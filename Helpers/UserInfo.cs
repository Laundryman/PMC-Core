using Dplo.ViewModels;
using Umbraco.Cms.Core.Security;

namespace CoreSystem2024.Helpers
{
    public class UserInfo
    {
        private static UserViewModel user;
        public UserInfo(MemberIdentityUser mUser)
        {
            user = AuthHelper.GetUserInfo(mUser);
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