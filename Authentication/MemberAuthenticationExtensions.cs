using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Security.Claims;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Core.Security;
using Umbraco.Extensions;

namespace CoreSystemII.Authentication
{
    public static class MemberAuthenticationExtensions
    {
        public static IUmbracoBuilder AddOpenIdConnectAuthentication(this IUmbracoBuilder builder)
        {
            builder.Services.ConfigureOptions<AzureB2CMembersExternalLoginProviderOptions>();



            builder.AddMemberExternalLogins(logins =>
            {
                //const string schema = MicrosoftAccountDefaults.AuthenticationScheme;

                logins.AddMemberLogin(
                    membersAuthenticationBuilder =>
                    {
                        membersAuthenticationBuilder.AddOpenIdConnect(
                        membersAuthenticationBuilder.SchemeForMembers(AzureB2CMembersExternalLoginProviderOptions.SchemeName),
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

                            options.Events.OnTokenValidated = async context =>
                            {
                                var claims = context?.Principal?.Claims.ToList();
                                var email = claims?.SingleOrDefault(x => x.Type == "extension_userEmailAddress");
                                if (email != null)
                                {
                                    // The email claim is required for auto linking.
                                    // So get it from another claim and put it in the email claim.
                                    claims?.Add(new Claim(ClaimTypes.Email, email.Value));
                                }

                                var name = claims?.SingleOrDefault(x => x.Type == "name");
                                if (name != null)
                                {
                                    // The name claim is required for auto linking.
                                    // So get it from another claim and put it in the name claim.
                                    claims?.Add(new Claim(ClaimTypes.Name, name.Value));
                                }

                                if (context != null)
                                {

                                    var authenticationType = context.Principal?.Identity?.AuthenticationType;
                                    context.Principal = new ClaimsPrincipal(new ClaimsIdentity(claims, authenticationType));
                                }

                                await Task.FromResult(0);
                            };
                            options.Events.OnRedirectToIdentityProviderForSignOut = async notification =>
                            {
                                var protocolMessage = notification.ProtocolMessage;

                                var logoutUrl = config["OpenIdConnect:LogoutUrl"];
                                var returnAfterLogout = config["OpenIdConnect:ReturnAfterLogout"];
                                if (!string.IsNullOrEmpty(logoutUrl) && !string.IsNullOrEmpty(returnAfterLogout))
                                {
                                    // Some external login providers require an IssuerAddress.
                                    // It requires the logout URL on the external login provider.
                                    // It also need the client_id and a URL which it needs to return to after logout.
                                    protocolMessage.IssuerAddress =
                                        $"{config["OpenIdConnect:LogoutUrl"]}" +
                                        $"?client_id={config["OpenIdConnect:ClientId"]}" +
                                        $"&returnTo={WebUtility.UrlEncode(config["OpenIdConnect:ReturnAfterLogout"])}";
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
                            options.Events.OnRemoteFailure = context =>
                            {
                                // This is where you can handle the error from the external login provider.
                                // For example, you can log the error or redirect the user to a custom error page.
                                context.HandleResponse();
                                context.Response.Redirect("/error?message=" + context.Failure.Message);
                                return Task.CompletedTask;
                            };
                        });
                    });
            });
            return builder;
        }
    }
}
