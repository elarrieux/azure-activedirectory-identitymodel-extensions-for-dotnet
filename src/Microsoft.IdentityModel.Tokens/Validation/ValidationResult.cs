// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Diagnostics;

namespace Microsoft.IdentityModel.Tokens
{
    /// <summary>
    /// Contains artifacts obtained when a SecurityToken is validated.
    /// A <see cref="TokenValidationResult"/> returns a collection of <see cref="ValidationResult"/> for each step in the token validation.
    /// </summary>
    public abstract class ValidationResult
    {
        private Exception _exception;
        private bool _hasIsValidOrExceptionBeenRead = false;
        private bool _isValid = false;

        /// <summary>
        /// Creates an instance of <see cref="TokenValidationResult"/>
        /// </summary>
        public ValidationResult()
        {
        }

        /// <summary>
        /// Gets or sets the <see cref="Exception"/> that occurred during validation.
        /// </summary>
        public Exception Exception
        {
            get
            {
                _hasIsValidOrExceptionBeenRead = true;
                return _exception;
            }
            set
            {
                _exception = value;
            }
        }

        /// <summary>
        /// True if the token was successfully validated, false otherwise.
        /// </summary>
        public bool IsValid
        {
            get
            {
                _hasIsValidOrExceptionBeenRead = true;
                return _isValid;
            }
            set
            {
                _isValid = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool HasValidOrExceptionWasRead => _hasIsValidOrExceptionBeenRead;

        /// <summary>
        /// Gets or sets the <see cref="StackFrame"/> that was validated.
        /// </summary>
        public StackFrame StackFrame { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="StackTrace"/> that was validated.
        /// </summary>
        public StackTrace StackTrace { get; set; }

        /// <summary>
        /// Gets the <see cref="ValidationFailureType"/> indicating why the validation was not satisfied.
        /// This should not be set to null.
        /// </summary>
        public ValidationFailureType ValidationFailureType
        {
            get;
            set;
        } = ValidationFailureType.ValidationNotEvaluated;
    }
}
