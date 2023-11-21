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
        private Vector3[] vertCoord;
        public Vector3[] vertNormals;
        public Vector2[]? texCoord;
        public (int v1, int v2, int v3)[] faces;
        public Vector3[]? facesNormals;
        public Color[,] texture = new[,] { { Color.White } };
        //Освещение
        public (int n1, int n2, int n3)[]? facesVertNormals;

        //Текстурирование
        public (int tx1, int tx2, int tx3)[]? facesTexs;

        public Color[]? colors;
        /// <summary>
        /// Матрица, выполняющая сдвиги относительно центра мира.
        /// </summary>
        private Matrix transformationMatrix = Matrix.Identity;
        public void GenerateRandomColors()
        {
            var r = new Random();
            var t = new Color[faces.Length];
            for (int i = 0; i < t.Length; i++)
            {
                t[i] = new Color(r.Next(255), r.Next(255), r.Next(255), 255);
            }
            colors = t;
        }
        public void GenerateSpecificColor(Color color)
        {
            var r = new Random();
            var t = new Color[faces.Length];
            for (int i = 0; i < t.Length; i++)
            {
                t[i] = color;
            }
            colors = t;
        }
        //Сгенерирует нормали для граней, если отсутствуют, 
        public void GenerateTriangleVertexNormals()
        {
            if (facesNormals is null)
                GenerateNormals();
            IEnumerable<int> getIndices((int, int, int) s)
            {
                yield return s.Item1;
                yield return s.Item2;
                yield return s.Item3;
            }
            //Ключ - индекс вершины, значение - сумма нормалей сторон, содержащих эту вершину 
            var t = new List<Vector3>(Enumerable.Repeat(Vector3.Zero, vertCoord.Length));
            for (int i = 0; i < facesNormals.Length; i++)
            {
                foreach (var item in getIndices(faces[i]))
                {
                    t[item] += facesNormals[i];
                }
            }
            for (int i = 0; i < t.Count; i++)
            {
                if (t[i] != Vector3.Zero)
                    t[i].Normalize();
            }
            facesVertNormals = faces.ToArray();
            vertNormals = t.ToArray();
        }
        public void GenerateNormals()
        {
            var vertices = this.vertCoord;
            facesNormals = faces.Select((x) =>
            {
                
                var t1 = vertices[x.v1] - vertices[x.v2];
                var t2 = vertices[x.v2] - vertices[x.v3];
                var n = Vector3.Cross(t1, t2);
                if (n != Vector3.Zero)
                    n.Normalize();
                return n;
            }
            ).ToArray(); 
        }
        //public void Generate
        public Object3D()
        {
            
        }
        public static Object3D Cube()
        {
            var v = new Vector3[] { new(-1, -1, -1), new(1, -1, -1), new(1, -1, 1), new(-1, -1, 1), new(-1, 1, -1), new(1, 1, -1), new(1, 1, 1), new(-1, 1, 1), };
            var tr = new (int, int, int)[] {(0, 1, 2), (0, 3, 2), (0, 1, 5), (0, 4, 5), (1, 2, 6), (1, 6, 5), (4, 5, 6), (4, 7, 6), (0, 3, 4), (3, 4, 7), (2, 3, 7), (2, 7, 6) };
            return new Object3D() { vertCoord = v, faces =  tr };
        }

        public Vector3[] Vertices
        {
            get => vertCoord;
            set => vertCoord = value;
        }

        public Matrix TransformationMatrix
        {
            get => transformationMatrix;
            set => transformationMatrix = value;
        }
    }
}
