namespace CVRP_Viewer
{
    public class Location
    {
        public int X, Y;

        public Location(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }

        public override string ToString()
        {
            return $"({X,3}, {Y,3})";
        }

        public override bool Equals(object obj)
        {
            Location other = obj as Location;

            return this.X.Equals(other.X) && this.Y.Equals(other.Y);
        }
    }
}