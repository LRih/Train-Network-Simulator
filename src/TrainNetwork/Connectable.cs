using System;
using System.Collections.Generic;

namespace TrainNetworkSimulator
{
    public abstract class Connectable<T>
    {
        //===================================================================== VARIABLES
        public readonly int Number;
        private Train _train = null;

        private List<T> _destinations = new List<T>();

        //===================================================================== INITIALIZE
        public Connectable(int number)
        {
            Number = number;
        }

        //===================================================================== FUNCTIONS
        public void AddDestination(T destination)
        {
            _destinations.Add(destination);
        }
        public void Occupy(Train train)
        {
            _train = train;
        }
        public void Unoccupy()
        {
            _train = null;
        }

        //===================================================================== PROPERTIES
        public IList<T> Destinations
        {
            get { return _destinations; }
        }
        public bool IsOccupied
        {
            get { return _train != null; }
        }
    }
}
