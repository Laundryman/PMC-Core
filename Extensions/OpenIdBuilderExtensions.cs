using CoreSystem2024.OpenIdProvider;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Security.Claims;
using Microsoft.CodeAnalysis.Diagnostics;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Core.Models.Membership;
using Umbraco.Cms.Core.Security;
using Umbraco.Extensions;

namespace CoreSystem2024.Extensions;

public static class OpenIdBuilderExtensions
{
    public static IUmbracoBuilder AddOpenIdConnectAuthentication(this IUmbracoBuilder builder)
    {
        builder.Services.ConfigureOptions<OpenIdConnectMemberExternalLoginProviderOptions>();

        builder.AddMemberExternalLogins(logins =>
        {
            logins.AddMemberLogin(
                memberAuthenticationBuilder =>
                {
                    memberAuthenticationBuilder.AddOpenIdConnect(
                        // The scheme must be set with this method to work for the umbraco members
                        memberAuthenticationBuilder.SchemeForMembers(OpenIdConnectMemberExternalLoginProviderOptions.SchemeName),
                        options =>
                        {
                            var config = builder.Config;
                            options.ResponseType = "token id_token";
                            options.ResponseMode = "form_post";
                            options.Scope.Add("https://deeplan.onmicrosoft.com/dssapi/data.Read");
                            options.Scope.Add("https://deeplan.onmicrosoft.com/dssapi/data.Write");
                            options.Scope.Add("openid");
                            options.Scope.Add("profile");
                            options.Scope.Add("offline_access");
                            //options.Scope.Add("email");
                            //options.Scope.Add("phone");
                            //options.Scope.Add("address");
                            options.RequireHttpsMetadata = true;
                            options.MetadataAddress = config["AzureB2c:MetadataAddress"];
                            options.ClientId = config["AzureB2c:ClientId"];

                            // Normally the ClientSecret should not be in the Github repo.
                            // These settings are valid and only used for this example.
                            // So it's ok these are public.
                            options.ClientSecret = config["AzureB2c:ClientSecret"];
                            options.SaveTokens = true;
                            options.TokenValidationParameters.SaveSigninToken = true;
                            options.CallbackPath = "/login";
                            options.UseTokenLifetime = true;
                            //options.Events.OnAuthorizationCodeReceived = async context =>
                            //{
                            //    await Task.FromResult(0);
                            //};
                            options.Events.OnRedirectToIdentityProvider = async context =>
                            {
                                // This event is called when the redirect to the external login provider is made.
                                // This is the place to add custom logic.
                                await Task.FromResult(0);
                            };
                            options.Events.OnTokenValidated = async context =>
                            {
                                var claims = context?.Principal?.Claims.ToList();
                                var userBrands = claims?.SingleOrDefault(x => x.Type == "extension_brands");
                                var siteBrand = config["AppSettings:ClientBrandId"];
                                var IsAuthenticated = true;
                                if (userBrands != null && siteBrand != null)
                                {
                                    if (!userBrands.Value.Contains(siteBrand))
                                    {
                                        context.Fail("User is not authorised to access this site. Incorrect brand.");
                                        IsAuthenticated = false;
                                    }
                                }
                                else
                                {
                                    context.Fail("User is not authorised to access this site. Incorrect brand.");
                                    IsAuthenticated = false;
                                }

                                if (IsAuthenticated)
                                {
                                    var name = claims?.SingleOrDefault(x => x.Type == "name");
                                    if (name != null)
                                    {
                                        // The name claim is required for auto linking.
                                        // So get it from another claim and put it in the name claim.
                                        claims?.Add(new Claim(ClaimTypes.Name, name.Value));
                                    }

                                    var email = claims?.SingleOrDefault(x => x.Type == "extension_userEmailAddress");
                                    if (email != null)
                                    {
                                        // The email claim is required for auto linking.
                                        // So get it from another claim and put it in the email claim.
                                        var newEmail = name.Value.Replace(" ", string.Empty) + email.Value;
                                        //email.Value = newEmail;
                                        claims?.Add(new Claim(ClaimTypes.Email, newEmail));
                                    }

                                    if (context != null)
                                    {

                                        var authenticationType = context.Principal?.Identity?.AuthenticationType;
                                        context.Principal =
                                            new ClaimsPrincipal(new ClaimsIdentity(claims, authenticationType));
                                    }
                                }

                                await Task.FromResult(0);
                            };
                            options.Events.OnRedirectToIdentityProviderForSignOut = async notification =>
                            {
                                var protocolMessage = notification.ProtocolMessage;

                                var logoutUrl = config["AzureB2c:LogoutUrl"];
                                var returnAfterLogout = config["AzureB2c:ReturnAfterLogout"];
                                if (!string.IsNullOrEmpty(logoutUrl) && !string.IsNullOrEmpty(returnAfterLogout))
                                {
                                    // Some external login providers require an IssuerAddress.
                                    // It requires the logout URL on the external login provider.
                                    // It also need the client_id and a URL which it needs to return to after logout.
                                    protocolMessage.IssuerAddress =
                                        $"{config["AzureB2c:LogoutUrl"]}" +
                                        $"?client_id={config["AzureB2c:ClientId"]}" +
                                        $"&returnTo={WebUtility.UrlEncode(config["AzureB2c:ReturnAfterLogout"])}";
                                }

                                // Since we're in a static extension method we need this approach to get the member manager. 
                                var memberManager = notification.HttpContext.RequestServices.GetService<IMemberManager>();

                                if (memberManager != null)
                                {
                                    var currentMember = await memberManager.GetCurrentMemberAsync();

                                    // On the current member we can find all their login tokens from the external login provider.
                                    // These tokens are stored in the umbracoExternalLoginToken table.
                                    var idToken = currentMember?.LoginTokens.FirstOrDefault(x => x.Name == "id_token");
                                    if (idToken != null && !string.IsNullOrEmpty(idToken.Value))
                                    {
                                        // Some external login providers need the IdTokenHint.
                                        // By setting the IdTokenHint the user can be redirected back from the external login provider to this website. 
                                        protocolMessage.IdTokenHint = idToken.Value;
                                    }
                                }

                                await Task.FromResult(0);
                            };
                            options.Events.OnAuthenticationFailed = async context =>
                            {
                                await Task.FromResult(0);
                            };

                            options.Events.OnAuthorizationCodeReceived = async context =>
                            {
                                await Task.FromResult(0);
                            };
                            options.Events.OnRemoteFailure = async context =>
                            {
                                var errorMessage = context.Failure.Message;
                                if (errorMessage == "User is not authorised to access this site. Incorrect brand.")
                                {
                                    context.HandleResponse();
                                    context.Response.Redirect("/Error?err=1", false);
                                }
                                else
                                {
                                    context.HandleResponse();
                                    context.Response.Redirect("/Error?err=2", false);
                                }
                                await Task.CompletedTask;

                            };
                            options.Events.OnTokenResponseReceived = async context =>
                            {
                                await Task.FromResult(0);
                            };
                            options.Events.OnUserInformationReceived = async notification =>
                            {
                                // This event is called when the message is received.
                                // This is the place to add custom logic.
                                await Task.FromResult(0);
                            };

                        });
                });
        });
        return builder;
    }
}