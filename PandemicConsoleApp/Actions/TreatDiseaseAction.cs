using System;
using Action = PandemicConsoleApp.Actions.Action;

namespace PandemicConsoleApp
{
    internal class TreatDiseaseAction : Action
    {
        public TreatDiseaseAction()
        {
            ActionType = ActionType.TreatDisease;
        }

        public override void PrintAction(int i)
        {
            Console.WriteLine($"{i}) Treat Disease");
        }
    }
}