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
        private ILogger<UserManager<MemberIdentityUser>> _logger;
        public DiamMemberManager(IIpResolver ipResolver, IMemberUserStore store, IOptions<IdentityOptions> optionsAccessor, IPasswordHasher<MemberIdentityUser> passwordHasher, IEnumerable<IUserValidator<MemberIdentityUser>> userValidators, IEnumerable<IPasswordValidator<MemberIdentityUser>> passwordValidators, IdentityErrorDescriber errors, IServiceProvider services, ILogger<UserManager<MemberIdentityUser>> logger, IOptionsSnapshot<MemberPasswordConfigurationSettings> passwordConfiguration, IPublicAccessService publicAccessService, IHttpContextAccessor httpContextAccessor) : base(ipResolver, store, optionsAccessor, passwordHasher, userValidators, passwordValidators, errors, services, logger, passwordConfiguration, publicAccessService, httpContextAccessor)
        {
            _logger = logger;
        }

        //public override async Task<MemberIdentityUser?> GetUserAsync(ClaimsPrincipal principal)
        //{
        //    var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";

        //    var customUser = new MemberIdentityUser();
        //    customUser.id = userId;
        //    customUser.UserName = principal.identity.Name;
        //    customUser.IsApproved = true;

        //    //foreach (var claim in principal.Claims.Where(x => x.Type == ClaimTypes.Role))
        //    //{
        //    //    customUser.AddRole(claim.Value);
        //    //}

        //    return await Task.FromResult(customUser);
        //}

        public override async Task<MemberIdentityUser?> GetCurrentMemberAsync()
        {
            try
            {
                var baseUser = await base.GetCurrentMemberAsync();

                var token = baseUser.LoginTokens.Where(t => t.Name == "id_token").FirstOrDefault();
                var accessToken = baseUser.LoginTokens.FirstOrDefault(t => t.Name == "access_token");
                //read claims from token
                var tokenHandler = new JwtSecurityTokenHandler();
                var tokenValues = tokenHandler.ReadJwtToken(token.Value);
                var accessTokenValues = tokenHandler.ReadJwtToken(accessToken.Value);
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting current member");
                return null;
            }
        }
    }
}
