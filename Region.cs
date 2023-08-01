using System;

namespace quarantine
{
    class Region
    {
        public Region(int i, int sa, double pop, string[] names)
        {
            Id = i;
            SurfaceArea = sa;
            Population = (long)Math.Ceiling(pop * c_Million);  // pop is in millions
            RegionNames = names;
        }

        public int Id { get; private set; }
        public int SurfaceArea { get; private set; }
        public long Population { get; private set; }
        public string[] RegionNames { get; private set; }

        #region private
        private const long c_Million = 1_000_000;
        #endregion
    }
}
