using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode2020
{
    public static class Q24
    {
        public static void Solve()
        {
            var input = Tools.GetInput(24, false);
            var directions = ParseInput(input);
            var initial = InitialPositions(directions);
            Console.WriteLine($"Part 1: {Part1(initial)}");
            Console.WriteLine($"Part 2: {Part2(initial)}");
        }

        private static int Part1(Dictionary<Position, bool> flippedOver)
        {
            return flippedOver.Values.Count(f => f);
        }
        
        private static Dictionary<Position, bool> InitialPositions(List<List<Direction>> directions)
        {
            var flippedOver = new Dictionary<Position, bool>();
            foreach (var setOfDirections in directions)
            {
                var position = new Position(0, 0);
                foreach (var direction in setOfDirections)
                {
                    position = position.Move(direction);
                }
                
                flippedOver.TryGetValue(position, out var existing); 
                flippedOver[position] = !existing;
            }

            return flippedOver;
        }

        private static int Part2(Dictionary<Position, bool> flippedOver)
        {
            var today = flippedOver;
            foreach (var day in Enumerable.Range(1, 100))
            {
                today = SimulateDay(today);
            }
            return today.Values.Count(f => f);
        }

        private static Dictionary<Position, bool> SimulateDay(Dictionary<Position, bool> today)
        {
            var tomorrow = new Dictionary<Position, bool>();
            var positionsToInspect = new List<Position>();

            // Identify all the tiles that need to be considered.
            foreach (var (position, black) in today)
            {
                // The only tiles that may need to flip are either currently black or next to a black tile.    
                if (!black) continue;
                positionsToInspect.Add(position);
                positionsToInspect.AddRange(Neighbours(position));
            }
            
            var inspected = new HashSet<Position>();
            foreach (var p in positionsToInspect)
            {
                if (inspected.Contains(p)) continue;
                var blackNeighbours = Neighbours(p).Count(q => IsBlack(today, q));
                //  "Any black tile with zero or more than 2 black tiles immediately adjacent to it is flipped to white." 
                if (IsBlack(today, p))
                {
                    
                    // Hence tile remains black if has one black neighbour.
                    if (blackNeighbours is 1 or 2)
                    {
                        tomorrow[p] = true;
                    }
                }
                // "Any white tile with exactly 2 black tiles immediately adjacent to it is flipped to black."
                else
                {
                    if (blackNeighbours == 2)
                    {
                        tomorrow[p] = true;
                    }
                }

                inspected.Add(p);
            }
            return tomorrow;
        }

        private static bool IsBlack(Dictionary<Position, bool> flippedOver, Position position)
        {
            return flippedOver.ContainsKey(position) && flippedOver[position];
        }

        private static List<Position> Neighbours(Position position)
        {
            return Enum.GetValues<Direction>().Select(position.Move).ToList();
        }

        private static List<List<Direction>> ParseInput(string[] input)
        {
            var directions = new List<List<Direction>>();
            foreach (var line in input)
            {
                var remaining = line;
                var next = new List<Direction>();
                while (remaining.Length > 0)
                {
                    foreach (var direction in Enum.GetValues<Direction>())
                    {
                        if (remaining.StartsWith(direction.ToString()))
                        {
                            next.Add(direction);
                            remaining = remaining[direction.ToString().Length..];
                        }
                    }
                }
                directions.Add(next);
            }
            return directions;
        }

        internal enum Direction { nw, w, sw, se, e, ne }

        internal record Position(int X, int Y)
        {
            public Position Move(Direction direction)
            {
                switch (direction)
                {
                    case  Direction.nw:
                        return new Position(X: X - 1, Y: Y - 1);
                    case  Direction.w:
                        return new Position(X: X - 1, Y: Y);
                    case Direction.sw:
                        return new Position(X: X, Y: Y + 1);
                    case Direction.se:
                        return new Position(X: X + 1, Y: Y + 1);
                    case Direction.e:
                        return new Position(X: X + 1, Y: Y);
                    case Direction.ne:
                        return new Position(X: X, Y: Y - 1);
                    default:
                        throw new Exception($"Unexpected direction: {direction}");
                }
            }
        }
    }
}