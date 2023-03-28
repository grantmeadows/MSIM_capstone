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
            int tagRefreshRate = 24;

            string tag1 = "5772";
            string tag2 = "6985";

            SimEnvironment sim = SimEnvironment.Instance;

            var host = "10.0.0.254";
            var port = 1883;

           sim.Initialize(host, port, "March28_4.txt", tagRefreshRate);


            //sim.Initialize("March21(4).txt", tagRefreshRate);

            Tag T1 = sim.newTag(tag1, tagRefreshRate);
            Tag T2 = sim.newTag(tag2, tagRefreshRate);
            SimObject S = new SimObject();

            S.AddTag(T1);
            //S.AddTag(T2);
            sim.StartEnvironment();

            while (!sim.ConnectedStatus) ;

            S.Calibrate(sim, 0.0f, 0.0f, 0.0f);

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
