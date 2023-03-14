using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PozyxSubscriber.Application
{
    public class Program
    {
        static public void Main(string[] args)
        {
            var host = "10.0.0.254";
            var port = 1883;
            int numTags = 1;

            int tagRefreshRate = 15;

            SimEnvironment sim = SimEnvironment.Instance;


            //sim.Initialize(host, port, 1, numTags);
            sim.Initialize("rotation1.txt", tagRefreshRate);
            //string TAGID = "5772";
            //sim.newTag(TAGID, tagRefreshRate);
            //sim.Initialize(host, port, 1, numTags, "rotation1.txt", tagRefreshRate);
            //sim.Initialize("log.txt");


            while (sim.ConnectedStatus)
            {
                Console.WriteLine($"Tag 0 X position: {sim.getLatestposition("0").pos.x}");
                Console.WriteLine($"Tag 0 X position: {sim.getLatestposition("0").pos.y}");
                Console.WriteLine($"Tag 0 X position: {sim.getLatestposition("0").pos.z}");
            }
        }


    }
}
