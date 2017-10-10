namespace PandemicConsoleApp
{
    internal class ShuttleFlightAction : MovementAction
    {

        #region Public Constructors

        public ShuttleFlightAction(int destinationCityId)
        {
            ActionType = ActionType.DirectFlight;
            Destiantion = destinationCityId;
        }

        #endregion Public Constructors

    }
}