using System;
using Action = PandemicConsoleApp.Actions.Action;

namespace PandemicConsoleApp
{
    internal class BuildResearchStationAction : Action
    {
        #region Public Constructors

        public BuildResearchStationAction(int playerLocation)
        {
            ActionType = ActionType.BuildResearchStation;
            Cost.Add(playerLocation);
        }

        #endregion Public Constructors

        public override void PrintAction(int i)
        {
            Console.WriteLine($"{i}) Build Research Station.");
        }
    }
}