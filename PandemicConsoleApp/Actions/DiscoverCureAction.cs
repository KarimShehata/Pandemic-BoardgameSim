using System;
using System.Collections.Generic;
using Action = PandemicConsoleApp.Actions.Action;

namespace PandemicConsoleApp
{
    internal class DiscoverCureAction : Action
    {
        public DiscoverCureAction(List<int> cardsOfSameColor, int cureColor, Player player)
        {
            ActionType = ActionType.DiscoverCure;
            Cards = cardsOfSameColor;
            CureColor = cureColor;
            Player = player;
        }

        public int CureColor { get; set; }

        public List<int> Cards { get; set; }
        public Player Player { get; set; }

        public override void PrintAction(int i)
        {
            Console.Write($"{i}) Discover Cure: ");

            foreach (var card in Cards)
            {
                Console.Write(card + " ");
            }

            Console.WriteLine();
        }
    }
}