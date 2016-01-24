
namespace G
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (NinjitsuGame game = new NinjitsuGame())
            {
                game.Run();
            }
        }
    }
}

