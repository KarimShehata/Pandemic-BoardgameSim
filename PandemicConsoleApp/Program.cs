using System;

namespace PandemicConsoleApp
{
    public class Program
    {
        public static Random random = new Random();

        public static int winCounnt;

        private static void Main()
        {
            var numGames = 100000000.0;

            var c = 0;
            for (var i = 0; i < numGames; i++)
            {
                var pandemic = new Pandemic(Difficulty.Standard, 2);
                //pandemic.PrintBoardState();

                //Console.WriteLine($"Game {i} created.");

                if (i < c) continue;

                c += 100000;
                Console.WriteLine("Game: " + i);
            }

            Console.WriteLine(winCounnt/numGames);

            Console.WriteLine("No Winner");

            Console.ReadLine();
        }
    }
}