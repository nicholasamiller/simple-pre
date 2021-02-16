using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using ProductionRulesEngine;

namespace SimpleProductionRulesEngine
{
    public class ProductionRuleEngine<T> : IProductionRuleEngine<T>
    {

        readonly List<IProductionRule<T>> _unactivatedRules = new List<IProductionRule<T>>();
        List<IProductionRule<T>> _agenda = new List<IProductionRule<T>>();
        private IList<IProductionRule<T>> _ruleBase;
        private IList<RuleRunLog<T>> _log = new List<RuleRunLog<T>>();

        public IEnumerable<RuleRunLog<T>> Log => _log;

        public ProductionRuleEngine(IEnumerable<IProductionRule<T>> rules)
        {
            _ruleBase = rules.ToList();
            _unactivatedRules.AddRange(_ruleBase);
        }


        public T Run(T wm)
        {
            ActivateRules(wm);

            while (_agenda.Count > 0)
            {
                FireRulesOnAgenda(wm);
                ActivateRules(wm);
            }
            return wm;
        }

        private T FireRulesOnAgenda(T wm)
        {
            var orderedBySalienceAscending = _agenda.OrderBy(i => i.Salience).ToList();
            while (orderedBySalienceAscending.Any())
            {
                Fire(orderedBySalienceAscending.First(),wm);
                orderedBySalienceAscending = _agenda.OrderBy(i => i.Salience).ToList();
            }
            return wm;
        }

        private void Fire(IProductionRule<T> rule, T wm)
        {
            var before = CloneJson(wm);
            rule.Action(wm);
            var after = CloneJson(wm);
            _log.Add(new RuleRunLog<T>(before, after, rule));
            _agenda.Remove(rule);
        }

        private void ActivateRules(T wm)
        {
            foreach (var h in _unactivatedRules)
            {
                if (h.Condition(wm))
                    _agenda.Add(h);
            }
            foreach (var h in _agenda)
            {
                _unactivatedRules.Remove(h);
            }
        }

        private static T CloneJson(T source)
        {
            // Don't serialize a null object, simply return the default for that object
            if (Object.ReferenceEquals(source, null))
            {
                return default(T);
            }
            var deserializeSettings = new JsonSerializerSettings { ObjectCreationHandling = ObjectCreationHandling.Replace };
            return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(source), deserializeSettings);
        }

    }
}