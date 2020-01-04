using TelloCommander.Commander;

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
