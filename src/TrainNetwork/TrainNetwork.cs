using System;
using System.Collections.Generic;
using System.Drawing;

namespace TrainNetworkSimulator
{
    public class TrainNetwork
    {
        //===================================================================== VARIABLES
        private Dictionary<string, Station> _stations = new Dictionary<string, Station>();
        private List<Link> _links = new List<Link>();
        private List<Train> _trains = new List<Train>();

        private TimeSpan _time = new TimeSpan(8, 0, 0);

        private Point _lastAddPt;

        //===================================================================== INITIALIZE
        public TrainNetwork()
        {
            DataManager dataManager = new DataManager();
            _stations = dataManager.LoadStations();
            _links = dataManager.LoadLinks();

            _stations["Caulfield"].Links["Glenhuntly"].Efficiency = 0.8f;
        }

        //===================================================================== FUNCTIONS
        private int _deploysRemaining = 1;
        public void Update()
        {
            if (_deploysRemaining > 0 && _time.Minutes % 15 == 0 && _time.Seconds == 0)
            {
                _deploysRemaining--;
                //_trains.Add(new Bullet(_stations["Glenhuntly"].GetPlatform(1)));
                //_trains.Add(new Bullet(_stations["Windsor"].GetPlatform(1)));
                //_trains.Add(new Bullet(_stations["Hawthorn"].GetPlatform(1)));
                //_trains.Add(new Bullet(_stations["Jolimont"].GetPlatform(1)));
                for (int i = 0; i < 75; i++)
                {
                    Station station = _links[MathUtils.Rand(0, _links.Count - 1)].InStation;
                    Platform platform = station.GetPlatform(MathUtils.Rand(1, station.PlatformCount));
                    if (!platform.IsOccupied)
                    {
                        Train train = Train.Generate(platform);
                        platform.Occupy(train);
                        _trains.Add(train);
                    }
                }
            }

            _time = _time.Add(new TimeSpan(0, 0, 1));
            foreach (Train train in _trains)
                train.Update();
        }

        private void AddStation(string name, int rX, int rY, bool isTunnel, int platformCount)
        {
            _lastAddPt = new Point(_lastAddPt.X + rX, _lastAddPt.Y + rY);
            _stations.Add(name, new Station(name, _lastAddPt.X, _lastAddPt.Y, isTunnel, platformCount));
        }
        private Link AddLink(string inStation, string outStation, bool isTunnel, int trackCount)
        {
            return AddLink(inStation, outStation, isTunnel, trackCount, new Point[] { });
        }
        private Link AddLink(string inStation, string outStation, bool isTunnel, int trackCount, Point[] nodes)
        {
            Link link = new Link(_stations[inStation], _stations[outStation], isTunnel, trackCount, nodes);
            _stations[inStation].Links.Add(outStation, link);
            _stations[outStation].Links.Add(inStation, link);
            _links.Add(link);
            return link;
        }

        private void SetLastPt(string stationName)
        {
            _lastAddPt = new Point(_stations[stationName].X, _stations[stationName].Y);
        }
        private void ResetLastAddPt()
        {
            _lastAddPt = new Point(0, 0);
        }

        //===================================================================== PROPERTIES
        public IDictionary<string, Station> Stations
        {
            get { return _stations; }
        }
        public IList<Link> Links
        {
            get { return _links; }
        }
        public IList<Train> Trains
        {
            get { return _trains; }
        }

        public TimeSpan Time
        {
            get { return _time; }
        }
    }
}
