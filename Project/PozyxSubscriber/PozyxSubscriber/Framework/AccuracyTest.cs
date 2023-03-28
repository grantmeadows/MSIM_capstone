using System;

namespace PozyxSubscriber.Framework
{
    /// <summary>
    /// Test the positional accuracy given 2 tags
    /// </summary>
    class AccuracyTest
    {
        public float experimentalDist;
        public float acceptedDist;

        private SimEnvironment _simEnv;
        private SimObject[]? _simObj;
        private Tag[] _tags;


        /// <summary>
        /// Raw data test
        /// </summary>
        /// <param name="simEnv"></param>
        /// <param name="tag1"></param>
        /// <param name="tag2"></param>
        /// <param name="trueDistance">Measured distance between two tags</param>

        public AccuracyTest(SimEnvironment simEnv, Tag tag1, Tag tag2, float trueDistance)
        {
            _simEnv = simEnv;
            _tags = new Tag[2];
            _tags[0] = tag1;
            _tags[1] = tag2;

            acceptedDist = trueDistance;

            StartTest();
        }

        /// <summary>
        /// Normalized data test
        /// </summary>
        /// <param name="simEnv"></param>
        /// <param name="simObj1">Sim object with tag 1 attatched</param>
        /// <param name="simObj2">Sim object with tag 2 attatched</param>
        /// <param name="trueDistance">Measured distance between two tags</param>
        public AccuracyTest(SimEnvironment simEnv, SimObject simObj1, SimObject simObj2, float trueDistance) 
        {
            _simEnv = simEnv;
            _simObj = new SimObject[2];
            _simObj[0] = simObj1;
            _simObj[1] = simObj2;

            _tags = new Tag[2];
            _tags[0] = simObj1.Tags[0];
            _tags[1] = simObj2.Tags[1];


            acceptedDist = trueDistance;

            StartTest();
        }

        private void StartTest()
        {
            bool cont = true;
            while(cont)
            {
                Console.WriteLine("Place the tags at their measured distance");
                Console.WriteLine("Press any key when they are placed and still");
                while (Console.ReadKey().Key != ConsoleKey.Enter)
                { }

                Console.WriteLine($"The distance measured by Pozyx is {Distance().ToString()} mm");
                Console.WriteLine($"The percent error is: {Error().ToString()}%\n");
                Console.WriteLine("Press enter to run another test");
                if(Console.ReadKey().Key != ConsoleKey.Enter)
                {
                    cont = false;
                }

            }
        }

        private float Distance()
        {
            
            float lhs = MathF.Pow(_tags[1].Position.x - _tags[0].Position.x, 2);
            float rhs = MathF.Pow(_tags[1].Position.y - _tags[0].Position.y, 2);

            return MathF.Sqrt(lhs + rhs);
        }

        private float Error()
        {
            float num = Math.Abs(acceptedDist - experimentalDist); 
            
            return (num / acceptedDist) * 100;
            
        }


    }
}
