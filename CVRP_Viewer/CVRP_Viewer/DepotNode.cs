namespace CVRP_Viewer
{
    public class DepotNode : Node
    {
        public int TruckId;

        public DepotNode(Location pos, int truckId) : base(pos)
        {
            TruckId = truckId;
        }
    }
}