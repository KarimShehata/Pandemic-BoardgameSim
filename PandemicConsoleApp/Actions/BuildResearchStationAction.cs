namespace PandemicConsoleApp
{
    internal class BuildResearchStationAction : Action
    {
        #region Public Constructors

        public BuildResearchStationAction(int playerLocation)
        {
            Cost.Add(playerLocation);
        }

        #endregion Public Constructors
    }
}