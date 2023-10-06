using Microsoft.Xna.Framework;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab4
{
    public class DirPolygon
    {
        public List<Vector2> vertices;
        public List<(int,int)> edges;

        public DirPolygon()
        {
            vertices = new List<Vector2>();
            edges = new List<(int, int)>();
        }

        public void AddPoint(Vector2 p)
        {
            if (vertices.Count == 0)
            {
                vertices.Add(p);
            }
            else if(vertices.Count==1)
            {
                vertices.Add(p);
                edges.Add((0,1));
            }
            else
            {
                if(vertices.Count!=2)
                    edges.RemoveAt(edges.Count-1);
                vertices.Add(p);
                edges.Add((vertices.Count-2,vertices.Count-1));
                edges.Add((vertices.Count-1,0));
            }
        }

        public bool IsInside(Vector2 point)
        {
            if (vertices.Count < 3)
            {
                return false;
            }
            var initialValue = Utilities.PointOnTheRight(point,vertices[edges[0].Item1],vertices[edges[0].Item2]);
            for (int i = 1; i < edges.Count; i++)
            {
                var val = Utilities.PointOnTheRight(point,vertices[edges[i].Item1],vertices[edges[i].Item2]);
                if (val != initialValue)
                {
                    return false;
                }
            }
            return true;
        }

        public Matrix2 LocalTransformations { get; set; } = Matrix2.Identity;
    }
}
