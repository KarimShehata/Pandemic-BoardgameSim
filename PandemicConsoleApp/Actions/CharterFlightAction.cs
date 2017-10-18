using System;

namespace PandemicConsoleApp
{
    internal class CharterFlightAction : MovementAction
    {
        #region Public Constructors

        public CharterFlightAction(int currentCityCard, int destinationCityId)
        {
            ActionType = ActionType.CharterFlight;
            Destiantion = destinationCityId;
            Cost = currentCityCard;
        }

        public int Cost { get; set; }

        #endregion Public Constructors

        public override void PrintAction(int i)
        {
            Console.WriteLine($"{i}) Charter Flight to {Map.CityNames[Destiantion]}");

        }
    }
}