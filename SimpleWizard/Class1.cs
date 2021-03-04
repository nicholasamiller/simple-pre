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
                var answerTyped = ConvertStringAnswerToType(answer, nextScreen.QuestionType);
                nextScreen = WizardManager.GetNextQuestion(answerTyped,context);
            }
        }

        private object ConvertStringAnswerToType(string answer, Type targetType)
        {
            if (targetType == typeof(bool))
            {
                return Convert.ToBoolean(answer);
            }
            return answer;
        }

        
    }

    public class ScreenLink<TContext>
    {
        public QuestionScreen<TContext> Source { get; set; }
        public QuestionScreen<TContext> Target {get;set;}
        public Predicate<TContext> TraverseCondition { get; set; }
    }

    public class WizardManager<TContext>
    {

        public Node<TContext> StartingNode { get; private set; }
        private Node<TContext> _currentNode;

        public WizardManager(IEnumerable<QuestionScreen<TContext>> questionScreens, IEnumerable<ScreenLink<TContext>> screenLinks)
        {
            var rootNode = BuildTree(questionScreens, screenLinks);
            StartingNode = rootNode;
            _currentNode = StartingNode;
        }

        public QuestionScreen<TContext> GetFirstScreen()
        {
            return StartingNode.QuestionScreen;
        }

        private static Node<TContext> BuildTree(IEnumerable<QuestionScreen<TContext>> questionScreens, IEnumerable<ScreenLink<TContext>> screenLinks)
        {
            var nodes = questionScreens.Select(qs => new Node<TContext> { QuestionScreen = qs }).ToList();
            foreach (var l in screenLinks)
            {
                var source = nodes.FirstOrDefault(n => n.QuestionScreen == l.Source);
                var target = nodes.FirstOrDefault(n => n.QuestionScreen == l.Target);
                var edge = new Edge<TContext>() { Source = source, Target = target, TraverseCondition = l.TraverseCondition };
                source.AddOutEdge(edge);
                target.AddInEdge(edge);
            }

            var rootNodes = nodes.Where(n => !n.InEdges.Any());
            if (rootNodes.Count() != 1)
            {
                throw new WizardException("Must be one root node exactly.");
            }

            return rootNodes.Single();
        }

        public QuestionScreen<TContext> GetNextQuestion(object answerToLastQuestion, TContext context)
        {
            _currentNode.QuestionScreen.ReflectAnswer(answerToLastQuestion, context);
            var traversibleOutEdges = _currentNode.OutEdges.Where(e => e.TraverseCondition(context));
            if (traversibleOutEdges.Count() > 1)
            {
                throw new WizardException("Mulitple traversible out edges for " + _currentNode);
            }
            if (!traversibleOutEdges.Any())
            {
                return null;
            }
            else
            {
                _currentNode = traversibleOutEdges.Single().Target;
                return _currentNode.QuestionScreen;
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
        public IEnumerable<Edge<TContext>> OutEdges => _OutEdges;
        public IEnumerable<Edge<TContext>> InEdges => _InEdges;
    }

    public class QuestionScreen<TContext>
    {
        public string QuestionText { get; set; }
        
        public Type QuestionType { get; set; }
        public Action<object,TContext> ReflectAnswer { get; set; }
    }

    public class Edge<TContext>
    {
        public Node<TContext> Source { get; set; }
        public Node<TContext> Target { get; set; }
        public Predicate<TContext> TraverseCondition { get; set; }
    }

  



    


}
