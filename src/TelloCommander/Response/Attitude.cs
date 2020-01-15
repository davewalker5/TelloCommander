﻿namespace TelloCommander.Response
{
    public class Attitude
    {
        public decimal Pitch { get; set; }
        public decimal Roll { get; set; }
        public decimal Yaw { get; set; }

        public override string ToString()
        {
            return $"Pitch: {Pitch} Roll: {Roll} Yaw: {Yaw}";
        }

        public string ToCsv()
        {
            return $"\"{Pitch}\",\"{Roll}\",\"{Yaw}\"";
        }
    }
}
