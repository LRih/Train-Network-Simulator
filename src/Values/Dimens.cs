using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace TrainNetworkSimulator
{
    public static class Dimens
    {
        //#==================================================================== FUNCTIONS
        public static int GetStationRadius(Station station)
        {
            return 200 + station.PlatformCount * 25;
        }
        public static int GetLinkWidth(Link link)
        {
            return link.TrackCount * 60;
        }
        public static int GetTrainRadius()
        {
            return 60;
        }
    }
}
