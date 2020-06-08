/**
 * Auteur       : Cirieco Stefano
 * Version      : 1.0
 * Date         : 25.05.2020
 * Class        : IFA-P3B
 * Description  : This class represents the clients and depot
 */
using System;

namespace CVRP_Viewer
{
    public class Node
    {
        // Properties
        public Location Position, DrawPos;
        public int Demande, Index;
        public Node Next, Previous;

        // Constructors
        public Node(Location pos, int index, int demande)
        {
            this.Position = pos;

            int amp = 4;
            this.DrawPos = new Location(pos.X * amp, pos.Y * amp);
 
            this.Demande = demande;
            this.Index = index;
        }

        public Node(Location pos, int index) : this(pos, index, 0) { }

        // Methodes
        public override string ToString()
        {
            return $"Position: {Position}; Demande: {Demande,3}";
        }

        public int CalcDistance(Node other)
        {
            int a = this.Position.X - other.Position.X;
            int b = this.Position.Y - other.Position.Y;

            return (int)Math.Round(Math.Sqrt(Math.Pow(a, 2) + Math.Pow(b, 2)), 0);
        }
        
        public static Node operator ++(Node node) => node.Next;
        public static Node operator +(Node node, int nb)
        {
            for (int i = 0; i < nb; i++)
            {
                node++;
            }

            return node;
        }
    }
}