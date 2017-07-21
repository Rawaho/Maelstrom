namespace WorldServer.Game.Map
{
    public struct Vector2G
    {
        public int X { get; }
        public int Y { get; }

        public Vector2G(int x, int y)
        {
            X = x;
            Y = y;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            return this == (Vector2G)obj;
        }

        public static bool operator ==(Vector2G v1, Vector2G v2)
        {
            return v1.X == v2.X && v1.Y == v2.Y;
        }

        public static bool operator !=(Vector2G v1, Vector2G v2)
        {
            return !(v1 == v2);
        }

        public override int GetHashCode()
        {
            return X ^ Y;
        }
    }
}
