/**
 * Auteur       : Cirieco Stefano
 * Version      : 1.0
 * Date         : 25.05.2020
 * Class        : IFA-P3B
 * Description  : This class represents a copy of the depot for the Head of a truck
 */
namespace CVRP_Viewer
{
    public class DepotNode : Node
    {
        // Properties
        public int TruckId;

        // Constructor
        public DepotNode(Location pos, int index, int truckId) : base(pos, index)
        {
            TruckId = truckId;
        }
    }
}