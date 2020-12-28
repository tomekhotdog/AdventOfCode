using System;
using System.Linq;

namespace AdventOfCode2020
{
    public static class Q18
    {
        public static void Solve()
        {
            var inputs = Tools.GetInput(18);
            
            // Note: remove early evaluation of addition expressions for Part 1.
            EvaluateAndSumExpressions(inputs);
            
            // Remark: more efficient solution could use a stack of tokens.
        }

        private static void EvaluateAndSumExpressions(string[] expressions)
        {
            var sum = 0L;
            foreach (var expression in expressions)
            {
                var result = Evaluate(expression);
                Console.WriteLine($"{expression}. Answer = {result}");
                sum += result;
            }

            Console.WriteLine($"Sum = {sum}");
        }

        private static long Evaluate(string expression)
        {
            if (expression.Contains('('))
            {
                return EvaluateExpressionWithBrackets(expression);
            }
            
            // Evaluate addition first (for Part 2 only).
            if (expression.Contains('+'))
            {
                return EvaluateExpressionWithAdditionPrecedence(expression);
            }
            
            return EvaluateExpandedExpression(expression);
        }

        private static long EvaluateExpressionWithBrackets(string expression)
        {
            var indexOfBracket = expression.LastIndexOf('(');
            var indexOfOtherBracket = expression.IndexOf(')', indexOfBracket);
            var expressionInBrackets = expression.Substring(indexOfBracket + 1, indexOfOtherBracket - (indexOfBracket + 1));
            var indexOfNextBit = indexOfOtherBracket + 1;

            var beforeBracket = expression.Substring(0, indexOfBracket);
            var inBracket = $"{Evaluate(expressionInBrackets)}";
            var afterBracket = expression.Substring(indexOfNextBit, expression.Length - indexOfNextBit);

            return Evaluate(beforeBracket + inBracket + afterBracket);
        }

        private static long EvaluateExpressionWithAdditionPrecedence(string expression)
        {
            var tokens = expression.Split(" ");
            var tokenIndex = Array.FindIndex(tokens, s => s == "+");
            var evaluatedAddition = int.Parse(tokens[tokenIndex - 1]) + int.Parse(tokens[tokenIndex + 1]);
            var pre = string.Join(" ", tokens.Take(tokenIndex - 1));
            var post = string.Join(" ", tokens.Skip(tokenIndex + 2));
            var replacement = (pre + $" {evaluatedAddition} " + post).Trim();
            return Evaluate(replacement);
        }

        private static long EvaluateExpandedExpression(string expression)
        {
            var tokens = expression.Split(" ");
            var nextOperator = Operator.Multiply;
            var accumulatedResult = 1L;
            for (var i = 0L; i < tokens.Length; i++)
            {
                if (tokens[i] == "*")
                {
                    nextOperator = Operator.Multiply;
                } 
                else if (tokens[i] == "+")
                {
                    nextOperator = Operator.Add;
                }
                else
                {
                    var operand = int.Parse(tokens[i]);
                    switch (nextOperator)
                    {
                        case Operator.Add:
                            accumulatedResult += operand;
                            break;
                        case Operator.Multiply:
                            accumulatedResult *= operand;
                            break;
                        default:
                            throw new NotImplementedException();
                    }
                }
            }

            return accumulatedResult;
        }

        enum Operator { Add, Multiply }
    }
}