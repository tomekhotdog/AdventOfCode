using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode2020
{
    public static class Q23
    {
        public static void Solve()
        {
            // var input = "389125467";
            var input = "247819356";
            
            var cups = input.Select(i => new Cup(int.Parse(i.ToString()))).ToList();
            cups = PadUpToNCup(cups, 1000000);
            PlayGame(cups, 10000000);
        }

        private static List<Cup> PadUpToNCup(List<Cup> original, int n)
        {
            var paddedList = new List<Cup>(original);
            var max = original.Max(c => c.Label);
            for (var i = max + 1; i <= n; i++)
            {
                paddedList.Add(new Cup(i));
            }
            return paddedList;
        }

        private static void PlayGame(List<Cup> startingCups, int moves, bool verbose = false)
        {
            var game = new CrabCubGame(startingCups);
            for (var i  = 1; i <= moves; i++)
            {
                if (i % 1_000_000 == 0) Console.WriteLine($"Game {i / 1_000_000} million...");
                if (verbose) Console.WriteLine($"Round #{i}: {game}");
                game.Move();
            }

            if (verbose) Console.WriteLine($"Final labelling: {game.LabellingAfter1()}");
            Console.WriteLine($"Stars location: {game.FindTheStars()}");
        }

        public class CrabCubGame
        {
            private readonly Dictionary<int, Cup> _labelToCup;
            private readonly CircularLinkedList<Cup> _cups;
            private readonly Cup _maxLabelCup;
            private readonly Cup _minLabelCup;

            public CrabCubGame(List<Cup> cups)
            {
                _cups = new CircularLinkedList<Cup>(cups);
                _labelToCup = cups.ToDictionary(c => c.Label, c => c);
                _maxLabelCup = cups.Aggregate((c1,c2) => c1.Label > c2.Label ? c1 : c2);
                _minLabelCup = cups.Aggregate((c1,c2) => c1.Label > c2.Label ? c2 : c1);
            }

            public void Move()
            {
                // Remove 3 cups.
                var currentCup = _cups.CurrentHead;
                var removedCups = _cups.RemoveAfter(_labelToCup[currentCup.Value.Label], 3);
                var removedCupLabels = removedCups.Select(c => c.Label).ToList();

                // Find destination cup.
                var destinationCupLabel = GetNextDestination(currentCup.Value);
                while (removedCupLabels.Contains(destinationCupLabel.Label))
                {
                    destinationCupLabel = GetNextDestination(destinationCupLabel);
                }

                _cups.InsertAfter(destinationCupLabel, removedCups);
                _cups.MoveHead();
            }

            private Cup GetNextDestination(Cup currentCup)
            {
                return currentCup.Label == _minLabelCup.Label ? _maxLabelCup : _labelToCup[currentCup.Label - 1];
            }

            public string LabellingAfter1()
            {
                var cups = _cups.Items();
                var positionOfCup1 = cups.FindIndex(c => c.Label == 1);
                var labels = Enumerable.Range(0, cups.Count-1)
                    .Select(i => cups[(positionOfCup1 + 1 + i) % cups.Count].Label.ToString())
                    .ToList();
                return string.Join(string.Empty, labels);
            }

            public string FindTheStars()
            {
                var itemsAfterCup1 = _cups.RemoveAfter(_labelToCup[1], 2);
                var fstCup = itemsAfterCup1[0];
                var sndCup = itemsAfterCup1[1];
                return $"1st item = [{fstCup}], 2nd item = [{sndCup}]. Product = {(long) fstCup.Label * (long) sndCup.Label}";
            }

            public override string ToString()
            {
                _cups.Items();
                return string.Join(" ", _cups.Items().Select(c => c.Label.ToString()));
            }
        }

        public readonly struct Cup : ILabelled
        {
            public int Label { get; }
            public Cup(int val) { Label = val; }
            public override string ToString() => $"{Label}";
        }

        public interface ILabelled
        {
            int Label { get; }
        }
        

        public interface ICircularLinkedList<T>
        {
            List<T> RemoveAfter(T nodeLabel, int itemsToRemove);
            void InsertAfter(T nodeLabel, List<T> items);
            void MoveHead();
            List<T> Items();
            Node<T> CurrentHead { get; }
        }

        public class CircularLinkedList<T> : ICircularLinkedList<T> where T : ILabelled
        {
            private readonly Dictionary<T, Node<T>> _valueToNodes = new Dictionary<T, Node<T>>();
            public CircularLinkedList(List<T> items)
            {
                var nodes = LinkUpNodes(items);
                CurrentHead = nodes[0];
            }

            private Node<T>[] LinkUpNodes(List<T> items)
            {
                var nodes = items.Select(i => new Node<T>(i, null)).ToArray();
                for (var i = 0; i < nodes.Length; i++)
                {
                    nodes[i].Next = nodes[(i + 1) % nodes.Length];
                    _valueToNodes[nodes[i].Value] = nodes[i];
                }
                return nodes;
            }

            public List<T> Items()
            {
                var current = CurrentHead;
                var loopedAround = false;
                var visitedFirst = false;
                var items = new List<T>();
                while (!(visitedFirst &&loopedAround))
                {
                    items.Add(current.Value);
                    current = current.Next;
                    loopedAround = current.Value.Equals(CurrentHead.Value);
                    visitedFirst = true;
                }
                return items;
            }

            public List<T> RemoveAfter(T label, int itemsToRemove)
            {
                var startingNode = _valueToNodes[label];
                var removedItems = new List<T>();
                var next = startingNode.Next;
                for (var i = 0; i < itemsToRemove; i++)
                {
                    removedItems.Add(next.Value);
                    next = next.Next;
                }

                startingNode.Next = next;
                return removedItems;
            }

            public void InsertAfter(T label, List<T> items)
            {
                var startingNode = _valueToNodes[label];
                var nodesToInsert = LinkUpNodes(items);
                var end = startingNode.Next;
                
                startingNode.Next = nodesToInsert[0];
                nodesToInsert[^1].Next = end;
            }

            public Node<T> CurrentHead { get; private set; }
            public void MoveHead()
            {
                CurrentHead = CurrentHead.Next;
            }
        }

        public class Node<T>
        {
            public readonly T Value;
            public Node<T> Next;
            
            public Node(T value, Node<T> next)
            {
                Value = value;
                Next = next;
            }
        }
    }
}