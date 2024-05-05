// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.IdentityModel.Tokens
{
    /// <summary>
    /// Contains the result of validating the issuer.
    /// The <see cref="TokenValidationResult"/> contains a collection of <see cref="ValidationResult"/> for each step in the token validation.
    /// </summary>
    public class IssuerValidationResult : ValidationResult
    {
        /// <summary>
        /// Creates an instance of <see cref="IssuerValidationResult"/>
        /// </summary>
        public IssuerValidationResult()
        {
        }

        /// <summary>
        /// Gets or sets the issuer that was validated.
        /// </summary>
        public string Issuer { get; set; } 
    }
}
