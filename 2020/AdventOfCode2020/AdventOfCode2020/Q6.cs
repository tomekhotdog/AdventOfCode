using System;
using System.Linq;

namespace AdventOfCode2020
{
    public static class Q6
    {
        public static void Solve(Question questionPart)
        {
            var inputs = Tools.GetInput(6);
            var currentGroup = string.Empty;
            var uniqueAnswerSum = 0;
            var startOfNewGroup = true;
            
            for (var i = 0; i <= inputs.Length; i++)
            {
                // Reached end of group.
                if (i == inputs.Length || inputs[i].Trim() == string.Empty)
                {
                    var uniqueAnswers = string.Concat(currentGroup.Distinct());
                    uniqueAnswerSum += uniqueAnswers.Length;
                    // Console.WriteLine($"{currentGroup}: unique count = {uniqueAnswers.Length}");
                    currentGroup = string.Empty;
                    startOfNewGroup = true;
                }
                else
                {
                    currentGroup = startOfNewGroup ? inputs[i].Trim() : Combine(currentGroup, inputs[i].Trim(), questionPart);
                    startOfNewGroup = false;
                }
            }

            Console.WriteLine($"Sum = {uniqueAnswerSum}");

        }

        private static string Combine(string existingCombinedAnswers, string nextAnswer, Question questionPart)
        {
            switch (questionPart)
            {
                case Question.Part1:
                    return string.Concat(existingCombinedAnswers.ToCharArray().Union(nextAnswer.ToCharArray()));
                case Question.Part2:
                    return string.Concat(existingCombinedAnswers.ToCharArray().Intersect(nextAnswer.ToCharArray()));
                default:
                    throw new NotImplementedException();
            }
        }

        public enum Question
        {
            // First part of question asks for union of customs form answers.
            Part1, 
            // Second part of question asks for intersection of customs form answers.
            Part2
        }
    }
}