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
    }
}