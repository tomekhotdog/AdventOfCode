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
            // var productOfCornerTiles = Part1(tiles);
            // Console.WriteLine($"Part 1: {productOfCornerTiles}");
            var arrangement = Arrange(tiles, null);
            Console.WriteLine($"Part 2:");
        }

        private static long Part1(Tile[] tiles)
        {
            // Corner tiles have (at least) 2 matching borders.
            var tilesWithNMatchingBorders = AnalyseTiles(tiles);
            var cornerTiles = tilesWithNMatchingBorders[2];
            Console.WriteLine($"Corner tiles: {string.Join(",", cornerTiles)}");
            return cornerTiles.Select(t => t.TileId).Aggregate(1L, (acc, val) => acc * val);
        }

        private static Tile[,] Arrange(Tile[] tiles, ArrangedBorders? outerLayer)
        {
            var dimensions = (int) Math.Sqrt(tiles.Length);
            if (Math.Pow(dimensions, 2) - tiles.Length > 1e-6) throw new Exception($"Tile arrangement not square! (#{tiles.Length} tiles)");
            var arrangement = new Tile[dimensions,dimensions];

            if (tiles.Length == 1)
            {
                var t = ArrangeBorders(tiles.ToList(), tiles.ToList(), outerLayer);
                arrangement[0, 0] = OrientFinalCentreTile(tiles.First(), outerLayer);
                return arrangement;
            }
            
            var tilesWithNMatchingBorders = AnalyseTiles(tiles);
            var cornerTiles = tilesWithNMatchingBorders[2];
            var edgeTiles = tilesWithNMatchingBorders[3];
            var centreTiles = tilesWithNMatchingBorders[4];
            
            var arrangedBorders = ArrangeBorders(cornerTiles, edgeTiles, outerLayer);
            arrangement = OrientTilesInBorders(arrangedBorders, outerLayer, arrangement);
            var arrangedCentreTiles = Arrange(centreTiles.ToArray(), arrangedBorders);
            // Insert arranged centre tiles to arrangement.
            for (var i = 0; i < arrangedCentreTiles.Length; i++)
            {
                for (var j = 0; j < arrangedCentreTiles.Length; j++)
                {
                    arrangement[i+1,j+1] = arrangedCentreTiles[i, j];
                }
            }

            return arrangement;
        }

        // Extracts the borders: top, right, bottom, left.
        public static ArrangedBorders ArrangeBorders(List<Tile> cornerTiles, List<Tile> edgeTiles, ArrangedBorders? outerLayer)
        {
            Tile firstTile;
            if (!outerLayer.HasValue)
            {
                // Randomly select any corner tile to set (no constraints).
                firstTile = cornerTiles.First();
            }
            else
            {
                List<IConstraint> constraints = new List<IConstraint>
                {
                    new Constraint(Position.L, new[] {outerLayer.Value.Left[1].GetBorder(Opposite(Position.L))}),
                    new Constraint(Position.T, new[] {outerLayer.Value.Top[1].GetBorder(Opposite(Position.T))})
                };
                firstTile = cornerTiles.First(t => t.OrientToSatisfy(constraints));
            }
             
            var tiles = new List<Tile>(edgeTiles);
            tiles.AddRange(cornerTiles.Except(new List<Tile>{firstTile}));

            var setTiles = new List<Tile>{firstTile};
            var currentTile = firstTile;
            while (tiles.Any())
            {
                var current = currentTile;
                var possibleNextTiles = tiles.Where(t => t.BordersWithAdjacents.Any(
                    candidate => current.AllBorderCombinations.Contains(candidate))).ToList();
                // if (possibleNextTiles.Count > 1) throw new Exception("cry.");
                var matchingTile = possibleNextTiles.First();
                setTiles.Add(matchingTile);
                tiles.Remove(matchingTile);
                currentTile = matchingTile;
            }
            
            var dimSize = (setTiles.Count / 4) + 1;
            setTiles.Add(firstTile); // The final corner is also the first corner - so add it here.
            var borders = new List<Tile[]>();
            for (var i = 0; i < 4; i++)
            {
                borders.Add(setTiles.Skip(i * (dimSize - 1)).Take(dimSize).ToArray());
            }

            // Border tiles are left->right, top->bottom (hence bottom and left row are reversed here).
            //N.B. tiles are in the correct position, but not oriented correctly yet.
            return new ArrangedBorders(borders[0], borders[1], borders[2].Reverse().ToArray(), borders[3].Reverse().ToArray());
        }

        public interface IConstraint
        {
            bool Satisfies(Tile tile);
        }

        public struct Constraint : IConstraint
        {
            private readonly Position _position;
            private readonly TileBorder[] _mustBeIn;

            public Constraint(Position position, TileBorder[] mustBeIn)
            {
                _position = position;
                _mustBeIn = mustBeIn;
            }
            
            public bool Satisfies(Tile tile)
            {
                return  _mustBeIn.Contains(tile.GetBorder(_position));
            }

            public override string ToString()
            {
                return $"{_position}: {string.Join(" | ", _mustBeIn)}";
            }
        }

        public static Tile OrientFinalCentreTile(Tile tile, ArrangedBorders? outerLayer)
        {
            List<IConstraint> constraints = new List<IConstraint>();

            if (!outerLayer.HasValue) throw new Exception("Must have an outer layer (unless only one tile in input).");
            constraints.Add(new Constraint(Position.T, new []{outerLayer.Value.Top[1].GetBorder(Opposite(Position.T))}));
            constraints.Add(new Constraint(Position.R, new []{outerLayer.Value.Right[1].GetBorder(Opposite(Position.R))}));
            constraints.Add(new Constraint(Position.B, new []{outerLayer.Value.Bottom[1].GetBorder(Opposite(Position.B))}));
            constraints.Add(new Constraint(Position.L, new []{outerLayer.Value.Left[1].GetBorder(Opposite(Position.L))}));
            if (!tile.OrientToSatisfy(constraints)) throw new Exception("Cannot orient tile to match constraints :(");
            return tile;
        }

        public static Tile[,] OrientTilesInBorders(ArrangedBorders borders, ArrangedBorders? outerLayer, Tile[,] arrangement)
        {
            List<IConstraint> constraints = new List<IConstraint>();
            var borderLength = borders.Top.Length;

            /* Top border */
            for (var i = 0; i < borderLength; i++)
            {
                constraints.Clear();
                var tile = borders.Top[i];
                // First tile is a corner. 
                if (i == 0)
                {
                    if (outerLayer.HasValue)
                    {
                        constraints.Add(new Constraint(Position.L, new []{outerLayer.Value.Left[i+1].GetBorder(Opposite(Position.L))}));
                        constraints.Add(new Constraint(Position.T, new []{outerLayer.Value.Top[1].GetBorder(Opposite(Position.T))}));
                    }
                    constraints.Add(new Constraint(Position.B, borders.Left[1].AllBorderCombinations ));
                    constraints.Add(new Constraint(Position.R, borders.Top[1].AllBorderCombinations));
                    if (!tile.OrientToSatisfy(constraints)) throw new Exception("Cannot orient tile to match constraints :(");
                    arrangement[i, 0] = tile;
                } 
                else if (i == borderLength - 1)
                {
                    // Final tile is oriented by next border orientation.
                }
                // Edge tile.
                else
                {
                    if (outerLayer.HasValue)
                    {
                        constraints.Add(new Constraint(Position.T, new []{outerLayer.Value.Top[i+1].GetBorder(Opposite(Position.T))}));
                    }
                    constraints.Add(new Constraint(Position.L, new [] {arrangement[i-1,0].GetBorder(Opposite(Position.L))}));
                    constraints.Add(new Constraint(Position.R, borders.Top[i+1].AllBorderCombinations));
                    if (!tile.OrientToSatisfy(constraints)) throw new Exception("Cannot orient tile to match constraints :(");
                    arrangement[i, 0] = tile;
                }
            }

            /* Right border */
            for (var i = 0; i < borderLength; i++)
            {
                constraints.Clear();
                var tile = borders.Right[i];
                // First tile is a corner. 
                if (i == 0)
                {
                    if (outerLayer.HasValue)
                    {
                        constraints.Add(new Constraint(Position.T, new []{outerLayer.Value.Top[i+1].GetBorder(Opposite(Position.T))}));
                        constraints.Add(new Constraint(Position.R, new []{outerLayer.Value.Right[1].GetBorder(Opposite(Position.R))}));
                    }
                    constraints.Add(new Constraint(Position.B, borders.Right[i+1].AllBorderCombinations));
                    constraints.Add(new Constraint(Position.L, new [] {arrangement[borderLength-2,i].GetBorder(Opposite(Position.L))}));

                    if (!tile.OrientToSatisfy(constraints)) throw new Exception("Cannot orient tile to match constraints :(");
                    arrangement[borderLength-1, i] = tile;
                } 
                else if (i == borderLength - 1)
                {
                    // Final tile is oriented by next border orientation.
                }
                // Edge tile.
                else
                {
                    if (outerLayer.HasValue)
                    {
                        constraints.Add(new Constraint(Position.R, new []{outerLayer.Value.Right[i+1].GetBorder(Opposite(Position.R))}));
                    }
                    constraints.Add(new Constraint(Position.T, new [] {arrangement[borderLength-1,i-1].GetBorder(Opposite(Position.T))}));
                    constraints.Add(new Constraint(Position.B, borders.Right[i+1].AllBorderCombinations));
                    if (!tile.OrientToSatisfy(constraints)) throw new Exception("Cannot orient tile to match constraints :(");
                    arrangement[borderLength-1, i] = tile;
                }
            }
            
            /* Bottom border */
            for (var i = borderLength-1; i >= 0; i--)
            {
                constraints.Clear();
                var tile = borders.Bottom[i];
                if (i == 0)
                {
                    // Final tile is oriented by next border orientation.
                } 
                // Last tile is also a corner.
                else if (i == borderLength - 1)
                {
                    if (outerLayer.HasValue)
                    {
                        constraints.Add(new Constraint(Position.R, new []{outerLayer.Value.Right[i+1].GetBorder(Opposite(Position.R))}));
                        constraints.Add(new Constraint(Position.B, new []{outerLayer.Value.Bottom[i+1].GetBorder(Opposite(Position.B))}));
                    }
                    constraints.Add(new Constraint(Position.L, borders.Bottom[i-1].AllBorderCombinations));
                    constraints.Add(new Constraint(Position.T, new [] {arrangement[i,borderLength-2].GetBorder(Opposite(Position.T))}));
                    if (!tile.OrientToSatisfy(constraints)) throw new Exception("Cannot orient tile to match constraints :(");
                    arrangement[i, borderLength-1] = tile;
                }
                // Edge tile.
                else
                {
                    if (outerLayer.HasValue)
                    {
                        constraints.Add(new Constraint(Position.B, new []{outerLayer.Value.Bottom[i+1].GetBorder(Opposite(Position.B))}));
                    }
                    constraints.Add(new Constraint(Position.L, borders.Bottom[i-1].AllBorderCombinations));
                    constraints.Add(new Constraint(Position.R, new [] {arrangement[i+1,borderLength-1].GetBorder(Opposite(Position.R))}));
                    if (!tile.OrientToSatisfy(constraints)) throw new Exception("Cannot orient tile to match constraints :(");
                    arrangement[i, borderLength-1] = tile;
                }
            }
            
            /* Left border */
            for (var i = borderLength-1; i >= 0; i--)
            {
                constraints.Clear();
                var tile = borders.Left[i];
                if (i == 0)
                {
                    // Final tile is oriented by next border orientation.
                } 
                // Bottom-left corner.
                else if (i == borderLength - 1)
                {
                    if (outerLayer.HasValue)
                    {
                        constraints.Add(new Constraint(Position.L, new []{outerLayer.Value.Left[i+1].GetBorder(Opposite(Position.L))}));
                        constraints.Add(new Constraint(Position.B, new []{outerLayer.Value.Bottom[i+1].GetBorder(Opposite(Position.B))}));
                    }
                    constraints.Add(new Constraint(Position.T, borders.Left[i-1].AllBorderCombinations));
                    constraints.Add(new Constraint(Position.R, new [] {arrangement[1,borderLength-1].GetBorder(Opposite(Position.R))}));
                    if (!tile.OrientToSatisfy(constraints)) throw new Exception("Cannot orient tile to match constraints :(");
                    arrangement[0, i] = tile;
                }
                // Edge tile.
                else
                {
                    if (outerLayer.HasValue)
                    {
                        constraints.Add(new Constraint(Position.L, new []{outerLayer.Value.Left[i+1].GetBorder(Opposite(Position.L))}));
                    }
                    constraints.Add(new Constraint(Position.T, borders.Left[i-1].AllBorderCombinations));
                    constraints.Add(new Constraint(Position.B, new [] {arrangement[0,i+1].GetBorder(Opposite(Position.B))}));
                    if (!tile.OrientToSatisfy(constraints)) throw new Exception("Cannot orient tile to match constraints :(");
                    arrangement[0, i] = tile;
                }
            }

            return arrangement;
        }

        public static Position Opposite(Position pos)
        {
            switch (pos)
            {
                case Position.B:
                    return Position.T;
                case Position.T:
                    return Position.B;
                case Position.L:
                    return Position.R;
                case Position.R:
                    return Position.L;
                default:
                    throw new Exception($"Unknown: {pos}");
            }
            
        }
        public enum Position { T = 0, R = 1, B = 2, L = 3 } // Top, Right, Bottom, Left.

        public struct ArrangedBorders
        {
            // Tiles are always left->right, top->bottom (for each orientation).
            public Tile[] Left;
            public Tile[] Right;
            public Tile[] Top;
            public Tile[] Bottom;

            public ArrangedBorders(Tile[] t, Tile[] r, Tile[] b, Tile[] l)
            {
                Left = l;
                Right = r;
                Top = t;
                Bottom = b;
            }
        }

        public static Dictionary<int, List<Tile>> AnalyseTiles(Tile[] tiles)
        {
            var tilesWithNMatchingBorders = new Dictionary<int, List<Tile>>();
            foreach (var tile in tiles.ToArray())
            {
                var matchingBorders = tile.AllBorderCombinations.Where(
                b => tiles.Any(t => t.AllBorderCombinations.Contains(b) && tile.TileId != t.TileId)).ToArray();
                tile.SetAdjacents(matchingBorders.ToArray());
                var nMatchingBorders = matchingBorders.Length / 2;
                
                if (!tilesWithNMatchingBorders.ContainsKey(nMatchingBorders)) {tilesWithNMatchingBorders[nMatchingBorders] = new List<Tile>();}
                tilesWithNMatchingBorders[nMatchingBorders].Add(tile);
            }
            return tilesWithNMatchingBorders;
        }

        private static Tile[] ParseTiles(string[] inputs)
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
            return currentTiles.ToArray();
        }

        public struct TileBorder : IEquatable<TileBorder>
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

            public TileBorder Flip()
            {
                return new TileBorder(_raw.Reverse().ToArray());
            }

            public override bool Equals(object? obj)
            {
                return obj is TileBorder other && Equals(other);
            }

            private static int ConvertFromTileElems(char[] elems)
            {
                return Convert.ToInt32(string.Concat(elems.Select(i => i == '#' ? 1 : 0)), 2);
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

        public struct TileOrientation
        {
            public int ClockwiseRotation;
            public bool HorizontalFlipped;
            
            public void Rotate(int times) { ClockwiseRotation = (ClockwiseRotation + times) % 4; }
            public void HorizontalFlip() { HorizontalFlipped = false; }

            public TileOrientation(int rotation, bool horizontalFlip)
            {
                ClockwiseRotation = rotation;
                HorizontalFlipped = horizontalFlip;
            }

            public static List<TileOrientation> GetPossibleOrientations()
            {
                var orientations = new List<TileOrientation>();
                foreach (var flipHorizontally in new[] {false, true})
                {
                    for (var rotation = 0; rotation < 4; rotation++)
                    {
                        orientations.Add(new TileOrientation(rotation, flipHorizontally)); 
                    }
                }
                return orientations;
            }
        }

        public class Tile
        {
            public int TileId;
            public string[] Raw;

            private TileBorder _originalTop;
            private TileBorder _originalRight;
            private TileBorder _originalBottom;
            private TileBorder _originalLeft;

            public TileBorder[] OriginalBorders; // Always in order: T, R, B, L
            public TileBorder[] Borders;
            public TileBorder[] AllBorderCombinations;
            public TileBorder[] BordersWithAdjacents;

            public TileOrientation Orientation;


            public Tile(List<string> rows, int tileId)
            {
                TileId = tileId;
                Raw = rows.ToArray();
                
                _originalLeft   = new TileBorder(rows.Select(r => r[0]).ToArray());
                _originalRight  = new TileBorder(rows.Select(r => r[rows.Count - 1]).ToArray());
                _originalTop    = new TileBorder(rows[0].ToCharArray());
                _originalBottom = new TileBorder(rows[^1].ToCharArray());
                
                Orientation = new TileOrientation();
                OriginalBorders = new[] {_originalTop, _originalRight, _originalBottom, _originalLeft};
                AllBorderCombinations = new[] { 
                    _originalTop, _originalTop.Flip(),_originalRight, _originalRight.Flip(),
                    _originalBottom, _originalBottom.Flip(), _originalLeft, _originalLeft.Flip()  };
                Borders = OriginalBorders;
            }

            public void SetAdjacents(TileBorder[] bordersWithAdjacents)
            {
                BordersWithAdjacents = bordersWithAdjacents;
            }

            public bool OrientToSatisfy(IList<IConstraint> constraints)
            {
                foreach (var orientation in TileOrientation.GetPossibleOrientations())
                {
                    Orient(orientation);
                    if (constraints.All(c => c.Satisfies(this)))
                    {
                        return true;
                    }
                }
                return false;
            }

            public void Orient(TileOrientation orientation)
            {
                var oriented = new TileBorder[4];
                // Rotation.
                for (var borderPos = 0; borderPos < 4; borderPos++)
                {
                    bool flip;
                    switch (orientation.ClockwiseRotation % 4)
                    {
                        case 0:
                            flip = false;
                            break;
                        case 1:
                            flip = borderPos == 1 || borderPos == 3;
                            break;
                        case 2:
                            flip = true;
                            break;
                        case 3:
                            flip = borderPos == 0 || borderPos == 2;
                            break;
                        default:
                            throw new Exception("Not possible.");
                    }

                    var flippedOrNot = flip ? OriginalBorders[borderPos].Flip() : OriginalBorders[borderPos];
                    oriented[(borderPos + orientation.ClockwiseRotation) % 4] = flippedOrNot;
                }
                // Horizontal flip.
                if (orientation.HorizontalFlipped)
                {
                    var tempA = oriented[0];
                    oriented[0] = oriented[2];
                    oriented[2] = tempA;
                    oriented[1] = oriented[1].Flip();
                    oriented[3] = oriented[3].Flip();
                }
                Orientation = orientation;
                Borders = oriented;
            }

            public TileBorder GetBorder(Position pos)
            {
                return Borders[(int) pos];
            }

            public override string ToString()
            {
                return TileId.ToString();
            }
        }
        
    }
}