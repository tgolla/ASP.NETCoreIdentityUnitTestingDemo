using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication.Identity
{
    /// <summary>
    /// Contains extension methods to IdentityResult.
    /// </summary>
    public static class IdentityResultExtensions
    {
        /// <summary>
        /// Returns a comma/space seperated string of the identity result error description(s).
        /// </summary>
        /// <param name="identityResult">The IdentityResult.</param>
        /// <returns>A comma/space seperated string of the identity result error description(s).</returns>
        public static string ErrorsToString(this IdentityResult identityResult)
        {
            string errors = "";
            string commaSpace = "";

            foreach(IdentityError error in identityResult.Errors)
            {
                errors = commaSpace + error.Description;
                commaSpace = ", ";
            }
            
            return errors;
        }
    }
}
