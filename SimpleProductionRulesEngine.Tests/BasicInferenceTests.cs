using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleProductionRulesEngine.Tests
{
    [TestClass]
    public class BasicInferenceTests
    {
        class TestContext
        {
            public bool? IsHappy { get; set; }
            public bool? IsSunShining { get; set; }
            public bool? IsHot { get; set; }

            public bool? ItIsClearWeather { get; set; }
            public bool? ItIsDay { get; set; }

            public bool? IsDawn { get; set; }
            public bool? IsBirdsSinging { get; set; }

        }

        class TestRule : IProductionRule<TestContext>
        {
            public string Description => "The person is happy if the sun is shining";

            public Predicate<TestContext> Condition => t => t.IsSunShining == true;

            public Action<TestContext> Action => t => t.IsHappy = true;

            public int Salience => 1;
        }

        class RuleThatAlwaysFires : IProductionRule<TestContext>
        {
            public string Description => "It is hot if the sun is shining, otherwise it is cold";

            public Predicate<TestContext> Condition => t => true;

            public Action<TestContext> Action => t =>
            {
                if (t.IsSunShining == true)
                    t.IsHot = true;
                else
                    t.IsHot = false;
            };

            public int Salience => 2;
        }

        class ConflictingRule1 : IProductionRule<TestContext>
        {
            public string Description => "The person is happy if the sun is NOT shining";

            public Predicate<TestContext> Condition => t => t.IsSunShining == false;

            public Action<TestContext> Action => t => t.IsHappy = true;

            public int Salience => 2;
        }

        class SunShines : IProductionRule<TestContext>
        {
            public string Description => "The sun only shines when it is day and the weather is clear.";

            public Predicate<TestContext> Condition => t => t.ItIsClearWeather.HasValue && t.ItIsDay.HasValue;

            public Action<TestContext> Action => t =>
            {
                t.IsSunShining = t.IsSunShining.Value == true && t.ItIsClearWeather.Value == true;
            };

            public int Salience => 1;
        }

        [TestMethod]
        public void NoInferenceOnNullValues()
        { 
            var underTest = new ProductionRuleEngine<TestContext>(new List<IProductionRule<TestContext>>() { new TestRule() });
            var blankContext = new TestContext();
            underTest.Run(blankContext);
            Assert.IsTrue(blankContext.IsHappy.HasValue == false);
        }
        
        [TestMethod]
        public void Inference()
        {
            var underTest = new ProductionRuleEngine<TestContext>(new List<IProductionRule<TestContext>>() { new TestRule() });
            var blankContext = new TestContext() { IsSunShining = true };
            underTest.Run(blankContext);
            Assert.IsTrue(blankContext.IsHappy == true);
        }

        [TestMethod]
        public void RuleThatAlwaysFiresTest()
        {
            var underTest = new ProductionRuleEngine<TestContext>(new List<IProductionRule<TestContext>>() {  new RuleThatAlwaysFires() });
            var blankContext = new TestContext();
            underTest.Run(blankContext);
            Assert.IsTrue(blankContext.IsHot == false);
        }

        [TestMethod]
        public void HigherSalienceRulesOverride()
        {
            var underTest = new ProductionRuleEngine<TestContext>(new List<IProductionRule<TestContext>>() {  new TestRule(), new ConflictingRule1() });
            var blankContext = new TestContext() { IsSunShining = false };
            underTest.Run(blankContext);
            Assert.IsTrue(blankContext.IsHappy == true);
        }

        class WhenHappyRule : IProductionRule<TestContext>
        {
            public string Description => "The person is happy if the sun is shining or the birds are singing.";

            public Predicate<TestContext> Condition => t => t.IsSunShining == true || t.IsBirdsSinging == true;

            public Action<TestContext> Action => t => t.IsHappy = true;

            public int Salience => 0;
        }

        class WhenBirdsSingingRule : IProductionRule<TestContext>
        {
            public string Description => "Birds sing when it's morning";

            public Predicate<TestContext> Condition => t => t.IsDawn == true;

            public Action<TestContext> Action => t => t.IsBirdsSinging = true;

            public int Salience => 0;
        }




        [TestMethod]
        public void ReEvaluateRule()
        {
            var underTest = new ProductionRuleEngine<TestContext>(new List<IProductionRule<TestContext>>() { new WhenHappyRule(), new WhenBirdsSingingRule() });
            var context = new TestContext() { IsSunShining = false, IsDawn = true };
            underTest.Run(context);
            Assert.IsTrue(context.IsHappy == true);
        }

        [TestMethod]
        public void TestTracing()
        {
            var underTest = new ProductionRuleEngine<TestContext>(new List<IProductionRule<TestContext>>() { new TestRule() });
            var blankContext = new TestContext() { IsSunShining = true };
            underTest.Run(blankContext);
            var log = underTest.Log;
            Assert.IsTrue(log.Count() == 1);
            Assert.IsTrue(log.First().Before.IsHappy.HasValue == false);
            Assert.IsTrue(log.First().After.IsHappy.HasValue == true);

        }

    }
}
