namespace TelloCommander.Response
{
    public class Temperature
    {
        public decimal Minimum { get; set; }
        public decimal Maximum { get; set; }

        public override string ToString()
        {
            return $"Minimum: {Minimum} Maximum: {Maximum}";
        }
    }
}
