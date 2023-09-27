using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Dynamic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;

namespace Lab3;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        // TODO: Add your initialization logic here

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        //texture = new Texture2D(GraphicsDevice, 1000, 1000);
        texture = Texture2D.FromFile(GraphicsDevice, "Icon.bmp");
        // TODO: use this.Content to load your game content here
    }
    Texture2D texture;
    Point mousePos = new Point(0, 0);
    bool pressed = false;
    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();
        var data = new Color[texture.Width*texture.Height];
        texture.GetData(data);
        var state = Mouse.GetState();
        mousePos = state.Position;
        if (state.LeftButton == ButtonState.Pressed){
            if (!pressed) {
                if (texture.Bounds.Contains(mousePos))
                {
                    var color = data[GetInd(mousePos, texture)];
                    Fill(data, texture, color, Color.Black, mousePos);
                    pressed = true;
                }
            }
        }
        else
        {
            pressed = false;
        }
        texture.SetData(data);
        // TODO: Add your update logic here

        base.Update(gameTime);
    }
    public void Fill(Color[] data, Texture2D texture, Color from, Color target, Point point){
        if (!texture.Bounds.Contains(point)) {return;}
        if (from == target) { return; }
        var currColor = data[GetInd(point, texture)];
        if (currColor == from) {
            //Console.WriteLine($"{from} -- {currColor}");
            Point leftBound = point;
            Color leftColor =  data[GetInd(leftBound, texture)];
            while (leftBound.X > 0 && (leftColor == from)){
                leftBound.X -= 1;
                leftColor =  data[GetInd(leftBound, texture)];
            }
            if (leftBound.X != 0) {leftBound.X += 1;}
            Point rightBound = point;
            Color rightColor =  data[GetInd(rightBound, texture)];
            while (rightBound.X < texture.Width && (rightColor == from)){
                rightBound.X += 1;
                if (rightBound.X < texture.Width)
                    rightColor =  data[GetInd(rightBound, texture)];
            }
            rightBound.X -= 1;
            if (leftBound.X < rightBound.X)
                for (var i = leftBound; i.X < rightBound.X; i.X++)
                {
                    data[GetInd(i, texture)] = target;
                }
            else
                data[GetInd(point, texture)] = target;
            for (var i = leftBound; i.X < rightBound.X; i.X++) {
                var iShift = i;
                iShift.Y += 1;
                Fill(data, texture, from, target, iShift);
            } 
            for (var i = leftBound; i.X < rightBound.X; i.X++) {
                var iShift = i;
                iShift.Y -= 1;
                Fill(data, texture, from, target, iShift);
            } 
        }
    }
    public List<Point> GetBorder(Color[] data, Texture2D texture, Point startingPoint)
    {
        if (!texture.Bounds.Contains(startingPoint)) { return new List<Point>(); }
        var borderColor = data[GetInd(startingPoint, texture)];
        var border = new List<Point>{startingPoint};
        //var visited = new HashSet<Point>();
        var currentPoint = startingPoint;
        var bound = texture.Bounds;
        var points = new List<Point>() { new Point(0, 1), new Point(-1, 1), new Point(-1, 0), new Point(-1, -1), new Point(0, -1), new Point(1, -1), new Point(1, 0), new Point(1, 1) };
        while (currentPoint != startingPoint){
            var t = currentPoint;
            foreach (var point in points)
            {
                t = currentPoint + point;
                if (bound.Contains(t) && data[GetInd(t, texture)] == borderColor && !border.Contains(t))
                {
                    border.Add(t);
                    currentPoint = t;
                    goto o;
                }
            }
            break;
        o:;
        }
        return border;
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
    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);
        
        // TODO: Add your drawing code here
        _spriteBatch.Begin();
        //for (var i = 0; i < 100; i++)
        _spriteBatch.Draw(texture, texture.Bounds, Color.White);
        _spriteBatch.DrawPoint(mousePos.X, mousePos.Y, Color.Black, 3.0f);
        _spriteBatch.End();
        base.Draw(gameTime);
    }
}
