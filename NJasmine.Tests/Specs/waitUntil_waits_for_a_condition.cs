﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using NJasmine;
using NJasmineTests.Export;
using NUnit.Framework;

namespace NJasmineTests.Specs
{
    [Explicit]
    public class waitUntil_waits_for_a_condition : GivenWhenThenFixture, INJasmineInternalRequirement
    {
        public int WaitsLeft;

        public bool Ready()
        {
            return WaitsLeft-- <= 0;
        }

        public override void Specify()
        {
            given("either waitUntil or expectEventually", delegate
            {
                foreach(var kvp in new Tuple<Action<Expression<Func<bool>>, int?,int?>, string>[]
                {
                    new Tuple<Action<Expression<Func<bool>>, int?,int?>, string> ( waitUntil, "waitUntil"),
                    new Tuple<Action<Expression<Func<bool>>, int?,int?>, string> ( expectEventually, "expectEventually")
                })
                {
                    var delayedAssertionImplementation = kvp.Item1;
                    var delayedAssertionName = kvp.Item2;

                    given("a condition that eventually evaluates to true", delegate
                    {
                        it("a normal expect works when no waits are left", delegate
                        {
                            WaitsLeft = 0;
                            expect(() => Ready());
                        });

                        it("a normal expect fails when waits are left", delegate
                        {
                            WaitsLeft = 1;
                            expect(() => Ready());
                        });

                        it(delayedAssertionName + " will try multiple times", delegate
                        {
                            setWaitTimeout(100);
                            setWaitIncrement(5);

                            WaitsLeft = 1;

                            delayedAssertionImplementation(() => Ready(), null, null);

                            delayedAssertionImplementation(() => Ready(), 2000, null);
                        });
                    });

                    describe(delayedAssertionName + " can be called during discovery", delegate
                    {
                        setWaitTimeout(5);
                        setWaitIncrement(3);

                        delayedAssertionImplementation(() => true, null, null);
                        delayedAssertionImplementation(() => false, null, null);

                        it("doesnt prevent discovery", delegate
                        {

                        });
                    });
                }
            });
        }

        public void Verify(TestResult testResult)
        {
            testResult.failed();

            testResult.hasTest("NJasmineTests.Specs.waitUntil_waits_for_a_condition, given either waitUntil or expectEventually, given a condition that eventually evaluates to true, a normal expect works when no waits are left")
                .thatSucceeds();

            testResult.hasTest("NJasmineTests.Specs.waitUntil_waits_for_a_condition, given either waitUntil or expectEventually, given a condition that eventually evaluates to true, a normal expect fails when waits are left")
                .thatErrors();

            testResult.hasTest("NJasmineTests.Specs.waitUntil_waits_for_a_condition, given either waitUntil or expectEventually, given a condition that eventually evaluates to true, waitUntil will try multiple times")
                .thatSucceeds();

            testResult.hasTest("NJasmineTests.Specs.waitUntil_waits_for_a_condition, given either waitUntil or expectEventually, given a condition that eventually evaluates to true, expectEventually will try multiple times")
                .thatSucceeds();

            testResult.hasTest("NJasmineTests.Specs.waitUntil_waits_for_a_condition, given either waitUntil or expectEventually, waitUntil can be called during discovery, doesnt prevent discovery")
                .thatErrors();

            testResult.hasTest("NJasmineTests.Specs.waitUntil_waits_for_a_condition, given either waitUntil or expectEventually, expectEventually can be called during discovery, doesnt prevent discovery")
                .thatErrors();
        }
    }
}
