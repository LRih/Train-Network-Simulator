using System;

namespace TrainNetworkSimulator
{
    public class Bullet : Train
    {
        //===================================================================== INITIALIZE
        public Bullet(Platform platform) : base(platform)
        {
        }

        //===================================================================== PROPERTIES
        protected override string Prefix
        {
            get { return "B"; }
        }
        protected override int Speed
        {
            get { return 88; }
        }
    }
}
