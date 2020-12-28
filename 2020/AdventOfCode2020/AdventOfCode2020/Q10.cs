using System;
using System.Linq;

namespace AdventOfCode2020
{
    public static class Q10
    {
        public static void Solve()
        {
            var inputs = Tools.GetInput(10);
            var adapters = inputs.Select(int.Parse).ToArray();

            Part1(adapters);
            Part2(adapters);
        }

        /// <summary>
        /// "What is the number of 1-jolt differences multiplied by the number of 3-jolt differences?"
        /// </summary>
        private static void Part1(int[] adapters)
        {
            Array.Sort(adapters);

            var currentJoltage = 0;
            var joltageDifferencesOf1 = 0;
            var joltageDifferencesOf3 = 0;
            for (var i = 0; i < adapters.Length; i++)
            {
                // var nextAdapterJoltage = adapters[i];
                var joltageDifference = adapters[i] - currentJoltage;
                if (joltageDifference < 1) throw new Exception("Gone backwards.");
                if (joltageDifference == 1)
                {
                    joltageDifferencesOf1++;
                } 
                else if (joltageDifference == 3)
                {
                    joltageDifferencesOf3++;
                } 
                else if (joltageDifference > 3) throw new Exception("Too large jump");
                
                currentJoltage = adapters[i];
            }
            
            // Final jolt jump to device.
            joltageDifferencesOf3++;

            Console.WriteLine($"Jolt jumps of 1 = {joltageDifferencesOf1}, jumps of 3 = {joltageDifferencesOf3}" +
                              $" (product = {joltageDifferencesOf1 * joltageDifferencesOf3}).");
        }

        private static void Part2(int[] adapters)
        {
            // Additional element at beginning represents mains.
            adapters = (new [] {0}).Concat(adapters).ToArray();
            Array.Sort(adapters);
            
            var combinationsFromIndex = new long[adapters.Length];
            combinationsFromIndex[adapters.Length - 1] = 1;
            for (var i = adapters.Length - 2; i >= 0; i--)
            {
                combinationsFromIndex[i] = 0L;
                for (var j = i + 1; j < adapters.Length && j <= i + 3; j++)
                {
                    if (adapters[j] - adapters[i] <= 3) combinationsFromIndex[i] += combinationsFromIndex[j];
                }
            }

            Console.WriteLine($"Combinations: {combinationsFromIndex[0]}");
        }
    }
}