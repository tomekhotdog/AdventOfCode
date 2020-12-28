using System;
using System.Linq;

namespace AdventOfCode2020
{
    public static class Q2
    {
        public static int Solve(Rule rule)
        {
            var pathToInputs = @"/Users/tomaszszymaniec/Documents/Coding/Classic/AdventOfCode/2020/inputs/q2.txt";
            var passwordsWithRules = Tools.ReadText(pathToInputs);

            var validPasswords = 0;
            var invalidPasswords = 0;

            foreach (var passwordWithRule in passwordsWithRules)
            {
                if (!passwordWithRule.Contains(':')) throw new Exception($"Unexpected password with rule: {passwordWithRule}");
                var elements = passwordWithRule.Split(':');
                if (elements.Length != 2) throw new Exception($"Unexpected password with rule: {passwordWithRule}");
                var ruleString = elements[0];
                var password = elements[1].Trim();
                var schema = new PasswordSchema(ruleString);

                if (IsValid(rule, schema, password))
                {
                    Console.WriteLine($"Valid password: {passwordWithRule}");
                    validPasswords++;
                }
                else
                {
                    Console.WriteLine($"Invalid password: {passwordWithRule}");
                    invalidPasswords++;
                }
            }

            Console.WriteLine($"Valid={validPasswords}, Invalid={invalidPasswords}");
            
            return validPasswords;
        }

        private static bool IsValid(Rule rule, PasswordSchema schema, string password)
        {
            if (rule == Rule.Q1)
            {
                var characterCountInPassword = password.ToCharArray().Count(c => c == schema.Character);
                return characterCountInPassword >= schema.FirstPosition && characterCountInPassword <= schema.SecondPosition;    
            }

            if (rule == Rule.Q2)
            {
                var position1Correct = 
                    password.Length + 1 > schema.FirstPosition && password[schema.FirstPosition - 1] == schema.Character;
                var position2Correct = 
                    password.Length + 1 > schema.SecondPosition && password[schema.SecondPosition - 1] == schema.Character;
                
                return (position1Correct && !position2Correct) || (!position1Correct && position2Correct);
            }

            return false;
        }

        public struct PasswordSchema
        {
            public char Character;
            public int FirstPosition;
            public int SecondPosition;
            
            public PasswordSchema(string rule)
            {
                if (!rule.Contains(' ')) throw new Exception($"Unexpected rule: {rule}");
                var elems = rule.Split(' ');
                var minMaxCounts = elems[0];
                
                if (elems[1].Length != 1) throw new Exception($"Unexpected rule: {rule}");
                Character = elems[1][0];

                if (!minMaxCounts.Contains('-')) throw new Exception($"Unexpected minmax count: {minMaxCounts}");
                var minMaxElems = minMaxCounts.Split('-');
                FirstPosition = int.Parse(minMaxElems[0]);
                SecondPosition = int.Parse(minMaxElems[1]);
            }
        }

        public enum Rule { Q1, Q2 };
    }
}