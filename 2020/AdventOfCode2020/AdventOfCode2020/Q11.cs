using System;
using System.Linq;

namespace AdventOfCode2020
{
    public static class Q11
    {
        public static void Solve()
        {
            var inputs = Tools.GetInput(11);
            var seats = inputs.Select(row => row.ToCharArray()).ToArray();

            FindEquilibrium(seats, true);
        }

        private static void FindEquilibrium(char[][] seats, bool part2Rules)
        {
            var currentState = seats;
            var nextState = NextGeneration(currentState, part2Rules);
            var iterations = 1;
            
            while (!AreEquivalent(currentState, nextState))
            {
                currentState = nextState;
                nextState = NextGeneration(currentState, part2Rules);
                iterations++;
            }
            
            var occupiedSeats = nextState.Sum(e => e.Count(c => c == '#'));
            Console.WriteLine($"Reached steady state after {iterations} iterations. ({occupiedSeats} occupied).");
        }

        private static char[][] NextGeneration(char[][] seats, bool part2Rules = false)
        {
            var nextGeneration = new char[seats.Length][];
            for (var i = 0; i < seats.Length; i++)
            {
                nextGeneration[i] = new char[seats[i].Length];
                for (var j = 0; j < seats[i].Length; j++)
                {
                    var adjacentSeats = part2Rules ? VisibleAdjacentSeats(i, j, seats) : AdjacentSeats(i, j, seats);
                    var occupied = adjacentSeats.Count(seat => seat == '#');
                    if (seats[i][j] == 'L' && occupied == 0)
                    {
                        nextGeneration[i][j] = '#';
                    } 
                    else if (seats[i][j] == '#' && occupied >= (part2Rules ? 5 : 4))
                    {
                        nextGeneration[i][j] = 'L';
                    }
                    else
                    {
                        nextGeneration[i][j] = seats[i][j];
                    }
                }
            }
            return nextGeneration;
        }

        // Part 1.
        private static char[] AdjacentSeats(int i, int j, char[][] seats)
        {
            var adjacentSeats = "";
            foreach (var rowDelta in new[]{-1,0,1})
            {
                foreach (var columnDelta in new[] {-1,0,1})
                {
                    if (rowDelta == 0 && columnDelta == 0) continue;
                    if (SeatExists(i + rowDelta, j + columnDelta, seats))
                    {
                        adjacentSeats += seats[i + rowDelta][j + columnDelta];
                    }
                }
            }
            return adjacentSeats.ToCharArray();
        }

        // Part 2.
        private static char[] VisibleAdjacentSeats(int i, int j, char[][] seats)
        {
            var adjacentSeats = "";
            foreach (var rowDelta in new[]{-1,0,1})
            {
                foreach (var columnDelta in new[] {-1,0,1})
                {
                    if (rowDelta == 0 && columnDelta == 0) continue;

                    var moves = 1;
                    while (true)
                    {
                        var currentRow = i + rowDelta * moves;
                        var currentColumn = j + columnDelta * moves;
                        
                        if (!SeatExists(currentRow, currentColumn, seats)) break;
                        if (seats[currentRow][currentColumn] != '.')
                        {
                            adjacentSeats += seats[currentRow][currentColumn];
                            break;
                        }
                        moves ++;
                    }
                }
            }
            return adjacentSeats.ToCharArray();
        }

        private static bool SeatExists(int i, int j, char[][] seats)
        {
            return i >= 0 && i < seats.Length && j >= 0 && j < seats[i].Length;
        }

        private static void PrintSeats(char[][] seats)
        {
            for (var i = 0; i < seats.Length; i++)
            {
                for (var j = 0; j < seats[i].Length; j++)
                {
                    Console.Write(seats[i][j]);
                }
                Console.WriteLine();
            }
        }

        private static bool AreEquivalent(char[][] a, char[][] b)
        {
            var aString = string.Join("", a.Select(e => new string(e)));
            var bString = string.Join("", b.Select(e => new string(e)));
            return aString == bString;
        }
    }
}