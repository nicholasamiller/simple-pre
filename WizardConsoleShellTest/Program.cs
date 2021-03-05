using System;
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

        static void Main(string[] args)
        {
            var context = new Context();

            // rules
            var re = new ProductionRuleEngine<Context>(new[] { new SunShiningRule() });
            
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
            
            

            var edge = new ScreenLink<Context>() {Source = screen1, Target = screen2,  TraverseCondition = c => !c.IsPersonHappy.HasValue };

            var wm = new WizardManager<Context>(new[] { screen1, screen2 }, new[] { edge }, c=> re.Run(c));

            var cc = new ConsoleWizardClient<Context>(wm, context);
            cc.Start();
            var outcome = context.IsPersonHappy;
            Console.WriteLine("Outcome: " + outcome);
            Console.ReadKey();

        }


    }
}
