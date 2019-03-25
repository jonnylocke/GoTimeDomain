using System;
using System.Collections.Generic;

namespace GoTime.Models
{
    public class Move
    {
        private Move() { }

        public Guid PlayerId { get; internal set; }
        public int Number { get; internal set; }
        public int X { get; internal set; }
        public int Y { get; internal set; }
        public ColourSelection StoneColour { get; internal set; }

        public static Move New()
        {
            return new Move();
        }
    }

    public static class MoveExtenstions
    {
        public static void Update(this Move currentMove, Guid playerId, string xPosition, int yPosition, ColourSelection stoneColour)
        {
            currentMove.PlayerId = playerId;
            currentMove.Number += 1;
            currentMove.X = xPosition.LetterCoordinateToNumber();
            currentMove.Y = yPosition;
            currentMove.StoneColour = stoneColour;
        }

        public static int LetterCoordinateToNumber(this string letterCoordinate)
        {
            var mappings = new Dictionary<string, int>
            {
                { "a", 0 },
                { "b", 1 },
                { "c", 2 },
                { "d", 3 },
                { "e", 4 },
                { "f", 5 },
                { "g", 6 },
                { "h", 7 },
                { "i", 8 },
                { "j", 9 },
                { "k", 10 },
                { "l", 11 },
                { "m", 12 },
                { "n", 13 },
                { "o", 14 },
                { "p", 15 },
                { "q", 16 },
                { "r", 17 },
                { "s", 18 }
            };

            return mappings[letterCoordinate];
        }
    }
}
