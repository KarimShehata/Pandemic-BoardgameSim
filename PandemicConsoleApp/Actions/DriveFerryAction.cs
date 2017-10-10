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
    }
}