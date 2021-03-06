﻿/**
 * Auteur       : Cirieco Stefano
 * Version      : 1.0
 * Date         : 26.05.2020
 * Class        : IFA-P3B
 * Description  : This class represents a route for a truck that starts and ends at the Head
 */
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
        /// <param name="nodes"></param>
        /// <param name="nbNodes">Number of nodes to remove</param>
        public void RemoveNode(Node[] nodes)
        {
            Node previous = nodes[0].Previous;
            Node next = nodes[0] + nodes.Length;

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

        public void Paint(object sender, PaintEventArgs e)
        {
            for (Node n = Head.Next; n.Next != Head; n++)
            {
                e.Graphics.DrawLine(Pens.Black, n.DrawPos.X, n.DrawPos.Y, n.Next.DrawPos.X, n.Next.DrawPos.Y);
            }
        }
    }
}