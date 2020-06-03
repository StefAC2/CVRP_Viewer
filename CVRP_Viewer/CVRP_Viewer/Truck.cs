using System.Drawing;
using System.Windows.Forms;

namespace CVRP_Viewer
{
    public class Truck
    {
        public static int Capacity;

        // Properties
        public Node Head;

        // Constructor
        public Truck(Node head, int truckId)
        {
            this.Head = new DepotNode(head.Position, head.Index, truckId);
            this.Head.Next = this.Head;
            this.Head.Previous = this.Head;
        }

        // Methodes
        /// <summary>
        /// Add node after the chosen previous node
        /// </summary>
        /// <param name="previous"></param>
        /// <param name="node"></param>
        public void AddNodeAfter(Node previous, Node node)
        {
            node.Next = previous.Next;
            node.Next.Previous = node;
            node.Previous = previous;
            previous.Next = node;
        }

        public void AddNodeAfter(Node previous, Node[] nodes)
        {
            Node first = nodes[0], last = nodes[nodes.Length - 1];

            last.Next = previous.Next;
            last.Next.Previous = last;
            first.Previous = previous;
            previous.Next = first;
        }

        /// <summary>
        /// Removes node
        /// </summary>
        /// <param name="node"></param>
        /// <param name="nbNodes">Number of nodes to remove</param>
        public void RemoveNode(Node node, int nbNodes)
        {
            Node previous = node.Previous;
            Node next = node + nbNodes;

            previous.Next = next;
            next.Previous = previous;
        }

        public int CalcCapacity()
        {
            int demandeSum = 0;

            for (Node n = Head.Next; n.Position != Head.Position; n++)
            {
                demandeSum += n.Demande;
            }

            return demandeSum;
        }

        public int CalcCost()
        {
            Node node = Head;

            int totalCost = 0;

            do
            {
                totalCost += node.CalcDistance(node.Next);

                node++;
            } while (node.Position != Head.Position);

            return totalCost;
        }

        public bool IsNodeInRoute(Node node)
        {
            for (Node n = Head.Next; n != Head; n++)
            {
                if (n == node)
                {
                    return true;
                }
            }

            return false;
        }

        public void Paint(object sender, PaintEventArgs e)
        {
            for (Node n = Head.Next; n.Next != Head; n++)
            {
                e.Graphics.DrawLine(Pens.Black, n.DrawPos.X, n.DrawPos.Y, n.Next.DrawPos.X, n.Next.DrawPos.Y);
            }
        }
    }
}