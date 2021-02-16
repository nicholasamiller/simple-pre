using System;

namespace SimpleProductionRulesEngine
{
    public interface IProductionRule<T>
    {
        string Description { get; }
        Predicate<T> Condition { get; }
        Action<T> Action { get; }
        int Salience { get; }
        
    }

   
}