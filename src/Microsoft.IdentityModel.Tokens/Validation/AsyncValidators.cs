// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Abstractions;
using Microsoft.IdentityModel.Logging;

namespace Microsoft.IdentityModel.Tokens
{
    /// <summary>
    /// Common validators for <see cref="SecurityToken"/>s.
    /// </summary>
    internal static class AsyncValidators
    {
        /// <summary>
        /// Determines if an issuer found in a <see cref="SecurityToken"/> is valid.
        /// </summary>
        /// <param name="issuer">The issuer to validate</param>
        /// <param name="securityToken">The <see cref="SecurityToken"/> that is being validated.</param>
        /// <param name="validationParameters"><see cref="TokenValidationParameters"/> required for validation.</param>
        /// <returns>The issuer to use when creating the "Claim"(s) in a "ClaimsIdentity".</returns>
        /// <exception cref="ArgumentNullException">If 'validationParameters' is null.</exception>
        /// <exception cref="ArgumentNullException">If 'issuer' is null or whitespace and <see cref="TokenValidationParameters.ValidateIssuer"/> is true.</exception>
        /// <exception cref="SecurityTokenInvalidIssuerException">If <see cref="TokenValidationParameters.ValidIssuer"/> is null or whitespace and <see cref="TokenValidationParameters.ValidIssuers"/> is null.</exception>
        /// <exception cref="SecurityTokenInvalidIssuerException">If 'issuer' failed to matched either <see cref="TokenValidationParameters.ValidIssuer"/> or one of <see cref="TokenValidationParameters.ValidIssuers"/>.</exception>
        /// <remarks>An EXACT match is required.</remarks>
        public static async Task<IssuerValidationResult> ValidateIssuerAsync(
            string issuer,
            SecurityToken securityToken,
            TokenValidationParameters validationParameters)
        {
            if (string.IsNullOrWhiteSpace(issuer))
            {
                IssuerValidationResult result = new IssuerValidationResult
                {
                    Exception = LogHelper.LogArgumentNullException(nameof(issuer)),
                    IsValid = false,
                    StackTrace = new StackTrace(true),
                    StackFrame = new StackFrame(true),
                    ValidationFailureType = ValidationFailureType.NullArgument
                };

                return result;
            }

            if (validationParameters == null)
                throw LogHelper.LogArgumentNullException(nameof(validationParameters));

            if (securityToken == null)
                throw LogHelper.LogArgumentNullException(nameof(securityToken));

            if (validationParameters.IssuerValidatorAsync != null)
                return await validationParameters.IssuerValidatorAsync(issuer, securityToken, validationParameters).ConfigureAwait(false);

            if (string.IsNullOrWhiteSpace(issuer))
                throw LogHelper.LogExceptionMessage(new SecurityTokenInvalidIssuerException(LogMessages.IDX10211)
                { InvalidIssuer = issuer });

            BaseConfiguration configuration = null;
            if (validationParameters.ConfigurationManager != null)
                configuration = await validationParameters.ConfigurationManager.GetBaseConfigurationAsync(CancellationToken.None).ConfigureAwait(false);

            // Throw if all possible places to validate against are null or empty
            if (string.IsNullOrWhiteSpace(validationParameters.ValidIssuer)
                && validationParameters.ValidIssuers.IsNullOrEmpty()
                && string.IsNullOrWhiteSpace(configuration?.Issuer))
            {
                return new IssuerValidationResult
                {
                    Exception = new SecurityTokenInvalidIssuerException(LogMessages.IDX10204)
                    { InvalidIssuer = issuer },
                    IsValid = false,
                    StackTrace = new StackTrace(5, true),
                    ValidationFailureType = ValidationFailureType.IssuerValidationFailed
                };
            }

            if (configuration != null)
            {
                if (string.Equals(configuration.Issuer, issuer))
                {
                    if (LogHelper.IsEnabled(EventLogLevel.Informational))
                        LogHelper.LogInformation(LogMessages.IDX10236, LogHelper.MarkAsNonPII(issuer));

                    return new IssuerValidationResult
                    {
                        Issuer = issuer,
                        IsValid = true
                    };
                }
            }

            if (string.Equals(validationParameters.ValidIssuer, issuer))
            {
                if (LogHelper.IsEnabled(EventLogLevel.Informational))
                    LogHelper.LogInformation(LogMessages.IDX10236, LogHelper.MarkAsNonPII(issuer));

                return new IssuerValidationResult
                {
                    Issuer = issuer,
                    IsValid = true
                };
            }

            if (validationParameters.ValidIssuers != null)
            {
                foreach (string str in validationParameters.ValidIssuers)
                {
                    if (string.IsNullOrEmpty(str))
                    {
                        LogHelper.LogInformation(LogMessages.IDX10262);
                        continue;
                    }

                    if (string.Equals(str, issuer))
                    {
                        if (LogHelper.IsEnabled(EventLogLevel.Informational))
                            LogHelper.LogInformation(LogMessages.IDX10236, LogHelper.MarkAsNonPII(issuer));

                        return new IssuerValidationResult
                        {
                            Issuer = issuer,
                            IsValid = true
                        };
                    }
                }
            }

            SecurityTokenInvalidIssuerException ex = new SecurityTokenInvalidIssuerException(
                LogHelper.FormatInvariant(LogMessages.IDX10205,
                    LogHelper.MarkAsNonPII(issuer),
                    LogHelper.MarkAsNonPII(validationParameters.ValidIssuer ?? "null"),
                    LogHelper.MarkAsNonPII(Utility.SerializeAsSingleCommaDelimitedString(validationParameters.ValidIssuers)),
                    LogHelper.MarkAsNonPII(configuration?.Issuer)))
            { InvalidIssuer = issuer };

            return new IssuerValidationResult
            {
                Exception = ex,
                IsValid = false,
                StackTrace = new StackTrace(5, true),
                ValidationFailureType = ValidationFailureType.IssuerValidationFailed
            };
        }
    }
}
