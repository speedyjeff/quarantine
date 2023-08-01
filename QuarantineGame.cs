using System;

namespace quarantine
{
    public class QuarantineGame
    {
        public QuarantineGame()
        {
            // initialize
            Rand = new Random();
            Casualties = 0;
            Captured = 0;
            Cured = 0;

            // initialize the map
            if (true)
            {
                Map = new WorldMap();
            }
            else
            {
                Map = new Pangea();
            }

            // place infected zones
            for (int i = 0; i < QuarantineConstants.InitialSpaces; i++)
            {
                // choose an available space to infect
                int ver, hor;
                do
                {
                    ver = Rand.Next() % Map.Spaces.Length;
                    hor = Rand.Next() % Map.Spaces[ver].Length;
                }
                while (Map.Spaces[ver][hor] == null);

                // add infected
                var infected = Math.Min(QuarantineConstants.InitialInfected, Map.Spaces[ver][hor].Population);
                Map.Spaces[ver][hor].NextInfectedQueue[Map.Spaces[ver][hor].NextInfectedQueue.Length - 1] += infected;
                Map.Spaces[ver][hor].NextPopulation -= infected;
            }

            // setup the board
            MyBoard = new SpaceInfo[Map.Spaces.Length][];
            for (var ver = 0; ver < MyBoard.Length; ver++) MyBoard[ver] = new SpaceInfo[Map.Spaces[ver].Length];

            // capture a view of the world
            CaptureBoard();

            // capture the initial population
            DoubleCheckFirstPopulation = Population;

            // set the day
            Day = 0;
        }

        public long Population { get; private set; }
        public long Dead { get; private set; }
        public long Infected { get; private set; }
        public long Casualties { get; private set; }
        public long Captured { get; private set; }
        public long Cured { get; private set; }
        public int Day { get; private set; }

        // return the board
        public SpaceInfo[][] Board
        {
            get
            {
                return MyBoard;
            }
        }

        // advance one day
        public void NextDay()
        {
            // advance the day
            Day++;

            // switch the 'current' and 'next'
            for (int ver = 0; ver < Map.Spaces.Length; ver++)
            {
                for (int hor = 0; hor < Map.Spaces[ver].Length; hor++)
                {
                    if (Map.Spaces[ver][hor] != null) Map.Spaces[ver][hor].Swap();
                }
            }

            // advance to the next day
            for (int ver = 0; ver < Map.Spaces.Length; ver++)
            {
                for (int hor = 0; hor < Map.Spaces[ver].Length; hor++)
                {
                    CalculateSpread(ver, hor);
                }
            }

            // capture board
            CaptureBoard();

            // This game is a zero sum game, so there should never be more people than the initial reading
            if (DoubleCheckFirstPopulation != (Population + Infected + Dead))
            {
                throw new Exception($"A different number of people now exist in the world than originally where there! Initial({DoubleCheckFirstPopulation}) now ({(Population + Infected + Dead)})");
            }
        }

        public bool TryApplyDeterrent(int ver, int hor, DeterrentType det)
        {
            // sanity check the input
            if (ver < 0 || ver >= Map.Spaces.Length ||
                hor < 0 || hor >= Map.Spaces[ver].Length) throw new Exception("invalid coordinate");
            if (Map.Spaces[ver][hor] == null) throw new Exception("Invalid space (null!)");
            if ((int)det < 0 || det == DeterrentType.LENGTH || (int)det >= (int)DeterrentType.NONE) throw new Exception("Invalid deterrent");

            // check if this route has already applied this deterrent
            if ((int)det < (int)DeterrentType.LENGTH && Map.Spaces[ver][hor].Deterrents[(int)det]) return false;

            // grab the route
            var routes = CalculateRoutes(new Route(ver,hor), QuarantineConstants.DeterrentProximity[(int)det]);

            // sort for unique entries
            var uniqueRoutes = new List<Route>(); ;
            foreach (Route[] route in routes)
            {
                for (int i = 0; i < route.Length; i++)
                {
                    if (!uniqueRoutes.Contains(route[i])) uniqueRoutes.Add(route[i]);
                }
            }

            // apply deterrent
            switch (det)
            {
                case DeterrentType.TerminateSmall:
                case DeterrentType.TerminateLarge:
                    // wipe out an area
                    foreach (Route route in uniqueRoutes)
                    {
                        // wipe out the healthy
                        var dead = (long)Math.Ceiling((double)Map.Spaces[route.rVer][route.rHor].NextPopulation * QuarantineConstants.PopulationCapture[(int)det]);
                        Map.Spaces[route.rVer][route.rHor].NextPopulation -= dead;
                        Map.Spaces[route.rVer][route.rHor].NextDead += dead;
                        Casualties += dead;

                        // wipe out the infected
                        for (int i = 0; i < Map.Spaces[route.rVer][route.rHor].NextInfectedQueue.Length; i++)
                        {
                            dead = (long)Math.Ceiling((double)Map.Spaces[route.rVer][route.rHor].NextInfectedQueue[i] * QuarantineConstants.InfectedCapture[(int)det]);
                            Map.Spaces[route.rVer][route.rHor].NextInfectedQueue[i] -= dead;
                            Map.Spaces[route.rVer][route.rHor].NextDead += dead;
                            Captured += dead;
                        }
                    }
                    break;

                case DeterrentType.Evacuate:
                    // move inner to outer
                    var population = (long)Math.Ceiling((double)Map.Spaces[ver][hor].NextPopulation * QuarantineConstants.PopulationCapture[(int)det]);

                    foreach (Route[] rs in routes)
                    {
                        Route route = rs[rs.Length - 1];

                        if (route.rVer == ver && route.rHor == hor)
                        {
                            // this is the root, and should remain untouched
                        }
                        else
                        {
                            // distribute an equal portion of the population to each route
                            var ppop = population / (routes.Count);
                            Map.Spaces[ver][hor].NextPopulation -= ppop;
                            Map.Spaces[route.rVer][route.rHor].NextPopulation += ppop;
                        }
                    }
                    break;

                case DeterrentType.AddInfected:
                    Map.Spaces[ver][hor].NextInfectedQueue[Map.Spaces[ver][hor].NextInfectedQueue.Length - 1] += QuarantineConstants.AdditionalInfected;
                    DoubleCheckFirstPopulation += QuarantineConstants.AdditionalInfected;
                    break;

                case DeterrentType.AddPopulation:
                    Map.Spaces[ver][hor].NextPopulation += QuarantineConstants.AdditionalPopulation;
                    DoubleCheckFirstPopulation += QuarantineConstants.AdditionalPopulation;
                    break;

                default:
                    Map.Spaces[ver][hor].Deterrents[(int)det] = true;
                    break;
            }

            return true;
        }

        #region private
        private IMap Map;
        private SpaceInfo[][] MyBoard;
        private Random Rand;
        private long DoubleCheckFirstPopulation;

        private void CaptureBoard()
        {
            // reset the global counters
            Population = 0;
            Dead = 0;
            Infected = 0;

            // iterate through the playing field and collect information on the board
            for (int ver = 0; ver < Map.Spaces.Length; ver++)
            {
                for (int hor = 0; hor < Map.Spaces[ver].Length; hor++)
                {
                    if (Map.Spaces[ver][hor] != null)
                    {
                        string[] airds = null;

                        // set global constants
                        Population += Map.Spaces[ver][hor].Population;
                        Dead += Map.Spaces[ver][hor].Dead;
                        Infected += Map.Spaces[ver][hor].Infected;

                        // capture air port destinations
                        if (Map.Spaces[ver][hor].AirDestinations != null)
                        {
                            var airList = new List<string>();

                            foreach (Route route in Map.Spaces[ver][hor].AirDestinations)
                            {
                                foreach (string name in Map.Information(Map.Spaces[route.rVer][route.rHor].Id))
                                {
                                    airList.Add(name);
                                }
                            }

                            airds = airList.ToArray();
                        }

                        // capture the board
                        MyBoard[ver][hor] = new SpaceInfo(
                            Map.Information(Map.Spaces[ver][hor].Id),
                            Map.Spaces[ver][hor].Population,
                            Map.Spaces[ver][hor].Dead,
                            Map.Spaces[ver][hor].Infected,
                            Map.Spaces[ver][hor].Deterrents,
                            airds);
                    }
                    else
                    {
                        MyBoard[ver][hor] = null;
                    }
                }
            }
        }

        private void CalculateSpread(int ver, int hor)
        {
            // Algorithm:
            //  InfectedQueue [2]                  [1] [0]
            //   Percentages   [W] [C] [B] [P] [H]  ...
            //
            //  For each Queue x Percentage, calculate all the possible routes based on Proximity
            //  Then distribute the set of infected into each of the affected spaces
            //  TODO: Remove based on 'special' abilities applied to a space, and effectiveness of the infected

            // only consider spaces that have infected people
            if (Map.Spaces[ver][hor] != null && Map.Spaces[ver][hor].Infected > 0)
            {
                // walk through the queue
                for (int i = Map.Spaces[ver][hor].InfectedQueue.Length - 1; i >= 0; i--)
                {
                    // total number of infected people in this space
                    var totalInfected = Map.Spaces[ver][hor].InfectedQueue[i];

                    // consider each infected type separately (ie. Walking, car, etc)
                    for (int j = 0; j < QuarantineConstants.Percentages.Length && totalInfected > 0; j++)
                    {
                        List<Route[]> routes = null;

                        // calculate how many of the current infected are this type (intentional CE_ILING)
                        var infected = Math.Min((long)Math.Ceiling(QuarantineConstants.Percentages[j] * Map.Spaces[ver][hor].InfectedQueue[i]), totalInfected);

                        // track how many infected have been considered
                        totalInfected -= infected;
                        if (totalInfected < 0) throw new Exception("Moving more infected than exist");

                        // calculate the possible routes
                        if ((InfectedType)j == InfectedType.Plane && Map.Spaces[ver][hor].AirDestinations != null)
                        {
                            // air travel is different
                            routes = new List<Route[]>();

                            for (int k = 0; k < Map.Spaces[ver][hor].AirDestinations.Length; k++)
                            {
                                var route = new Route[2];
                                route[0] = new Route(ver, hor);
                                route[1] = new Route(Map.Spaces[ver][hor].AirDestinations[k].rVer, Map.Spaces[ver][hor].AirDestinations[k].rHor);
                                routes.Add(route);
                            }
                        }
                        else
                        {
                            // ground transportation
                            routes = CalculateRoutes(new Route(ver, hor), QuarantineConstants.Proximity[j]);
                        }

                        // exit early if there is nothing to do
                        if (routes == null || routes.Count == 0)
                        {
                            // transfer the infected
                            if (i == 0)
                            {
                                // die
                                Map.Spaces[ver][hor].NextDead += infected;
                            }
                            else
                            {
                                // move these infected ahead
                                Map.Spaces[ver][hor].NextInfectedQueue[i - 1] += infected;
                            }
                        }
                        else
                        {
                            // infect more people in each space until all the infected people have moved
                            var orgInfected = infected;
                            var index = 0;
                            while (infected > 0)
                            {
                                // advance index
                                index = Rand.Next() % routes.Count;

                                // destination
                                var dst = routes[index][QuarantineConstants.Proximity[j] - 1];

                                // distribute 0-10% of the infected (Intentional CE_ILING)
                                var movedInfected = Math.Min((long)Math.Ceiling((double)(Rand.Next() % 10) / 100.0) * orgInfected, infected);

                                // apply any deterrents in this region
                                var capturedInfected = ApplyDeterrent(routes[index], movedInfected, (InfectedType)j, out var casualties);
                                casualties = Math.Min(casualties, Map.Spaces[dst.rVer][dst.rHor].NextPopulation);
                                Map.Spaces[dst.rVer][dst.rHor].NextDead += capturedInfected + casualties; // all captured infected are put to death!
                                movedInfected -= capturedInfected;
                                Map.Spaces[dst.rVer][dst.rHor].NextPopulation -= casualties;

                                // calculate how many get reinfected
                                var newlyInfected = Math.Min(movedInfected * QuarantineConstants.Reinfection[j], Map.Spaces[dst.rVer][dst.rHor].NextPopulation);

                                // apply any post
                                var curedInfected = ApplyBlocker(dst, newlyInfected);
                                newlyInfected -= curedInfected;

                                // place the infected
                                Map.Spaces[dst.rVer][dst.rHor].NextInfectedQueue[Map.Spaces[dst.rVer][dst.rHor].NextInfectedQueue.Length - 1] += newlyInfected;
                                Map.Spaces[dst.rVer][dst.rHor].NextPopulation -= newlyInfected;

                                // advance the moved infected
                                if (i == 0)
                                {
                                    // die
                                    Map.Spaces[dst.rVer][dst.rHor].NextDead += movedInfected;
                                }
                                else
                                {
                                    // move these infected ahead
                                    Map.Spaces[dst.rVer][dst.rHor].NextInfectedQueue[i - 1] += movedInfected;
                                }

                                // decrement the infected that are moving
                                infected -= (movedInfected + capturedInfected);

                                // stats
                                Casualties += casualties;
                                Cured += curedInfected;
                                Captured += capturedInfected;
                            } // while infected
                        } // if routes
                    } // foreach InfectionType
                } // foreach infection
            } // if infected
        }

        private long ApplyDeterrent(Route[] routes, long infected, InfectedType infType, out long casualties)
        {
            var reduction = 0d;
            var roadBlock = false;

            // apply all casualties to the destination
            casualties = 0;
            for (int i = 0; i < Map.Spaces[routes[routes.Length - 1].rVer][routes[routes.Length - 1].rHor].Deterrents.Length; i++)
            {
                if (Map.Spaces[routes[routes.Length - 1].rVer][routes[routes.Length - 1].rHor].Deterrents[i])
                {
                    casualties += (long)Math.Ceiling((double)infected * QuarantineConstants.PopulationCapture[i]);
                }
            }

            // check if there are any road blocks
            for (int i = 0; i < routes.Length; i++)
            {
                roadBlock = Map.Spaces[routes[i].rVer][routes[i].rHor].Deterrents[(int)DeterrentType.RoadBlock] || roadBlock;
            }

            // reduce the infection
            switch (infType)
            {
                case InfectedType.Walking:
                    // check for any interference
                    if (Map.Spaces[routes[0].rVer][routes[0].rHor].Deterrents[(int)DeterrentType.StreetPatrol])
                    {
                        reduction += (1.0 - reduction) * QuarantineConstants.InfectedCapture[(int)DeterrentType.StreetPatrol];
                    }

                    if (Map.Spaces[routes[0].rVer][routes[0].rHor].Deterrents[(int)DeterrentType.RovingVigilantly])
                    {
                        reduction += (1.0 - reduction) * QuarantineConstants.InfectedCapture[(int)DeterrentType.RovingVigilantly];
                    }
                    break;

                case InfectedType.Car:
                    // check for road block
                    if (roadBlock)
                    {
                        reduction += (1.0 - reduction) * QuarantineConstants.InfectedCapture[(int)DeterrentType.RoadBlock];
                    }
                    break;

                case InfectedType.BusTrain:
                    // check the destination or source for a screener
                    if (Map.Spaces[routes[0].rVer][routes[0].rHor].Deterrents[(int)DeterrentType.TerminalScreening]
                        || Map.Spaces[routes[routes.Length - 1].rVer][routes[routes.Length - 1].rHor].Deterrents[(int)DeterrentType.TerminalScreening])
                    {
                        reduction += (1.0 - reduction) * QuarantineConstants.InfectedCapture[(int)DeterrentType.TerminalScreening];
                    }

                    // check for road block
                    if (roadBlock)
                    {
                        reduction += (1.0 - reduction) * QuarantineConstants.InfectedCapture[(int)DeterrentType.RoadBlock];
                    }
                    break;

                case InfectedType.Plane:
                    // check the destination or source for a screener
                    if (Map.Spaces[routes[0].rVer][routes[0].rHor].Deterrents[(int)DeterrentType.TerminalScreening]
                        || Map.Spaces[routes[routes.Length - 1].rVer][routes[routes.Length - 1].rHor].Deterrents[(int)DeterrentType.TerminalScreening])
                    {
                        reduction += (1.0 - reduction) * QuarantineConstants.InfectedCapture[(int)DeterrentType.TerminalScreening];
                    }
                    break;

                case InfectedType.Home:
                    // nothing
                    break;
            }

            return Math.Min((long)Math.Ceiling((double)infected * reduction), infected);
        }

        private long ApplyBlocker(Route route, long infected)
        {
            if (Map.Spaces[route.rVer][route.rHor].Deterrents[(int)DeterrentType.Medics])
            {
                // reduce the newly infected
                return (long)Math.Floor((double)infected * QuarantineConstants.InfectedBlocker[(int)DeterrentType.Medics]);
            }

            return 0;
        }

        private List<Route[]> CalculateRoutes(Route route, int proximity)
        {
            if (proximity <= 0) return null;

            // trace all the possible routes of depth 'proximity'
            var tree = new Tree<Route>(parent: null, -1, route);
            Path(proximity - 1, tree, route);

            // return valid routes
            return ValidRoutes(tree, proximity);
        }

        private List<Route[]> ValidRoutes(Tree<Route> tree, int depth)
        {
            // walk the tree and prune the following:
            //  1. Paths that are not the largest distance away
            //  2. Paths that contain invalid spaces (out of bounds, or null)

            var head = tree;
            var route = new Route[depth];
            var routes = new List<Route[]>();
            var index = 0;
            var pindex = 0;

            while (head != null)
            {
                // if any of the children are null, backup
                if (head.Children[0] == null
                    || head.Children[1] == null
                    || head.Children[2] == null
                    || head.Children[3] == null)
                {
                    // build the tree
                    route[index++] = head.Data;

                    // store? Walk the array and check for invalids
                    var vLen = 0;
                    var hLen = 0;
                    var valid = true;
                    for (int i = 0; i < route.Length; i++)
                    {
                        if (route[i].rVer < 0 || route[i].rVer >= Map.Spaces.Length ||
                            route[i].rHor < 0 || route[i].rHor >= Map.Spaces[route[i].rVer].Length)
                        {
                            valid = false;
                            break;
                        }

                        if (Map.Spaces[route[i].rVer][route[i].rHor] == null)
                        {
                            valid = false;
                            break;
                        }

                        // measure the distance traveled
                        if (i > 0)
                        {
                            vLen += route[i - 1].rVer - route[i].rVer;
                            hLen += route[i - 1].rHor - route[i].rHor;
                        }
                    }

                    // to be counted must be both 'valid' and have an absolute path of depth
                    if (valid && Math.Abs(vLen) + Math.Abs(hLen) == depth - 1)
                    {
                        var nr = new Route[depth];
                        for (int i = 0; i < nr.Length; i++) nr[i] = route[i];
                        routes.Add(nr);
                    }

                    // backup
                    index -= 2;
                    pindex = head.Index + 1;
                    head = head.Parent;
                }
                else if (pindex > 3)
                {
                    // backup
                    index--;
                    pindex = head.Index + 1;
                    head = head.Parent;
                }
                else
                {
                    // build the tree
                    route[index++] = head.Data;

                    // keep walking down the tree
                    head = head.Children[pindex];

                    // reset the index to 0 for the first
                    pindex = 0;
                }
            }

            return routes;
        }

        private void Path(int level, Tree<Route> tree, Route route)
        {
            if (level == 0)
            {
                // set the children to null and return
                tree.Children[0] = tree.Children[1] = tree.Children[2] = tree.Children[3] = null;
                return;
            }
            else
            {
                // down
                var nroute = new Route(route.rVer - 1, route.rHor);
                var ntree = new Tree<Route>(tree, 0, nroute);
                tree.Children[0] = ntree;
                Path(level - 1, ntree, nroute);

                // up
                nroute = new Route(route.rVer + 1, route.rHor);
                ntree = new Tree<Route>(tree, 1, nroute);
                tree.Children[1] = ntree;
                Path(level - 1, ntree, nroute);

                // left
                nroute = new Route(route.rVer, route.rHor - 1);
                ntree = new Tree<Route>(tree, 2, nroute);
                tree.Children[2] = ntree;
                Path(level - 1, ntree, nroute);

                // right
                nroute = new Route(route.rVer, route.rHor + 1);
                ntree = new Tree<Route>(tree, 3, nroute);
                tree.Children[3] = ntree;
                Path(level - 1, ntree, nroute);

                return;
            }
        }
        #endregion
    }
}
