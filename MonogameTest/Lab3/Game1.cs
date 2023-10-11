using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Dynamic;
using System.IO;
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
        texture = Texture2D.FromFile(GraphicsDevice, "conture.png");
        fillTexture = Texture2D.FromFile(GraphicsDevice, "Icon.bmp");
        // TODO: use this.Content to load your game content here
    }
    Texture2D texture;
    Texture2D fillTexture;
    public void Fill(Color[] data, Texture2D texture, Color from, Color target, Point point, Color[]? fillImage, Rectangle? tectureBound, Point? imageOffset){
        
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
                    if (fillImage != null) {
                        var imOf2 = imageOffset.Value;
                        imOf2.X += i.X - point.X;
                        if (tectureBound.Value.Contains(imOf2)){
                            
                            data[GetInd(i, texture)] = fillImage[GetInd(imOf2, tectureBound.Value)];
                        }
                        else 
                            data[GetInd(i, texture)] = target;
                    }
                    else{
                        data[GetInd(i, texture)] = target;

                    }
                }
            else
                data[GetInd(point, texture)] = target;
            for (var i = leftBound; i.X <= rightBound.X; i.X++) {
                var iShift = i;
                iShift.Y += 1;
                if (fillImage != null) {
                    var imOf = imageOffset.Value;
                    imOf.Y +=1;
                    imOf.X += i.X - point.X;
                    Fill(data, texture, from, target, iShift, fillImage, tectureBound, imOf);
                }
                else Fill(data, texture, from, target, iShift, null, null, null);
            } 
            for (var i = leftBound; i.X <= rightBound.X; i.X++) {
                var iShift = i;
                iShift.Y -= 1;
                if (fillImage != null) {
                    var imOf = imageOffset.Value;
                    imOf.Y -=1;
                    imOf.X += i.X - point.X ;
                    Fill(data, texture, from, target, iShift, fillImage, tectureBound, imOf);
                }
                else Fill(data, texture, from, target, iShift, null, null, null);
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
        var angle_45 = 0;
        var points = new List<Point>() { new Point(0, 1), new Point(-1, 1), new Point(-1, 0), new Point(-1, -1), new Point(0, -1), new Point(1, -1), new Point(1, 0), new Point(1, 1) };
        while (true){
            var t = currentPoint;
            var tries = 0;
            for (tries = 0; tries < points.Count; ++tries)
            {
                t = currentPoint + points[angle_45];
                if (bound.Contains(t) && data[GetInd(t, texture)] == borderColor && !border.Contains(t))
                {
                    angle_45 = (angle_45 + 2) % points.Count;
                    border.Add(t);
                    currentPoint = t;
                    goto o;
                }
                else { 
                    angle_45 = angle_45 <= 0 ? points.Count - 1 : angle_45 - 1;
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
    public static int GetInd(int x, int y, Rectangle texture){
        return x + y * texture.Width;
    }
    public static int GetInd(float x, float y, Rectangle texture){
        return GetInd((int)x, (int)y, texture);
    }
    public static int GetInd(Point point, Rectangle texture){
        return GetInd(point.X, point.Y, texture);
    }
    Point mousePos = new Point(0, 0);
    bool pressedLeft = false;
    bool pressedRight = false;
    bool pressedMiddle = false;
    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();
        var data = new Color[texture.Width * texture.Height];
        texture.GetData(data);
        var state = Mouse.GetState();
        mousePos = state.Position;
        if (state.LeftButton == ButtonState.Pressed)
        {
            if (!pressedLeft)
            {
                if (texture.Bounds.Contains(mousePos))
                {
                    var color = data[GetInd(mousePos, texture)];
                    Fill(data, texture, color, Color.Black, mousePos, null, null, null);
                    pressedLeft = true;
                }
            }
        }
        else
            pressedLeft = false;
        if (state.MiddleButton == ButtonState.Pressed)
        {
            if (!pressedMiddle)
            {
                if (texture.Bounds.Contains(mousePos))
                {
                    var img = new Color[fillTexture.Width * fillTexture.Height];
                    fillTexture.GetData(img);
                    var color = data[GetInd(mousePos, texture)];
                    Fill(data, texture, color, Color.Black, mousePos, img, fillTexture.Bounds, new Point(fillTexture.Width / 2, fillTexture.Height / 2));
                    pressedMiddle = true;
                }
            }
        }
        else
            pressedMiddle = false;
        texture.SetData(data);

        if (state.RightButton == ButtonState.Pressed)
        {
            if (!pressedRight) 
                if (texture.Bounds.Contains(mousePos))
                {
                    dots = GetBorder(data, texture, mousePos);
                    pressedRight = true;
                }
        }
        else pressedRight = false;
        // TODO: Add your update logic here

        base.Update(gameTime);
    }
    List<Point>? dots = null;
    long frames = 0;
    protected override void Draw(GameTime gameTime)
    {
        frames++;
        GraphicsDevice.Clear(Color.CornflowerBlue);
        
        // TODO: Add your drawing code here
        _spriteBatch.Begin();
        //for (var i = 0; i < 100; i++)
        _spriteBatch.Draw(texture, texture.Bounds, Color.White);
        _spriteBatch.DrawPoint(mousePos.X, mousePos.Y, Color.Black, 3.0f);
        if (dots != null)
        {
            foreach (var item in dots)
            {
                if (frames % 30 < 15)
                {
                    _spriteBatch.DrawPoint(item.ToVector2(), Color.Black, 3.0f);
                }
                else
                    _spriteBatch.DrawPoint(item.ToVector2(), Color.Gray, 3.0f);

            }
        }
        _spriteBatch.End();
        base.Draw(gameTime);
    }
}
