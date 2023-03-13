using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PozyxSubscriber;
using PozyxSubscriber.Framework;

namespace MyFirstGame
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            SimEnvironment sim = SimEnvironment.Instance;

            var host = "10.0.0.254";
            var port = 1883;
            int numTags = 1;

            int tagRefreshRate = 24;

            //Comment out for real ltime tracking
            //sim.Initialize(host, port, 1, numTags);

            sim.Initialize("March2nd5.txt", tagRefreshRate);

            //id of tag to track
            string TAGID = "5772";
            sim.newTag(TAGID, tagRefreshRate);
            using (var game = new Game1(sim, TAGID))
                game.Run();
        }
    }
}
