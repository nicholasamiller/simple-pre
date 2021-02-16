using System.Collections.Generic;
using SimpleProductionRulesEngine;

namespace ProductionRulesEngine
{
    public interface IProductionRuleEngine<T>
    {
        T Run(T context);

        IEnumerable<RuleRunLog<T>> Log { get; }
    }
}
