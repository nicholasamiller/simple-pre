using System;

namespace SimpleWizard
{
    public class ScreenLink<TContext>
    {
        public QuestionScreen<TContext> Source { get; set; }
        public QuestionScreen<TContext> Target {get;set;}
        public Predicate<TContext> TraverseCondition { get; set; }
    }

  



    


}
