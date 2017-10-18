using System.Collections.Generic;

namespace PandemicConsoleApp
{
    internal class Player
    {
        public int Id { get; set; }
        public List<int> Hand = new List<int>();
        public int Location;
        public Role Role;
    }
}