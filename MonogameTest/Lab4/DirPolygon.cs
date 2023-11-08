using Microsoft.Xna.Framework;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MLEM.Extensions;
using MonoGame.Extended.Shapes;

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
                /*for (int i = 0; i < edges.Count-1; i++)
                {
                    var t1 = Utilities.CrossingCoordinates(vertices[vertices.Count - 1],
                        p, vertices[edges[i].Item1], vertices[edges[i].Item2]);
                    var t2 = Utilities.CrossingCoordinates(vertices[0],
                        p, vertices[edges[i].Item1], vertices[edges[i].Item2]);
                    if (t1.HasValue)
                    {
                        if (Vector2.Distance(t1.Value,vertices[edges[i].Item1]) > 0.0001f && Vector2.Distance(t1.Value,vertices[edges[i].Item2]) > 0.0001f)
                        {
                            return;
                        }
                    }

                    if (t2.HasValue)
                    {
                        if (Vector2.Distance(t2.Value,vertices[edges[i].Item1]) > 0.0001f && Vector2.Distance(t2.Value,vertices[edges[i].Item2]) > 0.0001f)
                        {
                            return;
                        }
                    }
                }*/
                
                
                
                if(vertices.Count!=2)
                    edges.RemoveAt(edges.Count-1);
                vertices.Add(p);
                edges.Add((vertices.Count-2,vertices.Count-1));
                edges.Add((vertices.Count-1,0));
            }
        }

        public bool IsInside(Vector2 point)
        {
            return CheckInsideNonConvex(point);
        }

        private bool CheckInsideNonConvex(Vector2 point)
        {
            //Console.WriteLine(LocalTransformations.ToString());
            int intersections = 0;
            //Console.WriteLine($"p = {point.ToString()}");
            foreach (var edge in edges)
            {
                if (vertices[edge.Item1].X > point.X || vertices[edge.Item2].X > point.X)
                {
                    if (Utilities.CrossingCoordinates(point, new Vector2(point.X+1000000,point.Y),
                            vertices[edge.Item1],vertices[edge.Item2]).HasValue)
                    {
                        //Console.WriteLine($"Edge: p1={vertices[edge.Item1].ToString()}, p2 = {vertices[edge.Item2].ToString()}");
                        intersections++;
                    }
                }
            }
            //Console.WriteLine(intersections);
            return intersections % 2 != 0;
        }

        private bool CheckInsideConvex(Vector2 point)
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

        public DirPolygon GetTransformedCopy()
        {
            DirPolygon copy = new DirPolygon();
            copy.vertices = new List<Vector2>(vertices);
            copy.edges = new List<(int, int)>(edges);
            for (int i = 0; i < copy.vertices.Count; i++)
            {
                copy.vertices[i] = Vector3.Transform(new Vector3(vertices[i],0),LocalTransformations).ToVector2();
            }

            return copy;
        }

        public Matrix2 LocalTransformations { get; set; } = Matrix2.Identity;

        public List<DirPolygon> Triangulate()
        {
            var triangles = new List<DirPolygon>();
            int[] vertIndices = new int[vertices.Count];
            for (int i = 0; i < vertices.Count; i++)
            {
                vertIndices[i] = i;
            }
            vertIndices = vertIndices.OrderBy((v) => vertices[v].X).ToArray();

            bool[] upperChain = new bool[vertices.Count];

            int it = 0;
            while(edges[it].Item1 != vertIndices[0])
            {
                ++it;
            }
            //Помечаем все вершины верхней цепи
            while (edges[it].Item2 != vertIndices[^1])
            {
                upperChain[edges[it].Item2] = true;
                ++it;
                if (it == vertices.Count) it = 0;
            }
            //Остальные автоматически принадлежат нижней
           /* for (int i = 0; i < upperChain.Length; i++)
            {
                Console.WriteLine($"{i}) {upperChain[i]}");
            }*/
            Stack<int> stack = new Stack<int>();
            stack.Push(vertIndices[0]);
            stack.Push(vertIndices[1]);
            for (int i = 2; i < vertIndices.Length-1; i++)
            {
                //Если в одной цепочке
                if (upperChain[stack.Peek()] == upperChain[vertIndices[i]] || stack.Peek() == vertIndices[0])
                {
                    var top = stack.Pop();
                    while (stack.Count > 0 && Utilities.PointOnTheRight(vertices[top],vertices[vertIndices[i]], vertices[stack.Peek()]) == upperChain[top])
                    {
                        var tr = new DirPolygon
                        {
                            vertices = new List<Vector2> { vertices[vertIndices[i]], vertices[top],vertices[stack.Peek()]},
                            edges = new List<(int,int)> {(0,2),(2,1),(1,0)},
                            LocalTransformations = this.LocalTransformations
                        };
                        triangles.Add(tr);
                        top = stack.Pop();
                    }
                    stack.Push(top);
                    stack.Push(vertIndices[i]);
                }
                else
                {
                    int vl = stack.Peek();
                    int vt=-1;
                    while (stack.Count > 0)
                    {
                        //if (stack.Count != 1)
                        {
                            if (vt != -1)
                            {
                                var tr = new DirPolygon
                                {
                                    vertices = new List<Vector2> { vertices[vertIndices[i]], vertices[vt],vertices[stack.Peek()]},
                                    edges = new List<(int,int)> {(0,2),(2,1),(1,0)},
                                    LocalTransformations = this.LocalTransformations
                                };
                                triangles.Add(tr);
                            }
                            vt = stack.Pop();
                        }
                    }
                    stack.Push(vl);
                    stack.Push(vertIndices[i]);
                }
            }

            var last = stack.Pop();
            while (stack.Count > 0)
            {
                var tr = new DirPolygon
                {
                    vertices = new List<Vector2> { vertices[vertIndices[^1]], vertices[last],vertices[stack.Peek()]},
                    edges = new List<(int,int)> {(0,2),(2,1),(1,0)},
                    LocalTransformations = this.LocalTransformations
                };
                triangles.Add(tr);
                last = stack.Pop();
            }
            return triangles;
        }
    }
}
