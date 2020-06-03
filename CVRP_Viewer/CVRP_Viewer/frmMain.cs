using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
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

            CreateRandomRoutes();

            //ShowOptRoute();
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

        private void CreateRandomRoutes()
        {
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
                Truck truck = new Truck(depotManager.Depot, i);

                // Calculate how many clients are going to be in the truck's initiale route
                int clientsForTruck = clients.Count / (trucks.Capacity - trucks.Count);

                // Add clients to truck route and remove them from clients list
                for (int j = 0; j < clientsForTruck; j++)
                {
                    int randIndex = rnd.Next(clients.Count);
                    if (truck.CalcCapacity() + clients[randIndex].Demande <= Truck.Capacity)
                    {
                        truck.AddNodeAfter(truck.Head, clients[randIndex]);
                        clients.RemoveAt(randIndex);
                    }
                    else
                    {
                        j--;
                    }
                }

                Paint += truck.Paint;

                trucks.Add(truck);
            }
        }

        public void ShowOptRoute()
        {
            trucks = new List<Truck>();

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

            Node depot = depotManager.Depot;

            foreach (int[] tmp in locations)
            {
                Truck truck = new Truck(depot, -1);

                for (int i = tmp.Length - 1; i >= 0; i--)
                {
                    truck.AddNodeAfter(truck.Head, depotManager.GetClient(tmp[i]));
                }

                Paint += truck.Paint;

                trucks.Add(truck);
            }

            Console.WriteLine(TotalCost());
        }

        private void Rollback(int truck, Node previous, Node node)
        {
            trucks[truck].AddNodeAfter(previous, node);
        }

        private void Rollback(int truck, Node previous, Node[] nodes)
        {
            trucks[truck].AddNodeAfter(previous, nodes);
        }

        private void ApplyMovement(Movement movement)
        {
            trucks[movement.OriginalTruck].RemoveNode(movement.Node, movement.NbNodes);

            trucks[movement.NewTruck].AddNodeAfter(movement.NewPrevious, movement.Node);
        }

        private int TotalCost()
        {
            int sum = 0;

            foreach (Truck truck in trucks)
            {
                sum += truck.CalcCost();
            }

            return sum;
        }

        /// <summary>
        /// Searches all trucks for the given node
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private int FindTruck(Node node)
        {
            while (node.Position != depotManager.Depot.Position)
            {
                node++;
            }

            return (node as DepotNode).TruckId;
        }

        private void AlgoWith1()
        {
            for (int i = 0; i < depotManager.NbClients; i++)
            {
                if (i != depotManager.DepotIndex)
                {
                    // Get Node
                    Node node = depotManager.GetClient(i);

                    // Find truck index of node
                    int truckIndex = FindTruck(node);
                    Truck currentTruck = trucks[truckIndex];

                    // Find the node that comes before the current one in the trucks route
                    Node previous = currentTruck.GetPreviousNode(node);

                    List<Movement> movements = new List<Movement>();

                    // Calculate cost of route before we remove the node
                    int beforeRemoval = currentTruck.CalcCost();

                    currentTruck.RemoveNode(node, 1);

                    // Calculate cost of route after we removed the node
                    int afterRemoval = currentTruck.CalcCost();

                    for (int j = 0; j < trucks.Count; j++)
                    {
                        Truck newTruck = trucks[j];

                        int truckCapacity = newTruck.CalcCapacity();

                        // if truck doesn't have enough room for the node, then skip this truck
                        if (truckCapacity + node.Demande > Truck.Capacity)
                        {
                            continue;
                        }

                        int oldCost = beforeRemoval + newTruck.CalcCost();

                        Node tmp = newTruck.Head;

                        do
                        {
                            Movement movement = new Movement
                            {
                                NbNodes = 1,
                                Node = node,
                                OriginalPrevious = previous,
                                OriginalTruck = truckIndex,
                                NewPrevious = tmp,
                                NewTruck = j
                            };

                            newTruck.AddNodeAfter(tmp, node);

                            int newCost = afterRemoval + newTruck.CalcCost();

                            movement.Cost = newCost - oldCost;

                            // We add the movement only if we save money
                            if (movement.Cost < 0)
                            {
                                if (movement.OriginalTruck == movement.NewTruck)
                                {
                                    movement.Cost /= 2;
                                }

                                movements.Add(movement);
                            }

                            newTruck.RemoveNode(node, 1);

                            tmp++;
                        } while (tmp != newTruck.Head);
                    }

                    Rollback(truckIndex, previous, node);

                    movements.Sort();

                    if (movements.Count > 0)
                    {
                        ApplyMovement(movements[0]);
                    }
                    Refresh();

                    Console.WriteLine($"{i} : {TotalCost()}");
                }
            }
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            //AlgoWith1();

            int totalCost = TotalCost(), oldTotalCost = 0;

            for (int nb = 2; nb > 0; nb--)
            {
                while (totalCost != oldTotalCost)
                {
                    oldTotalCost = totalCost;

                    for (int i = 0; i < depotManager.NbClients; i++)
                    {
                        if (i != depotManager.DepotIndex)
                        {
                            // Get Node
                            List<Node> nodes = new List<Node>
                            {
                                depotManager.GetClient(i)
                            };

                            for (int j = 1; j < nb; j++)
                            {
                                Node n = nodes[j - 1].Next;

                                if (n.Position != depotManager.Depot.Position)
                                {
                                    nodes.Add(n);
                                }
                                else
                                {
                                    break;
                                }
                            }

                            // Find truck index of node
                            int truckIndex = FindTruck(nodes[0]);
                            Truck currentTruck = trucks[truckIndex];

                            // Find the node that comes before the current one in the trucks route
                            Node previous = currentTruck.GetPreviousNode(nodes[0]);
                            
                            List<Movement> movements = new List<Movement>();

                            // Calculate cost of route before we remove the node
                            int beforeRemoval = currentTruck.CalcCost();

                            currentTruck.RemoveNode(nodes[0], nodes.Count);

                            // Calculate cost of route after we removed the node
                            int afterRemoval = currentTruck.CalcCost();

                            for (int j = 0; j < trucks.Count; j++)
                            {
                                Truck newTruck = trucks[j];

                                int truckCapacity = newTruck.CalcCapacity();
                                int nodesDemande = nodes.Sum(x => x.Demande);

                                // if truck doesn't have enough room for the node, then skip this truck
                                if (truckCapacity + nodesDemande > Truck.Capacity)
                                {
                                    continue;
                                }

                                int oldCost = beforeRemoval + newTruck.CalcCost();

                                Node tmp = newTruck.Head;

                                do
                                {
                                    Movement movement = new Movement
                                    {
                                        NbNodes = nodes.Count,
                                        Node = nodes[0],
                                        OriginalPrevious = previous,
                                        OriginalTruck = truckIndex,
                                        NewPrevious = tmp,
                                        NewTruck = j
                                    };

                                    newTruck.AddNodeAfter(tmp, nodes.ToArray());

                                    int newCost = afterRemoval + newTruck.CalcCost();

                                    // Calculate the difference in cost before and after modifications
                                    movement.Cost = newCost - oldCost;

                                    // We add the movement only if we save money
                                    if (movement.Cost < 0)
                                    {
                                        if (movement.OriginalTruck == movement.NewTruck)
                                        {
                                            movement.Cost /= 2;
                                        }

                                        movements.Add(movement);
                                    }

                                    newTruck.RemoveNode(nodes[0], nodes.Count);

                                    tmp++;
                                } while (tmp != newTruck.Head);
                            }

                            Rollback(truckIndex, previous, nodes.ToArray());

                            movements.Sort();

                            if (movements.Count > 0)
                            {
                                ApplyMovement(movements[0]);
                            }
                            Refresh();

                            Console.WriteLine($"{i} : {TotalCost()}");
                        }
                    }

                    totalCost = TotalCost();
                }
            }

            /*for (int i = 0; i < trucks.Count; i++)
            {
                Truck currentTruck = trucks[i];

                Node previous = currentTruck.Head;

                for (Node node = previous.Next; node != currentTruck.Head; previous++)
                {
                    node = previous.Next;

                    if (previous == node)
                    {
                        trucks.RemoveAt(i);
                        i--;

                        continue;
                    }

                    List<Movement> movements = new List<Movement>();

                    // Calculate cost of route before we remove the node
                    int beforeRemoval = currentTruck.CalcCost();

                    currentTruck.RemoveNode(node, 1);

                    // Calculate cost of route after we removed the node
                    int afterRemoval = currentTruck.CalcCost();

                    for (int j = 0; j < trucks.Count; j++)
                    {
                        Truck newTruck = trucks[j];

                        int oldCost = beforeRemoval + newTruck.CalcCost();

                        Node tmp = newTruck.Head;

                        do
                        {
                            Movement movement = new Movement
                            {
                                NbNodes = 1,
                                Node = node,
                                OriginalPrevious = previous,
                                OriginalTruck = i,
                                NewPrevious = tmp,
                                NewTruck = j
                            };

                            newTruck.AddNodeAfter(tmp, node);

                            int newCost = afterRemoval + newTruck.CalcCost();

                            movement.Cost = newCost - oldCost;

                            // We add the movement only if we save money
                            if (movement.Cost < 0)
                            {
                                if (movement.OriginalTruck == movement.NewTruck)
                                {
                                    movement.Cost /= 2;
                                }

                                movements.Add(movement);
                            }

                            newTruck.RemoveNode(node, 1);

                            tmp++;
                        } while (tmp != newTruck.Head);
                    }

                    Rollback(i, previous, node);

                    movements.Sort();

                    if (movements.Count > 0)
                    {
                        ApplyMovement(movements[0]);
                    }
                    Refresh();

                    Console.WriteLine($"{i} : {TotalCost()}");
                }
            }*/
        }
    }
}