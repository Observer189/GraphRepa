using Microsoft.Xna.Framework;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab5_1
{
    public class Parser
    {
        public string StartingExpression { get; private set; }
        public float StartingAngle { get; private set; }
        public string Expression { get; private set; }
        public List<(string, string)> Substitutions { get; private set; }
        public bool Randomness { get; set; }
        public Parser(string description) {
            var t = description.Split('\n').Select(x=>x.Trim()).ToArray();
            var begin = t[0];
            var beginParse = begin.Split(' ');
            Expression = StartingExpression = beginParse[0];
            RotationAngle = float.Parse(beginParse[1], System.Globalization.CultureInfo.InvariantCulture);
            StartingAngle = float.Parse(beginParse[2], System.Globalization.CultureInfo.InvariantCulture);
            Substitutions = new List<(string, string)>();
            for (int i = 1; i < t.Length; i++)
            {
                var subsParse = t[i];
                var subsSplit = subsParse.Split("->").Select(x=>x.Trim()).ToArray();
                Substitutions.Add((subsSplit[0], subsSplit[1]));
            }
        }
        
        public Vector2 VectorForward { get; set; } = new Vector2(0, 10);
        public int Iterations { get; private set; }
        public float RotationAngle { get; private set; }
        public Color StartingColor { get; set; } = Color.Black;
        public Color EndingColor { get; set; } = Color.Black;
        public float StartingThickness { get; set; } = 2;
        public float ThicknessModifier { get; set; } = 0.9f;
        public struct State
        {
            public float Angle { get; set; } 
            public Vector2 Position { get; set; }
            public float Thickness { get; set; }
            //public Color Color { get; set; }
            public int Iteration { get; set; } 
            public Vector2 VectorForward { get; set; }
        }
        public void Reset()
        {
            Iterations = 0;
            Expression = StartingExpression;
            //Angle = StartingAngle;
        }
        public void Iterate()
        {
            foreach ((var from, var to) in Substitutions)
            {
                if (Expression.Contains(from))
                {
                    Expression = Expression.Replace(from, to);
                }
            }
            Iterations++;
        }
        public Color GetColorProportional(float proportion)
        {
            proportion = Math.Clamp(proportion, 0, 1);
            var r = (byte)Math.Clamp(StartingColor.R * (1 - proportion) + EndingColor.R * proportion, 0, 255);
            var g = (byte)Math.Clamp(StartingColor.G * (1 - proportion) + EndingColor.G * proportion, 0, 255);
            var b = (byte)Math.Clamp(StartingColor.B * (1 - proportion) + EndingColor.B * proportion, 0, 255);
            return new Color(r, g, b);
        }
        public List<(Vector2, Vector2, Color, float thickness)> GetLines()
        {
            List<(Vector2, Vector2, Color, float thickness)> edges = new();
            Stack<State> saveStates = new Stack<State>(){};
            var curState = new State() { Angle = StartingAngle, Position = new Vector2(),
                Thickness = this.StartingThickness, Iteration = 0, VectorForward = this.VectorForward
            };
            //var oldState = curState;
            var pos = 0;
            //var intimidation = 0;
            var r = new Random();
            while (true) {
                if (pos >= Expression.Length) { break; }
                var c = Expression[pos];
                
                switch (c)
                {
                    case 'F':
                        {
                            var moveForward = curState; //new State() { Angle = curState.Angle, Position = curState.Position + VectorForward.Rotate(curState.Angle * MathF.PI / 180)};
                            moveForward.Position += moveForward.VectorForward.Rotate(curState.Angle * MathF.PI / 180);

                            edges.Add((curState.Position, moveForward.Position, GetColorProportional(Iterations == 0 ? 0 : (float)moveForward.Iteration / (float)Iterations), moveForward.Thickness));
                            curState = moveForward;
                        }
                        break;
                    case '+':
                        {
                            if (Randomness)
                                curState.Angle += r.NextSingle() * RotationAngle;
                            else
                                curState.Angle += RotationAngle;
                        }
                        break;
                    case '-':
                        {
                            if (Randomness)
                                curState.Angle -= r.NextSingle() * RotationAngle;
                            else
                                curState.Angle -= RotationAngle;
                        }
                        break;
                    case '[':
                    case '(':
                        {
                            saveStates.Push(curState);
                        }
                        break;
                    case ']':
                    case ')':
                        {
                            curState = saveStates.Pop();
                        }
                        break;
                    case '@':
                        {
                            curState.Iteration += 1;
                            curState.Thickness *= ThicknessModifier;
                            curState.VectorForward *= ThicknessModifier;
                        }
                        break;
                    default:
                        break;
                }
                pos++;
            }
            return edges;
        }
    }
}
