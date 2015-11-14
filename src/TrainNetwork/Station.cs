using System;
using System.Collections.Generic;

namespace TrainNetworkSimulator
{
    public class Station
    {
        //===================================================================== VARIABLES
        public readonly string Name;
        public readonly int X; // in metres
        public readonly int Y;
        public readonly bool IsTunnel;

        private readonly Dictionary<int, Platform> _platforms = new Dictionary<int, Platform>();
        public readonly Dictionary<string, Link> Links = new Dictionary<string, Link>();

        //===================================================================== INITIALIZE
        public Station(string name, int x, int y, bool isTunnel, int platformCount)
        {
            Name = name;
            X = x;
            Y = y;
            IsTunnel = isTunnel;

            for (int i = 1; i <= platformCount; i++)
                _platforms.Add(i, new Platform(i, this));
        }

        //===================================================================== PROPERTIES
        public Platform GetPlatform(int number)
        {
            return _platforms[number];
        }
        public int PlatformCount
        {
            get { return _platforms.Count; }
        }
    }
}
