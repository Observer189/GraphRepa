using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab6_9
{
    public struct Object3D
    {
        /// <summary>
        /// Координаты вершин в локальных к объекту вершинах
        /// Y вверх, X враво, Z на нас
        /// </summary>
        public Vector3[] vertixes; 
        public (int v1, int v2, int v3)[] triangles;
        /// <summary>
        /// Матрица, выполняющая сдвиги относительно центра мира.
        /// </summary>
        public Matrix transformationMatrix = Matrix.Identity;

        public Object3D()
        {
            
        }
        public static Object3D Cube()
        {
            var v = new Vector3[] { new(-1, -1, -1), new(1, -1, -1), new(1, -1, 1), new(-1, -1, 1), new(-1, 1, -1), new(1, 1, -1), new(1, 1, 1), new(-1, 1, 1), };
            var tr = new (int, int, int)[] {(0, 1, 2), (0, 3, 2), (0, 1, 5), (0, 4, 5), (1, 2, 6), (1, 6, 5), (4, 5, 6), (4, 7, 6), (0, 3, 4), (3, 4, 7), (2, 3, 7), (2, 7, 6) };
            return new Object3D() { vertixes = v, triangles =  tr };
        }
    }
}
