using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
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
            var textureCoordinates = new List<Vector3>();
            var vertexNormals = new List<Vector3>();
            var faces = new List<List<int>>();

            foreach (var rawLine in objfile.Split('\n', '\r'))
            {
                var line = rawLine.Trim().ToLowerInvariant();
                if (string.IsNullOrWhiteSpace(line)) continue;
                if (line.StartsWith('#')) continue;
                var code = line.Split(' ', options: StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                switch (code[0])
                {
                    case "vt":
                        break;
                    case "vn":
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
                            var succ = true;
                            var list = new List<int>();
                            for (int i = 1;  i < code.Length; i++) {
                                var split = code[i].Split('/');
                                //TODO: Texture coordinate and vertex normal parsing
                                succ &= int.TryParse(split?[0], out var ind);
                                if (ind > 0)
                                    list.Add(ind - 1); //В .OBJ-файлах индексация вершин начинается с 1.
                                else
                                {
                                    list.Add(ind);
                                }
                            }
                            if (succ) faces.Add(list);
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
            return new PrimitiveShape() { polygons = faces.Select(x => x.ToArray()).ToArray(), Vertices = vertices.Select(t => new Vector3(t.X, t.Y, t.Z) / t.W).ToArray() };
            
        }
    }
}
