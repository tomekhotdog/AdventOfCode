using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode2020
{
    public static class Q20
    {
        public static void Solve()
        {
            var inputs = Tools.GetInput(20, false);
            var tiles = ParseTiles(inputs);
            var arrangement = Arrange(tiles);
            Console.WriteLine($"Part 1: {Part1(arrangement)}");
            Console.WriteLine($"Part 2: {Part2(arrangement)}");
        }

        private static ulong Part1(Arrangement arrangement)
        {
            return arrangement.ProductOfCornerTileIds();
        }
        
        private static int Part2(Arrangement arrangement)
        {
            var extracted = arrangement.ExtractImage();
            var combinedImageTile = new Tile(extracted.ToList(), 0);
            var seaMonsters = TileOrientation.AllOrientations()
                .Max(o => SeaMonstersInImage(combinedImageTile.ApplyOrientation(o)));
            var allHashes = string.Join("", combinedImageTile.Raw).Count(c => c == '#');
            const int elementsInSeaMonster = 15;
            return allHashes - (seaMonsters * elementsInSeaMonster);
        }

        private static int SeaMonstersInImage(string[] image)
        {
            const int seaMonsterWidth = 20;
            const int seaMonsterHeight = 2;
            var seaMonsters = 0;
            for (var y = 0; y < image.Length - seaMonsterHeight; y++)
            {
                for (var x = 0; x < image.Length - seaMonsterWidth; x++)
                {
                    seaMonsters = SeaMonsterAtOffset(new Coordinates(x, y), image) ? seaMonsters + 1 : seaMonsters;
                }
            }
            return seaMonsters;
        }

        // Starting at $, checks the coordinates offsets to determine if there is a sea monster at the location.
        // $                 # 
        // #    ##    ##    ###
        //  #  #  #  #  #  #
        private static bool SeaMonsterAtOffset(Coordinates start, string[] image)
        {
            var expectedSeaMonsterOffsets = new List<Coordinates>
            {
                new(0, 1), new(1, 2), new(4, 2), new(7, 2), new (10, 2), new (13, 2), new (16, 2), new (5, 1), 
                new (6, 1), new (11, 1), new (12, 1), new (18, 0), new (17, 1), new (18, 1), new (19, 1),
            };
            var coordinates = expectedSeaMonsterOffsets.Select(
                o => new Coordinates(start.X + o.X, start.Y + o.Y)).ToList();
            var size = image.Length;
            return coordinates.All(c => c.X >= 0 && c.Y >= 0 && c.X < size && c.Y < size && image[c.Y][c.X] == '#');
        }

        private static Arrangement Arrange(List<Tile> tiles)
        {
            var corners = FindCorners(tiles);
            var topLeftCorner = corners[0];
            var topLeftCornerOrientation = FindOrientationForTopLeftCorner(topLeftCorner, tiles);
            topLeftCorner.ApplyBorderOrientation(topLeftCornerOrientation);
            var dimensions = (int) Math.Sqrt(tiles.Count);
            
            var initialSetupWithCorner = new Tile[dimensions, dimensions];
            initialSetupWithCorner[0, 0] = topLeftCorner;
            var remainingTiles = new List<Tile>(tiles);
            remainingTiles.Remove(topLeftCorner);
            
            var done = new List<Arrangement>();
            var initialArrangement = new Arrangement(initialSetupWithCorner, remainingTiles);
            var candidates = new Queue<Arrangement>(new List<Arrangement> {initialArrangement});
            while (candidates.Count > 0)
            {
                var next = candidates.Dequeue();
                if (next.Done)
                {
                    done.Add(next);
                }
                else
                {
                    foreach (var possibility in FitOneTile(next))
                    {
                        candidates.Enqueue(possibility);
                    }                    
                }
            }

            if (done.Count != 1)
            {
                throw new Exception($"Unexpectedly found multiple ({done.Count}) possible arrangements!");
            }

            return done.First();
        }
        
        private record Arrangement(Tile[,] Arranged, List<Tile> Unarranged)
        {
            internal bool Done => Unarranged.Count == 0;

            internal ulong ProductOfCornerTileIds()
            {
                var d = Arranged.GetLength(0);
                return Convert.ToUInt64(Arranged[0, 0].TileId) * Convert.ToUInt64(Arranged[0, d-1].TileId) * 
                       Convert.ToUInt64(Arranged[d-1, 0].TileId) * Convert.ToUInt64(Arranged[d-1, d-1].TileId);
            }

            internal string[] ExtractImage()
            {
                var image = new List<string>();
                var nTiles = Arranged.GetLength(0);
                for (var x = 0; x < nTiles; x++)
                {
                    for (var y = 0; y < nTiles; y++)
                    {
                        var orientedTile = Arranged[y, x].ApplyOrientation(Arranged[y, x].Orientation);
                        var tileImageSize = Arranged[y, x].Size - 2;
                        for (var i = 0; i < tileImageSize; i++)
                        {
                            var nthTileImageRow = y * tileImageSize + i;
                            if (image.Count <= nthTileImageRow) image.Add(string.Empty);
                            image[nthTileImageRow] += orientedTile[1 + i].Substring(1, tileImageSize);
                        }
                    }
                }
                return image.ToArray();
            }
        }

        private static List<Arrangement> FitOneTile(Arrangement arrangement)
        {
            var nextCoords = NextTileCoordinates(arrangement.Arranged);
            var nextRequirements = GetRequirements(arrangement.Arranged, nextCoords);
            var viableNextTile = new List<Tuple<Tile, TileOrientation>>();
            foreach (var candidate in arrangement.Unarranged)
            {
                foreach (var orientation in TileOrientation.AllOrientations())
                {
                    candidate.ApplyBorderOrientation(orientation);
                    if (TileSatisfiesRequirements(candidate, nextRequirements))
                    {
                        viableNextTile.Add(new Tuple<Tile, TileOrientation>(candidate, orientation));
                    }
                }
            }
            
            var possibleArrangements = new List<Arrangement>();
            foreach (var nextTile in viableNextTile)
            {
                var newArrangement = new Arrangement(
                    DeepCopy(arrangement.Arranged), new List<Tile>(arrangement.Unarranged));
                var newTile = nextTile.Item1.Copy();
                newTile.ApplyBorderOrientation(nextTile.Item2);
                newArrangement.Arranged[nextCoords.Y, nextCoords.X] = newTile;
                newArrangement.Unarranged.Remove(nextTile.Item1);
                possibleArrangements.Add(newArrangement);
            }

            return possibleArrangements;
        }

        private static bool TileSatisfiesRequirements(Tile tile, Dictionary<Position, TileBorder> requirements)
        {
            foreach (var (position, requiredBorder) in requirements)
            {
                if (!tile.GetBorder(position).Equals(requiredBorder))
                {
                    return false;
                }
            }
            return true;
        }

        private static Coordinates NextTileCoordinates(Tile[,] arranged)
        {
            for (var y = 0; y < arranged.GetLength(1); y++)
            {
                for (var x = 0; x < arranged.GetLength(0); x++)    
                {
                    if (arranged[y, x] == null)
                    {
                        return new Coordinates(x, y);
                    }
                }
            }
            throw new Exception("Tile arrangement complete!");
        }

        private static Dictionary<Position, TileBorder> GetRequirements(Tile[,] arranged, Coordinates coordinates)
        {
            var size = arranged.GetLength(0);
            var requirements = new Dictionary<Position, TileBorder>();
            var top = coordinates with { Y = coordinates.Y - 1 };
            if (top.Y >= 0 && arranged[top.Y, top.X] != null)
            {
                requirements[Position.T] = arranged[top.Y, top.X].GetBorder(Position.B);
            }
            var bottom = coordinates with { Y = coordinates.Y + 1 };
            if (bottom.Y < size && arranged[bottom.Y, bottom.X] != null)
            {
                requirements[Position.B] = arranged[bottom.Y, bottom.X].GetBorder(Position.T);
            }
            var left = coordinates with { X = coordinates.X - 1 };
            if (left.X >= 0 && arranged[left.Y, left.X] != null)
            {
                requirements[Position.L] =  arranged[left.Y, left.X].GetBorder(Position.R);
            }
            var right = coordinates with { X = coordinates.X + 1 };
            if (right.X < size && arranged[right.Y, right.X] != null)
            {
                requirements[Position.R] = arranged[right.Y, right.X].GetBorder(Position.L);
            }

            return requirements;
        }

        private record Coordinates(int X, int Y);
        
        public enum Position { T = 0, R = 1, B = 2, L = 3 } // Top, Right, Bottom, Left.
        
        private static List<Tile> FindCorners(List<Tile> tiles)
        {
            var corners = new List<Tile>();
            foreach (var tile in tiles)
            {
                var matchingBorders = tile.AllBorderCombinations.Count(
                    tb => !tiles.Any(t => t.AllBorderCombinations.Contains(tb) && tile.TileId != t.TileId));
                if (matchingBorders == 4)
                {
                    corners.Add(tile);
                }
            }
            return corners;
        }

        private static TileOrientation FindOrientationForTopLeftCorner(Tile corner, List<Tile> allTiles)
        {
            var uniqueBorders =  corner.AllBorderCombinations.Where(
                tb => !allTiles.Any(t => t.AllBorderCombinations.Contains(tb) && corner.TileId != t.TileId)).ToList();
            foreach (var orientation in TileOrientation.AllOrientations())
            {
                corner.ApplyBorderOrientation(orientation);
                if (uniqueBorders.Contains(corner.GetBorder(Position.L)) && 
                    uniqueBorders.Contains(corner.GetBorder(Position.T)))
                {
                    return orientation;
                }
            }
            throw new Exception($"Failed to find appropriate {nameof(TileOrientation)} for corner tile!");
        }

        private static Tile[,] DeepCopy(Tile[,] original)
        {
            var rows = original.GetLength(0);
            var cols = original.GetLength(1);
            var copy = new Tile[rows, cols];
            for (var i = 0; i < rows; i++)
            {
                for (var j = 0; j < cols; j++)
                {
                    var originalTile = original[i, j];
                    if (originalTile == null)
                    {
                        continue;
                    }

                    copy[i, j] = originalTile.Copy();
                }
            }
            return copy;
        }
        
        private static List<Tile> ParseTiles(string[] inputs)
        {
            var currentTileId = 0;
            var currentTile = new List<string>();
            var currentTiles = new List<Tile>();
            foreach (var line in inputs)
            {
                if (line.Contains("Tile"))
                {
                    var idSection = line.Split(" ")[1];
                    currentTileId = int.Parse(idSection.Substring(0, idSection.Length - 1));
                } 
                else if (line == string.Empty)
                {
                    currentTiles.Add(new Tile(currentTile, currentTileId));
                    currentTile.Clear();
                }
                else
                {
                    currentTile.Add(line);
                }
            }
            return currentTiles;
        }

        public readonly struct TileBorder : IEquatable<TileBorder>
        {
            private readonly char[] _raw;
            public readonly int Original;
            public readonly int Flipped;
            
            public TileBorder(char[] elems)
            {
                _raw = elems;
                Original = ConvertFromTileElems(elems);
                Flipped = ConvertFromTileElems(elems.Reverse().ToArray());
            }

            private TileBorder(char[] elems, int original, int flipped)
            {
                _raw = elems;
                Original = original;
                Flipped = flipped;
            }

            public TileBorder Reverse()
            {
                return new TileBorder(_raw.Reverse().ToArray());
            }

            public override bool Equals(object? obj)
            {
                return obj is TileBorder other && Equals(other);
            }

            private static int ConvertFromTileElems(char[] elems)
            {
                return Convert.ToInt32(string.Concat(elems.Take(32).Select(i => i == '#' ? 1 : 0)), 2);
            }

            public bool Equals(TileBorder other)
            {
                return Original == other.Original;
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(_raw, Original, Flipped);
            }

            public override string ToString() => string.Join("", _raw);
        }

        internal struct TileOrientation
        {
            public readonly int Rotations;
            // This always means flipped horizontally. A vertical flip is equivalent to a horizontal flip + two rotations.
            public readonly bool Flipped;

            public TileOrientation(int rotation, bool flip)
            {
                Rotations = rotation;
                Flipped = flip;
            }

            public static List<TileOrientation> AllOrientations()
            {
                var orientations = new List<TileOrientation>();
                for (var rotation = 0; rotation < 4; rotation++)
                {
                    foreach (var flipHorizontally in new[] {false, true})    
                    {
                        orientations.Add(new TileOrientation(rotation, flipHorizontally)); 
                    }
                }
                return orientations;
            }
        }

        internal class Tile
        {
            public int TileId;
            public string[] Raw;

            public TileBorder[] OriginalBorders; // Always in order: T, R, B, L
            public TileBorder[] Borders;
            public TileBorder[] AllBorderCombinations;

            public TileOrientation Orientation { get; private set; }
            public int Size => Raw.Length;

            internal Tile(List<string> rows, int tileId, TileOrientation orientation = new())
            {
                TileId = tileId;
                Raw = rows.ToArray();
                Orientation = orientation;
                
                var originalLeft = new TileBorder(rows.Select(r => r[0]).ToArray());
                var originalRight = new TileBorder(rows.Select(r => r[rows.Count - 1]).ToArray());
                var originalTop = new TileBorder(rows[0].ToCharArray());
                var originalBottom = new TileBorder(rows[^1].ToCharArray());
                
                OriginalBorders = new[] {originalTop, originalRight, originalBottom, originalLeft};
                AllBorderCombinations = new[] { 
                    originalTop, originalTop.Reverse(),originalRight, originalRight.Reverse(),
                    originalBottom, originalBottom.Reverse(), originalLeft, originalLeft.Reverse()  };
                Borders = new[] {originalTop, originalRight, originalBottom, originalLeft};;
                ApplyBorderOrientation(Orientation);
            }
            
            public void ApplyBorderOrientation(TileOrientation orientation)
            {
                for (var sourceIdx = 0; sourceIdx < 4; sourceIdx++)
                {
                    var targetIdxAfterFlip = orientation.Flipped && sourceIdx % 2 == 0 ? (sourceIdx + 2) % 4 : sourceIdx;
                    var reversalDueToFlip = orientation.Flipped && sourceIdx is 1 or 3;
                    var targetIdx = (targetIdxAfterFlip + orientation.Rotations) % 4;
                    var reversalDueToRotation = 
                        (targetIdxAfterFlip is 0 or 1 && targetIdx is 2 or 3) || 
                        (targetIdxAfterFlip is 2 or 3 && targetIdx is 0 or 1);

                    var border = OriginalBorders[sourceIdx];
                    if (reversalDueToFlip)
                    {
                        border = border.Reverse();
                    }
                    if (reversalDueToRotation)
                    {
                        border = border.Reverse();
                    }
                    Borders[targetIdx] = border;
                }
                Orientation = orientation;
            }
            
            public TileBorder GetBorder(Position pos)
            {
                return Borders[(int) pos];
            }

            public override string ToString()
            {
                return TileId.ToString();
            }

            internal Tile Copy()
            {
                return new Tile(Raw.ToList(), TileId, Orientation);
            }

            internal string[] ApplyOrientation(TileOrientation orientation)
            {
                var size = Raw.Length;
                var result = new char[size, size];

                for (int y = 0; y < size; y++)
                {
                    for (int x = 0; x < size; x++)
                    {
                        result[y, x] = Raw[y][x];
                    }
                }

                if (orientation.Flipped)
                {
                    result = HorizontalFlip(result);
                }
                for (var i = 0; i < orientation.Rotations; i++)
                {
                    result = Rotate(result);
                }
                
                var converted = new string[size];
                for (var y = 0; y < size; y++)
                {
                    var rowChars = new char[size];
                    for (var x = 0; x < size; x++)
                    {
                        rowChars[x] = result[y, x];
                    }
                    converted[y] = new string(rowChars);
                }
                
                return converted;
            }

            private static char[,] Rotate(char[,] input)
            {
                var size = input.GetLength(0);
                var result = new char[size, size]; 
                for (var x = 0; x < size; x++)
                {
                    for (var y = 0; y < size; y++)
                    {
                        result[y, x] = input[size - x - 1, y];
                    }
                }
                return result;
            }

            private static char[,] HorizontalFlip(char[,] input)
            {
                var size = input.GetLength(0);
                var result = new char[size, size]; 
                for (var x = 0; x < size; x++)
                {
                    for (var y = 0; y < size; y++)
                    {
                        result[y, x] = input[size - y - 1, x];
                    }
                }
                return result;
            }
        }
    }
}