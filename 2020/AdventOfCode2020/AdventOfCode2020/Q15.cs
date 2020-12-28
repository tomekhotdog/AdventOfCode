using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode2020
{
    public static class Q15
    {
        public static void Solve()
        {
            var testInput = "2,3,1";
            var input = "9,12,1,4,17,0,18";
            
            var startingNumbers = input.Split(',').Select(long.Parse).ToArray();
            var lastSpokenNumber = NthSpokenNumber(startingNumbers, 30_000_000);
            Console.WriteLine($"(Test input). Nth number spoken: {lastSpokenNumber}");
            lastSpokenNumber = NthSpokenNumber(startingNumbers, 30_000_000);
            Console.WriteLine($"Nth number spoken: {lastSpokenNumber}");
        }

        // What is the 2020th number spoken?
        private static long NthSpokenNumber(long[] startingNumbers, long totalTurns)
        {
            var turnLastSpoken = new Dictionary<long, long>();
            var spoken = -1L;
            for (var turn = 1; turn <= totalTurns; turn++)
            {
                var previouslySpoken = spoken;
                if (turn <= startingNumbers.Length)
                {
                    spoken = startingNumbers[turn - 1];
                }
                else if (turnLastSpoken.ContainsKey(previouslySpoken))
                {
                    spoken = (turn - 1) - turnLastSpoken[previouslySpoken];
                }
                else
                {
                    spoken = 0;
                }
                turnLastSpoken[previouslySpoken] = turn - 1;
            }
            return spoken;
        }
    }
}