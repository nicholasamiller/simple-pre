﻿using System;
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


        static void Main(string[] args)
        {
            var context = new Context();
            var screen1 = new SimpleWizard.QuestionScreen<Context>()
            {
                QuestionText = "Shining?",
                QuestionType = typeof(bool),
                ReflectAnswer = (a, c) => c.IsSunShining = a
            };

            var screen2 = new SimpleWizard.QuestionScreen<Context>()
            {
                QuestionText = "Likes sun?",
                QuestionType = typeof(bool),
                ReflectAnswer = (a, c) => c.PersonLikesSun = a
            };

            var edge = new SimpleWizard.Edge<Context>() { Source = screen1, Target = screen2, TraverseCondition = c => true };

            var tree = SimpleWizard.WizardManager<Context>.BuildTree(new[] { screen1, screen2 }, new[] { edge });
            var wm = new WizardManager<Context>(tree,context);

            var cc = new ConsoleWizardClient<Context>(wm);
            cc.Start();

        }


    }
}