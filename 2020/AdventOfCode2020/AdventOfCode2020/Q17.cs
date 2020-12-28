using System;
using System.Linq;

namespace AdventOfCode2020
{
    public static class Q17
    {
        public static void Solve()
        {
            var inputs = Tools.GetInput(17, true);

            var cycles = 6;
            var cube = MakeCube(inputs, cycles);
            SimulateCycles(cube, cycles);
            
            // TODO: Need solution generic on cube dimension.
            // Consider Cube object with single dimension array, and system of
            // indexing to figure out the element in arbitrary dimensional space.
        }

        private static void SimulateCycles(bool[,,,] cube, int cycles)
        {
            Console.WriteLine("Before any cycles:");
            PrintCube(cube);
            for (var cycle = 0; cycle < cycles; cycle++)
            {
                cube = SimulateCycle(cube);
                var activeCubes = CountActiveCubes(cube);
                PrintCube(cube);
                Console.WriteLine($"After {cycle + 1} cycles: ({activeCubes} active cubes).");
            }
        }

        private static bool[,,,] MakeCube(string[] startingConfig, int requiredCycles)
        {
            var dimLength = startingConfig.Length + (2 * requiredCycles);
            var input = startingConfig.Select(s => s.ToCharArray()).ToArray();
            var chosenDimSize = dimLength + 2;
            var cube = new bool[chosenDimSize, chosenDimSize, chosenDimSize, chosenDimSize];

            for (var x = 0; x < input.Length; x++)
            {
                for (var y = 0; y < input.Length; y++)
                {
                    cube[requiredCycles + x, requiredCycles + y, dimLength / 2, dimLength / 2] = input[y][x] == '#';
                }
            }

            return cube;
        }

        private static bool[,,,] SimulateCycle(bool[,,,] cube)
        {
            var length = cube.GetLength(1);
            var newCube = new bool[length, length, length, length];

            // Don't consider outer dimensions of cube.
            for (var x = 0; x < length; x++)
            {
                for (var y = 0; y < length; y++)
                {
                    for (var z = 0; z < length; z++)
                    {
                        for (var w = 0; w < length; w++)
                        {
                            var currentlyActive = cube[x, y, z, w];
                            var activeNeighbours = ActiveNeighbours(cube, x, y, z, w);
                            if (currentlyActive)
                            {
                                newCube[x, y, z, w] = (activeNeighbours == 2 || activeNeighbours == 3);
                            }
                            else
                            {
                                newCube[x, y, z, w] = activeNeighbours == 3;
                            }
                        }
                    }
                }
            }

            return newCube;
        }

        private static int ActiveNeighbours(bool[,,,] cube, int x, int y, int z, int w)
        {
            var activeNeighbours = 0;
            var max = cube.GetLength(1);
            foreach (var xDelta in Enumerable.Range(-1, 3))
            {
                foreach (var yDelta in Enumerable.Range(-1, 3))
                {
                    foreach (var zDelta in Enumerable.Range(-1, 3))
                    {
                        foreach (var wDelta in Enumerable.Range(-1, 3))
                        {
                            if (xDelta == 0 && yDelta == 0 && zDelta == 0 && wDelta == 0) continue;
                            if (x + xDelta < 0 || x + xDelta >= max) continue;
                            if (y + yDelta < 0 || y + yDelta >= max) continue;
                            if (z + zDelta < 0 || z + zDelta >= max) continue;
                            if (w + wDelta < 0 || w + wDelta >= max) continue;
                            if (cube[x + xDelta, y + yDelta, z + zDelta, w + wDelta]) activeNeighbours++;
                        }
                    }
                }
            }
            return activeNeighbours;
        }

        private static int CountActiveCubes(bool[,,,] cube)
        {
            var active = 0;
            var length = cube.GetLength(1);
            for (var z = 0; z < length; z++)
            {
                for (var y = 0; y < length; y++)
                {
                    for (var x = 0; x < length; x++)
                    {
                        for (var w = 0; w < length; w++)
                        {
                            if (cube[x, y, z, w]) active++;
                        }
                    }
                }
            }
            return active;
        }

        private static void PrintCube(bool[,,,] cube)
        {
            var length = cube.GetLength(1);
            for (var w = 0; w < length; w++)
            {
                Console.WriteLine($"w={w - (length / 2)}");
                for (var z = 0; z < length; z++)
                {
                    Console.WriteLine($"z={z - (length / 2)}");
                    for (var y = 0; y < length; y++)
                    {
                        for (var x = 0; x < length; x++)
                        {
                            var active = cube[x, y, z, w];
                            Console.Write(active ? "#" : ".");
                        }

                        Console.WriteLine();
                    }
                    Console.WriteLine();
                }
            }
        }
    }
}