using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Net;
using System.Threading.Tasks;
using WebApplication.Identity;

namespace WebApplication.Controllers
{
    /// <summary>
    /// The API authentication web service.
    /// </summary>
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : Controller
    {
        private readonly TokenValidation tokenValidationSettings;
        private readonly UserManager<ApplicationUser> userManager;

        /// <summary>
        /// AuthenticationController constructor.
        /// </summary>
        /// <param name="tokenValidationSettings">The token validation settings.</param>
        /// <param name="userManager">The ASP.NET Core Identity user manager service.</param>
        public AuthenticationController(IOptions<TokenValidation> tokenValidationSettings, UserManager<ApplicationUser> userManager)
        {
            this.tokenValidationSettings = tokenValidationSettings.Value;
            this.userManager = userManager;
        }

        /// <summary>
        /// Login information.
        /// </summary>
        public class LoginInfo
        {
            /// <summary>
            /// The user's username.
            /// </summary>
            public string UserName;

            /// <summary>
            /// The user's password.
            /// </summary>
            public string Password;
        }

        /// <summary>
        /// Authentication information returned when user is authenticated.
        /// </summary>
        public class ReturnAuthenticationInfo
        {
            /// <summary>
            /// The authenticated identity user.
            /// </summary>
            public ApplicationUser User;

            /// <summary>
            /// The authentication JWT token.
            /// </summary>
            public string Token;
        }

        /// <summary>
        /// Authenticates a user based on a username and password.
        /// </summary>
        /// <param name="loginInfo">The user's login information.</param>
        /// <returns>An IActionResult.</returns>
        [AllowAnonymous]
        [HttpPost("authenticate")]
        public async Task<IActionResult> AuthenticateAsync([FromBody] LoginInfo loginInfo)
        {
            try
            {
                var user = await userManager.FindByNameAsync(loginInfo.UserName);

                if (user != null)
                {
                    if (await userManager.CheckPasswordAsync(user, loginInfo.Password))
                    {
                        return Ok(new ReturnAuthenticationInfo()
                        {
                            User = user,
                            Token = await userManager.GenerateJwtSecurityTokenAsync(
                                  user,
                                  tokenValidationSettings.Issuer,
                                  tokenValidationSettings.IssuerSigningKey,
                                  tokenValidationSettings.Audience,
                                  DateTime.Now.AddMinutes(tokenValidationSettings.Expires)
                                )
                        });
                    }
                }

                return Unauthorized();
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// Test API to determine if user is authenticated (valid token).
        /// </summary>
        /// <returns>Return HTTP status 200.</returns>
        [HttpGet("isAuthenticated")]
        [Authorize]
        public IActionResult IsAuthenticated()
        {
            return Ok();
        }

        /// <summary>
        /// Test API to determine if user is authorized (valid token with Administrator role).
        /// </summary>
        /// <returns>Return HTTP status 200.</returns>
        [HttpGet("isAdministrator")]
        [Authorize(Roles = "Administrator")]
        public IActionResult IsAdministrator()
        {
            return Ok();
        }
    }
}