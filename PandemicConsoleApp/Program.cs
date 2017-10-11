using System;

namespace PandemicConsoleApp
{
    public class Program
    {
        public static Random random = new Random();

        public static int winCounnt;

        private static void Main()
        {
            var numGames = 1000;
            var interval = (int)(numGames * 0.1);

            var c = 0;
            for (var i = 0; i < numGames; i++)
            {
                var pandemic = new Pandemic(Difficulty.Standard, 2);
                //pandemic.PrintBoardState();

                if (i < c) continue;

                c += interval;
                Console.WriteLine("Game: " + i);
            }

            Console.ReadLine();
        }
    }
}