using Microsoft.Extensions.Options;
using System.Collections.Generic;
using WebApplication.Identity;

namespace WebApplication.Identity.Initialize
{
    public class AppSettings : IOptions<AppSettings>
    {
        /// <summary>
        /// A list of the applications roles to be created.
        /// </summary>
        public List<string> Roles { get; set; }

        /// <summary>
        /// The user to be created. Username, email, first name, middle initial, last name, rank are only required.
        /// </summary>
        public ApplicationUser User { get; set; }

        /// <summary>
        /// The user's password.
        /// </summary>
        public string UserPassword { get; set; }

        /// <summary>
        /// A list of the user's roles.
        /// </summary>
        public List<string> UserRoles { get; set; }

        public AppSettings Value => this;
    }
}
