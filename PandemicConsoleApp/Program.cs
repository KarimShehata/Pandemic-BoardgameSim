using System;
using System.Threading;
using PandemicConsoleApp.Properties;

namespace PandemicConsoleApp
{
    public class Program
    {
        public static Random random = new Random();

        public static int winCounnt;
        private static int _numGames = 1;

        private static void Main()
        {
            //_numGames = Settings.Default.NumGames;
            //var threads = 8;

            //for (var i = 0; i < threads; i++)
            //{
            //    new Thread(StartGame).Start();
            //}

            StartGame();
        }

        private static void StartGame()
        {
            var interval = (int)(_numGames * 0.1);

            var c = 0;
            for (var i = 0; i < _numGames; i++)
            {
                if (i == c)
                {
                    c += interval;
                    Console.WriteLine("Game: " + (i + 1));
                }

                var pandemic = new Pandemic(Difficulty.Standard, 2, true);
                //pandemic.PrintBoardState();

            }

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(">>>>> GAME OVER <<<<<");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(winCounnt);
            Console.ReadLine();
        }
    }
}