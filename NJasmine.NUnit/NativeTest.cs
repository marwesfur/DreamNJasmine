using System;
using System.Collections.Generic;
using System.Linq;
using NJasmine.Core;
using NJasmine.Core.NativeWrappers;
using NJasmine.NUnit.TestElements;
using NUnit.Core;

namespace NJasmine.NUnit
{
    public class NativeTest : INativeTest
    {
        readonly Test _test;

        public NJasmine.Core.TestName Name { get; private set; }

        public NativeTest(Test test, NJasmine.Core.TestName name)
        {
            _test = test;
            Name = name;
        }

        public void AddCategory(string category)
        {
            _test.Categories.Add(category);
        }

        public void AddChild(INativeTest test)
        {
            (_test as global::NUnit.Core.TestSuite).Add((Test)test.GetNative());
        }

        public void MarkTestIgnored(string reasonIgnored)
        {
            TryApplyRunState(RunState.Explicit, reasonIgnored);
        }

        public void MarkTestInvalid(string reason)
        {
            TryApplyRunState(RunState.NotRunnable, reason);
        }

        public void MarkTestFailed(Exception exception)
        {
            (_test as IPrefailable).SetPendingException(exception);
        }

        public object GetNative()
        {
            return _test;
        }

        public void TryApplyRunState(RunState state, string reason)
        {
            if (_runStatePriorities[state] < _runStatePriorities[_test.RunState])
            {
                _test.RunState = state;
                _test.IgnoreReason = reason;
            }
        }

        private Dictionary<RunState, int> _runStatePriorities = new Dictionary<RunState, int>
        {
            // kind of arbitrary, but I know:
            //  NotRunnable should override Ignored or Explicit.
            //  Everything should override runnable
            {RunState.NotRunnable, 0},
            {RunState.Skipped, 1},
            {RunState.Ignored, 2},
            {RunState.Explicit, 3},
            {RunState.Runnable, 4},
        };
    }
}
