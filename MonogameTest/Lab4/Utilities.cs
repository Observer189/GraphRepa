using Microsoft.Xna.Framework;

namespace Lab4;

public static class Utilities
{
    public static bool PointOnTheRight(Vector2 point, Vector2 p1, Vector2 p2)
    {
        point -= p1;
        p2 -= p1;

        return p2.Y * point.X - p2.X * point.Y < 0;
    }
}