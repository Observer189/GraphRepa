using System;
using Microsoft.Xna.Framework;

namespace Lab6_9;

public static class DrawUtilities
{
             /// <summary>
            /// 
            /// </summary>
            /// <param name="buffer">Ожидается первая координата Y, вторая - X</param>
            /// <param name="line"></param>
            /// <param name="scale"></param>
            /// <param name="offset"></param>
            /// <param name="color"></param>
            public static void DrawLine(Color[,] buffer, (Vector2 begin, Vector2 end) line, float scale, Vector2 offset, Color color)
            {
                (var x1, var y1) = (line.begin * scale + offset).ToPoint();
                (var x2, var y2) = (line.end * scale + offset).ToPoint();
    
                int dx = Math.Abs(x2 - x1);
                int dy = Math.Abs(y2 - y1);
                int sx = (x1 < x2) ? 1 : -1;
                int sy = (y1 < y2) ? 1 : -1;
                int err = dx - dy;
                var bufferlimX = buffer.GetLength(1);
                var bufferlimY = buffer.GetLength(0);
    
                while (true)
                {
                    if (x1 >= 0 && x1 < bufferlimX && y1 >= 0 && y1 < bufferlimY)
                        buffer[y1, x1] = color;
                    //Console.WriteLine($"{x1}, {y1}");
                    if (x1 == x2 && y1 == y2)
                    {
                        break;
                    }
    
                    int e2 = 2 * err;
                    if (e2 > -dy)
                    {
                        err -= dy;
                        x1 += sx;
                    }
                    if (e2 < dx)
                    {
                        err += dx;
                        y1 += sy;
                    }
                }
            }
}