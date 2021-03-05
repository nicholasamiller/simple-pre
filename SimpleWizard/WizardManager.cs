using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleWizard
{
    public class WizardManager<TContext>
    {

        public Node<TContext> StartingNode { get; private set; }
        private Node<TContext> _currentNode;
        private readonly Action<TContext> onContextUpdated;

        public WizardManager(IEnumerable<QuestionScreen<TContext>> questionScreens, IEnumerable<ScreenLink<TContext>> screenLinks,Action<TContext> onContextUpdated)
        {
            var rootNode = BuildTree(questionScreens, screenLinks);
            StartingNode = rootNode;
            _currentNode = StartingNode;
            this.onContextUpdated = onContextUpdated;
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
            onContextUpdated(context); 
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

  



    


}
