using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode2020
{
    public static class Q16
    {
        public static void Solve()
        {
            var inputs = Tools.GetInput(16);
            var document = ParseInput(inputs);

            // Part 1.
            var errorRate = CalculateTicketScanningErrorRate(document);
            Console.WriteLine($"TicketScanningErrorRate: {errorRate}");

            // Part 2.
            var product = CalculateProductOfDepartureRelatedNumbers(document);
            Console.WriteLine($"Product of 'departure' field values: {product}");
        }

        private static int CalculateTicketScanningErrorRate(Document document)
        {
            var invalidNumbers = new List<int>();
            var invalidTickets = new List<Ticket>();
            foreach (var ticket in document.NearbyTickets)
            {
                foreach (var number in ticket.Numbers)
                {
                    var satisfiesAnyRule = document.Rules.Any(r => r.NumberSatisfiesRule(number));
                    if (!satisfiesAnyRule)
                    {
                        invalidNumbers.Add(number);
                        if (!invalidTickets.Contains(ticket)) invalidTickets.Add(ticket);
                    }
                }
            }
            return invalidNumbers.Sum();
        }

        private static List<Ticket> FindInvalidTickets(Document document)
        {
            return document.NearbyTickets.Where(
                t => t.Numbers.Any(
                    n => !document.Rules.Any(
                        r => r.NumberSatisfiesRule(n))))
                .ToList();
        }

        private static long CalculateProductOfDepartureRelatedNumbers(Document document)
        {
            document = CleanDocument(document);
            var allTickets = document.NearbyTickets.Concat(new []{document.MyTicket}).ToArray();
            var fieldIndexToPossibleRules = new Dictionary<int, List<Rule>>();
            var fieldIndexToRule = new Dictionary<int, Rule>();
            
            // Complete first pass through the fields.
            for (var fieldIndex = 0; fieldIndex < document.MyTicket.Numbers.Length; fieldIndex++)
            {
                var numbersForNthField = allTickets.Select(t => t.Numbers[fieldIndex]).ToArray();
                var satisfiedRules = document.Rules.Where(r => numbersForNthField.All(r.NumberSatisfiesRule)).ToList();
                
                switch (satisfiedRules.Count)
                {
                    case 0:
                        throw new Exception($"Field index {fieldIndex} does not satisfy any rules!");
                    case 1:
                        fieldIndexToRule[fieldIndex] = satisfiedRules.First();
                        break;
                    default:
                        fieldIndexToPossibleRules[fieldIndex] = satisfiedRules;
                        break;
                }
            }
            
            // Some fields may satisfy multiple rules. Go back and fix up.
            while (fieldIndexToPossibleRules.Count > 0)
            {
                foreach (var fieldIndex in fieldIndexToPossibleRules.Keys.ToList())
                {
                    var possibleRules = fieldIndexToPossibleRules[fieldIndex];
                    var updatedPossibleRules = possibleRules.Except(fieldIndexToRule.Values).ToList();
                    switch (updatedPossibleRules.Count)
                    {
                        case 0:
                            throw new Exception($"No rules remain to satisfy field index {fieldIndex}!");
                        case 1:
                            fieldIndexToRule[fieldIndex] = updatedPossibleRules.First();
                            fieldIndexToPossibleRules.Remove(fieldIndex);
                            break;
                        default:
                            fieldIndexToPossibleRules[fieldIndex] = updatedPossibleRules;
                            break;
                    }
                }
            }

            var indexesOfRulesThatStartWithWordDeparture =
                fieldIndexToRule.Where(pair => pair.Value.Name.ToLower().StartsWith("departure")).Select(p => p.Key);
            var productOfThoseFieldsOnMyTicket = indexesOfRulesThatStartWithWordDeparture
                .Select(i => document.MyTicket.Numbers[i]).Aggregate(1L, (acc, val) => acc * val);
            return productOfThoseFieldsOnMyTicket;
        }

        private static Document CleanDocument(Document document)
        {
            var invalidTickets = FindInvalidTickets(document);
            return new Document(document.MyTicket, document.NearbyTickets.Except(invalidTickets).ToArray(), document.Rules.ToArray());
        }

        private static Document ParseInput(string[] inputs)
        {
            var parsePhase = ParsePhase.Rules;

            var rules = new List<Rule>();
            var nearbyTickets = new List<Ticket>();
            Ticket? myTicket = null;
            
            foreach (var input in inputs)
            {
                if (input == string.Empty) continue;
                if (input == "your ticket:")
                {
                    parsePhase = ParsePhase.MyTicket;
                    continue;
                }
                if (input == "nearby tickets:")
                {
                    parsePhase = ParsePhase.NearbyTickets;
                    continue;
                }

                switch (parsePhase)
                {
                    case ParsePhase.Rules:
                        rules.Add(new Rule(input));
                        break;
                    case ParsePhase.MyTicket:
                        myTicket = new Ticket(input);
                        break;
                    case ParsePhase.NearbyTickets:
                        nearbyTickets.Add(new Ticket(input));
                        break;
                }

            }
            
            if (!myTicket.HasValue) throw new Exception($"Didn't find my ticket?");
            return new Document(myTicket.Value, nearbyTickets.ToArray(), rules.ToArray());
        }
        
        internal enum ParsePhase { Rules, MyTicket, NearbyTickets }

        internal struct Rule
        {
            private readonly string _raw;
            public string Name;
            public int LowerBoundA;
            public int UpperBoundA;
            public int LowerBoundB;
            public int UpperBoundB;

            public Rule(string raw)
            {
                _raw = raw;
                var elems = raw.Split(':');
                Name = elems[0].Trim();
                LowerBoundA = int.Parse(elems[1].Trim().Split(' ')[0].Split('-')[0]);
                UpperBoundA = int.Parse(elems[1].Trim().Split(' ')[0].Split('-')[1]);
                LowerBoundB = int.Parse(elems[1].Trim().Split(' ')[2].Split('-')[0]);
                UpperBoundB = int.Parse(elems[1].Trim().Split(' ')[2].Split('-')[1]);
            }

            public bool NumberSatisfiesRule(int number)
            {
                return (number >= LowerBoundA && number <= UpperBoundA) ||
                       (number >= LowerBoundB && number <= UpperBoundB);
            }

            public override string ToString() { return _raw; }
        }

        internal struct Ticket
        {
            public int[] Numbers;

            public Ticket(string raw)
            {
                Numbers = raw.Split(',').Select(int.Parse).ToArray();
            }

            public override string ToString()
            {
                return $"[{string.Join(",", Numbers)}]";
            }
        }

        private struct Document
        {
            public Ticket MyTicket;
            public Ticket[] NearbyTickets;
            public Rule[] Rules;

            public Document(Ticket myTicket, Ticket[] nearbyTickets, Rule[] rules)
            {
                MyTicket = myTicket;
                NearbyTickets = nearbyTickets;
                Rules = rules;
            }
        }
        
    }
}