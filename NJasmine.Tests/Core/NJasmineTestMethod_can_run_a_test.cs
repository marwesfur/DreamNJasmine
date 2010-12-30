﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NJasmine;
using NJasmine.Core;
using NUnit.Core;
using NUnit.Framework;

namespace NJasmineTests.Core
{
    [TestFixture]
    public class NJasmineTestMethod_can_run_a_test : ExpectationsFixture
    {
        private class AFixture : ObservableNJasmineFixture
        {
            public override void Tests()
            {
                Observe("1");

                beforeEach(delegate()
                {
                    Observe("2");
                });

                describe("first describe", delegate()
                {
                    Observe("3");

                    afterEach(delegate()
                    {
                        Observe("8");
                    });

                    describe("skipped describe", delegate()
                    {
                        Observe("skipped describe");
                    });

                    it("skipped it", delegate()
                    {
                        Observe("skipped it");
                    });

                    describe("second describe", delegate()
                    {
                        Observe("4");

                        afterEach(delegate()
                        {
                            Observe("7");
                        });

                        beforeEach(delegate()
                        {
                            Observe("5");
                        });

                        it("the test", delegate()
                        {
                            Observe("6");
                        });

                        Observe("-1");
                    });

                    beforeEach(delegate()
                    {
                        Observe("-1");
                    });

                    Observe("-1");
                });

                afterEach(delegate()
                {
                    Observe("-1");
                });

                Observe("-1");
            }
        }

        [Test]
        public void can_be_ran()
        {
            AFixture fixture = new AFixture();

            var sut = new NJasmineTestMethod(fixture, new TestPosition(1, 3, 2), new NUnitFixtureCollection());

            sut.RunTestMethod(new TestResult(new TestName()));

            expect(fixture.Observations.ToArray()).to.Equal(
                Enumerable.Range(1, 8).Select(i => i.ToString()).ToArray());
        }

        [Test]
        public void duplicated_runs_dont_accidentally_accumulate_afterEach_calls()
        {
            AFixture fixture = new AFixture();

            var sut = new NJasmineTestMethod(fixture, new TestPosition(1, 3, 2), new NUnitFixtureCollection());

            sut.RunTestMethod(new TestResult(new TestName()));

            fixture.ResetObservations();

            sut.RunTestMethod(new TestResult(new TestName()));

            expect(fixture.Observations.ToArray()).to.Equal(
                Enumerable.Range(1, 8).Select(i => i.ToString()).ToArray());
        }

    }
}
