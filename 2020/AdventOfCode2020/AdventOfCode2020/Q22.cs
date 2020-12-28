using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode2020
{
    public static class Q22
    {
        public static void Solve()
        {
            var inputs = Tools.GetInput(22, false);
            var decks = ParseStartingDeck(inputs);
            
            // Part 1.
            Console.WriteLine(PlayGame(new Deck(decks.p1Cards), new Deck(decks.p2Cards), false, true));
            // Part 2.
            Console.WriteLine(PlayGame(new Deck(decks.p1Cards), new Deck(decks.p2Cards), true, true));
        }

        private static (List<Card> p1Cards, List<Card> p2Cards) ParseStartingDeck(string[] inputs)
        {
            var p1Cards = new List<Card>();
            var p2Cards = new List<Card>();
            var parsingP1Cards = true;

            foreach (var input in inputs)
            {
                if (input.Contains("Player"))
                {
                    if (input.Contains("1")) parsingP1Cards = true;
                    if (input.Contains("2")) parsingP1Cards = false;
                }
                else if (input == string.Empty) { }
                else
                {
                    if (parsingP1Cards)
                    {
                        p1Cards.Add(new Card(int.Parse(input)));
                    }
                    else
                    {
                        p2Cards.Add(new Card(int.Parse(input)));
                    }
                }
            }
            return (p1Cards, p2Cards);
        }

        internal static RoundResult PlayGame(Deck p1Deck, Deck p2Deck, bool recursiveGame = false, bool calculateScore = false)
        {
            var game = new CombatGame(p1Deck, p2Deck, recursiveGame);
            while (!game.GameIsOver) game.PlayRound();
            return game.GetRoundResult(calculateScore);
        }

        public class CombatGame
        {
            private readonly Deck _player1Deck;
            private readonly Deck _player2Deck;
            private HashSet<string> _roundSnapshots;
            private readonly bool _recursiveGame;
            
            private bool _gameTerminated;
            private Player? _winner;
            
            private int _roundsPlayed;

            public CombatGame(Deck player1Deck, Deck player2Deck, bool recursiveGame)
            {
                _roundSnapshots = new HashSet<string>();
                _recursiveGame = recursiveGame;
                _player1Deck = player1Deck;
                _player2Deck = player2Deck;
            }

            public bool GameIsOver => _gameTerminated || _player1Deck.NumberOfCards == 0 || _player2Deck.NumberOfCards == 0;

            public void PlayRound()
            {
                if (GameIsOver) throw new Exception("Cannot play round: game is over!");

                var result = DetermineRoundWinner(_player1Deck, _player2Deck, _recursiveGame);
                if (_gameTerminated) return;
                var winningDeck = result.winner == Player.One ? _player1Deck : _player2Deck;
                winningDeck.AddToBottomOfDeck(result.winningCard);
                winningDeck.AddToBottomOfDeck(result.losingCard);

                _roundsPlayed++;
            }

            private (Player winner, Card winningCard, Card losingCard) DetermineRoundWinner(
                Deck p1Deck, Deck p2Deck, bool recursiveGame)
            {
                if (recursiveGame)
                {
                    var snapshot = $"{p1Deck}:{p2Deck}";
                    if (_roundSnapshots.Contains(snapshot))
                    {
                        _gameTerminated = true;
                        _winner = Player.One;
                    }
                    _roundSnapshots.Add(snapshot);
                }

                var p1Card = _player1Deck.Draw();
                var p2Card = _player2Deck.Draw();

                var eligibleForRecursiveGame = 
                    _player1Deck.NumberOfCards >= p1Card.Value && _player2Deck.NumberOfCards >= p2Card.Value; 
                if (eligibleForRecursiveGame && recursiveGame)
                {
                    var p1RemainingDeck = new Deck(_player1Deck, p1Card.Value);
                    var p2RemainingDeck = new Deck(_player2Deck, p2Card.Value);
                    var subGameResult = PlayGame(p1RemainingDeck, p2RemainingDeck, _recursiveGame);
                    var winningCard = subGameResult.Winner == Player.One ? p1Card : p2Card;
                    var losingCard = subGameResult.Winner == Player.One ? p2Card : p1Card;
                    return (subGameResult.Winner, winningCard, losingCard);
                }

                return p1Card.Value > p2Card.Value ? (Player.One, p1Card, p2Card) : (Player.Two, p2Card, p1Card);
            }

            public RoundResult GetRoundResult(bool calculateScore)
            {
                if (!GameIsOver) throw new Exception("Can only get round result once game is over.");
                if (_gameTerminated && !_winner.HasValue) throw new Exception("Game terminated early but winner wasn't specified.");
                
                var winner = _gameTerminated ? _winner.Value : _player1Deck.NumberOfCards == 0 ? Player.Two : Player.One;
                var winningDeck = _player1Deck.NumberOfCards == 0 ? _player2Deck : _player1Deck;
                var score = calculateScore ? winningDeck.CalculateDeckScore() : 0;
                return new RoundResult(winner, score, _roundsPlayed);
            }
        }

        public struct RoundResult
        {
            public Player Winner;
            public int WinningScore;
            public int RoundsPlayed;
            
            public RoundResult(Player winner, int winningScore, int roundsPlayed)
            {
                Winner = winner;
                WinningScore = winningScore;
                RoundsPlayed = roundsPlayed;
            }
            public override string ToString() => $"Winner is {Winner}. Rounds played = {RoundsPlayed}. Score = {WinningScore}.";
        }

        public class Deck
        {
            internal LinkedList<Card> Cards;

            public Deck(List<Card> cards)
            {
                Cards = new LinkedList<Card>(cards);
            }

            public Deck(Deck other, int cardsToTake)
            {
                Cards = new LinkedList<Card>(other.Cards.Take(cardsToTake));
            }

            public Card Draw()
            {
                var topCard = Cards.First();
                Cards.RemoveFirst();
                return topCard;
            }

            public void AddToBottomOfDeck(Card card)
            {
                Cards.AddLast(card);
            }

            public int NumberOfCards => Cards.Count;

            public int CalculateDeckScore()
            {
                var score = 0;
                var deckArray = Cards.ToArray();
                for (var i = 1; i <= deckArray.Length; i++)
                {
                    score += i * deckArray[^i].Value;
                }
                return score;
            }

            public override string ToString()
            {
                return "[" + string.Join(",", Cards.ToArray().Select(c => c.Value)) + "]";
            }
        }
        
        public struct Card
        {
            public readonly int Value;

            public Card(int value)
            {
                Value = value;
            }
            
            public static Card None => new Card(-1);
        }
        
        public enum Player { One, Two }
    }
}