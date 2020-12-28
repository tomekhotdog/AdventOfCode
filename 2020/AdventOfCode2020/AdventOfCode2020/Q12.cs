using System;

namespace AdventOfCode2020
{
    public static class Q12
    {
        public static void Solve()
        {
            var inputs = Tools.GetInput(12);

            Part1(inputs);
            Part2(inputs);
        }

        private static void Part1(string[] instructions)
        {
            var position = new Position();
            foreach (var instruction in instructions) position.ApplyInstruction(instruction);

            Console.WriteLine($"Final position: {position}.");
            Console.WriteLine($"Manhattan distance from start: Abs({position.X:F}) + Abs({position.Y:F}) =" +
                              $" {Math.Abs(position.X) + Math.Abs(position.Y):F}");
        }

        private static void Part2(string[] instructions)
        {
            var position = new PositionWithWaypoint(10, 1);
            foreach (var instruction in instructions) position.ApplyInstruction(instruction);

            Console.WriteLine($"Final position: {position}.");
            Console.WriteLine($"Manhattan distance from start: Abs({position.ShipX:F}) + Abs({position.ShipY:F}) =" +
                              $" {Math.Abs(position.ShipX) + Math.Abs(position.ShipY):F}");
        }

        // Rules from Part 1.
        public class Position
        {
            // Position along East/West axis.
            public double X { get; private set; }
            // Position along North/South axis.
            public double Y { get; private set; }
            // Degrees of rotation (0-360). 0 represents facing East, rotation clockwise (90 represents facing South).
            public double Orientation { get; private set; }

            public Position()
            {
                X = 0;
                Y = 0;
                Orientation = 0;
            }

            public void ApplyInstruction(string instruction)
            {
                var instructionType = instruction[0];
                var value = int.Parse(instruction.Substring(1, instruction.Length - 1));

                switch (instructionType)
                {
                    case 'N': Y += value;
                        break;
                    case 'S': Y -= value;
                        break;
                    case 'E': X += value;
                        break;
                    case 'W': X -= value;
                        break;
                    case 'L': Orientation -= value;
                        break;
                    case 'R': Orientation += value;
                        break;
                    case 'F':
                        X += Math.Cos(Orientation / 360 * 2 * Math.PI) * value;
                        Y -= Math.Sin(Orientation / 360 * 2 * Math.PI) * value;
                        break;
                    default:
                        throw new Exception($"Unexpected instruction: {instruction}");
                }
            }

            public override string ToString()
            {
                return $"Ship position: [{X:F},{Y:F}], Orientation={Orientation:F}";
            }
        }

        // For part 2.
        public class PositionWithWaypoint
        {
            // Position along East/West axis.
            public double ShipX { get; private set; }
            // Position along North/South axis.
            public double ShipY { get; private set; }
            
            // Position relative to ship - hypothenuse (polar coordinates).
            public double WaypointR { get; private set; }
            // Position relative to ship - theta (polar coordinates).
            public double WaypointTheta { get; private set; }

            public PositionWithWaypoint(double startX, double startY)
            {
                ShipX = 0;
                ShipY = 0;
                WaypointR = Math.Sqrt(Math.Pow(startX, 2) + Math.Pow(startY, 2));
                WaypointTheta = CalculatePolarAngle(startX, startY);
            }
            
            public void ApplyInstruction(string instruction)
            {
                var instructionType = instruction[0];
                var value = int.Parse(instruction.Substring(1, instruction.Length - 1));

                switch (instructionType)
                {
                    case 'N': UpdateWayPointCoordinates(0, value);
                        break;
                    case 'S': UpdateWayPointCoordinates(0, -value);
                        break;
                    case 'E': UpdateWayPointCoordinates(value, 0);
                        break;
                    case 'W': UpdateWayPointCoordinates(-value, 0);
                        break;
                    case 'L': WaypointTheta += (value / 360.0 * 2 * Math.PI);
                        break;
                    case 'R': WaypointTheta -= (value / 360.0 * 2 * Math.PI);
                        break;
                    case 'F':
                        ShipX += WaypointR * Math.Cos(WaypointTheta) * value;
                        ShipY += WaypointR * Math.Sin(WaypointTheta) * value;
                        break;
                    default:
                        throw new Exception($"Unexpected instruction: {instruction}");
                }
            }
            
            private void UpdateWayPointCoordinates(int xDelta, int yDelta)
            {
                var currentXPos = WaypointR * Math.Cos(WaypointTheta);
                var currentYPos = WaypointR * Math.Sin(WaypointTheta);
                currentXPos += xDelta;
                currentYPos += yDelta;
                
                WaypointR = Math.Sqrt(Math.Pow(currentXPos, 2) + Math.Pow(currentYPos, 2));
                WaypointTheta = CalculatePolarAngle(currentXPos, currentYPos);
            }

            // Polar coordinates - deal with the 'quadrants' (https://www.mathsisfun.com/polar-cartesian-coordinates.html)
            private static double CalculatePolarAngle(double x, double y)
            {
                var theta = Math.Atan(y / x);
                if (x < 0 && y < 0)
                {
                    theta -= Math.PI;
                }
                else if (x < 0)
                {
                    theta += Math.PI;
                }
                return theta;
            }
            
            public override string ToString()
            {
                return $"Ship position: [{ShipX:F},{ShipY:F}]. Waypoint: [{WaypointR:F},{WaypointTheta:F}]";
            }
        }
    }
}