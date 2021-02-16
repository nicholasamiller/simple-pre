using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleProductionRulesEngine
{
    public class RuleRunLog<TWorkingMemory>
    {
        public RuleRunLog(TWorkingMemory before, TWorkingMemory after, IProductionRule<TWorkingMemory> rule)
        {
            Before = before;
            After = after;
            Rule = rule;
        }

        public TWorkingMemory Before { get; }
        public TWorkingMemory After { get; }
        public IProductionRule<TWorkingMemory> Rule { get;}
    }
}
