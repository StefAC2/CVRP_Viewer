namespace CVRP_Viewer
{
    public class DepotManager
    {
        // Properties
        public Node[] Clients;

        public Node Depot => Clients[DepotIndex];
        public int DepotIndex;
        public int NbClients => Clients.Length;

        // Constructor
        public DepotManager(int nbClients)
        {
            Clients = new Node[nbClients];
        }

        // Methodes
        public void AddClient(int index, Node client)
        {
            Clients[index] = client;
        }

        public Node GetClient(int index)
        {
            return Clients[index];
        }

        /// <summary>
        /// Assigns Depot to the corresponding node
        /// </summary>
        /// <param name="index">index where the node is located</param>
        public void DeclareDepot(int index)
        {
            DepotIndex = index;
        }
    }
}