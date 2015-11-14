using System;
using System.Drawing;

namespace TrainNetworkSimulator
{
    public abstract class Train
    {
        //===================================================================== CONSTANTS
        private static int CurrentID = 0;

        //===================================================================== VARIABLES
        private readonly int _id;

        private Platform _platform;
        private Track _track;

        private float _distance = 0;
        private int _standbyTime = 0;
        private int _nextTrack;

        //===================================================================== INITIALIZE
        public Train(Platform platform)
        {
            _id = CurrentID;
            CurrentID++;
            _platform = platform;
            _nextTrack = MathUtils.Rand(0, platform.Destinations.Count - 1);
        }
        public static Train Generate(Platform platform)
        {
            switch (MathUtils.Rand(1, 3))
            {
                case 1: return new Comeng(platform);
                case 2: return new Siemens(platform);
                case 3: return new XTrapolis(platform);
                default: throw new Exception("Unreachable code reached");
            }
        }

        //===================================================================== FUNCTIONS
        public void Update()
        {
            if (_standbyTime > 0)
            {
                _standbyTime--;
                return;
            }


            if (IsOnPlatform)
            {
                if  (_platform.Destinations.Count > 0 && !_platform.Destinations[_nextTrack].IsOccupied) MoveToDestinationTrack();
            }
            else // on track
            {
                if (!_track.Destinations[_nextTrack].IsOccupied || _distance < _track.Link.Length / 2)
                {
                    _distance += (Speed - MathUtils.Rand(0, 5)) * _track.Link.Efficiency;
                    if (_track.Destinations.Count > 0 && _distance > _track.Link.Length) MoveToDestinationPlatform();
                }
            }
        }

        private void MoveToDestinationTrack()
        {
            // TODO: don't use Destinations[Rand], only for testing
            _track = _platform.Destinations[_nextTrack];
            _track.Occupy(this);
            _platform.Unoccupy();
            _platform = null;
            _nextTrack = MathUtils.Rand(0, _track.Destinations.Count - 1);
        }
        private void MoveToDestinationPlatform()
        {
            // TODO: don't use Destinations[Rand], only for testing
            _platform = _track.Destinations[_nextTrack];
            _platform.Occupy(this);
            _track.Unoccupy();
            _track = null;
            _distance = 0;
            _standbyTime = MathUtils.Rand(40, 50);
            _nextTrack = MathUtils.Rand(0, _platform.Destinations.Count - 1);
        }

        //===================================================================== PROPERTIES
        public string Name
        {
            get { return Prefix + _id.ToString().PadLeft(3, '0'); }
        }
        public string Status
        {
            get
            {
                if (IsOnPlatform)
                    return _standbyTime + "s";
                else
                    return (int)(_distance / _track.Link.Length * 100) + " %";
            }
        }
        public string TrackNumber
        {
            get
            {
                if (IsOnPlatform)
                    return "P" + _platform.Number;
                else
                    return "T" + _track.Number;
            }
        }

        public Point Position
        {
            get
            {
                if (IsOnPlatform)
                    return new Point(_platform.Station.X, _platform.Station.Y);
                else
                {
                    // TODO: don't use Destinations[Rand], only for testing
                    float percentage = _distance / _track.Link.Length;
                    if (_track.Destinations[0].Station == _track.Link.InStation)
                        percentage = 1 - percentage;

                    return _track.Link.GetPosition(percentage);
                }
            }
        }
        private bool IsOnPlatform
        {
            get { return _platform != null; }
        }

        protected abstract string Prefix { get; }
        protected abstract int Speed { get; }
    }
}
