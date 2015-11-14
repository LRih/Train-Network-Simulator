using System;
using System.Collections.Generic;

namespace TrainNetworkSimulator
{
    public class Platform : Connectable<Track>
    {
        //===================================================================== VARIABLES
        public readonly Station Station;

        //===================================================================== INITIALIZE
        public Platform(int number, Station station) : base(number)
        {
            Station = station;
        }
    }
}
