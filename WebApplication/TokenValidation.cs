using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebApplication
{
    /// <summary>
    /// Necessary token validation settings.
    /// </summary>
    public class TokenValidation
    {
        /// <summary>
        /// Gets or sets a string that represents an audience that will be used to check against the token's audience. i.e. sso.example.com
        /// </summary>
        public string Audience { get; set; } = "";

        /// <summary>
        /// Gets or sets the clock skew (minutes) to apply when validating a time.  
        /// </summary>
        public int ClockSkew { get; set; } = 5;

        /// <summary>
        /// Gets or sets a the token expiration (minutes). 
        /// </summary>
        public int Expires { get; set; } = 20;

        /// <summary>
        /// Gets or sets a string that represents an issuer that will be used to check against the token's issuer. i.e. aud.example.com
        /// </summary>
        public string Issuer { get; set; } = "";

        /// <summary>
        /// Gets or sets the string that is to be used for signature validation.
        /// </summary>
        public string IssuerSigningKey { get; set; } = "";

        /// <summary>
        /// Gets or sets a string that represents a valid audience that will be used to check against the token's audience.
        /// </summary>
        public bool ValidateAudience { get; set; } = true;

        /// <summary>
        /// Gets or sets a boolean to control if the issuer will be validated during token validation.
        /// </summary>
        public bool ValidateIssuer { get; set; } = true;

        /// <summary>
        /// Gets or sets a boolean that controls if validation of the signing key that signed the security token is called.
        /// </summary>
        public bool ValidateIssuerSigningKey { get; set; } = true;

        /// <summary>
        /// Gets or sets a boolean to control if the lifetime will be validated during token validation.
        /// </summary>
        public bool ValidateLifetime { get; set; } = true;
        
        /// <summary>
        /// 
        /// </summary>
        public TokenValidation Value => this;
    }
}