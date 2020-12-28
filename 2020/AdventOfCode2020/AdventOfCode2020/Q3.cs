using System;
using System.Linq;

namespace AdventOfCode2020
{
    public static class Q3
    {
        public static void Solve()
        {
            var pathToInputs = @"/Users/tomaszszymaniec/Documents/Coding/Classic/AdventOfCode/2020/inputs/q3.txt";
            var lines = Tools.ReadText(pathToInputs);

            var map = lines.ToArray();
            
            var route1 = EncounteredTrees(map, 1, 1);
            var route2 = EncounteredTrees(map, 3, 1);
            var route3 = EncounteredTrees(map, 5, 1);
            var route4 = EncounteredTrees(map, 7, 1);
            var route5 = EncounteredTrees(map, 1, 2);

            long product = route1 * route2 * route3 * route4 * route5;
            Console.WriteLine($"Final product: {product}");
        }

        private static long EncounteredTrees(string[] map, int xIncrement, int yIncrement)
        {
            var encounteredTrees = 0;
            var encounteredNoTrees = 0;
            
            var xPosition = 0;
            var yPosition = 0;

            while (yPosition < map.Length)
            {
                // Console.WriteLine(map[yPosition]);
                if (map[yPosition][xPosition] == '#')
                {
                    encounteredTrees++;
                    // Console.WriteLine("tree!");
                } 
                else if (map[yPosition][xPosition] == '.')
                {
                    encounteredNoTrees++;
                    // Console.WriteLine("no tree!");
                }
                else
                {
                    throw new Exception("Encountered strange environment.");
                }

                xPosition = (xPosition + xIncrement) % map[yPosition].Length;
                yPosition += yIncrement;
            }

            Console.WriteLine($"(Right {xIncrement}, Down {yIncrement}). Encountered {encounteredTrees} trees. ({encounteredNoTrees} free sections)");
            return encounteredTrees;
        }
    }
}