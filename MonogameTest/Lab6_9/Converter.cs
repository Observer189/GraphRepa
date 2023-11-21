using Microsoft.Xna.Framework;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab6_9
{
    public static class Converter
    {
        //https://en.wikipedia.org/wiki/Wavefront_.obj_file
        public static PrimitiveShape DotObjToPrimitiveShape(string objfile)
        {
            var vertices = new List<Vector4>();
            var textureCoordinates = new List<Vector2>();
            var vertexNormals = new List<Vector3>();
            var faces = new List<List<int>>();
            var facesNormals = new List<List<int>>();
            var facesTextures = new List<List<int>>();

            foreach (var rawLine in objfile.Split('\n', '\r'))
            {
                var line = rawLine.Trim().ToLowerInvariant();
                if (string.IsNullOrWhiteSpace(line)) continue;
                if (line.StartsWith('#')) continue;
                var code = line.Split(' ', options: StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                switch (code[0])
                {
                    case "vt":
                        {
                            var v2 = new Vector2() { Y = 0 };
                            var succ = float.TryParse(code?[1], System.Globalization.CultureInfo.InvariantCulture, out v2.X);
                            if (code.Length >= 3)
                            {
                                succ &= float.TryParse(code?[2], System.Globalization.CultureInfo.InvariantCulture, out v2.Y);
                            }
                            if (succ)    
                                textureCoordinates.Add(v2);
                        }
                        break;
                    case "vn":
                        {
                            var vec3 = new Vector3();
                            var succ = float.TryParse(code?[1], System.Globalization.CultureInfo.InvariantCulture, out vec3.X);
                            succ &= float.TryParse(code?[2], System.Globalization.CultureInfo.InvariantCulture, out vec3.Y);
                            succ &= float.TryParse(code?[3], System.Globalization.CultureInfo.InvariantCulture, out vec3.Z);
                            vertexNormals.Add(vec3);
                        }
                        break;
                    case "vp":
                        break;
                    case "v":
                        {
                            if (code.Length < 4) continue;
                            var vec4 = new Vector4() { W = 1.0f };
                            var succ = float.TryParse(code?[1], System.Globalization.CultureInfo.InvariantCulture, out vec4.X);
                            succ &= float.TryParse(code?[2], System.Globalization.CultureInfo.InvariantCulture, out vec4.Y);
                            succ &= float.TryParse(code?[3], System.Globalization.CultureInfo.InvariantCulture, out vec4.Z);
                            if (code.Length >= 5) {
                                succ &= float.TryParse(code?[4], System.Globalization.CultureInfo.InvariantCulture, out vec4.W);
                            }
                            if (succ) vertices.Add(vec4);
                        }
                        break;
                    case "f":
                        {
                            var coordsucc = true;
                            var normalsucc = true;
                            var texsucc = true;

                            var faceCoords = new List<int>();
                            var faceNormals = new List<int>();
                            var faceTexs = new List<int>();
                            for (int i = 1;  i < code.Length; i++) {
                                var split = code[i].Split('/');
                                //TODO: Texture coordinate and vertex normal parsing
                                coordsucc &= int.TryParse(split?[0], out var ind);
                                texsucc &= int.TryParse(split?[1], out var texind);
                                normalsucc &= int.TryParse(split?[2], out var normind);

                                faceCoords.Add(ind - 1);
                                faceTexs.Add(texind - 1);
                                faceNormals.Add(normind - 1);

                            }
                            if (coordsucc) { faces.Add(faceCoords);
                                facesNormals.Add(faceNormals);
                                facesTextures.Add(faceTexs);
                             };

                        }
                        break;
                    case "l":
                        break;
                    case "mtllib":
                        break;
                    case "usemtl":
                        break;
                    case "o":
                        break;
                    case "g":
                        break;
                    case "s":
                        break;
                    default:
                        Console.Error.WriteLine($"Unable to parse line: {line}");
                        break;
                };
            }
            Vector2[] textureCoordinates_ = null;
            Vector3[] vertexNormals_ = null;
            int[][] facesNormals_ = null;
            int[][] facesTextures_ = null;
            if (vertexNormals.Count > 0)
            {
                vertexNormals_ = vertexNormals.ToArray();
                facesNormals_ = facesNormals.Select(x => x.ToArray()).ToArray();
            }
            if (textureCoordinates.Count > 0)
            {
                textureCoordinates_ = textureCoordinates.ToArray();
                facesTextures_ = facesTextures.Select(x => x.ToArray()).ToArray();
            }
            return new PrimitiveShape() { faces = faces.Select(x => x.ToArray()).ToArray(),
                Vertices = vertices.Select(t => new Vector3(t.X, t.Y, t.Z) / t.W).ToArray(),
                facesVertNormals = facesNormals_,
                facesTexs = facesTextures_,
                texCoord = textureCoordinates_,
                vertNormals = vertexNormals_,

            };
            
        }
        public static string ObjectToText(Object3D obj)
        {
            var stringBuilder = new StringBuilder();
            foreach (var item in obj.Vertices)
            {
                stringBuilder.AppendLine($"v {item.X.ToString(CultureInfo.InvariantCulture)} {item.Y.ToString(CultureInfo.InvariantCulture)} {item.Z.ToString(CultureInfo.InvariantCulture)}");
            }
            foreach (var item in obj.faces)
            {
                stringBuilder.AppendLine($"f {item.v1 + 1} {item.v2 + 1} {item.v3 + 1}");
            }
            return stringBuilder.ToString();
        }
        public static string PrimitiveShapeToText(PrimitiveShape obj)
        {
            var stringBuilder = new StringBuilder();
            foreach (var item in obj.Vertices)
            {
                stringBuilder.AppendLine($"v {item.X.ToString(CultureInfo.InvariantCulture)} {item.Y.ToString(CultureInfo.InvariantCulture)} {item.Z.ToString(CultureInfo.InvariantCulture)}");
            }
            foreach (var item in obj.faces)
            {
                stringBuilder.Append("f");
                foreach (var p in item)
                {
                    stringBuilder.Append($" {p+1}");
                }
                stringBuilder.AppendLine();
            }
            return stringBuilder.ToString();
        }
        public static Microsoft.Xna.Framework.Color[,] LoadTextureByPath(string path)
        {
            var imgRed = Image.Load<Rgba32>(path);
            var arr = new Microsoft.Xna.Framework.Color[imgRed.Width,imgRed.Height];

            for (int i = 0; i < arr.GetLength(0); i++)
            {
                for (int j = 0; j < arr.GetLength(1); j++)
                {
                    arr[i, j] = new Microsoft.Xna.Framework.Color(imgRed[i, j].PackedValue);
                }
            }

            return arr;

        }
    }
}
