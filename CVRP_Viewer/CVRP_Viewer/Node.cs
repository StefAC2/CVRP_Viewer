using System;

namespace CVRP_Viewer
{
    public class Node
    {
        // Fields
        int amp = 4;

        // Properties
        public Location Position, DrawPos;
        public int Demande;
        public Node Next;

        // Constructors
        public Node(Location pos, int demande)
        {
            this.Position = pos;
            this.DrawPos = new Location(pos.X * amp, pos.Y * amp);
            this.Demande = demande;
        }

        public Node(Location pos) : this(pos, 0) { }

        // Methodes

        public override string ToString()
        {
            return $"Position: ({Position.X,3},{Position.Y,3}); Capacity: {Demande,3}";
        }

        public int CalcDistance(Node other)
        {
            int a = this.Position.X - other.Position.X;
            int b = this.Position.Y - other.Position.Y;

            return (int)Math.Round(Math.Sqrt(Math.Pow(a, 2) + Math.Pow(b, 2)), 0);
        }

        public Node CycleThrew(Node node)
        {
            if (this == node)
            {
                return this;
            }
            else
            {
                return this.Next.CycleThrew(node);
            }
        }

        public Node Clone()
        {
            return new Node(this.Position, this.Demande);
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

        public static bool operator ==(Node left, Node right) => left.Equals(right);
        public static bool operator !=(Node left, Node right) => !left.Equals(right);

        public override bool Equals(object obj)
        {
            return Position.Equals(((Node)obj).Position);
        }
    }
}