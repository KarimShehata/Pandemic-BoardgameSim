namespace PandemicConsoleApp
{
    class BaseConstants
    {
        public const int ActionCount = 4;
        public const int FullCubeCount = 24;
        public const int HandLimit = 7;
        public const int ResearchStationReserve = 6;

        public static readonly int[] StartingHands = { 4, 3, 2 };

        public static int[] InfectionRates = { 2, 2, 2, 3, 3, 4, 4 };
    }
}
