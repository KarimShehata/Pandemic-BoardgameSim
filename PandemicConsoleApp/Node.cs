using System;
using System.Collections.Generic;
using System.Linq;

namespace PandemicConsoleApp
{
    internal class City
    {
        public int Id;
        public List<City> Neighbours = new List<City>();

        public int[] Cubes = new int[4]; // yellow: 0-11 -- red: 12-23 -- blue: 24-35 -- black: 36-47
        public bool IsInfected => Cubes.Sum() > 0;

        internal void AddNeighbours(List<City> allNodes, List<int> idList)
        {
            foreach (var i in idList)
            {
                Neighbours.Add(allNodes[i]);
            }
        }

        public void PrintState()
        {
            Console.WriteLine($"City #{Id:D2} has {Cubes[0]}/{Cubes[1]}/{Cubes[2]}/{Cubes[3]} Cubes");
        }
    }
}