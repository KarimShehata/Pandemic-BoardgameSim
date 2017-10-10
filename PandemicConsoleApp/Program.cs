using System;

namespace PandemicConsoleApp
{
    public class Program
    {
        private static void Main()
        {
            for (var i = 0; i < 1; i++)
            {
                var pandemic = new Pandemic(Difficulty.Standard, 2);
                pandemic.PrintBoardState();

                Console.WriteLine($"Game {i} created.");
            }

            Console.ReadLine();
        }
    }
}