using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab6_9
{
    /// <summary>
    /// Более быстрая реализация, но с использованием только и только треугольных полигонов
    /// Но кому другие полигоны нужны в 3D-моделях?
    /// </summary>
    public struct Object3D:IEditableShape
    {
        /// <summary>
        /// Координаты вершин в локальных к объекту вершинах
        /// Y вверх, X враво, Z от нас
        /// </summary>
        private Vector3[] vertices; 
        public (int v1, int v2, int v3)[] triangles;
        public Color[]? colors;
        public Vector3[]? facesNormals;
        /// <summary>
        /// Матрица, выполняющая сдвиги относительно центра мира.
        /// </summary>
        private Matrix transformationMatrix = Matrix.Identity;
        public void GenerateRandomColors()
        {
            var r = new Random();
            var t = new Color[triangles.Length];
            for (int i = 0; i < t.Length; i++)
            {
                t[i] = new Color(r.Next(255), r.Next(255), r.Next(255), 255);
            }
            colors = t;
        }
        public void GenerateNormals()
        {
            var vertices = this.vertices;
            facesNormals = triangles.Select((x) =>
            {
                
                var t1 = vertices[x.v1] - vertices[x.v2];
                var t2 = vertices[x.v2] - vertices[x.v3];
                return Vector3.Cross(t1, t2);
            }
            ).ToArray(); 
        }
        public Object3D()
        {
            
        }
        public static Object3D Cube()
        {
            var v = new Vector3[] { new(-1, -1, -1), new(1, -1, -1), new(1, -1, 1), new(-1, -1, 1), new(-1, 1, -1), new(1, 1, -1), new(1, 1, 1), new(-1, 1, 1), };
            var tr = new (int, int, int)[] {(0, 1, 2), (0, 3, 2), (0, 1, 5), (0, 4, 5), (1, 2, 6), (1, 6, 5), (4, 5, 6), (4, 7, 6), (0, 3, 4), (3, 4, 7), (2, 3, 7), (2, 7, 6) };
            return new Object3D() { vertices = v, triangles =  tr };
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
