using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.ColorSpaces;
using SixLabors.ImageSharp.ColorSpaces.Conversion;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonogameTest;

public class Game3 : Game
{
    private GraphicsDeviceManager graphics;
    private SpriteBatch _spriteBatch;
    SpriteFont font;

    public Game3()
    {
        graphics = new GraphicsDeviceManager(this);
        graphics.PreferredBackBufferWidth = 1750;
        graphics.PreferredBackBufferHeight = 700;
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    (float H, float S, float V)[,] image = null;
    (float H, float S, float V)[,] image_new = null;

    protected override void Initialize()
    {
        var default_name = "XXXL.webp";
        var args = Environment.GetCommandLineArgs();
        if (args.Length > 1)
        {
            default_name = args[1];
        }
        var ig = Image.Load<Rgba32>(default_name);
        var t = ColorSpaceConverter.ToHsv(ig[0, 0]);
        image = new (float , float , float)[ig.Height, ig.Width];
        image_new = new (float, float, float)[ig.Height, ig.Width];
        for (int y = 0; y < ig.Height; y++)
        {
            for (int x = 0; x < ig.Width; x++)
            {
                var c = ColorSpaceConverter.ToHsv(ig[x, y]);
                image[y, x] = (c.H, c.S, c.V);
            }
        }
        //Console.WriteLine($"{t.H} {t.S} {t.V}");
        base.Initialize();
        float.Ieee754Remainder(t.H, 360);

    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        font = Content.Load<SpriteFont>("defaultfont");

        base.LoadContent();
    }
    float shiftH = 0;
    float shiftS = 0;
    float shiftV = 0;
    bool changed = true;
    protected override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        var ks = Keyboard.GetState();
        var press = false;
        if (ks.IsKeyDown(Keys.Q))
        {
            press = true;
            shiftH += 0.5f;
        }
        if (ks.IsKeyDown(Keys.A))
        {
            press = true;
            shiftS += 0.02f;
        }
        if (ks.IsKeyDown(Keys.Z))
        {
            press = true;
            shiftV += 0.02f;
        }
        if (ks.IsKeyDown(Keys.W))
        {
            press = true;
            shiftH -= 0.5f;
        }
        if (ks.IsKeyDown(Keys.S))
        {
            press = true;
            shiftS -= 0.02f;
        }
        if (ks.IsKeyDown(Keys.X))
        {
            press = true;
            shiftV -= 0.02f;
        }
        if (ks.IsKeyDown(Keys.E))
        {
            press = true;
            shiftH = 0f;
        }
        if (ks.IsKeyDown(Keys.D))
        {
            press = true;
            shiftS = 0f;
        }
        if (ks.IsKeyDown(Keys.C))
        {
            press = true;
            shiftV = 0f;
        }
        changed = press;
        if (changed || txt == null)
        {
            for (int y = 0; y < image.GetLength(0); y++)
            {
                for (int x = 0; x < image.GetLength(1); x++)
                {
                    (var H, var S, var V) = image[y, x];
                    var new_H = H + shiftH;
                    while (new_H > 360){
                        new_H -= 360;
                    }
                    while (new_H < 0)
                    {
                        new_H += 360;
                    }
                    var new_S = float.Clamp(S + shiftS, 0, 1);
                    
                    var new_V = float.Clamp(V + shiftV, 0, 1);
                    image_new[y, x] = (new_H, new_S, new_V);
                }
            }
        }
        if (ks.IsKeyDown(Keys.Space))
        {
            var ig = new Image<Rgba32>(image_new.GetLength(1), image_new.GetLength(0));
            for (int y = 0; y < image_new.GetLength(0); y++)
            {
                for (int x = 0; x < image_new.GetLength(1); x++)
                {
                    var c = image_new[y, x];
                    var h = ColorSpaceConverter.ToRgb(new Hsv(c.H, c.S, c.V));
                    //Console.WriteLine($"{h}");
                    var R = (byte)(h.R * 255);
                    var G = (byte)(h.G * 255);
                    var B = (byte)(h.B * 255);
                    ig[x, y] = new(R, G, B, 255);
                }
            }
            ig.SaveAsPng("edited.png");
        }
    }
    Texture2D txt = null;
    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Microsoft.Xna.Framework.Color.CornflowerBlue);
        
        if (txt == null || changed) {
            var txtColor = new Microsoft.Xna.Framework.Color[image_new.Length];
            //Console.WriteLine($"SIZE: y: {image_new.GetLength(0)}, x: {image_new.GetLength(1)}");
            for (int y = 0; y < image_new.GetLength(0); y++)
            {
                for (int x = 0; x < image_new.GetLength(1); x++)
                {
                    var c = image_new[y, x];
                    var h = ColorSpaceConverter.ToRgb(new Hsv(c.H, c.S, c.V));
                    //Console.WriteLine($"{h}");
                    var R = (byte)(h.R * 255);
                    var G = (byte)(h.G * 255);
                    var B = (byte)(h.B * 255);
                    //Console.WriteLine((y * image_new.GetLength(1)) + x);
                    txtColor[y * image_new.GetLength(1) + x] = new Microsoft.Xna.Framework.Color(R, G, B, (byte)255);
                }

            }
            txt?.Dispose();
            txt = new Texture2D(GraphicsDevice, image_new.GetLength(1), image_new.GetLength(0));
            txt.SetData(txtColor);
        }
        
        _spriteBatch.Begin();
        //_spriteBatch.DrawRectangle(new Microsoft.Xna.Framework.Rectangle(0, 0, 300, 300), Microsoft.Xna.Framework.Color.Black);
        _spriteBatch.Draw(txt, txt.Bounds, Microsoft.Xna.Framework.Color.White);
        var str = $"""
            Shift in H: {shiftH}
            Shift in S: {shiftS}
            Shift in V: {shiftV}
            Press Q, W, E to change ShiftH
            Press A, S, D to change ShiftS
            Press Z, X, C to change ShiftV
            Press Space to save in file.
            """;
        _spriteBatch.DrawString(font, str, new Vector2(txt.Width, 0), Microsoft.Xna.Framework.Color.Black);
        _spriteBatch.End();
        
        base.Draw(gameTime);
    }
}
