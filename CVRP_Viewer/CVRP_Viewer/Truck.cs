using System.Drawing;
using System.Windows.Forms;

namespace CVRP_Viewer
{
    public class Truck
    {
        public static int Capacity;

        // Properties
        public Node Head;

        // Constructors
        public Truck(Node head)
        {
            this.Head = new Node(head.Position);
            this.Head.Next = this.Head;
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
            previous.Next = node;
        }

        /// <summary>
        /// Removes node
        /// </summary>
        /// <param name="node"></param>
        /// <param name="nbNodes">Number of nodes to remove</param>
        public void RemoveNode(Node node, int nbNodes)
        {
            Node previous = Head;
            Node tmp = previous.Next;

            while (tmp != Head)
            {
                if (tmp == node)
                {
                    tmp += nbNodes;

                    previous.Next = tmp;
                    break;
                }

                previous = tmp;
                tmp = previous.Next;
            }
        }

        public int CalcCapacity()
        {
            int demandeSum = 0;

            for (Node n = Head.Next; n != Head; n++)
            {
                demandeSum += n.Demande;
            }

            return demandeSum;
        }

        public int CalcCost()
        {
            Node previous = Head;
            Node tmp = previous.Next;

            int totalCost = 0;

            while (tmp != Head)
            {
                totalCost += previous.CalcDistance(tmp);

                previous = tmp;
                tmp = previous.Next;
            }

            return totalCost;
        }

        public void Paint(object sender, PaintEventArgs e)
        {
            for (Node n = Head.Next; n.Next != Head; n++)
            {
                e.Graphics.DrawLine(Pens.Black, n.DrawPos.X, n.DrawPos.Y, n.Next.DrawPos.X, n.Next.DrawPos.Y);
            }
        }

        public Truck Clone()
        {
            Truck truck = new Truck(this.Head.Clone());

            Node previous = truck.Head;

            for (Node n = this.Head.Next; n != this.Head; n++)
            {
                Node clone = n.Clone();

                truck.AddNodeAfter(previous, clone);

                previous = clone;
            }

            return truck;
        }
    }
}