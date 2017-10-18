using System;

namespace PandemicConsoleApp
{
    internal class ShuttleFlightAction : MovementAction
    {

        #region Public Constructors

        public ShuttleFlightAction(int destinationCityId)
        {
            ActionType = ActionType.ShuttleFlight;
            Destiantion = destinationCityId;
        }

        #endregion Public Constructors

        public override void PrintAction(int i)
        {
            Console.WriteLine($"{i}) Shuttle Flight to {Map.CityNames[Destiantion]}");
        }
    }
}