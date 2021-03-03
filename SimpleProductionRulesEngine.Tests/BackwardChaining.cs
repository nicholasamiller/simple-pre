using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleProductionRulesEngine.Tests
{
    [TestClass]
    public class BackwardChaining
    {
        class TestContext
        {
            public bool? IsPersonHappy { get; }
            public bool? IsSunShining { get; }
        }

        class TestRule : IProductionRule<TestContext>
        {
            public string Description => throw new NotImplementedException();

            public Predicate<TestContext> Condition => t => t.IsPersonHappy == true;

            public Action<TestContext> Action => throw new NotImplementedException();

            public int Salience => throw new NotImplementedException();
        }

        [TestMethod]
        public void TryExtractProperties()
        {
            var r = new TestRule();
            var condition = r.Condition;
            
        }
    }
}
