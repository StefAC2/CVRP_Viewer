using System;

namespace CVRP_Viewer
{
    public class Movement : IComparable<Movement>
    {
        public int Cost, NbNodes, OriginalTruck, NewTruck;
        public Node OriginalPrevious, NewPrevious, Node;

        public int CompareTo(Movement other)
        {
            return this.Cost.CompareTo(other.Cost);
        }
    }
}