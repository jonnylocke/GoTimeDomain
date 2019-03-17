namespace GoTime.Models
{
    public class Position
    {
        private Position() { }

        public static Position New()
        {
            return new Position();
        }

        public string X { get; internal set; }
        public int Y { get; internal set; }
    }
}
    