using System;
using System.Linq;

namespace AdventOfCode2020
{
    public static class Q13
    {
        public static void Solve()
        {
            var inputs = Tools.GetInput(13, false);
            if (inputs.Length != 2) throw new Exception("Unexpected input.");

            SolvePart1(int.Parse(inputs[0]), inputs[1]);
            SolvePart2(inputs[1]);
        }

        private static void SolvePart1(int arrivalTime, string busSchedule)
        {
            var timesToWait = busSchedule
                .Split(',')
                .Where(s => s != "x")
                .Select(int.Parse)
                .Select(id => (id, id - (arrivalTime % id)))
                .ToList();

            timesToWait.Sort((fst, snd) => fst.Item2.CompareTo(snd.Item2));
            var bestBus = timesToWait.First();
            Console.WriteLine($"Shortest time to wait is {bestBus.Item2}mins (bus id = {bestBus.id}).");
            Console.WriteLine($"BusId * time-to-wait = {bestBus.id} * {bestBus.Item2} = {bestBus.id * bestBus.Item2}");
        }

        private static void SolvePart2(string busSchedule)
        {
            var positionAndIntervals = busSchedule.Split(',')
                .Select((s, i) => (i, s))
                .Where(e => e.s != "x")
                .Select(tuple => (tuple.i, int.Parse(tuple.s))).ToList();

            long timestamp = 0L;
            long delta = 1;
            foreach (var bus in positionAndIntervals)
            {
                timestamp = NextTimeInCorrectPosition(timestamp, delta, bus.Item2, bus.Item1);
                delta *= LowestCommonMultiple(delta, bus.Item2);
            }

            Console.WriteLine($"First timestamp that satisfies requirement: {timestamp}");
        }

        private static long NextTimeInCorrectPosition(long startingPoint, long currentDelta, int nextBusInterval, int nextBusPosition)
        {
            var candidate = startingPoint;
            while ((candidate + nextBusPosition) % nextBusInterval != 0)
            {
                candidate += currentDelta;
            }
            return candidate;
        }

        static long GreatestCommonFactor(long a, long b)
        {
            while (b != 0)
            {
                long temp = b;
                b = a % b;
                a = temp;
            }
            return a;
        }

        static long LowestCommonMultiple(long a, long b)
        {
            return (a / GreatestCommonFactor(a, b)) * b;
        }
    }
}