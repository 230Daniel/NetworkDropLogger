namespace NetworkDropLogger.Configuration
{
    public class DetectorConfiguration
    {
        public string PingAddress { get; set; } = "www.google.com";
        public int DelayBetweenPings { get; set; } = 5000;
        public int PingTimeout { get; set; } = 1000;
    }
}
