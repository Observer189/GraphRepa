using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab6_9
{
    /// <summary>
    /// Более медленная реализация, позволяющая использовать полигоны с произвольным количеством вершин
    /// </summary>
    public struct PrimitiveShape
    {
        /// <summary>
        /// Координаты вершин в локальных к объекту вершинах
        /// Y вверх, X враво, Z на нас
        /// </summary>
        public Vector3[] vertixes;
        public int[][] polygons;
        /// <summary>
        /// Матрица, выполняющая сдвиги относительно центра мира.
        /// </summary>
        public Matrix transformationMatrix = Matrix.Identity;

        public PrimitiveShape()
        {
        }
        public static PrimitiveShape Cube()
        {
            var v = new Vector3[] { new(-1, -1, -1), new(1, -1, -1), new(1, -1, 1), new(-1, -1, 1), new(-1, 1, -1), new(1, 1, -1), new(1, 1, 1), new(-1, 1, 1), };
            int[][] tr = new [] { new[] { 0, 1, 2, 3 }, new[] { 0, 1, 5, 4 } , new[] { 1, 2, 6, 5 } , new[] { 2, 3, 7, 6 }, new[] { 0, 3, 7, 4 } , new[] {4, 5, 6, 7 } };
            return new PrimitiveShape() { vertixes = v, polygons = tr };
        }
        public static PrimitiveShape OXYZLines(float scale = 1)
        {
            var v = new Vector3[] { new(0, 0, 0), new(1, 0, 0), new(0, 1, 0), new(0, 0, 1)};
            int[][] tr = new[] { new[] { 0, 1}, new[] {0, 2 }, new[] { 0, 3 } };
            return new PrimitiveShape() { vertixes = v, polygons = tr, transformationMatrix = Matrix.Identity * Matrix.CreateScale(scale) };
        }
    }
}
