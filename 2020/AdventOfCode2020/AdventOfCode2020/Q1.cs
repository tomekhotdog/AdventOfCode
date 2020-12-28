using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode2020
{
    public static class Q1
    {
        public static int SolvePart1()
        {
            var pathToInputs = @"/Users/tomaszszymaniec/Documents/Coding/Classic/AdventOfCode/2020/inputs/q1.txt";
            var numbers = Tools.ReadText(pathToInputs).Select(int.Parse).ToList();
            var numbersRequiredFor2020 = new HashSet<int>();
            
            foreach (var number in numbers)
            {
                numbersRequiredFor2020.Add(2020 - number);
            }
            
            var result = 0;
            foreach (var fstNumberInPair in numbers)
            {
                if (numbersRequiredFor2020.Contains(fstNumberInPair))
                {
                    var sndNumberInPair = 2020 - fstNumberInPair;
                    result = fstNumberInPair * sndNumberInPair;
                    Console.WriteLine($"Found pair: {fstNumberInPair} and {sndNumberInPair} (product = {result})");
                }
            }

            return result;
        }
        
        // To improve: Avoid using the same number more than once. Avoid finding the same solution n times.
        public static int SolvePart2() {
            var pathToInputs = @"/Users/tomaszszymaniec/Documents/Coding/Classic/AdventOfCode/2020/inputs/q1.txt";
            var numbers = Tools.ReadText(pathToInputs).Select(int.Parse).ToList();
            var numbersRequiredFor2020 = new Dictionary<int, (int, int)>();
            
            foreach (var fstNumber in numbers)
            {
                foreach (var sndNumber in numbers)
                {
                    var requiredThirdNumber = 2020 - (fstNumber + sndNumber);
                    numbersRequiredFor2020[requiredThirdNumber] = (fstNumber, sndNumber);
                }
            }
            
            var result = 0;
            foreach (var thirdNumberInTuple in numbers)
            {
                if (numbersRequiredFor2020.ContainsKey(thirdNumberInTuple))
                {
                    var otherPair = numbersRequiredFor2020[thirdNumberInTuple];
                    var fstNumberInPair = otherPair.Item1;
                    var sndNumberInPair = otherPair.Item2;
                    result = fstNumberInPair * sndNumberInPair * thirdNumberInTuple;
                    Console.WriteLine($"Found tuple: {fstNumberInPair}, {sndNumberInPair} and {thirdNumberInTuple}. Product = {result}");
                }
            }

            return result;
        }
    }
}