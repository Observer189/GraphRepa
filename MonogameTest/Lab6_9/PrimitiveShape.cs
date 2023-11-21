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
        /// Y вверх, X враво, Z от нас
        /// </summary>
        private Vector3[] vertCoord;
        public int[][] faces;
        public Vector3[] vertNormals;
        public int[][]? facesVertNormals;

        public Vector3[]? facesNormals;
        public Vector2[]? texCoord;
        public Color[,] texture = new[,] { { Color.White } };
        //Освещение

        //Текстурирование
        public int[][]? facesTexs;
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
            return new PrimitiveShape() { vertCoord = v, faces = tr };
        }
        public static PrimitiveShape OXYZLines(float scale = 1)
        {
            var v = new Vector3[] { new(0, 0, 0), new(1, 0, 0), new(0, 1, 0), new(0, 0, 1)};
            int[][] tr = new[] { new[] { 0, 1}, new[] {0, 2 }, new[] { 0, 3 } };
            return new PrimitiveShape() { vertCoord = v, faces = tr, transformationMatrix = Matrix.Identity * Matrix.CreateScale(scale) };
        }

        public static PrimitiveShape Tetrahedron()
        {
             var v = new Vector3[] { new(0, 2, 0), new(1, 0, -1 / 1), new(-1, 0, -1 / 1), new(0, 0, 1) };
             int[][] tr = new [] { new [] { 0, 1, 2 },  new [] { 0, 1, 3 }, new [] { 0, 2, 3 }, new [] { 1, 2, 3 }  };
             return new PrimitiveShape() { vertCoord = v, faces = tr };
        }

        public static PrimitiveShape Octahedron()
        {
            var v = new Vector3[] { new(0, 1, 0), new(1, 0, 0), new(-1, 0, 0), new(0, 0, 1), new(0, 0, -1), new(0, -1, 0)  };
            int[][] tr = new [] {new [] { 0, 1, 3 }, new [] { 0, 2, 3 }, new [] { 0, 1, 4 }, new [] { 0, 2, 4 }, new [] { 5, 1, 3 }, 
                          new [] { 5, 2, 3 }, new [] { 5, 1, 4 },  new [] { 5, 2, 4 } };

            return new PrimitiveShape() { vertCoord = v, faces = tr };
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

            return new PrimitiveShape() { vertCoord = v, faces = tr };
        }

        public static PrimitiveShape Dodecahedron()
        {
            throw new NotImplementedException();
        }

        static List<Vector3> BuildSurfaceSegment(Func<float, float, float> function, float x0, float x1, float y0, float y1, int xSteps, int ySteps)
        {
            List<Vector3> surfacePoints = new List<Vector3>();

            float xStepSize = (x1 - x0) / xSteps;
            float yStepSize = (y1 - y0) / ySteps;

            for (int i = 0; i <= xSteps; i++)
            {
                for (int j = 0; j <= ySteps; j++)
                {
                    float x = x0 + i * xStepSize;
                    float y = y0 + j * yStepSize;
                    float z = function(x, y);

                    surfacePoints.Add(new Vector3(x, y, z));
                }
            }

            return surfacePoints;
        }

        static int[][] BuildSurfaceIndices(int xSteps, int ySteps)
        {
            int[][] indices = new int[xSteps * ySteps][];

            for (int i = 0; i < xSteps; i++)
            {
                for (int j = 0; j < ySteps; j++)
                {
                    int currentIndex = i * (ySteps + 1) + j;

                    // Соединение вершин для квадрата
                    indices[i * ySteps + j] = new int[]
                    {
                        currentIndex, currentIndex + 1, currentIndex + ySteps + 2, currentIndex + ySteps + 1
                    };
                }
            }

            return indices;
        }

        public static PrimitiveShape ModelGraphic(Func<float, float, float> function, float x0, float x1, float y0, float y1, int xSteps, int ySteps)
        {
            List<Vector3> surfacePoints = BuildSurfaceSegment(function, x0, x1, y0, y1, xSteps, ySteps);   
            int[][] surfaceIndices = BuildSurfaceIndices(xSteps, ySteps);

            return new PrimitiveShape { vertCoord = surfacePoints.ToArray(), faces = surfaceIndices };
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
        //Преобразует только треугольники и квадраты, потому что лень реализовывать алгоритм триангуляции
        public Object3D ToObject3D()
        {
            var triangles = new List<(int, int, int)>();
            var trianglesVertNormals = this.facesVertNormals is not null? new List<(int, int, int)>() : null;
            var trianglesTexs = this.facesTexs is not null ? new List<(int, int, int)>() : null;
            for (int i = 0; i < faces.Length; i++)
            {
                var poly = faces[i];
                var norms = facesVertNormals?[i];
                var texs = facesTexs?[i];
                switch (poly.Length)
                {
                    case 0:
                        break;
                    case 1:
                        triangles.Add((poly[0], poly[0], poly[0]));
                        if (norms is not null)
                            trianglesVertNormals?.Add((norms[0], norms[0], norms[0]));
                        if (texs is not null)
                            trianglesTexs?.Add((texs[0], texs[0], texs[0]));
                        break;
                    case 2:
                        triangles.Add((poly[0], poly[0], poly[1]));
                        if (norms is not null)
                            trianglesVertNormals?.Add((norms[0], norms[0], norms[1]));
                        if (texs is not null)
                            trianglesTexs?.Add((texs[0], texs[0], texs[1]));
                        break;
                    case 3:
                        triangles.Add((poly[0], poly[1], poly[2]));
                        if (norms is not null)
                            trianglesVertNormals?.Add((norms[0], norms[1], norms[2]));
                        if (texs is not null)
                            trianglesTexs?.Add((texs[0], texs[1], texs[2]));
                        break;
                    case 4:
                        triangles.Add((poly[0], poly[1], poly[3]));
                        if (norms is not null)
                            trianglesVertNormals?.Add((norms[0], norms[1], norms[3]));
                        if (texs is not null)
                            trianglesTexs?.Add((texs[0], texs[1], texs[3]));
                        triangles.Add((poly[1], poly[2], poly[3]));
                        if (norms is not null)
                            trianglesVertNormals?.Add((norms[1], norms[2], norms[3]));
                        if (texs is not null)
                            trianglesTexs?.Add((texs[1], texs[2], texs[3]));
                        break;
                    default:
                        Console.Error.WriteLine($"Polygons with length of {poly.Length} not supported");
                        break;
                }
            }
            return new Object3D() { TransformationMatrix = TransformationMatrix, 
                Vertices = Vertices, faces = triangles.ToArray(), vertNormals = vertNormals, texCoord = texCoord, texture = texture, facesNormals = facesNormals, facesTexs = trianglesTexs?.ToArray(), facesVertNormals = trianglesVertNormals?.ToArray()   };
        }
    }
}
