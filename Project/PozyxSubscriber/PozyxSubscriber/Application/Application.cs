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
            var host = "10.0.0.254";
            var port = 1883;
            int numTags = 1;

            int tagRefreshRate = 15;

            SimEnvironment sim = SimEnvironment.Instance;


            //sim.Initialize(host, port, 1, numTags);
            sim.Initialize("rotation.txt", tagRefreshRate);
            //string TAGID = "5772";
            //sim.newTag(TAGID, tagRefreshRate);
            //sim.Initialize(host, port, 1, numTags, "rotation1.txt", tagRefreshRate);
            //sim.Initialize("log.txt");



            Tag T1 = sim.newTag("5772", 15);
            Tag T2 = sim.newTag("7012", 15);

            SimObject S = new SimObject("Cat");
            S.AddTag(T1);
            S.AddTag(T2);

            sim.StartEnvironment();
            S.Calibrate();
            Console.WriteLine(S.Orientation.z);
            while (sim.ConnectedStatus)
            {
                Vector3D pos = S.Position;
                Vector3D o  = S.Orientation;
                    Console.Write("[ID: ");
                    Console.Write(S.ID);
                    Console.Write(": X: ");
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
