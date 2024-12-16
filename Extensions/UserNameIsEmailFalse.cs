using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.Configuration.Models;
using Umbraco.Cms.Core.DependencyInjection;

namespace CoreSystem2024.Extensions
{
    //public sealed class MyConfigureMemberIdentityOptions : IPostConfigureOptions<IdentityOptions>
    //{
    //    public void PostConfigure(string? name, IdentityOptions options)
    //    {
    //        options.User.RequireUniqueEmail = false;
    //    }
    //}

    public static class UmbbecurityServiceExtensions
    {
        public static IUmbracoBuilder EnableDupeEmailAddresses(this IUmbracoBuilder builder)
        {
            builder.Services.PostConfigure<SecuritySettings>(options =>
            {
                options.UsernameIsEmail = false;
            });

            return builder;
        }
    }
}
