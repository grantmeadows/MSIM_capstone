using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PozyxSubscriber.Framework;

namespace PozyxSubscriber.Application
{
    public class Program
    {
        static public void Main(string[] args)
        {
            int tagRefreshRate = 15;

            string tag1 = "5772";
            string tag2 = "7012";

            SimEnvironment sim = SimEnvironment.Instance;
            //var host = "10.0.0.254";
            //var port = 1883;
            //int numTags = 1;
            //sim.Initialize(host, port, "log.txt", tagRefreshRate);


            sim.Initialize("rotation.txt", tagRefreshRate);

            Tag T1 = sim.newTag(tag1, tagRefreshRate);
            Tag T2 = sim.newTag(tag2, tagRefreshRate);
            SimObject S = new SimObject();

            S.AddTag(T1);
            S.AddTag(T2);
            sim.StartEnvironment();

            while (!sim.ConnectedStatus) ;
            S.Calibrate(sim);

            while (sim.ConnectedStatus)
            {
                PozyxVector pos = S.Position;
                PozyxVector o  = S.Orientation;
                    Console.Write("Cat: [");
                    Console.Write(" X: ");
                    Console.Write((int)pos.x);
                    Console.Write("  Y: ");
                    Console.Write((int)pos.y);
                    Console.Write("  Z: ");
                    Console.Write((int)pos.z);
                    Console.Write("]      ");
                    Console.Write("Orientation: [ X: ");
                    Console.Write((int)(o.x * (180/Math.PI)));
                    Console.Write("  Y: ");
                    Console.Write((int)(o.y * (180 / Math.PI)));
                    Console.Write("  Z: ");
                    Console.Write((int)(o.z * (180 / Math.PI)));
                    Console.Write("]");
                Console.WriteLine();

                Thread.Sleep(1000);
            }
        }
    }
}
