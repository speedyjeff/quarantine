using System;

namespace quarantine
{
    class Pangea : IMap
    {
        public Pangea()
        {
            Spaces = new Space[ids.Length][];

            // initialize the board
            for (int ver = 0; ver < ids.Length; ver++)
            {
                Spaces[ver] = new Space[ids[ver].Length];
                for (int hor = 0; hor < ids[ver].Length; hor++)
                {
                    if (ids[ver][hor] != 0)
                    {
                        Spaces[ver][hor] = new Space(ids[ver][hor], population: 1000);

                        // add a few air transportation routes
                        if (ver == 7 && hor == 7)
                        {
                            Spaces[ver][hor] = new Space(ids[ver][hor], population: 1000, new Route[] { new Route(2, 2) });
                        }
                        else if (ver == 2 && hor == 2)
                        {
                            Spaces[ver][hor] = new Space(ids[ver][hor], population: 1000, new Route[] { new Route(7, 7) });
                        }

                        // enable all the possible deterrents
                        for (int i = 0; i < Spaces[ver][hor].Deterrents.Length; i++) Spaces[ver][hor].Deterrents[i] = true;
                    }
                }
            }
        }

        public override string[] Information(int id)
        {
            return new string[] { "Pangea" };
        }

        #region private
        private int[][] ids = new int[][]
        {
            new int[] {0,0,0,0,0,0,0,0,0,0},
            new int[] {0,0,0,0,0,0,0,0,0,0},
            new int[] {0,0,1,1,1,1,1,1,0,0},
            new int[] {0,0,0,0,1,1,0,0,0,0},
            new int[] {0,0,0,0,0,0,0,0,0,0},
            new int[] {0,0,1,1,1,1,1,1,0,0},
            new int[] {0,0,0,0,1,1,0,0,0,0},
            new int[] {0,0,1,1,1,1,1,1,0,0},
            new int[] {0,0,0,0,0,0,0,0,0,0},
            new int[] {0,0,0,0,0,0,0,0,0,0}
        };
        #endregion
    }
}
