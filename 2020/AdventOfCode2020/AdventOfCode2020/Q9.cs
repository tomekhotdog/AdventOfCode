using System;
using System.Linq;

namespace AdventOfCode2020
{
    public class Q9
    {
        public static void Solve()
        {
            var inputs = Tools.GetInput(9);
            var numbers = inputs.Select(long.Parse).ToArray();
            
            // Part 1.
            var firstElemWithoutProperty = FindFirstElementWithoutSumProperty(numbers, 25);
            Console.WriteLine($"First number without summing property: {firstElemWithoutProperty.value}");
            
            // Part 2.
            var contiguousSet = FindContiguousSetWithSum(numbers, firstElemWithoutProperty.value);
            Console.WriteLine($"Contiguous set: {string.Join(",", contiguousSet.Select(e => e.ToString()))}");
            var encryptionWeakness = contiguousSet.Min() + contiguousSet.Max();
            Console.WriteLine($"Encryption weakness (contiguous set min + max elems): {encryptionWeakness}");
        }

        private static (long index, long value) FindFirstElementWithoutSumProperty(long[] numbers, int lookBackLength)
        {
            for (var i = lookBackLength; i < numbers.Length; i++)
            {
                var targetSum = numbers[i];
                if (TwoElementsCanSum(numbers, i - lookBackLength, i - 1, targetSum)) continue;
                return (i, numbers[i]);
            }
            throw new Exception();
        }

        private static bool TwoElementsCanSum(long[] numbers, int startIndex, int endIndex, long target)
        {
            for (var i = startIndex; i <= endIndex; i++)
            {
                for (var j = startIndex; j <= endIndex; j++)
                {
                    if (i == j) continue;
                    if (numbers[i] + numbers[j] == target) return true;
                }
            }
            return false;
        }

        private static long[] FindContiguousSetWithSum(long[] numbers, long target)
        {
            if (numbers.Length == 0) throw new Exception("Cannot have empty array.");
            
            var currentSum = numbers[0];
            var lowerIndex = 0;
            var upperIndex = 0;
            while (lowerIndex < numbers.Length && upperIndex < numbers.Length)
            {
                if (currentSum == target)
                {
                    return numbers.Skip(lowerIndex).Take(upperIndex - lowerIndex).ToArray();
                }
                
                if (currentSum < target)
                {
                    upperIndex++;
                    currentSum += numbers[upperIndex];
                } 
                else if (currentSum > target)
                {
                    currentSum -= numbers[lowerIndex];
                    lowerIndex++;
                }
            }

            throw new Exception("Could not find contiguous set.");
        }
    }
}