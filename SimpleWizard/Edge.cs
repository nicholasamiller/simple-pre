using System;

namespace SimpleWizard
{
    public class Edge<TContext>
    {
        public Node<TContext> Source { get; set; }
        public Node<TContext> Target { get; set; }
        public Predicate<TContext> TraverseCondition { get; set; }
    }

  



    


}
