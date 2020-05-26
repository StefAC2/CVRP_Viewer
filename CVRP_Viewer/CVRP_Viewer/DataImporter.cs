using System;
using System.IO;

namespace CVRP_Viewer
{
    public class DataImporter
    {
        // Constantes
        const string DIMENSION = "DIMENSION", CAPACITY = "CAPACITY", NODE_COORD_SECTION = "NODE_COORD_SECTION", DEMAND_SECTION = "DEMAND_SECTION", DEPOT_SECTION = "DEPOT_SECTION";

        // Properties
        public DepotManager DepotManager;

        // Constructor
        public DataImporter() { }

        // Methodes
        public void ImportFromFile(string filePath)
        {
            StreamReader reader = new StreamReader(filePath);

            string extention = filePath.Substring(filePath.LastIndexOf('.') + 1).ToLower();

            switch (extention)
            {
                case "vrp":
                    ImportVRP(reader);
                    break;
                case "dat":
                    ImportDAT(reader);
                    break;
            }
        }

        private void ImportVRP(StreamReader reader)
        {
            string line;

            bool isNodeCoordSection = false, isDemandSection = false, isDepotSection = false;

            // Read lines one by one
            while ((line = reader.ReadLine()) != null)
            {
                line = line.Trim();
                string[] data = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                switch (data[0])
                {
                    case DIMENSION:
                        int dimension = int.Parse(data[2]);
                        DepotManager = new DepotManager(dimension);
                        break;
                    case CAPACITY:
                        int capacity = int.Parse(data[2]);
                        Truck.Capacity = capacity;
                        break;
                    case NODE_COORD_SECTION:
                        isNodeCoordSection = true;
                        break;
                    case DEMAND_SECTION:
                        isNodeCoordSection = false;
                        isDemandSection = true;
                        break;
                    case DEPOT_SECTION:
                        isDemandSection = false;
                        isDepotSection = true;
                        break;
                    default:
                        int.TryParse(data[0], out int num);
                        num--;

                        if (isNodeCoordSection)
                        {
                            Location pos = new Location(int.Parse(data[1]), int.Parse(data[2]));

                            DepotManager.AddClient(num, new Node(pos));
                        }

                        if (isDemandSection)
                        {
                            DepotManager.GetClient(num).Demande = int.Parse(data[1]);
                        }

                        if (isDepotSection)
                        {
                            DepotManager.DeclareDepot(num);
                            isDepotSection = false;
                        }
                        break;
                }
            }
        }

        private void ImportDAT(StreamReader reader)
        {
            string line;

            DepotManager = new DepotManager(101);
            Truck.Capacity = 50;

            while ((line = reader.ReadLine()) != null)
            {
                line = line.Trim();
                string[] data = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                int index = int.Parse(data[0]);

                Location pos = new Location(int.Parse(data[1]), int.Parse(data[2]));

                int demande = int.Parse(data[7]);

                DepotManager.AddClient(index, new Node(pos, demande));
            }

            DepotManager.DeclareDepot(0);
        }

        public DepotManager GetManager() => DepotManager;
    }
}