/**
 * Auteur       : Cirieco Stefano
 * Version      : 1.0
 * Date         : 25.05.2020
 * Class        : IFA-P3B
 * Description  : This class assembles the clients, depot and trucks, and it solve the CVRP
 */
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace CVRP_Viewer
{
    public class DepotManager
    {
        // Properties
        private Node[] Clients;

        public Node Depot => Clients[DepotIndex];
        public int DepotIndex;
        public int NbClients => Clients.Length;

        public List<Truck> Trucks;

        // Constructor
        public DepotManager(int nbClients)
        {
            Clients = new Node[nbClients];
        }

        // Methodes
        public void AddClient(Node client)
        {
            Clients[client.Index] = client;
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

        public void CreateRandomRoutes()
        {
            // Create list of trucks with a predetermined capacity
            Trucks = new List<Truck>(NbClients / 3);

            // Copy clients into a list so we can remove them once used
            List<Node> clients = new List<Node>(Clients);

            // Delete depot from list
            clients.RemoveAt(DepotIndex);

            Random rnd = new Random();

            for (int i = 0; i < Trucks.Capacity; i++)
            {
                // Create truck with the depot as the head
                Truck truck = new Truck(Depot, i);

                // Calculate how many clients are going to be in the truck's initiale route
                int clientsForTruck = clients.Count / (Trucks.Capacity - Trucks.Count);

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
                        if (clients.Count != 1)
                        {
                            j--;
                        }
                        else
                        {
                            foreach (Truck t in Trucks)
                            {
                                if (t.CalcCapacity() + clients[randIndex].Demande <= Truck.Capacity)
                                {
                                    t.AddNodeAfter(t.Head, clients[randIndex]);
                                    clients.RemoveAt(randIndex);
                                    break;
                                }
                            }
                        }
                    }
                }

                Trucks.Add(truck);
            }
        }

        public void Solve(frmMain frm)
        {
            for (int nb = 3; nb > 0; nb--)
            {
                int totalCost = TotalCost(), oldTotalCost = 0;

                while (totalCost != oldTotalCost)
                {
                    oldTotalCost = totalCost;

                    List<Movement> movements = new List<Movement>();

                    for (int i = 0; i < NbClients; i++)
                    {
                        if (i != DepotIndex)
                        {
                            // Get Node
                            List<Node> nodes = new List<Node>
                            {
                                GetClient(i)
                            };

                            for (int j = 1; j < nb; j++)
                            {
                                Node n = nodes[j - 1].Next;

                                if (n.Position != Depot.Position)
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
                            Truck currentTruck = Trucks[truckIndex];

                            // Find the node that comes before the current one in the trucks route
                            Node previous = nodes[0].Previous;

                            // Calculate cost of route before we remove the node
                            int beforeRemoval = currentTruck.CalcCost();

                            currentTruck.RemoveNode(nodes.ToArray());

                            // Calculate cost of route after we removed the node
                            int afterRemoval = currentTruck.CalcCost();

                            Movement movement = new Movement
                            {
                                Nodes = nodes.ToArray(),
                                OriginalTruck = truckIndex,
                                Cost = int.MaxValue
                            };

                            for (int j = 0; j < Trucks.Count; j++)
                            {
                                Truck newTruck = Trucks[j];

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
                                    newTruck.AddNodeAfter(tmp, nodes.ToArray());

                                    int newCost = afterRemoval + newTruck.CalcCost();

                                    // Calculate the difference in cost before and after modifications
                                    int cost = newCost - oldCost;

                                    if (movement.OriginalTruck == j)
                                    {
                                        cost /= 2;
                                    }

                                    if (cost < movement.Cost)
                                    {
                                        movement.Cost = cost;
                                        movement.NewPrevious = tmp;
                                        movement.NewTruck = j;
                                    }

                                    newTruck.RemoveNode(nodes.ToArray());

                                    tmp++;
                                } while (tmp != newTruck.Head);
                            }

                            movements.Add(movement);

                            // Put nodes back where they were
                            Rollback(truckIndex, previous, nodes.ToArray());
                        }
                    }

                    movements.Sort();

                    if (movements.Count > 0 && movements[0].Cost < 0)
                    {
                        ApplyMovement(movements[0]);
                    }

                    frm.Refresh();

                    totalCost = TotalCost();
                }
            }

            Console.WriteLine(ShowRoutes());
        }

        private string ShowRoutes()
        {
            string routes = "";

            int counter = 1;

            foreach (Truck truck in Trucks)
            {
                if (truck.Head == truck.Head.Next)
                {
                    continue;
                }

                string route = $"Route #{counter++} (Cost {truck.CalcCost(),3}):";

                for (Node n = truck.Head.Next; n != truck.Head; n++)
                {
                    route += $" {n.Index,2}";
                }

                routes += route + Environment.NewLine;
            }

            routes += "Total cost: " + TotalCost();

            return routes;
        }

        /// <summary>
        /// Searches all trucks for the given node
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private int FindTruck(Node node)
        {
            while (node.Position != Depot.Position)
            {
                node++;
            }

            return (node as DepotNode).TruckId;
        }

        public int TotalCost()
        {
            int sum = 0;

            foreach (Truck truck in Trucks)
            {
                sum += truck.CalcCost();
            }

            return sum;
        }

        public void Rollback(int truck, Node previous, Node[] nodes)
        {
            Trucks[truck].AddNodeAfter(previous, nodes);
        }

        public void ApplyMovement(Movement movement)
        {
            Trucks[movement.OriginalTruck].RemoveNode(movement.Nodes);

            Trucks[movement.NewTruck].AddNodeAfter(movement.NewPrevious, movement.Nodes);
        }

        public void OptRoutes()
        {
            Trucks = new List<Truck>();

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

            Node depot = Depot;

            foreach (int[] tmp in locations)
            {
                Truck truck = new Truck(depot, -1);

                for (int i = tmp.Length - 1; i >= 0; i--)
                {
                    truck.AddNodeAfter(truck.Head, GetClient(tmp[i]));
                }

                Trucks.Add(truck);
            }

            Console.WriteLine(ShowRoutes());
        }

        public void Paint(object sender, PaintEventArgs e)
        {
            int size = 7;

            for (int i = 0; i < NbClients; i++)
            {
                Node node = GetClient(i);

                Rectangle rect = new Rectangle(node.DrawPos.X - size / 2, node.DrawPos.Y - size / 2, size, size);

                if (i == DepotIndex)
                {
                    e.Graphics.FillEllipse(Brushes.Black, rect);
                }
                else
                {
                    e.Graphics.DrawEllipse(Pens.Black, rect);
                }
            }

            foreach (Truck truck in Trucks)
            {
                truck.Paint(sender, e);
            }
        }
    }
}