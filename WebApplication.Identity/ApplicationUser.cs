using Microsoft.AspNetCore.Identity;
using System;

namespace WebApplication.Identity
{
    /// <summary>
    /// Identity user class defined to force use of Guid for primary key.
    /// </summary>
    /// <remarks>
    /// This class can also be used to define additional data fields and constructors.
    /// </remarks>
    public class ApplicationUser : IdentityUser<Guid>
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public ApplicationUser()
        : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of ApplicationUser.
        /// </summary>
        /// <param name="userName">The user name.</param>
        /// <param name="email">The user's email address.</param>
        public ApplicationUser(string userName, string email)
        : base(userName)
        {
            base.Email = email ?? "";
        }
    }
}
