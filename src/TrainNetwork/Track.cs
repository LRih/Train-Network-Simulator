using System;
using System.Collections.Generic;

namespace TrainNetworkSimulator
{
    public class Track : Connectable<Platform>
    {
        //===================================================================== VARIABLES
        public readonly Link Link;

        //===================================================================== INITIALIZE
        public Track(int number, Link link) : base(number)
        {
            Link = link;
        }
    }
}
