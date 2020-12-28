using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode2020
{
    public static class Q7
    {
        public static void Solve()
        {
            var inputs = Tools.GetInput(7);
            var bagRules = new Dictionary<string, ColouredBag>();
            foreach (var ruleDefinition in inputs)
            {
                var bag = new ColouredBag(ruleDefinition);
                bagRules[bag.Colour] = bag;
            }

            // Part 1.
            var coloursThatMayContainShinyGold = new HashSet<string>();
            ContainsRequiredColour(bagRules, coloursThatMayContainShinyGold, "shinygold");
            var permittedColours = string.Concat(coloursThatMayContainShinyGold.Select(s => $"{s}, "));
            Console.WriteLine($"May contain ShinyGold = {coloursThatMayContainShinyGold.Count}. {permittedColours}");
            
            // Part 2.
            var bagsRequiredForShinyGold = RequiredBags(bagRules, new Dictionary<string, int>(), "shinygold");
            Console.WriteLine($"Bags required for ShinyGold = {bagsRequiredForShinyGold}. (Including ShinyGold one too).");

        }

        private static bool ContainsRequiredColour(
            IReadOnlyDictionary<string, ColouredBag> bagRules,
            HashSet<string> containsColour,
            string requiredColour)
        {
            foreach (var bagRule in bagRules)
            {

                var mayContainRequiredColour = ContainsRequiredColour(
                    bagRules, containsColour, bagRule.Value, requiredColour);
                if (mayContainRequiredColour) containsColour.Add(bagRule.Key);
            }

            return containsColour.Contains(requiredColour);
        }

        private static bool ContainsRequiredColour(
            IReadOnlyDictionary<string, ColouredBag> bagRules,
            HashSet<string> containsColour,
            ColouredBag bagRule,
            string requiredColour)
        {
            if (containsColour.Contains(requiredColour))
            {
                return true;
            }

            var mayContainRequiredColour = false;
            if (bagRule.RequiredInnerBags.Count == 0)
            {
                mayContainRequiredColour = false;
            }
            else if (bagRule.RequiredInnerBags.ContainsKey(requiredColour))
            {
                mayContainRequiredColour = true;
            } 
            else
            {
                foreach (var innerBagRule in bagRule.RequiredInnerBags)
                {
                    var innerBagPermits = ContainsRequiredColour(
                        bagRules, containsColour, bagRules[innerBagRule.Key], requiredColour);
                    mayContainRequiredColour |= innerBagPermits;
                    if (innerBagPermits) break;
                }
            }

            return mayContainRequiredColour;
        }

        private static int RequiredBags(
            Dictionary<string, ColouredBag> bagRules,
            Dictionary<string, int> requiredBagsForColour, // memoise answers
            string colour)
        {
            if (requiredBagsForColour.ContainsKey(colour)) return requiredBagsForColour[colour];
            
            var requiredBagCount = 1;
            var ruleForThisColour = bagRules[colour];
            
            if (ruleForThisColour.RequiredInnerBags.Count == 0) return requiredBagCount;

            foreach(var rule in ruleForThisColour.RequiredInnerBags)
            {
                var requiredBagsForInner = RequiredBags(bagRules, requiredBagsForColour, rule.Key);
                requiredBagsForColour[rule.Key] = requiredBagsForInner;
                requiredBagCount += rule.Value * requiredBagsForInner;
            }
            
            return requiredBagCount;
        }

        public class ColouredBag
        {
            public string Colour;
            public Dictionary<string, int> RequiredInnerBags;

            public ColouredBag(string rule)
            {
                RequiredInnerBags = new Dictionary<string, int>();
                ParseRule(rule);
            }

            private void ParseRule(string rule)
            {
                var tokens = rule.Split(" ");
                if (!rule.Contains("contain")) throw new Exception($"Unexpected rule: {rule}");
                Colour = tokens[0] + tokens[1];

                for (var i = 4; i < tokens.Length; i += 4)
                {
                    if (tokens[i] == "no") break;
                    
                    var requiredCount = int.Parse(tokens[i]);
                    var bagColour = tokens[i + 1] + tokens[i + 2];
                    RequiredInnerBags[bagColour] = requiredCount;
                }
            }
        }
    }
}