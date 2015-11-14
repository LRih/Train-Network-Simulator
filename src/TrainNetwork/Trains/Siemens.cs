using System;

namespace TrainNetworkSimulator
{
    public class Siemens : Train
    {
        //===================================================================== INITIALIZE
        public Siemens(Platform platform) : base(platform)
        {
        }

        //===================================================================== PROPERTIES
        protected override string Prefix
        {
            get { return "S"; }
        }
        protected override int Speed
        {
            get { return 32; } // 115 km/hr
        }
    }
}
