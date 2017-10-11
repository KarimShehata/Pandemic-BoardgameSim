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

    }
}