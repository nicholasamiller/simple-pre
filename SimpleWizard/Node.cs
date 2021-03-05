using System.Collections.Generic;

namespace SimpleWizard
{
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

  



    


}
