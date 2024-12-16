using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using Umbraco.Cms.Core.Configuration.Models;
using Umbraco.Cms.Core.Net;
using Umbraco.Cms.Core.Security;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Web.Common.Security;

namespace CoreSystem2024.Extensions
{
    internal class DiamMemberManager : MemberManager
    {
        public DiamMemberManager(IIpResolver ipResolver, IMemberUserStore store, IOptions<IdentityOptions> optionsAccessor, IPasswordHasher<MemberIdentityUser> passwordHasher, IEnumerable<IUserValidator<MemberIdentityUser>> userValidators, IEnumerable<IPasswordValidator<MemberIdentityUser>> passwordValidators, IdentityErrorDescriber errors, IServiceProvider services, ILogger<UserManager<MemberIdentityUser>> logger, IOptionsSnapshot<MemberPasswordConfigurationSettings> passwordConfiguration, IPublicAccessService publicAccessService, IHttpContextAccessor httpContextAccessor) : base(ipResolver, store, optionsAccessor, passwordHasher, userValidators, passwordValidators, errors, services, logger, passwordConfiguration, publicAccessService, httpContextAccessor)
        {
        }

        //public override async Task<MemberIdentityUser?> GetUserAsync(ClaimsPrincipal principal)
        //{
        //    var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";

        //    var customUser = new MemberIdentityUser();
        //    customUser.Id = userId;
        //    customUser.UserName = principal.Identity.Name;
        //    customUser.IsApproved = true;

        //    //foreach (var claim in principal.Claims.Where(x => x.Type == ClaimTypes.Role))
        //    //{
        //    //    customUser.AddRole(claim.Value);
        //    //}

        //    return await Task.FromResult(customUser);
        //}

        public override async Task<MemberIdentityUser?> GetCurrentMemberAsync()
        {
            var baseUser = await base.GetCurrentMemberAsync();

            var token = baseUser.LoginTokens.Where(t => t.Name == "access_token").FirstOrDefault();
            //read claims from token
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenValues = tokenHandler.ReadJwtToken(token.Value);
            //add in claims
            foreach (var claim in tokenValues.Claims)
            {
                baseUser.Claims.Add(new IdentityUserClaim<string>
                {
                    ClaimType = claim.Type,
                    ClaimValue = claim.Value,
                    UserId = baseUser.Id
                });
            }
            return baseUser;
        }
    }
}
