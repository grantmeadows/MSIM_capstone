using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PozyxPositioner;
using PozyxPositioner.Framework;

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
            int tagRefreshRate = 15;

            //Comment out for real ltime tracking
            sim.Initialize(host, port, "Demonstration.txt", tagRefreshRate);

            //sim.Initialize("rotation1.txt", tagRefreshRate);

            //id of tag to track
            string[] TAGID = new string[2];
            TAGID[0] = "6985";
            TAGID[1] = "6982";
            sim.newTag(TAGID[0], tagRefreshRate);
            sim.newTag(TAGID[1], tagRefreshRate);

            sim.StartEnvironment();


            using (var game = new Game1(sim, TAGID))
                game.Run();
        }
    }
}
