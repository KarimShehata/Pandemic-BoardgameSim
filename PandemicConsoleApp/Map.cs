using System;
using System.Collections.Generic;

namespace PandemicConsoleApp
{
    internal class Map
    {
        public List<City> Cities = new List<City>();

        public Map()
        {
            InitBoard();
        }

        private void InitBoard()
        {
            for (var i = 0; i < 49; i++)
            {
                Cities.Add(new City { Id = i });
            }

            Cities[0].AddNeighbours(Cities, new List<int> { 29, 34, 36, 1 });
            Cities[1].AddNeighbours(Cities, new List<int> { 0, 36, 37, 11, 2 });
            Cities[2].AddNeighbours(Cities, new List<int> { 1, 10,3 });
            Cities[3].AddNeighbours(Cities, new List<int> { 2, 10, 9, 4 });
            Cities[4].AddNeighbours(Cities, new List<int> { 3, 9, 7, 5 });
            Cities[5].AddNeighbours(Cities, new List<int> { 4, 7, 8,6 });
            Cities[6].AddNeighbours(Cities, new List<int> { 5, 13, 12 });
            Cities[7].AddNeighbours(Cities, new List<int> { 4, 5, 8, 9, 14 });
            Cities[8].AddNeighbours(Cities, new List<int> { 5, 7, 13 });
            Cities[9].AddNeighbours(Cities, new List<int> { 3, 7, 4, 14, 43 });
            Cities[10].AddNeighbours(Cities, new List<int> { 2, 3, 11, 38 });
            Cities[11].AddNeighbours(Cities, new List<int> { 1, 10, 38 });
            Cities[12].AddNeighbours(Cities, new List<int> { 6, 13, 17 });
            Cities[13].AddNeighbours(Cities, new List<int> { 6, 8, 12, 14, 15, 16 });
            Cities[14].AddNeighbours(Cities, new List<int> { 9, 7, 13, 15 });
            Cities[15].AddNeighbours(Cities, new List<int> { 13, 14, 16, 20, 45 });
            Cities[16].AddNeighbours(Cities, new List<int> { 13, 15, 17, 19, 20 });
            Cities[17].AddNeighbours(Cities, new List<int> { 12, 16, 18, 19 });
            Cities[18].AddNeighbours(Cities, new List<int> { 17, 19, 21, 22, 23 });
            Cities[19].AddNeighbours(Cities, new List<int> { 16, 17, 18, 20, 21 });
            Cities[20].AddNeighbours(Cities, new List<int> { 15, 16, 19 });
            Cities[21].AddNeighbours(Cities, new List<int> { 18, 19, 22 });
            Cities[22].AddNeighbours(Cities, new List<int> { 18, 21, 23, 24, 26 });
            Cities[23].AddNeighbours(Cities, new List<int> { 18, 22, 24, 25 });
            Cities[24].AddNeighbours(Cities, new List<int> { 22, 23, 25, 26, 27 });
            Cities[25].AddNeighbours(Cities, new List<int> { 23, 24, 27, 29, 30, 31 });
            Cities[26].AddNeighbours(Cities, new List<int> { 22, 24, 27, 28 });
            Cities[27].AddNeighbours(Cities, new List<int> { 24, 25, 26, 29 });
            Cities[28].AddNeighbours(Cities, new List<int> { 26, 29, 36 });
            Cities[29].AddNeighbours(Cities, new List<int> { 0, 25, 27, 28, 30 });
            Cities[30].AddNeighbours(Cities, new List<int> { 25, 29, 31, 35 });
            Cities[31].AddNeighbours(Cities, new List<int> { 25, 30, 32, 33, 34 });
            Cities[32].AddNeighbours(Cities, new List<int> { 31, 33 });
            Cities[33].AddNeighbours(Cities, new List<int> { 31, 32, 34 });
            Cities[34].AddNeighbours(Cities, new List<int> { 0, 31, 33, 35 });
            Cities[35].AddNeighbours(Cities, new List<int> { 30, 34 });
            Cities[36].AddNeighbours(Cities, new List<int> { 0, 1, 28, 37 });
            Cities[37].AddNeighbours(Cities, new List<int> { 36, 38, 1, 39, 40 });
            Cities[38].AddNeighbours(Cities, new List<int> { 10, 11, 37, 39 });
            Cities[39].AddNeighbours(Cities, new List<int> { 37, 38, 40, 42, 43 });
            Cities[40].AddNeighbours(Cities, new List<int> { 37, 39, 41 });
            Cities[41].AddNeighbours(Cities, new List<int> { 40 });
            Cities[42].AddNeighbours(Cities, new List<int> { 39, 43 });
            Cities[43].AddNeighbours(Cities, new List<int> { 42, 39, 9, 44 });
            Cities[44].AddNeighbours(Cities, new List<int> { 43, 45, 46 });
            Cities[45].AddNeighbours(Cities, new List<int> { 15, 44, 46, 47 });
            Cities[46].AddNeighbours(Cities, new List<int> { 44, 45, 47 });
            Cities[47].AddNeighbours(Cities, new List<int> { 45, 46 });

            TestConnections();
        }

        private void TestConnections()
        {
            foreach (var node in Cities)
            {
                foreach (var neighbour in node.Neighbours)
                {
                    if (!neighbour.Neighbours.Contains(node))
                        Console.WriteLine("Error" + node.Id + " is missing in " + neighbour.Id);
                }
            }
        }

        public void PrintState()
        {
            foreach (var node in Cities)
            {
                node.PrintState();
            }
        }

        public bool IsCityInfected(int cityId)
        {
           return Cities[cityId].IsInfected;
        }
    }
}