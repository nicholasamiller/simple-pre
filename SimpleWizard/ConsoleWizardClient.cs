using System;

namespace SimpleWizard
{
    public class ConsoleWizardClient<TContext>
    {
        private readonly TContext context;

        public ConsoleWizardClient(WizardManager<TContext> wizardManager, TContext context)
        {
            WizardManager = wizardManager;
            this.context = context;
        }
        
        public WizardManager<TContext> WizardManager { get; }

        private bool IsFinished()
        {
            return false;
        }

        public void Start()
        {

            var nextScreen = WizardManager.GetFirstScreen();
            while (nextScreen != null && IsFinished() == false)
            {
                Console.WriteLine(nextScreen.QuestionText);
                var answer = Console.ReadLine();
                while (ConvertStringAnswerToType(answer, nextScreen.QuestionType) == null)
                {
                    Console.WriteLine("Try again.");
                    answer = Console.ReadLine();
                }
                nextScreen = WizardManager.GetNextQuestion(ConvertStringAnswerToType(answer, nextScreen.QuestionType), context);
            }
        }

        private object ConvertStringAnswerToType(string answer, Type targetType)
        {
            try
            {
                if (targetType == typeof(bool))
                {
                    return Convert.ToBoolean(answer);
                }
                return answer;
            }
            catch
            {
                return null;
            }
        }

        
    }

  



    


}
