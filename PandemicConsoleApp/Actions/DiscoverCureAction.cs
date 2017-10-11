using System;
using System.Collections.Generic;
using Action = PandemicConsoleApp.Actions.Action;

namespace PandemicConsoleApp
{
    internal class DiscoverCureAction : Action
    {
        public DiscoverCureAction(List<int> cardsOfSameColor, int cureColor)
        {
            ActionType = ActionType.DiscoverCure;
            Cards = cardsOfSameColor;
            CureColor = cureColor;
        }

        public int CureColor { get; set; }

        public List<int> Cards { get; set; }

        public override void PrintAction(int i)
        {
            Console.WriteLine($"{i}) Discover Cure");
        }
    }
}