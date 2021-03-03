using System.Collections.Generic;
using SimpleProductionRulesEngine;

namespace ProductionRulesEngine
{
    public interface IProductionRuleEngine<T>
    {
        T Run(T context);

        IEnumerable<IProductionRule<T>> UnActivatedRules { get; }
        IEnumerable<RuleRunLog<T>> Log { get; }
    }
}
