using TelloCommander.CommandLine;

namespace TelloCommander
{
    public class Program
    {
        public static void Main(string[] args)
        {
            new ConsoleCommanderWrapper().Run();
        }
    }
}
