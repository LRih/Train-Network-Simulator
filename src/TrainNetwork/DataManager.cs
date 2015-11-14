using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace TrainNetworkSimulator
{
    public class DataManager
    {
        //===================================================================== CONSTANTS
        private static readonly string DATA_PATH = Application.StartupPath + @"\data\";

        //===================================================================== VARIABLES
        private Dictionary<string, Station> _stations = new Dictionary<string, Station>();
        private List<Link> _links = new List<Link>();

        //===================================================================== FUNCTIONS
        public Dictionary<string, Station> LoadStations()
        {
            _stations.Clear();

            Point pt = new Point();
            foreach (string line in File.ReadAllLines(DATA_PATH + "stations.txt"))
            {
                if (IsSkip(line))
                    continue;
                else if (line.StartsWith("="))
                    pt = SetReferencePt(line);
                else
                {
                    string[] split = line.Split(',');
                    for (int i = 0; i < split.Length; i++)
                        split[i] = split[i].Trim();

                    string[] splitNames = split[0].Split('/');
                    foreach (string name in splitNames)
                    {
                        pt.X += int.Parse(split[1]);
                        pt.Y += int.Parse(split[2]);
                        _stations.Add(name, new Station(name, pt.X, pt.Y, split[3] == "1", int.Parse(split[4])));
                    }
                }
            }
            return _stations;
        }

        public List<Link> LoadLinks()
        {
            _links.Clear();

            foreach (string line in File.ReadAllLines(DATA_PATH + "links.txt"))
            {
                if (IsSkip(line))
                    continue;
                else
                {
                    string[] split = line.Split(',');
                    for (int i = 0; i < split.Length; i++)
                        split[i] = split[i].Trim();

                    Point[] nodes = new Point[0];
                    if (split.Length > 3) nodes = GetNodes(split[3]);

                    string[] splitNames = split[0].Split('/');
                    for (int i = 1; i < splitNames.Length; i++)
                    {
                        Link link = new Link(_stations[splitNames[i - 1]], _stations[splitNames[i]], split[1] == "1", int.Parse(split[2]), nodes);
                        _stations[splitNames[i - 1]].Links.Add(splitNames[i], link);
                        _stations[splitNames[i]].Links.Add(splitNames[i - 1], link);
                        _links.Add(link);
                    }
                }
            }
            ConnectTracks();
            return _links;
        }
        private Point[] GetNodes(string nodes)
        {
            string[] splitNodes = nodes.Split(' ');
            Point[] result = new Point[splitNodes.Length];
            for (int i = 0; i < splitNodes.Length; i++)
            {
                string[] splitNode = splitNodes[i].Split('/');
                result[i] = new Point(int.Parse(splitNode[0]), int.Parse(splitNode[1]));
            }
            return result;
        }

        private void ConnectTracks()
        {
            foreach (string line in File.ReadAllLines(DATA_PATH + "tracks.txt"))
            {
                if (IsSkip(line))
                    continue;
                else
                {
                    string[] split = line.Split(',');
                    for (int i = 0; i < split.Length; i++)
                        split[i] = split[i].Trim();

                    string[] splitNames = split[0].Split('/');
                    for (int i = 1; i < splitNames.Length; i++)
                    {
                        if (split.Length == 4) // add two-way
                            ConnectTracks(_stations[splitNames[i - 1]], _stations[splitNames[i]], int.Parse(split[1]), int.Parse(split[2]), int.Parse(split[3]));
                        else
                            ConnectTracks(_stations[splitNames[i - 1]], _stations[splitNames[i]], split[1]);
                    }
                }
            }
        }
        private void ConnectTracks(Station origin, Station destination, int oPlatformStart, int dPlatformStart, int twoWayCount)
        {
            for (int i = 0; i < twoWayCount; i++)
            {
                int start = i * 2 + 1 + dPlatformStart;
                int end = i * 2 + 1 + oPlatformStart;
                ConnectTracks(destination, origin, start + "/" + Math.Min(start, end) + "/" + end);
                start = i * 2 + 2 + oPlatformStart;
                end = i * 2 + 2 + dPlatformStart;
                ConnectTracks(origin, destination, start + "/" + Math.Min(start, end) + "/" + end);
            }
        }
        private void ConnectTracks(Station origin, Station destination, string connections)
        {
            string[] splitConnections = connections.Split(' ');
            for (int i = 0; i < splitConnections.Length; i++)
            {
                Track track = origin.Links[destination.Name].GetTrack(GetTrack(splitConnections[i]));
                int[] originTracks = GetOrigins(splitConnections[i]);
                int[] destTracks = GetDestinations(splitConnections[i]);
                foreach (int destTrack in destTracks)
                    track.AddDestination(destination.GetPlatform(destTrack));
                foreach (int originTrack in originTracks)
                    origin.GetPlatform(originTrack).AddDestination(track);
            }
        }
        private int[] GetOrigins(string connection)
        {
            string[] split = connection.Split('/')[0].Split('+');
            int[] origins = new int[split.Length];
            for (int i = 0; i < split.Length; i++)
                origins[i] = int.Parse(split[i]);
            return origins;
        }
        private int GetTrack(string connection)
        {
            return int.Parse(connection.Split('/')[1]);
        }
        private int[] GetDestinations(string connection)
        {
            string[] split = connection.Split('/')[2].Split('+');
            int[] destinations = new int[split.Length];
            for (int i = 0; i < split.Length; i++)
                destinations[i] = int.Parse(split[i]);
            return destinations;
        }

        private Point SetReferencePt(string line)
        {
            string station = line.Substring(1);
            return new Point(_stations[station].X, _stations[station].Y);
        }

        private static bool IsSkip(string line)
        {
            return line.Trim().Length == 0 || line.StartsWith("#");
        }
    }
}
