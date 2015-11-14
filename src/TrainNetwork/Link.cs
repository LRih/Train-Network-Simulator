using System;
using System.Collections.Generic;
using System.Drawing;

namespace TrainNetworkSimulator
{
    public class Link
    {
        //===================================================================== VARIABLES
        public readonly bool IsTunnel;
        public readonly Point[] Nodes;
        private float _efficiency = 1;

        public readonly Station InStation;
        public readonly Station OutStation;

        private readonly Dictionary<int, Track> _tracks = new Dictionary<int, Track>();

        //===================================================================== INITIALIZE
        public Link(Station inStation, Station outStation, bool isTunnel, int trackCount, Point[] nodes)
        {
            InStation = inStation;
            OutStation = outStation;
            IsTunnel = isTunnel;

            for (int i = 1; i <= trackCount; i++)
                _tracks.Add(i, new Track(i, this));

            Nodes = nodes;
        }

        //===================================================================== FUNCTIONS
        public Point GetPosition(float proportion)
        {
            float targetLength = Length * proportion;
            float runningLength = 0;
            float sectionLength;
            int x1 = InStation.X;
            int y1 = InStation.Y;

            // iterate through each section and check if target resides within
            foreach (Point node in Nodes)
            {
                int x2 = x1 + node.X;
                int y2 = y1 + node.Y;
                sectionLength = MathUtils.GetMagnitude(x1, y1, x2, y2);
                runningLength += sectionLength;

                if (targetLength < runningLength)
                    return MathUtils.GetLinePoint(x1, y1, x2, y2, (targetLength - (runningLength - sectionLength)) / sectionLength);

                x1 = x2;
                y1 = y2;
            }
            sectionLength = MathUtils.GetMagnitude(x1, y1, OutStation.X, OutStation.Y);
            runningLength += sectionLength;
            return MathUtils.GetLinePoint(x1, y1, OutStation.X, OutStation.Y, (targetLength - (runningLength - sectionLength)) / sectionLength);
        }

        //===================================================================== PROPERTIES
        public float Efficiency
        {
            get { return _efficiency; }
            set { _efficiency = Math.Min(Math.Max(value, 0), 1); }
        }

        public Track GetTrack(int number)
        {
            return _tracks[number];
        }
        public int TrackCount
        {
            get { return _tracks.Count; }
        }

        public float Length
        {
            get
            {
                float length = 0;
                int x1 = InStation.X;
                int y1 = InStation.Y;
                foreach (Point node in Nodes)
                {
                    int x2 = x1 + node.X;
                    int y2 = y1 + node.Y;
                    length += MathUtils.GetMagnitude(x1, y1, x2, y2);
                    x1 = x2;
                    y1 = y2;
                }
                length += MathUtils.GetMagnitude(x1, y1, OutStation.X, OutStation.Y);
                return length;
            }
        }
    }
}
