using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CVRP_Viewer
{
    public partial class frmMain : Form
    {
        DepotManager depotManager;

        List<Truck> trucks;

        public frmMain()
        {
            InitializeComponent();

            DataImporter dataImporter = new DataImporter();

            //dataImporter.ImportFromFile(@".\1G2.DAT");
            dataImporter.ImportFromFile(@".\A-n37-k6.vrp");

            depotManager = dataImporter.GetManager();

            // Create list of trucks with a predetermined capacity
            trucks = new List<Truck>(depotManager.NbClients / 3);

            // Copy clients into a list so we can remove them once used
            List<Node> clients = new List<Node>(depotManager.Clients);

            // Delete depot from list
            clients.RemoveAt(depotManager.DepotIndex);

            Random rnd = new Random();

            for (int i = 0; i < trucks.Capacity; i++)
            {
                // Create truck with the depot as the head
                Truck truck = new Truck(depotManager.Depot);

                // Calculate how many clients are going to be in the truck's initiale route
                int clientsForTruck = clients.Count / (trucks.Capacity - trucks.Count);

                // Add clients to truck route and remove them from clients list
                for (int j = 0; j < clientsForTruck; j++)
                {
                    int randIndex = rnd.Next(clients.Count);
                    truck.AddNodeAfter(truck.Head, clients[randIndex]);
                    clients.RemoveAt(randIndex);
                }

                Paint += truck.Paint;

                trucks.Add(truck);
            }
        }

        private void frmMain_Paint(object sender, PaintEventArgs e)
        {
            //ShowOptRoute(e.Graphics);

            int size = 7;

            for (int i = 0; i < depotManager.NbClients; i++)
            {
                Node node = depotManager.GetClient(i);

                Rectangle rect = new Rectangle(node.DrawPos.X - size / 2, node.DrawPos.Y - size / 2, size, size);

                if (i == depotManager.DepotIndex)
                {
                    e.Graphics.FillEllipse(Brushes.Black, rect);
                }
                else
                {
                    e.Graphics.DrawEllipse(Pens.Black, rect);
                }
            }
        }

        public void ShowOptRoute(Graphics g)
        {
            List<int[]> locations = new List<int[]>();

            int[] tmp1 = { 7, 25, 35, 16 };
            locations.Add(tmp1);

            int[] tmp2 = { 18, 31, 19, 9, 21, 26 };
            locations.Add(tmp2);

            int[] tmp3 = { 14, 6, 36, 29, 24 };
            locations.Add(tmp3);

            int[] tmp4 = { 33, 2, 28, 23, 22, 12, 11, 10, 4 };
            locations.Add(tmp4);

            int[] tmp5 = { 13, 30, 15, 32, 27 };
            locations.Add(tmp5);

            int[] tmp6 = { 20, 8, 5, 3, 1, 34, 17 };
            locations.Add(tmp6);

            int totalCost = 0;

            Node depot = depotManager.Depot;

            foreach (int[] tmp in locations)
            {
                int routeCost = depot.CalcDistance(depotManager.GetClient(tmp[0]));
                routeCost += depot.CalcDistance(depotManager.GetClient(tmp[tmp.Length - 1]));

                for (int i = 0; i < tmp.Length - 1; i++)
                {
                    Node from = depotManager.GetClient(tmp[i]);
                    Node to = depotManager.GetClient(tmp[i + 1]);

                    routeCost += from.CalcDistance(to);

                    g.DrawLine(Pens.Black, from.Position.X * 4, from.Position.Y * 4, to.Position.X * 4, to.Position.Y * 4);
                }

                totalCost += routeCost;
                Console.WriteLine("Route cost = " + routeCost);
            }

            Console.WriteLine("Total cost = " + totalCost);
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            
        }
    }
}