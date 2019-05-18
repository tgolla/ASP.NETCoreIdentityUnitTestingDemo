using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace WebApplication.Identity
{
    /// <summary>
    /// Identity role class defined to force use of Guid for primary key.
    /// </summary>
    /// <remarks>
    /// This class can also be used to define additional data fields and constructors.
    /// </remarks>
    public class ApplicationRole : IdentityRole<Guid>
    {
        /// <summary>
        /// Initializes a new instance of ApplicationRole.
        /// </summary>
        public ApplicationRole()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of ApplicationRole.
        /// </summary>
        /// <param name="roleName">The role name.</param>
        public ApplicationRole(string roleName)
            : base(roleName)
        {
        }
    }
}
