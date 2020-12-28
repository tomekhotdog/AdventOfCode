using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode2020
{
    public static class Q19
    {
        public static void Solve()
        {
            var inputs = Tools.GetInput(19);
            var parsed = ParseInput(inputs);
            var exactMatches = CountMatches(parsed.ruleRegistry, parsed.messages);
            
            Console.WriteLine($"Total matches: {exactMatches}");
        }

        public static int CountMatches(Dictionary<int, IRule> registry, List<string> messages)
        {
            var matches = 0;
            var rootRule = registry[0];
            foreach (var message in messages)
            {
                var match = rootRule.Matches(registry, new List<int>(), message, out var remaining);
                var exactMatch = remaining.Any(m => m == string.Empty);
                if (exactMatch) matches++;
                Console.WriteLine($"{message}: {(exactMatch ? "Exact match" : "No exact match")}");
            }
            return matches;
        }

        public static (Dictionary<int, IRule> ruleRegistry, List<string> messages) ParseInput(string[] inputs)
        {
            var ruleRegistry = new Dictionary<int, IRule>();
            var messages = new List<string>();

            var parsedRules = false;
            foreach (var input in inputs)
            {
                if (input == string.Empty)
                {
                    if (parsedRules) throw new Exception("Not expecting to see two empty lines?");
                    parsedRules = true;
                }
                else if (!parsedRules)
                {
                    var ruleElems = input.Split(":");
                    var ruleNumber = int.Parse(ruleElems[0]);
                    var ruleBody = ruleElems[1].Trim();
                    if (ruleBody.Contains("\""))
                    {
                        ruleRegistry[ruleNumber] = new LeafRule(ruleBody);
                    }
                    else
                    {
                        ruleRegistry[ruleNumber] = new BranchRule(ruleBody);
                    }
                }
                else
                {
                    messages.Add(input);
                }
            }

            return (ruleRegistry, messages);
        }

        public interface IRule
        {
            bool Matches(Dictionary<int, IRule> ruleRegistry, IList<int> rulesMatchedSoFar, string message, out List<string> remaining);
        }

        public struct LeafRule : IRule
        {
            private readonly string _rawString;

            public LeafRule(string raw)
            {
                if (!raw.Contains("\"")) throw new Exception($"Invalid Leaf: {raw}");
                _rawString = raw.Substring(1, raw.Length - 2);
            }

            public bool Matches(Dictionary<int, IRule> ruleRegistry, IList<int> rulesMatchedSoFar, string message, out List<string> remaining)
            {
                if (message.StartsWith(_rawString))
                {
                    remaining = new List<string> {string.Concat(message.Skip(_rawString.Length))};
                    return true;
                }

                remaining = new List<string>();
                return false;
            }
        }

        public struct BranchRule : IRule
        {
            // 'or' list of 'and' rule lists.
            private readonly IList<IList<int>> _rules;

            public BranchRule(string rule)
            {
                _rules = new List<IList<int>>();
                var orRules = rule.Split('|');
                foreach (var orRule in orRules)
                {
                    var andRulesList = new List<int>();
                    var andRules = orRule.Trim().Split(" ");
                    foreach (var andRule in andRules)
                    {
                        var ruleNumber = int.Parse(andRule);
                        andRulesList.Add(ruleNumber);
                    }

                    _rules.Add(andRulesList);
                }
            }

            public bool Matches(Dictionary<int, IRule> ruleRegistry, IList<int> rulesMatchesSoFar, string message, out List<string> remaining)
            {
                var visualisation = string.Join("->", rulesMatchesSoFar);
                remaining = new List<string>();
                foreach (var andRules in _rules)
                {
                    var rulesMatched = new List<int>(rulesMatchesSoFar);
                    var candidates = new List<string> {message};
                    foreach (var andRule in andRules)
                    {
                        var matchesForThisAndRule = false;
                        var resolvedRule = ruleRegistry[andRule];
                        var candidatesForThisRule = new List<string>(candidates);
                        rulesMatched.Add(andRule);
                        candidates.Clear();
                        foreach (var candidate in candidatesForThisRule)
                        {
                            // Hugely unsatisfactory way of identifying that we are in a fruitless loop.
                            if (candidate == message && rulesMatched.Count(r => r == andRule) > 10)
                            {
                                continue;
                            }
                            
                            if (resolvedRule.Matches(ruleRegistry, rulesMatched, candidate, out var remainingFromThisMatch))
                            {
                                matchesForThisAndRule = true;
                                candidates.AddRange(remainingFromThisMatch);
                            }
                        }

                        if (!matchesForThisAndRule) break;
                    }
                    remaining.AddRange(candidates);
                }
                return remaining.Any();
            }
        }
    }
}