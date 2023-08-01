using System;

namespace quarantine
{
    public class SpaceInfo
    {
        public string[] Names { get; private set; }
        public long Population { get; private set; }
        public long Dead { get; private set; }
        public long Infected { get; private set; }
        public bool[] Deterrents { get; private set; }
        public string[] AirDestinations { get; private set; }

        public SpaceInfo(string[] names, long p, long d, long i, bool[] deterrents, string[] airds)
        {
            Names = names;
            Population = p;
            Dead = d;
            Infected = i;
            Deterrents = deterrents;
            AirDestinations = airds;
        }
    }
}
