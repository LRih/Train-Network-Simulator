using System;

namespace TrainNetworkSimulator
{
    public class XTrapolis : Train
    {
        //===================================================================== INITIALIZE
        public XTrapolis(Platform platform) : base(platform)
        {
        }

        //===================================================================== PROPERTIES
        protected override string Prefix
        {
            get { return "X"; }
        }
        protected override int Speed
        {
            get { return 25; } // 95 km/hr
        }
    }
}
