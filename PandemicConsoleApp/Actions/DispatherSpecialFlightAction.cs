using System;

namespace PandemicConsoleApp
{
    internal class DispatherSpecialFlightAction : MovementAction
    {
        public Player MovingPlayer;
        public Player DestinationPlayer;

        public DispatherSpecialFlightAction(Player movingPlayer, Player destinationPlayer)
        {
            ActionType = ActionType.DispatherSpecialFlightAction;
            MovingPlayer = movingPlayer;
            DestinationPlayer = destinationPlayer;
        }

        public override void PrintAction(int i)
        {
            Console.WriteLine($"{i}) Move P#{MovingPlayer.Id} the {MovingPlayer.Role} @ {Map.CityNames[MovingPlayer.Location]} to P#{DestinationPlayer.Id} the {DestinationPlayer.Role} @ {Map.CityNames[DestinationPlayer.Location]}");
        }
    }
}