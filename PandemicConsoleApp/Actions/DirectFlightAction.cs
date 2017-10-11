using System;

namespace PandemicConsoleApp
{
    internal class DirectFlightAction : MovementAction
    {
        #region Public Constructors

        public DirectFlightAction(int destinationCityId)
        {
            ActionType = ActionType.DirectFlight;
            Destiantion = destinationCityId;
            Cost.Add(destinationCityId);
        }

        #endregion Public Constructors

        public override void PrintAction(int i)
        {
            Console.WriteLine($"{i}) Direct Flight to {Destiantion}");
        }
    }
}