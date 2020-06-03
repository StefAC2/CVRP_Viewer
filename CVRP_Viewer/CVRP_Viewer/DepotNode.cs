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