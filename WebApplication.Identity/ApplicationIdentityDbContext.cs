using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;

namespace WebApplication.Identity
{
    /// <summary>
    /// Application identity db context derived from the base class (IdentityDbContext<TUser, TRole, TKey>)
    /// for the Entity Framework database context used for identity.
    /// </summary>
    public class ApplicationIdentityDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
    {
        /// <summary>
        /// Initializes a new instance of ApplicationIdentityDbContext.
        /// </summary>
        /// <param name="options">The options to be used by a DbContext.</param>
        public ApplicationIdentityDbContext(DbContextOptions<ApplicationIdentityDbContext> options)
            : base(options)
        {
        }
    }
}