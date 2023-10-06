using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace Lab4;

public static class Utilities
{
    public static bool PointOnTheRight(Vector2 point, Vector2 p1, Vector2 p2)
    {
        point -= p1;
        p2 -= p1;

        return p2.Y * point.X - p2.X * point.Y < 0;
    }
    /// <summary>
    /// Возвращает null, если отрезки не пересекаются.
    /// </summary>
    /// <param name="a">Точка первого отрезка</param>
    /// <param name="b">Точка первого отрезка</param>
    /// <param name="c">Точка второго отрезка</param>
    /// <param name="d">Точка второго отрезка</param>
    /// <returns></returns>
    public static Vector2? CrossingCoordinates(Vector2 a,  Vector2 b, Vector2 c, Vector2 d)
    {
        var n = (d - c).PerpendicularClockwise();
        var t = n.Dot(a - c) / n.Dot(a - b);
        if (t >= 0 && t <= 1)
        {
            return (1 - t) * a + t * b;
        }
        else return null;
    }
}