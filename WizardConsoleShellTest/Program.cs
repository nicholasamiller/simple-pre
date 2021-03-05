using System;
using System.Collections.Generic;
using SimpleProductionRulesEngine;
using SimpleWizard;

namespace WizardConsoleShellTest
{
    class Program
    {
        public class Context
        {
            public bool? IsSunShining { get; set; }
            public bool? PersonLikesSun { get; set; }
            public bool? SunGoneNova { get; set; }
            public bool? IsPersonHappy { get; set; }
        }


        public class SunShiningRule : IProductionRule<Context>
        {
            public string Description => "The person is happy if the sun is shining and they like sun.";

            public Predicate<Context> Condition => c => c.IsSunShining.HasValue || c.IsPersonHappy.HasValue;

            public Action<Context> Action => c =>
            {
                if (c.IsSunShining == false || c.PersonLikesSun == false)
                {
                    c.IsPersonHappy = false;
                }
                else if (c.IsSunShining == true && c.PersonLikesSun == true)
                {
                    c.IsPersonHappy = true;
                }
            };

            public int Salience => 0;
        }

        public class SuperNovaRule : IProductionRule<Context>
        {
            public string Description => "The person is not happy if the sun has gone nova.";

            public Predicate<Context> Condition => c => c.SunGoneNova == true;

            public Action<Context> Action => c => c.IsPersonHappy = false;

            public int Salience => 1;
        }

        static void Main(string[] args)
        {
            var context = new Context();

            // rules
            var re = new ProductionRuleEngine<Context>(new List<IProductionRule<Context>>() { new SunShiningRule(), new SuperNovaRule() });
            
            var screen1 = new SimpleWizard.QuestionScreen<Context>()
            {
                QuestionText = "Shining?",
                QuestionType = typeof(bool),
                ReflectAnswer = (a, c) => c.IsSunShining = (bool)a
            };

            var screen2 = new SimpleWizard.QuestionScreen<Context>()
            {
                QuestionText = "Likes sun?",
                QuestionType = typeof(bool),
                ReflectAnswer = (a, c) => c.PersonLikesSun = (bool)a
            };

            var screen3 = new QuestionScreen<Context>()
            {
                QuestionText = "Sun gone nova?",
                QuestionType = typeof(bool),
                ReflectAnswer = (a, c) => c.SunGoneNova = (bool)a
            };

            var edge = new ScreenLink<Context>() {Source = screen1, Target = screen2,  TraverseCondition = c => !c.IsPersonHappy.HasValue };
            var edge2 = new ScreenLink<Context>() { Source = screen2, Target = screen3, TraverseCondition = c => c.IsPersonHappy == true };

            var wm = new WizardManager<Context>(new[] { screen1, screen2, screen3 }, new[] { edge, edge2 }, c=> re.Run(c));

            var cc = new ConsoleWizardClient<Context>(wm, context);
            cc.Start();
            var outcome = context.IsPersonHappy;
            Console.WriteLine("Outcome: " + outcome);
            Console.ReadKey();
        }


    }
}
