using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;

namespace Lab3;

using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Dynamic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;

public class Game3:Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    
    private Point mousePos;
    private Point camPos = Point.Zero;
    private List<ColoredPoint> curPoints = new List<ColoredPoint>();
    private List<ColoredPoint[]> triangles = new List<ColoredPoint[]>();
    private List<Triangle> tris = new List<Triangle>();
    private Color currentPointColor;
    private Random random = new Random();
    bool pressed = false;
    private Color[] primaryColors = { Color.Red, Color.Green, Color.Blue };
    public Game3()
    {
        _graphics = new GraphicsDeviceManager(this);
        IsMouseVisible = true;
        Window.AllowUserResizing = true;
        currentPointColor = new Color(random.Next(0, 255), random.Next(0, 255), random.Next(0, 255));
    }

    protected override void Initialize()
    {
        // TODO: Add your initialization logic here

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();
        
        var state = Mouse.GetState();
        if (state.MiddleButton == ButtonState.Pressed)
        {
            camPos += state.Position - mousePos;
        }
        mousePos = state.Position;
        if (state.LeftButton == ButtonState.Pressed){
            if (!pressed)
            {
                pressed = true;
                curPoints.Add(new ColoredPoint{p = mousePos-camPos, color = currentPointColor});
                currentPointColor = new Color(random.Next(0, 255), random.Next(0, 255), random.Next(0, 255));
                //currentPointColor = primaryColors[curPoints.Count-1];
                if (curPoints.Count == 3)
                {
                    bool triangleImpossible = false;
                    triangles.Add(curPoints.ToArray());
                    var verts = curPoints.OrderBy((ColoredPoint p) => p.p.X).ToArray();
                    var width = verts[2].p.X - verts[0].p.X;
                    var minX = verts[0].p.X;
                    if (verts[0].p.X == verts[1].p.X || verts[1].p.X == verts[2].p.X)
                    {
                        triangleImpossible = true;
                    }

                    verts = verts.OrderBy((ColoredPoint p)=>p.p.Y).ToArray();
                    var minY = verts[0].p.Y;
                    var height = verts[2].p.Y - verts[0].p.Y;
                    if (verts[0].p.Y == verts[1].p.Y || verts[1].p.Y == verts[2].p.Y)
                    {
                        triangleImpossible = true;
                    }
                    if (!(width == 0 || height == 0 || triangleImpossible))
                    {
                        var v = verts.Select((point => new Point(point.p.X - minX, point.p.Y - minY))).ToArray();
                        Color[] colors = new[] { verts[0].color, verts[1].color, verts[2].color };
                        var tex = new Texture2D(GraphicsDevice,width+1,height+1);
                        DrawTriangleOnTex(v,colors,tex);
                        var tr = new Triangle { texture = tex, pos = new Vector2(minX,minY)};
                        tris.Add(tr);
                    }
                    curPoints.Clear();
                }
            }
        }
        else
        {
            pressed = false;
        }
        
        // TODO: Add your update logic here

        base.Update(gameTime);
    }
    public static int GetInd(int x, int y, Texture2D texture){
        return x + y * texture.Width;
    }
    public static int GetInd(float x, float y, Texture2D texture){
        return GetInd((int)x, (int)y, texture);
    }
    public static int GetInd(Point point, Texture2D texture){
        return GetInd(point.X, point.Y, texture);
    }
    //При подаче параметров в эту функцию считается, что
    //1) Точки отсортированы по параметру Y
    //2) Координаты треугольникка заданы в локальных координатах текстуры
    //3) Размер текстуры соответствует размеру треугольника
    protected void DrawTriangleOnTex(Point[] verts,Color[] vertColors, Texture2D tex)
    {
        var data = new Color[tex.Width*tex.Height];
        //Console.WriteLine($"Width = {tex.Width}, height = {tex.Height}");
        int clHeight = verts[1].Y;
        for (int i = 0; i <= clHeight; i++)
        {
            var farCoef = (float)i / tex.Height;
            var clCoef = (float)i / clHeight;
            float farX = verts[0].X +  farCoef*(verts[2].X - verts[0].X);
            float clX =  verts[0].X +  clCoef*(verts[1].X - verts[0].X);
            var farRound = (int)Math.Ceiling(clX);
            var clRound = (int)Math.Ceiling(farX);
            int min = clRound;
            int max = farRound;
            var clColor = InterpolateColor(vertColors[0],vertColors[1],clCoef);
            var farColor = InterpolateColor(vertColors[0],vertColors[2],farCoef);
            var startColor = farColor;
            var endColor = clColor;
            if (farRound < clRound)
            {
                min = farRound;
                max = clRound;
                startColor = clColor;
                endColor = farColor;
            }
            for (int j = min; j <= max; j++)
            {
                //Console.WriteLine($"x = {j}, y = {i}");
                var cl = InterpolateColor(startColor, endColor, (float)(j - min) / (max - min));
                /*if (GetInd(j, i, tex) > data.Length)
                {
                    Console.WriteLine($"x = {j}, y = {i}");
                }*/

                data[GetInd(j,i,tex)] = cl;
            }
        }
        
        clHeight = verts[2].Y - verts[1].Y;
        for (int i = verts[1].Y; i <= verts[2].Y; i++)
        {
            var farCoef = (float)i / tex.Height;
            var clCoef = (float)(i-verts[1].Y) / clHeight;
            float farX = verts[0].X +  farCoef*(verts[2].X - verts[0].X);
            float clX =  verts[1].X +  clCoef*(verts[2].X - verts[1].X);
            var farRound = (int)Math.Ceiling(clX);
            var clRound = (int)Math.Ceiling(farX);
            int min = clRound;
            int max = farRound;
            var clColor = InterpolateColor(vertColors[1],vertColors[2],clCoef);
            var farColor = InterpolateColor(vertColors[0],vertColors[2],farCoef);
            var startColor = farColor;
            var endColor = clColor;
            if (farRound < clRound)
            {
                min = farRound;
                max = clRound;
                startColor = clColor;
                endColor = farColor;
            }
            for (int j = min; j <= max; j++)
            {
                //Console.WriteLine($"x = {j}, y = {i}");
                /*if (GetInd(j, i, tex) >= data.Length)
                {
                    Console.WriteLine($"x = {j}, y = {i}");
                }*/
                var cl = InterpolateColor(startColor, endColor, (float)(j - min) / (max - min));
                data[GetInd(j,i,tex)] = cl;
            }
        }
        
        tex.SetData(data);
    }

    protected Color InterpolateColor(Color c1, Color c2, float t)
    {
        Color res = new Color();
        res.R = (byte)(c1.R+(c2.R - c1.R) * t);
        res.G = (byte)(c1.G+(c2.G - c1.G) * t);
        res.B = (byte)(c1.B+(c2.B - c1.B) * t);
        return res;
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);
        
        // TODO: Add your drawing code here
        _spriteBatch.Begin();
        for (int i = 0; i < tris.Count; i++)
        {
            _spriteBatch.Draw(tris[i].texture,tris[i].pos+camPos.ToVector2(),Color.White);
        }
        /*for (int i = 0; i < triangles.Count; i++)
        {
            _spriteBatch.DrawLine((triangles[i][0].p+camPos).ToVector2(),(triangles[i][1].p+camPos).ToVector2(),triangles[i][0].color);
            _spriteBatch.DrawLine((triangles[i][1].p+camPos).ToVector2(),(triangles[i][2].p+camPos).ToVector2(),triangles[i][1].color);
            _spriteBatch.DrawLine((triangles[i][0].p+camPos).ToVector2(),(triangles[i][2].p+camPos).ToVector2(),triangles[i][2].color);
        }*/
        for (int i = 0; i < curPoints.Count; i++)
        {
            _spriteBatch.DrawPoint(curPoints[i].p.X+camPos.X, curPoints[i].p.Y+camPos.Y, curPoints[i].color, 3.0f);
        }
        _spriteBatch.DrawPoint(mousePos.X, mousePos.Y, currentPointColor, 3.0f);
        _spriteBatch.End();
        base.Draw(gameTime);
    }

    struct ColoredPoint
    {
        public Point p;
        public Color color;
    }

    class Triangle
    {
        public Texture2D texture;
        public Vector2 pos;
    }
}