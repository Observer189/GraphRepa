using System;
using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace Lab5_3;

public class BezierPoint
{

    public Vector2 this[int index]
    {
        get
        {
            if (index == 1)
            {
                return position;
            }
            if(index == 0)
            {
                return new Vector2((float)(position.X + Math.Cos(angle) * dist0), (float)(position.Y + Math.Sin(angle) * dist0));
            }
            if(index == 2)
            {
                return new Vector2((float)(position.X + Math.Cos(angle+Math.PI) * dist2), (float)(position.Y + Math.Sin(Math.PI+angle) * dist2));
            }

            return Vector2.Zero;
        }

        set
        {
            if (index == 1)
            {
                position = value;
            }
            else if (index == 0)
            {
                var diff = value - position;
                var a = diff.ToAngle()-Math.PI/2;
                angle = (float)a;
                dist0 = Vector2.Distance(position, value);
            }
            else if (index == 2)
            {
                var diff = value - position;
                var a = diff.ToAngle()-Math.PI/2;
                angle = (float)(a+Math.PI);
                dist2 = Vector2.Distance(position, value);
            }
        }
    }
    private Vector2 position;
    private float angle;
    private float dist0;
    private float dist2;

    public BezierPoint(Vector2 pos)
    {
        position = pos;
        angle = (float)(Math.PI/3);
        dist0 = 25f;
        dist2 = 25f;
    }
}