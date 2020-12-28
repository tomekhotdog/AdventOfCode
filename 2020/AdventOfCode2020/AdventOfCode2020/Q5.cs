using System;
using System.Linq;

namespace AdventOfCode2020
{
    public static class Q5
    {
        public static void Solve()
        {
            var inputs = Tools.GetInput(5);
            
            // Part 1 - Find highest SeatId.
            var maxSeatId = inputs.Max(i => new BoardingPass(i).SeatId);
            Console.WriteLine($"Max SeatId = {maxSeatId}");
            
            // Part 2 - Find missing seat. (Not at very front or very back).
            var missingSeats = Enumerable.Range(1, 128 * 8)
                .Except(inputs.Select(i => new BoardingPass(i).SeatId))
                .Select(ms => new BoardingPass(ms));
            foreach (var missingSeat in missingSeats) Console.WriteLine(missingSeat);
            // (Deduced missing seat by inspection)
        }

        private static void TestKnownInputs()
        {
            Console.WriteLine(new BoardingPass("BFFFBBFRRR"));
            Console.WriteLine(new BoardingPass("FFFBBBFRRR"));
            Console.WriteLine(new BoardingPass("BBFFBBFRLL"));
        }

        public struct BoardingPass
        {
            public readonly string Representation;
            public readonly int Row;
            public readonly int Seat;
            public int SeatId => (Row * 8) + Seat;

            public BoardingPass(string representation)
            {
                if (representation.Length != 10) throw new Exception($"Invalid boarding pass: {representation}");
                Representation = representation;
                Row = ParseRow(representation.Substring(0, 7));
                Seat = ParseSeat(representation.Substring(7, 3));
            }

            public BoardingPass(int seatId)
            {
                Representation = "Unknown";
                Row = seatId / 8;
                Seat = seatId - (Row * 8);
            }

            private static int ParseRow(string rowString)
            {
                if (rowString.Length != 7) throw new Exception($"Invalid row string: {rowString}");
                var binaryRepresentation = string.Concat(rowString.ToCharArray().Select(c =>
                    c == 'B' ? '1' : c == 'F' ? '0' : throw new Exception($"Invalid char in row string: {rowString}.")));
                return Convert.ToInt32(binaryRepresentation, 2);
            }

            private static int ParseSeat(string seatString)
            {
                if (seatString.Length != 3) throw new Exception($"Invalid row string: {seatString}");
                var binaryRepresentation = string.Concat(seatString.ToCharArray().Select(c =>
                    c == 'R' ? '1' : c == 'L' ? '0' : throw new Exception($"Invalid char in row string: {seatString}.")));
                return Convert.ToInt32(binaryRepresentation, 2);
            }

            public override string ToString()
            {
                return $"Representation: {Representation}. Row {Row}. Seat {Seat}. SeatId {SeatId}";
            }
        }
    }
}