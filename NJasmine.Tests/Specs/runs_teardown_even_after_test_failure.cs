﻿using System;
using NJasmineTests.Core;
using NJasmineTests.Export;
using NUnit.Framework;

namespace NJasmineTests.Specs
{
    [Explicit]
    public class runs_teardown_even_after_test_failure : GivenWhenThenFixtureTracingToConsole, INJasmineInternalRequirement
    {
        public override void Specify()
        {
            beforeAll(ResetTracing);

            afterEach(delegate()
            {
                Trace("failure_in_test_doesnt_prevent_cleanup");
            });

            describe("failng describe", delegate()
            {
                afterEach(delegate()
                {
                    Trace("failure_in_test_doesnt_prevent_cleanup_in_same_scope");
                });

                it("failing test", delegate()
                {
                    throw new Exception("intended test failure");
                });
            });
        }

        public void Verify(TestResult testResult)
        {
            testResult.failed();

            testResult.containsTrace(@"
failure_in_test_doesnt_prevent_cleanup_in_same_scope
failure_in_test_doesnt_prevent_cleanup");
        }
    }
}