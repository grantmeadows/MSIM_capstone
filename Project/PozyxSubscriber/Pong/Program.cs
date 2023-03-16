using PozyxSubscriber;
using System;

namespace Ping_Pong
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (Game1 game = new Game1())
            {
                InitializeSimEnviornment();
                game.Run();
            }
        }

        static private void InitializeSimEnviornment()
        {
            var host = "10.0.0.254";
            var port = 1883;
            int numTags = 1;
            string tag1 = "";
            string tag2 = "";

            int tagRefreshRate = 5;

            SimEnvironment sim = SimEnvironment.Instance;

            sim.Initialize(host, port, 1, 2, "Pong.txt", tagRefreshRate);
            sim.newTag(tag1, 15);
            sim.newTag(tag2, 15);
        }
    }
}

