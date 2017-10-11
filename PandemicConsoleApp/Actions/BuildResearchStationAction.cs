using PandemicConsoleApp.Actions;

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
    }
}