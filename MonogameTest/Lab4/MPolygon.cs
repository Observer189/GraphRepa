using Microsoft.Xna.Framework;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab4
{
    public class MPolygon
    {
        public MPolygon()
        {

        }
        public MPolygon(List<Vector2> vertixes)
        {
            this.Vertixes = vertixes;
        }
        public MPolygon(List<Vector2> vertixes, Matrix2 matrix)
        {
            this.Vertixes = vertixes;
            this.LocalTransformations = matrix;
        }
        public List<Vector2> Vertixes { get; set; } = new List<Vector2>();
        public Matrix2 LocalTransformations { get; set; } = Matrix2.Identity;
    }
}
