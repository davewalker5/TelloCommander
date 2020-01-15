namespace TelloCommander.Response
{
    internal class Position
    {
        public decimal X { get; set; }
        public decimal Y { get; set; }
        public decimal Z { get; set; }

        public override string ToString()
        {
            return $"X: {X} Y: {Y} Z: {Z}";
        }

        public string ToCsv()
        {
            return $"\"{X}\",\"{Y}\",\"{Z}\"";
        }
    }
}
