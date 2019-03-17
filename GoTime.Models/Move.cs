using System;

namespace GoTime.Models
{
    public class Move
    {
        private Move()
        {
            Position = Position.New();
        }

        public Guid PlayerId { get; internal set; }
        public int Number { get; internal set; }
        public Position Position { get; internal set; }

        public static Move New()
        {
            return new Move();
        }
    }

    public static class MoveExtenstions
    {
        public static void Update(this Move currentMove, Guid playerId, string xPosition, int yPosition)
        {
            currentMove.PlayerId = playerId;
            currentMove.Number += 1;
            currentMove.Position.X = xPosition;
            currentMove.Position.Y = yPosition;
        }
    }
}
