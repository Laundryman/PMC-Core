using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Web.Common.Security;

namespace CoreSystem2024.OpenIdProvider;

public class OpenIdConnectMemberExternalLoginProviderOptions : IConfigureNamedOptions<MemberExternalLoginProviderOptions>
{
    public const string SchemeName = "OpenIdConnect";
    public readonly IMemberService _memberService;

    public OpenIdConnectMemberExternalLoginProviderOptions(IMemberService memberService)
    {
        _memberService = memberService;
    }

    public void Configure(string? name, MemberExternalLoginProviderOptions options)
    {
        if (name != Constants.Security.MemberExternalAuthenticationTypePrefix + SchemeName)
        {
            return;
        }

        Configure(options);
    }

    public void Configure(MemberExternalLoginProviderOptions options)
    {
        options.AutoLinkOptions = new MemberExternalSignInAutoLinkOptions(
            // Must be true for auto-linking to be enabled
            autoLinkExternalAccount: true,

            // Optionally specify the default culture to create
            // the user as. If null it will use the default
            // culture defined in the web.config, or it can
            // be dynamically assigned in the OnAutoLinking
            // callback.
            defaultCulture: null,

            // Optionally specify the default "IsApprove" status. Must be true for auto-linking.
            defaultIsApproved: true,

            // Optionally specify the member type alias. Default is "Member"
            defaultMemberTypeAlias: "DiamMember",

            // Optionally specify the member groups names to add the auto-linking user to.
            defaultMemberGroups: new List<string> { "Diagramm Users" }
        )
        {
            // Optional callback
            OnAutoLinking = (autoLinkUser, loginInfo) =>
            {
                // You can customize the user before it's linked.
                // i.e. Modify the user's groups based on the Claims returned
                // in the externalLogin info
            },
            OnExternalLogin = (user, loginInfo) =>
            {
                // You can customize the user before it's saved whenever they have
                // logged in with the external provider.
                // i.e. Sync the user's name based on the Claims returned

                //ADD NAMEIDENTIFIER for Azureb2c ID

                IMember? member = _memberService.GetByKey(user.Key);
                var extClaim = loginInfo
                    .Principal
                    .FindFirst(ClaimTypes.GivenName);
                user.Claims.Add(new IdentityUserClaim<string>
                {
                    ClaimType = extClaim.Type,
                    ClaimValue = extClaim.Value,
                    UserId = user.Id
                });
                var givenname = member.Properties.FirstOrDefault(p => p.Alias == "givenname");
                givenname.SetValue(extClaim.Value);

                extClaim = loginInfo
                    .Principal
                    .FindFirst(ClaimTypes.NameIdentifier);

                //var nameIdentifier = member.Properties.FirstOrDefault(p => p.Alias == "nameidentifier");
                user.Claims.Add(new IdentityUserClaim<string>
                {
                    ClaimType = extClaim.Type,
                    ClaimValue = extClaim.Value,
                    UserId = user.Id
                });
                //member.Properties.FirstOrDefault(p => p.Alias == "nameidentifier").SetValue(extClaim.Value);

                extClaim = loginInfo
                    .Principal
                    .FindFirst("name");
                user.Claims.Add(new IdentityUserClaim<string>
                {
                    ClaimType = extClaim.Type,
                    ClaimValue = extClaim.Value,
                    UserId = user.Id
                });

                member.Properties.FirstOrDefault(p => p.Alias == "displayname").SetValue(extClaim.Value);

                extClaim = loginInfo
                    .Principal
                    .FindFirst(ClaimTypes.Surname);
                user.Claims.Add(new IdentityUserClaim<string>
                {
                    ClaimType = extClaim.Type,
                    ClaimValue = extClaim.Value,
                    UserId = user.Id
                });
                member.Properties.FirstOrDefault(p => p.Alias == "surname").SetValue(extClaim.Value);


                extClaim = loginInfo
                    .Principal
                    .FindFirst("extension_userEmailAddress");
                user.Claims.Add(new IdentityUserClaim<string>
                {
                    ClaimType = extClaim.Type,
                    ClaimValue = extClaim.Value,
                    UserId = user.Id
                });


                extClaim = loginInfo
                    .Principal
                    .FindFirst("extension_diamRoles");
                user.Claims.Add(new IdentityUserClaim<string>
                {
                    ClaimType = extClaim.Type,
                    ClaimValue = extClaim.Value,
                    UserId = user.Id
                });
                member.Properties.FirstOrDefault(p => p.Alias == "roles").SetValue(extClaim.Value);

                extClaim = loginInfo
                    .Principal
                    .FindFirst("extension_diamCountryId");
                user.Claims.Add(new IdentityUserClaim<string>
                {
                    ClaimType = extClaim.Type,
                    ClaimValue = extClaim.Value,
                    UserId = user.Id
                });
                member.Properties.FirstOrDefault(p => p.Alias == "diamcountryid").SetValue(extClaim.Value);

                extClaim = loginInfo
                    .Principal
                    .FindFirst("extension_brands");
                user.Claims.Add(new IdentityUserClaim<string>
                {
                    ClaimType = extClaim.Type,
                    ClaimValue = extClaim.Value,
                    UserId = user.Id
                });
                member.Properties.FirstOrDefault(p => p.Alias == "brands").SetValue(extClaim.Value);
                _memberService.Save(member);

                return true; //returns a boolean indicating if sign in should continue or not.
            }
        };
    }
}