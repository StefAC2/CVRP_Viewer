/**
 * Auteur       : Cirieco Stefano
 * Version      : 1.0
 * Date         : 27.05.2020
 * Class        : IFA-P3B
 * Description  : This class holds all the information to move a node from one truck to another
 */
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