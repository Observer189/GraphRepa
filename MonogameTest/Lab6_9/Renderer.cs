using Microsoft.Xna.Framework;
using MLEM.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Lab6_9
{
    public record struct Vertice(Vector2 v, VerticeData d)
    {
        
    }
    public struct VerticeData
    {
        public float Z;
        public float Brightness;
        public float textureCoordinateX;
        public float textureCoordinateY;

        public override string ToString()
        {
            return $"{{Z:{Z}, Brightness: {Brightness}, Tex: [{textureCoordinateX}, {textureCoordinateY}] }}";
        }

        public static VerticeData operator + (VerticeData lhs, VerticeData rhs) 
        {
            return new VerticeData
            {
                Z = lhs.Z + rhs.Z,
                Brightness = lhs.Brightness + rhs.Brightness,
                textureCoordinateX = lhs.textureCoordinateX + rhs.textureCoordinateX,
                textureCoordinateY = lhs.textureCoordinateY + rhs.textureCoordinateY,
            };
        }
        public static VerticeData operator -(VerticeData lhs, VerticeData rhs)
        {
            return new VerticeData
            {
                Z = lhs.Z - rhs.Z,
                Brightness = lhs.Brightness - rhs.Brightness,
                textureCoordinateX = lhs.textureCoordinateX - rhs.textureCoordinateX,
                textureCoordinateY = lhs.textureCoordinateY - rhs.textureCoordinateY,
            };
        }
        public static VerticeData operator *(VerticeData lhs, float rhs)
        {
            return new VerticeData
            {
                Z = lhs.Z * rhs,
                Brightness = lhs.Brightness * rhs,
                textureCoordinateX = lhs.textureCoordinateX * rhs,
                textureCoordinateY = lhs.textureCoordinateY * rhs,
            };
        }

    }
    public static class Renderer
    {
        static public (Color c, float depth)[,] NewCanvas(int x, int y)
        {
            var t = new (Color c, float depth)[y, x];
            ClearCanvas(t);
            return t;
        } 
        public static void ClearCanvas((Color c, float depth)[,] canvas)
        {
            for (int i = 0; i < canvas.GetLength(0); i++)
            {
                for (int j = 0; j < canvas.GetLength(1); j++)
                {
                    canvas[i, j] = (new Color(0, 0, 0, 0), float.MaxValue);
                }
            }
        }
        public static void DrawOnCanvas((Color c, float depth)[,] canvas, Camera camera, LightSource ls, in Object3D obj, Color framecolor)
        {
            //var l = ProjectWireFrameWithCameraMatrixAndProjectionMatrix(camera.GetTransformationMatrix(), camera.ProjectionMatrix, obj);
            var v = Vector3.Transform(new Vector3(0, 0, 1), Matrix.Invert(camera.RotationMatrix));
            Draw3DObj(canvas, camera.GetTransformationMatrix(), camera.ProjectionMatrix, ls, v, obj, camera.Scale, new Vector2 { X = canvas.GetLength(1) / 2.0f, Y = canvas.GetLength(0) / 2.0f }, Color.Gray);
            //DrawWireFrame(canvas, l, camera.Scale, new Vector2 { X = canvas.GetLength(1) / 2.0f, Y = canvas.GetLength(0) / 2.0f }, framecolor);

        }
        public static void DrawOnCanvas((Color c, float depth)[,] canvas, Camera camera, LightSource ls, in PrimitiveShape prim, Color framecolor)
        {
            var l = ProjectWireFrameWithCameraMatrixAndProjectionMatrix(camera.GetTransformationMatrix(), camera.ProjectionMatrix, prim);
            DrawWireFrame(canvas, l, camera.Scale, new Vector2 { X = canvas.GetLength(1) / 2.0f, Y = canvas.GetLength(0) / 2.0f }, framecolor);
        }
        
        //В Z-координате хранится высота
        static List<(Vector3 begin, Vector3 end)> ProjectWireFrameWithCameraMatrixAndProjectionMatrix(in Matrix cameraMatrix, in Matrix projectionMatrix, in PrimitiveShape shape)
        {
            var wf = new List<(Vector3 begin, Vector3 end)>();
            var vertices = shape.Vertices;
            var resMatrix = shape.TransformationMatrix * cameraMatrix;
            foreach (var polygon in shape.faces)
            {
                for (int i = 0; i < polygon.Length; i++)
                {
                    var i1 = (i + 1 == polygon.Length) ? 0 : i + 1;
                    var v1 = new Vector4(vertices[polygon[i]], 1);
                    var v2 = new Vector4(vertices[polygon[i1]], 1);
                    var begin = Vector4.Transform(v1, resMatrix);
                    var end = Vector4.Transform(v2, resMatrix);
                    var begin_projected = Vector4.Transform(begin, projectionMatrix);
                    var end_projected = Vector4.Transform(end, projectionMatrix);

                    var vec3begin = new Vector3 { X = begin_projected.X / begin_projected.W, 
                        Y = -begin_projected.Y / begin_projected.W, 
                        Z = begin.Z / begin.W };
                    var vec3end = new Vector3 { X = end_projected.X / end_projected.W, 
                        Y = -end_projected.Y / end_projected.W, 
                        Z = end.Z / end.W };

                    wf.Add((vec3begin, vec3end));
                }
            }
            return wf;
        }
        //В Z-координате хранится высота
        static List<(Vector3 begin, Vector3 end)> ProjectWireFrameWithCameraMatrixAndProjectionMatrix(in Matrix cameraMatrix, in Matrix projectionMatrix, in Object3D shape)
        {
            var wf = new List<(Vector3 begin, Vector3 end)>();
            var vertixes = shape.Vertices;
            var resMatrix = shape.TransformationMatrix * cameraMatrix;
            foreach (var polygon in shape.faces)
            {

                var v1 = new Vector4(vertixes[polygon.v1], 1);
                var v2 = new Vector4(vertixes[polygon.v2], 1);
                var v3 = new Vector4(vertixes[polygon.v3], 1);
                var v1Tr = Vector4.Transform(v1, resMatrix);
                var v2Tr = Vector4.Transform(v2, resMatrix);
                var v3Tr = Vector4.Transform(v3, resMatrix);

                var v1TrPj = Vector4.Transform(v1Tr, projectionMatrix);
                var v2TrPj = Vector4.Transform(v2Tr, projectionMatrix);
                var v3TrPj = Vector4.Transform(v3Tr, projectionMatrix);

                var v1v2 = new Vector3 { X = v1TrPj.X / v1TrPj.W, Y = -v1TrPj.Y / v1TrPj.W, Z=v1Tr.Z / v1Tr.W };
                var v2v2 = new Vector3 { X = v2TrPj.X / v2TrPj.W, Y = -v2TrPj.Y / v2TrPj.W, Z=v2Tr.Z / v2Tr.W };
                var v3v2 = new Vector3 { X = v3TrPj.X / v3TrPj.W, Y = -v3TrPj.Y / v3TrPj.W, Z=v3Tr.Z / v3Tr.W };


                wf.Add((v1v2, v2v2));
                wf.Add((v2v2, v3v2));
                wf.Add((v3v2, v1v2));


            }
            return wf;
        }
        static void Draw3DObj((Color c, float depth)[,] canvas, in Matrix cameraMatrix, in Matrix projectionMatrix, LightSource ls, Vector3 vectorOfLooking, in Object3D shape, float scale, Vector2 offset, Color c)
        {
            var wf = new List<(Vector3 begin, Vector3 end)>();
            var vertixes = shape.Vertices;
            var resMatrix = shape.TransformationMatrix * cameraMatrix;
            var normals = shape.facesNormals;
            var lightingPos = ls.Position;
            var texCoords = shape.texCoord;
            for (int i = 0; i < shape.faces.Length; i++)
            {
                var tex = shape.facesTexs?[i] ?? (0, 0, 0);
                //Вычисляем вектор нормали
                var normal = normals?[i] ?? Vector3.Zero;
                //var n = new Vector4(normal, 1);
                //var s = new Vector4(Vector3.Zero, 1);
                //var nv = Vector4.Transform(n, shape.TransformationMatrix);
                //var sv = Vector4.Transform(s, shape.TransformationMatrix);
                //var nv_ = new Vector3(nv.X, nv.Y, nv.Z)/nv.W;
                //var ns_ = new Vector3(sv.X, sv.Y, sv.Z) / sv.W;
                //var diff = nv_ - ns_;
                var diff = Vector3.TransformNormal(normal, shape.TransformationMatrix);
                var len = (diff.Length() * vectorOfLooking.Length());
                if (len == 0) { len = 1; }
                var dot = Vector3.Dot(diff, vectorOfLooking) / len;
                if (dot > MathF.PI / 8)
                {
                    continue;
                }
                //
                var polygon = shape.faces[i];
                var v1 = new Vector4(vertixes[polygon.v1], 1);
                var v2 = new Vector4(vertixes[polygon.v2], 1);
                var v3 = new Vector4(vertixes[polygon.v3], 1);
                var v1Tr = Vector4.Transform(v1, resMatrix);
                var v2Tr = Vector4.Transform(v2, resMatrix);
                var v3Tr = Vector4.Transform(v3, resMatrix);

                var v1TrPj = Vector4.Transform(v1Tr, projectionMatrix);
                var v2TrPj = Vector4.Transform(v2Tr, projectionMatrix);
                var v3TrPj = Vector4.Transform(v3Tr, projectionMatrix);

                var v1v2 = new Vector3 { X = v1TrPj.X / v1TrPj.W, Y = -v1TrPj.Y / v1TrPj.W, Z = v1Tr.Z / v1Tr.W };
                var v2v2 = new Vector3 { X = v2TrPj.X / v2TrPj.W, Y = -v2TrPj.Y / v2TrPj.W, Z = v2Tr.Z / v2Tr.W };
                var v3v2 = new Vector3 { X = v3TrPj.X / v3TrPj.W, Y = -v3TrPj.Y / v3TrPj.W, Z = v3Tr.Z / v3Tr.W };

                if (v1v2.Z < 0 && v2v2.Z < 0 && v3v2.Z < 0) { continue; }
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                float Brightness(Vector4 vertexPos, Vector3 lighting, Vector3 normal)
                {

                    var v = new Vector3(vertexPos.X, vertexPos.Y, vertexPos.Z) / vertexPos.W;
                    var t = lighting - v;
                    var l = t.Length() * normal.Length();
                    if (MathF.Abs(l) < 0.00001) l = 1.0f; 
                    return (Vector3.Dot(t, normal) / l );
                }
                var q = shape.facesVertNormals[i];
                var t1 = texCoords?[tex.tx1] ?? Vector2.Zero;
                var t2 = texCoords?[tex.tx2] ?? Vector2.Zero;
                var t3 = texCoords?[tex.tx3] ?? Vector2.Zero;

                drawTriangle(canvas, 
                    new Vertice(v1v2.ToVector2(), new VerticeData() { Z = v1v2.Z, 
                        Brightness = Brightness(v1, lightingPos, shape.vertNormals[q.n1]), textureCoordinateX = t1.X, textureCoordinateY = t1.Y }),
                    new Vertice(v2v2.ToVector2(), new VerticeData() { Z = v2v2.Z, 
                        Brightness = Brightness(v2, lightingPos, shape.vertNormals[q.n2]), textureCoordinateX = t2.X, textureCoordinateY = t2.Y }),
                    new Vertice(v3v2.ToVector2(), new VerticeData() { Z = v3v2.Z, 
                        Brightness = Brightness(v3, lightingPos, shape.vertNormals[q.n3]), textureCoordinateX = t3.X, textureCoordinateY = t3.Y }), 
                    scale, offset, shape.colors?[i] ?? c,
                    ls.Color, shape.texture
                    );

            }

        }
        static void DrawWireFrame((Color c, float depth)[,] canvas, List<(Vector3 begin, Vector3 end)> lines, float scale, Vector2 offset, Color color)
        {

            foreach (var line in lines)
            {
                DrawLine(canvas, line, scale, offset, color);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="buffer">Ожидается первая координата Y, вторая - X</param>
        /// <param name="line"></param>
        /// <param name="scale"></param>
        /// <param name="offset"></param>
        /// <param name="color"></param>
        public static void DrawLine((Color c, float depth)[,] canvas, (Vector3 begin, Vector3 end) line, float scale, Vector2 offset, Color color)
        {
            //(var x1, var y1) = (line.begin * scale + offset).ToPoint();
            //(var x2, var y2) = (line.end * scale + offset).ToPoint();
            var z1 = line.begin.Z;
            var z2 = line.end.Z;
            
            if (z1 < 0 && z2 < 0) return;
            var x1 = (int)(line.begin.X * scale + offset.X);
            var y1 = (int)(line.begin.Y * scale + offset.Y);
            var x2 = (int)(line.end.X * scale + offset.X);
            var y2 = (int)(line.end.Y * scale + offset.Y);

            var stX1 = x1;
            var stY1 = y1;

            int dx = Math.Abs(x2 - x1);
            int dy = Math.Abs(y2 - y1);
            int sx = (x1 < x2) ? 1 : -1;
            int sy = (y1 < y2) ? 1 : -1;
            int err = dx - dy;
            var bufferlimX = canvas.GetLength(1);
            var bufferlimY = canvas.GetLength(0);


            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            float Portion(int x1, int y1, float totalLen ,int xP, int yP) {
                return Math.Clamp(MathF.Sqrt((float)((x1 - xP) * (x1 - xP) + (y1 - yP) * (y1 - yP))) / totalLen, 0.0f, 1.0f);
            }

            var totalLen = MathF.Sqrt(MathF.Pow(x1 - x2, 2) + MathF.Pow(y1 - y2, 2));
            while (true)
            {
                var portion = Portion(stX1, stY1, totalLen, x1, y1);
                var zRes = z1 * (1 - portion) + z2 * portion;
                if (x1 >= 0 && x1 < bufferlimX && y1 >= 0 && y1 < bufferlimY && canvas[y1, x1].depth > zRes && zRes > 0)
                    canvas[y1, x1] = (color, zRes);
                //Console.WriteLine($"{x1}, {y1}");
                if (x1 == x2 && y1 == y2)
                {
                    break;
                }

                int e2 = 2 * err;
                if (e2 > -dy)
                {
                    err -= dy;
                    x1 += sx;
                }
                if (e2 < dx)
                {
                    err += dx;
                    y1 += sy;
                }
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static void DrawLineScaleless((Color c, float depth)[,] canvas, Vertice begin, Vertice end,  Color color, Color lightColor, Color[,] tex)
        {
            //(var x1, var y1) = (line.begin * scale + offset).ToPoint();
            //(var x2, var y2) = (line.end * scale + offset).ToPoint();
            //var z1 = line.begin.Z;
            //var z2 = line.end.Z;

            if (begin.d.Z < 0 && end.d.Z < 0) return;
            var bufferlimX = canvas.GetLength(1);
            var bufferlimY = canvas.GetLength(0);
            var x1 = (int)(begin.v.X);
            var y1 = (int)(begin.v.Y);
            var x2 = (int)(end.v.X);
            var y2 = (int)(end.v.Y);
            if (x1 > bufferlimX || x1 < 0 ) { return; }
            if (x2 > bufferlimX || x2 < 0 ) { return; }

            var stX1 = x1;
            var stY1 = y1;

            int dx = Math.Abs(x2 - x1);
            int dy = Math.Abs(y2 - y1);
            int sx = (x1 < x2) ? 1 : -1;
            int sy = (y1 < y2) ? 1 : -1;
            int err = dx - dy;

            var texX = tex.GetLength(0);
            var texY = tex.GetLength(1);


            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            float Portion(int x1, int y1, float totalLen, int xP, int yP)
            {
                return Math.Clamp(MathF.Sqrt((float)((x1 - xP) * (x1 - xP) + (y1 - yP) * (y1 - yP))) / totalLen, 0.0f, 1.0f);
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            Color InterpolateColor(float brightness, Vector3 PointColor, Vector3 lighting)
            {
                return new Color((brightness + 1.2f) * (brightness - 1.0f) / (-1.2f) * PointColor + (brightness + 1.2f) * brightness / (2.0f * 1.2f) * lighting); 
                //if (brightness < 0.0f) { return Color.Black; }
                //else return lighting;
            }
            var totalLen = MathF.Sqrt(MathF.Pow(x1 - x2, 2) + MathF.Pow(y1 - y2, 2));
            var lightingColorVec = lightColor.ToVector3();
            while (true)
            {
                var portion = Portion(stX1, stY1, totalLen, x1, y1);
                var zRes = begin.d * (1 - portion) + end.d * portion;
                // 
                if (x1 >= 0 && x1 < bufferlimX && y1 >= 0 && y1 < bufferlimY && zRes.Z > 0 && canvas[y1, x1].depth > zRes.Z)
                {
                    var tx = (int)Math.Clamp(zRes.textureCoordinateX * texX, 0, texX - 1);
                    var ty = (int)Math.Clamp(texY - zRes.textureCoordinateY * texY, 0, texY - 1);

                    var resCol = color.ToVector3() * tex[tx, ty].ToVector3();
                    canvas[y1, x1] = (InterpolateColor(zRes.Brightness, resCol, lightingColorVec), zRes.Z);
                }
                //Console.WriteLine($"{x1}, {y1}");
                if (x1 == x2 && y1 == y2)
                {
                    break;
                }

                int e2 = 2 * err;
                if (e2 > -dy)
                {
                    err -= dy;
                    x1 += sx;
                }
                if (e2 < dx)
                {
                    err += dx;
                    y1 += sy;
                }
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static float ReplaceNaN(float num, float repl)
        {
            if (float.IsNaN(num)) { return repl; }
            else { return num; }
        }
        static void fillBottomFlatTriangle((Color c, float depth)[,] canvas,
            Vertice v1,
            Vertice v2,
            Vertice v3, Color c, Color lightColor, Color[,] tex)
        {
 
            float invslope1 = (v2.v.X - v1.v.X) / (v2.v.Y - v1.v.Y);
            float invslope2 = (v3.v.X - v1.v.X) / (v3.v.Y - v1.v.Y);

            float curx1 = v1.v.X;
            float curx2 = v1.v.X;

            for (int scanlineY = (int)v1.v.Y; scanlineY <= v2.v.Y; scanlineY++)
            {
                var zleft = (v1.d + (v2.d - v1.d) * ReplaceNaN( (scanlineY - v1.v.Y) / (v2.v.Y - v1.v.Y), 1f));
                var zRight = (v1.d + (v3.d - v1.d) * ReplaceNaN((scanlineY - v1.v.Y) / (v3.v.Y - v1.v.Y), 1f));

                DrawLineScaleless(canvas, 
                    new Vertice(new Vector2((int)curx1, scanlineY), zleft), 
                    new Vertice(new Vector2((int)curx2, scanlineY), zRight), 
                    c, lightColor, tex);
                curx1 += invslope1;
                curx2 += invslope2;
            }
        }
        static void fillTopFlatTriangle((Color c, float depth)[,] canvas, Vertice v1,
            Vertice v2,
            Vertice v3, Color c, Color lightColor, Color[,] tex)
        {
            float invslope1 = (v3.v.X - v1.v.X) / (v3.v.Y - v1.v.Y);
            float invslope2 = (v3.v.X - v2.v.X) / (v3.v.Y - v2.v.Y);

            float curx1 = v3.v.X;
            float curx2 = v3.v.X;

            for (int scanlineY = (int)(v3.v.Y); scanlineY > v1.v.Y; scanlineY--)
            {
                var zleft = (v1.d + (v3.d - v1.d) * ReplaceNaN((scanlineY - v1.v.Y) / (v3.v.Y - v1.v.Y), 1f));
                var zRight = (v2.d + (v3.d - v2.d) * ReplaceNaN((scanlineY - v2.v.Y) / (v3.v.Y - v2.v.Y), 1f));

                DrawLineScaleless(canvas,
                    new Vertice(new Vector2((int)curx1, scanlineY), zleft),
                    new Vertice(new Vector2((int)curx2, scanlineY), zRight), 
                    c, lightColor, tex);
                curx1 -= invslope1;
                curx2 -= invslope2;
            }
        }
        static public void drawTriangle((Color c, float depth)[,] canvas, Vertice v1_, Vertice v2_, Vertice v3_, float scale, Vector2 offset, Color c, Color lightColor, Color[,] tex)
        {
            /* at first sort the three vertices by y-coordinate ascending so v1 is the topmost vertice */
            Vertice[] t = new[] { 
                new Vertice((v1_.v * scale + offset).ToPoint().ToVector2(), v1_.d),
                new Vertice((v2_.v * scale + offset).ToPoint().ToVector2(), v2_.d),
                new Vertice((v3_.v * scale + offset).ToPoint().ToVector2(), v3_.d) };
            Array.Sort(t, (x, y) => x.v.Y.CompareTo(y.v.Y));
            var (v1, v2, v3) = (t[0], t[1], t[2]);
            /* here we know that v1.y <= v2.y <= v3.y */
            /* check for trivial case of bottom-flat triangle */
            if (Math.Abs(v1.v.Y - v3.v.Y) > canvas.GetLength(0)) return;
            if ((int)v2.v.Y == (int)v3.v.Y)
            {
                fillBottomFlatTriangle(canvas, v1, v2, v3, c, lightColor, tex);
            }
            /* check for trivial case of top-flat triangle */
            else if ((int)v1.v.Y == (int)v2.v.Y)
            {
                fillTopFlatTriangle(canvas, v1, v2, v3, c, lightColor, tex);
            }
            else
            {
                /* general case - split the triangle in a topflat and bottom-flat one */
                Vector2 v4 = new Vector2(
                  (v1.v.X + ((float)(v2.v.Y - v1.v.Y) / (float)(v3.v.Y - v1.v.Y)) * (v3.v.X - v1.v.X)), 
                  v2.v.Y);
                var vd4 = (v1.d + (v3.d - v1.d) * ((float)(v2.v.Y - v1.v.Y) / (float)(v3.v.Y - v1.v.Y)));
                fillBottomFlatTriangle(canvas, v1, v2, new Vertice(v4, vd4), c, lightColor, tex);

                //fillBottomFlatTriangle(g, vt1, vt2, v4);
                fillTopFlatTriangle(canvas, v2, new Vertice(v4, vd4), v3, c, lightColor, tex);

                //fillTopFlatTriangle(g, vt2, v4, vt3);
            }
        }
    }
}
