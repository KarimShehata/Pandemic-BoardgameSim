using System;
using Action = PandemicConsoleApp.Actions.Action;

namespace PandemicConsoleApp
{
    internal class BuildResearchStationAction : Action
    {
        #region Public Constructors

        public BuildResearchStationAction(int location, bool isOperationsExpert = false)
        {
            ActionType = ActionType.BuildResearchStation;

            Cost = isOperationsExpert ? -1 : location;
        }

        public int Cost { get; set; }

        #endregion Public Constructors

        public override void PrintAction(int i)
        {
            Console.WriteLine($"{i}) Build Research Station.");
        }
    }
}