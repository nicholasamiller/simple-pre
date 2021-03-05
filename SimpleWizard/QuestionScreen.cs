using System;

namespace SimpleWizard
{
    public class QuestionScreen<TContext>
    {
        public string QuestionText { get; set; }
        
        public Type QuestionType { get; set; }
        public Action<object,TContext> ReflectAnswer { get; set; }
    }

  



    


}
