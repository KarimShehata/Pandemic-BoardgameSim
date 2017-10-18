using System;
using Action = PandemicConsoleApp.Actions.Action;

namespace PandemicConsoleApp
{
    internal class TreatDiseaseAction : Action
    {
        public TreatDiseaseAction(int diseaseColor)
        {
            ActionType = ActionType.TreatDisease;
            DiseaseColor = diseaseColor;
        }

        public int DiseaseColor{ get; set; }

        public override void PrintAction(int i)
        {
            Console.WriteLine($"{i}) Treat Disease {DiseaseColor}");
        }
    }
}