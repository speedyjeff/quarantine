using System;

namespace quarantine
{
    public class Route
    {
        public int rVer { get; set; }
        public int rHor { get; set; }

        public Route(int ver, int hor)
        {
            rVer = ver;
            rHor = hor;
        }
    }
}
