using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleWizard
{
    public static class Extensions
    {

      

        // check the unfired rules
        // check the properties referenced in their condition
        // check if the properties are unknown in the context
        // if unknown, then put a question for those properties        
        // simple version:
        // check the unknown properties on the context
        // put a priority on the properties in the context
        // ask them in order of priority
        // each time we ask, pump them into the engine
        // then check the unknown ones again, ask in order of priority
        // one question at a time
        // have a tree of screen questions

    }


    [Serializable]
    public class WizardException : Exception
    {
        public WizardException() { }
        public WizardException(string message) : base(message) { }
        public WizardException(string message, Exception inner) : base(message, inner) { }
        protected WizardException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }

    public class ConsoleWizardClient<TContext>
    {

        public ConsoleWizardClient(WizardManager<TContext> wizardManager)
        {
            WizardManager = wizardManager;
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
                var answerTyped = ConvertStringAnswerToType(answer, nextScreen.QuestionType);
                nextScreen =  nextScreen.ReflectAnswer(answerTyped, WizardManager.Context);
            }
        }

        private dynamic ConvertStringAnswerToType(string answer, Type targetType)
        {
            if (targetType == typeof(bool))
            {
                return Convert.ToBoolean(answer);
            }
            return answer;
        }

        
    }

    public class WizardManager<TContext>
    {

        // build the tree
        // run the rules engine
        // set finish conditions


        public TContext Context { get; private set; }
        
        public Node<TContext> StartingNode { get; private set; }
        public Node<TContext> CurrentNode { get; private set; }

        public WizardManager(Node<TContext> rootNode, TContext context)
        {
            CurrentNode = rootNode;

            Context = context;
        }

        public QuestionScreen<TContext> GetFirstScreen()
        {
            return StartingNode.QuestionScreen;
        }

        public static Node<TContext> BuildTree(IEnumerable<QuestionScreen<TContext>> questionScreens, IEnumerable<Edge<TContext>> edges)
        {
            var nodes = questionScreens.Select(qs => new Node<TContext> { QuestionScreen = qs }).ToList();
            foreach (var e in edges)
            {
                var source = nodes.FirstOrDefault(n => n.QuestionScreen == e.Source);
                var target = nodes.FirstOrDefault(n => n.QuestionScreen == e.Target);
            }

            var rootNodes = nodes.Where(n => !n.InEdges.Any());
            if (rootNodes.Count() != 1)
            {
                throw new WizardException("Must be one root node exactly.");
            }

            return rootNodes.Single();
        }

        public QuestionScreen<TContext> GetNextQuestion(dynamic answerToLastQuestion)
        {
            CurrentNode.QuestionScreen.ReflectAnswer(answerToLastQuestion, Context);
            var traversibleOutEdges = CurrentNode.OutEdges.Where(e => e.TraverseCondition(Context));
            if (traversibleOutEdges.Count() > 1)
            {
                throw new WizardException("Mulitple traversible out edges for " + CurrentNode);
            }
            if (!traversibleOutEdges.Any())
            {
                return null;
            }
            else
            {
                return traversibleOutEdges.Single().Target;
            }
        }

      


    }

    public class Node<TContext>
    {
        private List<Edge<TContext>> _InEdges = new List<Edge<TContext>>();
        private List<Edge<TContext>> _OutEdges = new List<Edge<TContext>>();
        
        public void AddOutEdge(Edge<TContext> edge)
        {
            _OutEdges.Add(edge);
        }

        public void AddInEdge(Edge<TContext> edge)
        {
            _InEdges.Add(edge);
        }
        
        public QuestionScreen<TContext> QuestionScreen { get; set; }
        public IEnumerable<Edge<TContext>> OutEdges { get; set; }
        public IEnumerable<Edge<TContext>> InEdges { get; set; }
    }

    public class QuestionScreen<TContext>
    {
        public string QuestionText { get; set; }
        
        public Type QuestionType { get; set; }
        public Action<dynamic,TContext> ReflectAnswer { get; set; }
    }

    public class Edge<TContext>
    {
        public QuestionScreen<TContext> Source { get; set; }
        public QuestionScreen<TContext> Target { get; set; }
        public Predicate<TContext> TraverseCondition { get; set; }
    }

  



    


}
