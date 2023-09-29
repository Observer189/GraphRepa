using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Dynamic;
using System.IO;
using System.IO.IsolatedStorage;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;

namespace Lab3;

public class Game2 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private Texture2D pixel;
    private bool drawingLine = false;
    private List<Vector2[]> lines = new List<Vector2[]>();
    private Vector2[] currentLine;
    private float zoom = 1.0f;
    private SpriteFont _font;

    public Game2()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        currentLine = new Vector2[2];
        currentLine[0] = Vector2.Zero;
        currentLine[1] = Vector2.Zero;

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        pixel = new Texture2D(GraphicsDevice, 1, 1);
        pixel.SetData(new[] { Color.White });

        _font = Content.Load<SpriteFont>("defaultfont");
    }

    protected override void Update(GameTime gameTime)
    {
        KeyboardState keyboardState = Keyboard.GetState();
        MouseState mouseState = Mouse.GetState();
        Vector2 mousePosition = new Vector2(mouseState.X, mouseState.Y);
        Vector2 worldMousePosition = mousePosition / zoom;

        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
        {
            Exit();
        }

        if (mouseState.LeftButton == ButtonState.Pressed)
        {
            if (!drawingLine)
            {
                currentLine[0] = worldMousePosition;
                drawingLine = true;
            }
            else
            {
                currentLine[1] = worldMousePosition;
            }
        }
        else if (drawingLine)
        {
            drawingLine = false;
            lines.Add(new Vector2[] { currentLine[0], currentLine[1] });
        }
      
        if (keyboardState.IsKeyDown(Keys.Q))
            zoom += 0.1f; 

        if (keyboardState.IsKeyDown(Keys.E))
            zoom -= 0.1f; 

        //zoom = MathHelper.Clamp(zoom, 0.1f, 2.0f);

        base.Update(gameTime);
    }

    protected void Bresenheim(int x1, int y1, int x2, int y2)
    {
        int dx = Math.Abs(x2 - x1);
        int dy = Math.Abs(y2 - y1);
        int sx = (x1 < x2) ? 1 : -1;
        int sy = (y1 < y2) ? 1 : -1;
        int err = dx - dy;

        while (true)
        {
            _spriteBatch.Draw(pixel, new Vector2(x1, y1), Color.White);
          
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

    protected void VU(int x1, int y1, int x2, int y2, Color color)
    {
        bool steep = Math.Abs(y2 - y1) > Math.Abs(x2 - x1);

        if (steep)
        {
            Swap(ref x1, ref y1, ref x2, ref y2);
        }

        if (x1 > x2)
        {
            Swap(ref x1, ref x2, ref y1, ref y2);
        }

        int dx = x2 - x1;
        int dy = y2 - y1;
        float gradient = (float)dy / dx;
        float xend = x1;
        float yend = y1 + gradient * (xend - x1);
        float xgap = 1 - (x1 + 0.5f) % 1;
        int xpxl1 = (int)xend;
        int ypxl1 = (int)yend;
        if (steep)
        {
            _spriteBatch.Draw(pixel, new Vector2(ypxl1, xpxl1), Color.White * (1 - (yend % 1)) * xgap);
            _spriteBatch.Draw(pixel, new Vector2(ypxl1 + 1, xpxl1), Color.White * (yend % 1) * xgap);
        }
        else
        {
            _spriteBatch.Draw(pixel, new Vector2(xpxl1, ypxl1), Color.White * (1 - (yend % 1)) * xgap);
            _spriteBatch.Draw(pixel, new Vector2(xpxl1, ypxl1 + 1), Color.White * (yend % 1) * xgap);
        }

        float intery = yend + gradient;

        xend = x2;
        yend = y2 + gradient * (xend - x2);
        xgap = (x2 + 0.5f) % 1;
        int xpxl2 = (int)xend;
        int ypxl2 = (int)yend;

        if (steep)
        {
            _spriteBatch.Draw(pixel, new Vector2(ypxl2, xpxl2), Color.White * (1 - (yend % 1)) * xgap);
            _spriteBatch.Draw(pixel, new Vector2(ypxl2 + 1, xpxl2), Color.White * (yend % 1) * xgap);
        }
        else
        {
            _spriteBatch.Draw(pixel, new Vector2(xpxl2, ypxl2), Color.White * (1 - (yend % 1)) * xgap);
            _spriteBatch.Draw(pixel, new Vector2(xpxl2, ypxl2 + 1), Color.White * (yend % 1) * xgap);
        }

        if (steep)
        {
            for (int x = xpxl1 + 1; x <= xpxl2 - 1; x++)
            {
                _spriteBatch.Draw(pixel, new Vector2((int)intery, x), Color.White * (1 - (intery % 1)));
                _spriteBatch.Draw(pixel, new Vector2((int)intery + 1, x), Color.White * (intery % 1));
                intery += gradient;
            }
        }
        else
        {
            for (int x = xpxl1 + 1; x <= xpxl2 - 1; x++)
            {
                _spriteBatch.Draw(pixel, new Vector2(x, (int)intery), Color.White * (1 - (intery % 1)));
                _spriteBatch.Draw(pixel, new Vector2(x, (int)intery + 1), Color.White * (intery % 1));
                intery += gradient;
            }
        }
    }

    private void Swap(ref int x1, ref int y1, ref int x2, ref int y2)
    {
        int temp = x1;
        x1 = y1;
        y1 = temp;
        temp = x2;
        x2 = y2;
        y2 = temp;
    }

    protected void print_VU()
    {
        foreach (var line in lines)
        {
            VU((int)line[0].X, (int)line[0].Y, (int)line[1].X, (int)line[1].Y, Color.White);
        }

        if (drawingLine)
        {
            VU((int)currentLine[0].X, (int)currentLine[0].Y, (int)currentLine[1].X, (int)currentLine[1].Y, Color.White);
        }

    }

    protected void print_Bresenheim()
    {
        foreach (var line in lines)
        {
            Bresenheim((int)line[0].X, (int)line[0].Y, (int)line[1].X, (int)line[1].Y);
        }

        if (drawingLine)
        {
            Bresenheim((int)currentLine[0].X, (int)currentLine[0].Y, (int)currentLine[1].X, (int)currentLine[1].Y);
        }

    }


    protected override void Draw(GameTime gameTime)
    {
        
        GraphicsDevice.Clear(Color.CornflowerBlue);

        _spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, Matrix.CreateScale(zoom));

        _spriteBatch.DrawString(_font, "Q: Zoom +\nE: Zoom -", new Vector2(650, 0), Color.White);

        //print_Bresenheim();
        print_VU();

        _spriteBatch.End();

        base.Draw(gameTime);
    }



}
