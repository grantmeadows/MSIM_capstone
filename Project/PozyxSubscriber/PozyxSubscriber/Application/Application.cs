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
            SimEnvironment sim = SimEnvironment.Instance;
            //var host = "10.0.0.254";
            //var port = 1883;
            //int numTags = 1;
            //sim.Initialize(host, port, "log.txt", tagRefreshRate);


            sim.Initialize("rotation.txt", tagRefreshRate);

            Tag T1 = sim.newTag("5772", 15);
            Tag T2 = sim.newTag("7012", 15);
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
                    Console.Write("Orientation: ");
                    Console.Write((int)o.z);
                Console.WriteLine();

                Thread.Sleep(1000);
            }
        }
    }
}
