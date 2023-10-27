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
    public struct PrimitiveShape:IEditableShape
    {
        /// <summary>
        /// Координаты вершин в локальных к объекту вершинах
        /// Y вверх, X враво, Z на нас
        /// </summary>
        private Vector3[] vertices;
        public int[][] polygons;
        /// <summary>
        /// Матрица, выполняющая сдвиги относительно центра мира.
        /// </summary>
        private Matrix transformationMatrix = Matrix.Identity;


        public PrimitiveShape()
        {
            transformationMatrix = Matrix.Identity;
        }
        public static PrimitiveShape Cube()
        {
            var v = new Vector3[] { new(-1, -1, -1), new(1, -1, -1), new(1, -1, 1), new(-1, -1, 1), new(-1, 1, -1), new(1, 1, -1), new(1, 1, 1), new(-1, 1, 1), };
            int[][] tr = new [] { new[] { 0, 1, 2, 3 }, new[] { 0, 1, 5, 4 } , new[] { 1, 2, 6, 5 } , new[] { 2, 3, 7, 6 }, new[] { 0, 3, 7, 4 } , new[] {4, 5, 6, 7 } };
            return new PrimitiveShape() { vertices = v, polygons = tr };
        }
        public static PrimitiveShape OXYZLines(float scale = 1)
        {
            var v = new Vector3[] { new(0, 0, 0), new(1, 0, 0), new(0, 1, 0), new(0, 0, 1)};
            int[][] tr = new[] { new[] { 0, 1}, new[] {0, 2 }, new[] { 0, 3 } };
            return new PrimitiveShape() { vertices = v, polygons = tr, transformationMatrix = Matrix.Identity * Matrix.CreateScale(scale) };
        }

        public static PrimitiveShape Tetrahedron()
        {
             var v = new Vector3[] { new(0, 2, 0), new(1, 0, -1 / 1), new(-1, 0, -1 / 1), new(0, 0, 1) };
             int[][] tr = new [] { new [] { 0, 1, 2 },  new [] { 0, 1, 3 }, new [] { 0, 2, 3 }, new [] { 1, 2, 3 }  };
             return new PrimitiveShape() { vertices = v, polygons = tr };
        }

        public static PrimitiveShape Octahedron()
        {
            var v = new Vector3[] { new(0, 1, 0), new(1, 0, 0), new(-1, 0, 0), new(0, 0, 1), new(0, 0, -1), new(0, -1, 0)  };
            int[][] tr = new [] {new [] { 0, 1, 3 }, new [] { 0, 2, 3 }, new [] { 0, 1, 4 }, new [] { 0, 2, 4 }, new [] { 5, 1, 3 }, 
                          new [] { 5, 2, 3 }, new [] { 5, 1, 4 },  new [] { 5, 2, 4 } };

            return new PrimitiveShape() { vertices = v, polygons = tr };
        }

        public static PrimitiveShape Icosahedron()
        {

            var v = new Vector3[]
            {
        new(-1, 1.5f, 0),      //  0
        new(1, 1.5f, 0),       //  1
        new(-1, -1.5f, 0),     //  2
        new(1, -1.5f, 0),      //  3
        new(0, -1, 1.5f),      //  4
        new(0, 1, 1.5f),       //  5
        new(0, -1, -1.5f),     //  6
        new(0, 1, -1.5f),      //  7
        new(1.5f, 0, -1),      //  8
        new(1.5f, 0, 1),       //  9
        new(-1.5f, 0, -1),     //  10
        new(-1.5f, 0, 1)       //  11
            };

            int[][] tr = new []
            {
        new [] { 0, 11, 5 },    //  0
        new [] { 0, 5, 1 },     //  1
        new [] { 0, 1, 7 },     //  2
        new [] { 0, 7, 10 },    //  3
        new [] { 0, 10, 11 },   //  4
        new [] { 1, 5, 9 },     //  5
        new [] { 5, 11, 4 },    //  6
        new [] { 11, 10, 2 },   //  7
        new [] { 10, 7, 6 },    //  8
        new [] { 7, 1, 8 },     //  9
        new [] { 3, 9, 4 },     //  10
        new [] { 3, 4, 2 },     //  11
        new [] { 3, 2, 6 },     //  12
        new [] { 3, 6, 8 },     //  13
        new [] { 3, 8, 9 },     //  14
        new [] { 4, 9, 5 },     //  15
        new [] { 2, 4, 11 },    //  16
        new [] { 6, 2, 10 },    //  17
        new [] { 8, 6, 7 },     //  18
        new [] { 9, 8, 1 }      //  19
            };

            return new PrimitiveShape() { vertices = v, polygons = tr };
        }

        public static PrimitiveShape Dodecahedron()
        {
            throw new NotImplementedException();
        }

        public Vector3[] Vertices
        {
            get => vertices;
            set => vertices = value;
        }

        public Matrix TransformationMatrix
        {
            get => transformationMatrix;
            set => transformationMatrix = value;
        }
    }
}
