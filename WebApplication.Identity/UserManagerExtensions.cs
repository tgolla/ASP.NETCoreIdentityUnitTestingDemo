using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace WebApplication.Identity
{
    /// <summary>
    /// Contains extension methods to UserManager.
    /// </summary>
    public static class UserManagerExtensions
    {
        /// <summary>
        /// Changes the user's password.
        /// </summary>
        /// <typeparam name="TUser">The type encapsulating a user.</typeparam>
        /// <param name="manager">The user manager instance.</param>
        /// <param name="user">The user.</param>
        /// <param name="newPassword">The new password.</param>
        /// <returns></returns>
        public static async Task<IdentityResult> ChangePasswordAsync<TUser>(this UserManager<TUser> manager, TUser user, string newPassword)
            where TUser : class
        {
            string resetPasswordToken = await manager.GeneratePasswordResetTokenAsync(user);
            IdentityResult passwordChangeResult = await manager.ResetPasswordAsync(user, resetPasswordToken, newPassword);

            return new IdentityResult();
        }

        /// <summary>
        /// Generates a JWT token from the user's information.
        /// </summary>
        /// <typeparam name="TUser">The type encapsulating a user.</typeparam>
        /// <param name="manager">The user manager instance.</param>
        /// <param name="user">The user.</param>
        /// <param name="issuer">If this value is not null, a { iss, 'issuer' } claim will be added.</param>
        /// <param name="issuerSigningKey">The SigningCredentials that will be used to sign the JwtSecurityToken.</param>
        /// <param name="audience">If this value is not null, a { aud, 'audience' } claim will be added.</param>
        /// <param name="expires">If expires.HasValue a { exp, 'value' } claim is added.</param>
        /// <returns>A JWT token from the user's information.</returns>
        public static async Task<string> GenerateJwtSecurityTokenAsync<TUser>(this UserManager<TUser> manager, TUser user,
            string issuer, string issuerSigningKey, string audience, DateTime expires)
                where TUser : class
        {
            var claimsIdentity = new ClaimsIdentity();

            claimsIdentity.AddClaim(new Claim(ClaimTypes.NameIdentifier, await manager.GetUserNameAsync(user)));
            claimsIdentity.AddClaim(new Claim(ClaimTypes.Email, await manager.GetEmailAsync(user)));

            // Add users roles from security provider to identity claims only if account active.
            var roles = await manager.GetRolesAsync(user);

            foreach (string role in roles)
            {
                claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(issuerSigningKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claimsIdentity.Claims,
                expires: expires,
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
