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
            Cost.Add(currentCityCard);
        }

        #endregion Public Constructors

        public override void PrintAction(int i)
        {
            Console.WriteLine($"{i}) Charter Flight to {Destiantion}");

        }
    }
}