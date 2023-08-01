using System;

namespace quarantine
{
    struct QuarantineConstants
    {
        // controls for the infected
        public static int DaysToDeath = 3; // days
        public static long InitialInfected = 1;
        public static long InitialSpaces = 4;

        // one per InfectionType
        public static long[] Reinfection = new long[] { 5, 4, 2, 1, 0 };
        public static int[] Proximity = new int[] { 1, 2, 4, 2, 0 };
        public static double[] Percentages = new double[] { 0.5, 0.25, 0.15, 0.05, 0.05 };

        // one per DeterrentType
        public static double[] InfectedCapture = new double[] { 0.75, 0.75, 0.95, 0.50, 0.0, -1.0, 0.95, 0.85, 0.0 };
        public static double[] PopulationCapture = new double[] { 0.05, 0.05, 0.0, 0.50, 0.0, -1.0, 0.95, 0.85, .80 };
        public static double[] InfectedBlocker = new double[] { 0.0, 0.0, 0.0, 0.0, 0.25, -1.0, 0.0, 0.0, 0.0 };
        public static int[] DeterrentProximity = new int[] { 1, 1, 1, 1, 1, -1, 1, 2, 3, 1, 1 };

        // controls for the deterrents
        public static long AdditionalInfected = 1;
        public static long AdditionalPopulation = 1000;
    }
}
