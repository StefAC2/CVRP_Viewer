using System;

namespace CVRP_Viewer
{
    public class Movement : IComparable<Movement>
    {
        // Properties
        public int Cost, OriginalTruck, NewTruck;
        public Node NewPrevious;
        public Node[] Nodes;

        // Methode
        public int CompareTo(Movement other)
        {
            return this.Cost.CompareTo(other.Cost);
        }
    }
}