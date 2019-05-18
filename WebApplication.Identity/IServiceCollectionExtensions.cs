using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace WebApplication.Identity
{
    /// <summary>
    /// Contains extension methods to IServiceCollection.
    /// </summary>
    public static class IServiceCollectionExtensions
    {
        /// <summary>
        /// An extension method for configuring the application's ASP.NET Core Identity service.
        /// </summary>
        /// <param name="services">The service collection on which the extension operates.</param>
        /// <remarks>
        /// This extension method is for the purpose of keeping the AddIdentity call consistent in testing and initialization.
        /// </remarks>
        public static IServiceCollection AddApplicationIdentity(this IServiceCollection services)
        {
            services.AddIdentity<ApplicationUser, ApplicationRole>()
                .AddUserManager<UserManager<ApplicationUser>>()
                .AddEntityFrameworkStores<ApplicationIdentityDbContext>()
                .AddDefaultTokenProviders(); 

            return services;
        }
    }
}
