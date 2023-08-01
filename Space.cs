using System;

namespace quarantine
{
    public class Space
    {
        public int Id { get; private set; }
        public long Population { get; private set; }
        public long NextPopulation { get; set; } // next round
        public long Dead { get; private set; }
        public long NextDead { get; set; } // next round
        public long[] InfectedQueue { get; private set; }
        public long[] NextInfectedQueue { get; private set; } // next round
        public Route[] AirDestinations { get; private set; }
        public bool[] Deterrents { get; private set; }

        public Space(int id, long population)
        {
            Init(id, population, air: null);
        }

        public Space(int id, long population, Route[] air)
        {
            Init(id, population, air);
        }

        public long Infected
        {
            get
            {
                var inf = 0L;
                for (int i = 0; i < InfectedQueue.Length; i++) inf += InfectedQueue[i];
                return inf;
            }
        }

        public void Swap()
        {
            // Resize if DaysToDeath chances
            if (NextInfectedQueue.Length != QuarantineConstants.DaysToDeath)
            {
                // allocate a temporary queue to hold the current values
                var tmpQueue = new long[QuarantineConstants.DaysToDeath];

                if (NextInfectedQueue.Length <= tmpQueue.Length)
                {
                    // the new queue is longer or the same
                    for (int i = 0; i < NextInfectedQueue.Length; i++) tmpQueue[i] = NextInfectedQueue[i];
                }
                else
                {
                    // the new queue is shorter
                    var reaged = 0L;

                    // find all the infected that no longer fit
                    for (int i = tmpQueue.Length; i < NextInfectedQueue.Length; i++) reaged += NextInfectedQueue[i];

                    tmpQueue[tmpQueue.Length - 1] = reaged;

                    // transfer the rest
                    for (int i = 0; i < tmpQueue.Length; i++) tmpQueue[i] += NextInfectedQueue[i];
                }

                // set the new queues
                InfectedQueue = new long[QuarantineConstants.DaysToDeath];
                NextInfectedQueue = tmpQueue;
            }

            // switch the 'Next' with 'current'
            Population = NextPopulation;
            Dead = NextDead;
            for (int i = 0; i < InfectedQueue.Length; i++)
            {
                InfectedQueue[i] = NextInfectedQueue[i];
                NextInfectedQueue[i] = 0;
            }
        }

        #region private
        private void Init(int id, long population, Route[] air)
        {
            Id = id;
            Population = NextPopulation = population;
            AirDestinations = air;
            InfectedQueue = new long[QuarantineConstants.DaysToDeath];
            NextInfectedQueue = new long[QuarantineConstants.DaysToDeath];
            Deterrents = new bool[(int)DeterrentType.LENGTH];
            for (int i = 0; i < Deterrents.Length; i++) Deterrents[i] = false;
            Dead = 0;
            NextDead = 0;
        }
        #endregion
    }
}
