using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace TrainNetworkSimulator
{
    public static class Colors
    {
        //#==================================================================== CONSTANTS
        private static readonly Color LINK_GOOD = Color.SkyBlue;
        private static readonly Color LINK_GOOD_TUNNEL = Color.DeepSkyBlue;
        private static readonly Color LINK_BAD = Color.Orange;

        //#==================================================================== FUNCTIONS
        public static Color GetStationColor(Station station)
        {
            return station.IsTunnel ? LINK_GOOD_TUNNEL : LINK_GOOD;
        }
        public static Color GetLinkColor(Link link)
        {
            if (link.TrackCount == 0)
                return Color.LightGray;
            else if (link.Efficiency <= 0.8f)
                return LINK_BAD;
            else
                return link.IsTunnel ? LINK_GOOD_TUNNEL : LINK_GOOD;
        }
    }
}
