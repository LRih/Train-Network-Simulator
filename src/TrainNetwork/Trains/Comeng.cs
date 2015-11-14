using System;

namespace TrainNetworkSimulator
{
    public class Comeng : Train
    {
        //===================================================================== INITIALIZE
        public Comeng(Platform platform) : base(platform)
        {
        }

        //===================================================================== PROPERTIES
        protected override string Prefix
        {
            get { return "C"; }
        }
        protected override int Speed
        {
            get { return 32; } // 115 km/hr
        }
    }
}
