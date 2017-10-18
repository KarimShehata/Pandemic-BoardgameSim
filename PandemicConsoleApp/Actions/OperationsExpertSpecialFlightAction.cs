using System;

namespace PandemicConsoleApp.Actions
{
    internal class OperationsExpertSpecialFlightAction : MovementAction
    {
        public OperationsExpertSpecialFlightAction(int destiantion, int card)
        {
            ActionType = ActionType.OperationsExpertSpecialFlightAction;
            Destiantion = destiantion;
            Cost = card;
        }

        public int Cost { get; set; }

        public override void PrintAction(int i)
        {
            Console.WriteLine($"{i}) Operations Expert Special Flight to {Map.CityNames[Destiantion]} via {Map.CityNames[Cost]}");
        }
    }
}