using System;

namespace PandemicConsoleApp
{
    internal class DriveFerryAction : MovementAction
    {
        #region Public Constructors

        public DriveFerryAction(int connectedCityId)
        {
            ActionType = ActionType.DriveFerry;
            Destiantion = connectedCityId;
        }

        #endregion Public Constructors

        public override void PrintAction(int i)
        {
            Console.WriteLine($"{i}) Drive/Ferry to {Destiantion}");
        }
    }
}