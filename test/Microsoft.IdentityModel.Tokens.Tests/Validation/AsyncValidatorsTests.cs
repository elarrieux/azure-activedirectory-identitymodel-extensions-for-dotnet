// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Diagnostics;
using Microsoft.IdentityModel.TestUtils;
using Xunit;

namespace Microsoft.IdentityModel.Tokens.Tests
{
    public class AsyncValidatorTests
    {
        [Fact]
        public void AsyncValidator_IssuerValidationResult()
        {
            var issuerValidationResult = new IssuerValidationResult();
            Assert.False(issuerValidationResult.IsValid);
            Assert.Null(issuerValidationResult.Exception);
            Assert.Null(issuerValidationResult.Issuer);
        }

        [Theory, MemberData(nameof(AsyncIssuerValidatorTestCases))]
        public void AsyncIssuerValidatorTests(IssuerValidatorTheoryData theoryData)
        {
            CompareContext context = TestUtilities.WriteHeader($"{this}.AsyncIssuerValidatorTests", theoryData);

            IssuerValidationResult result =
                AsyncValidators.ValidateIssuerAsync(
                    theoryData.Issuer,
                    theoryData.SecurityToken,
                    theoryData.ValidationParameters).Result;

            Exception ex;

            try
            {
                throw new ArgumentException("test");
            }
            catch (Exception e)
            {
                ex = e;
            }

            StackFrame stackFrame = new StackFrame(true);
            StackFrame stackFrame1 = new StackFrame(1, true);

            ex = new Exception("test", ex);

        }

        public static TheoryData<IssuerValidatorTheoryData> AsyncIssuerValidatorTestCases
        {
            get
            {
                TheoryData<IssuerValidatorTheoryData> theoryData = new TheoryData<IssuerValidatorTheoryData>();

                theoryData.Add(new IssuerValidatorTheoryData
                {
                    Issuer = null,
                    ValidationParameters = new TokenValidationParameters(),
                });

                return theoryData;
            }
        }
    }

    public class IssuerValidatorTheoryData : TheoryDataBase
    {
        public string Issuer { get; set; }
        public TokenValidationParameters ValidationParameters { get; set; }
        public SecurityToken SecurityToken { get; set; }
    }
}
