using System;
using System.Drawing;

namespace TrainNetworkSimulator
{
    public static class MathUtils
    {
        //#==================================================================== VARIABLES
        private static Random _random = new Random();

        //#==================================================================== FUNCTIONS
        public static int Rand(int low, int high)
        {
            return _random.Next(low, high + 1);
        }
        public static float GetMagnitude(int x1, int y1, int x2, int y2)
        {
            return (float)Math.Sqrt(Math.Pow(x1 - x2, 2) + Math.Pow(y1 - y2, 2));
        }
        public static Point GetLinePoint(int x1, int y1, int x2, int y2, float proportion)
        {
            int x = (int)(x1 + (x2 - x1) * proportion);
            int y = (int)(y1 + (y2 - y1) * proportion);
            return new Point(x, y);
        }
    }
}
