using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using Color = Microsoft.Xna.Framework.Color;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace MonogameTest;

class Game2 : Game
{
    private GraphicsDeviceManager graphics;
    private SpriteBatch _spriteBatch;
    Texture2D image_automobile, image_automobile_red, image_automobile_green, image_automobile_blue;
    Dictionary<byte, int> histogramm_red = new();
    Dictionary<byte, int> histogramm_green = new();
    Dictionary<byte, int> histogramm_blue = new();
    private HistogramRenderer histogramRendererRed;
    private HistogramRenderer histogramRendererGreen;
    private HistogramRenderer histogramRendererBlue;

    public Game2()
    {
        graphics = new GraphicsDeviceManager(this);
        graphics.PreferredBackBufferWidth = 1850;
        graphics.PreferredBackBufferHeight = 700; 
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        histogramRendererRed = new HistogramRenderer(GraphicsDevice);
        histogramRendererGreen = new HistogramRenderer(GraphicsDevice);
        histogramRendererBlue = new HistogramRenderer(GraphicsDevice);

        var default_name = "automobile.jpg";
        var args = Environment.GetCommandLineArgs();
        if (args.Length > 1)
        {
            default_name = args[1];
        }
        var imgRed = Image.Load<Rgba32>(default_name);

        for (int i = 0; i < imgRed.Width; i++)
        {
            for (int j = 0; j < imgRed.Height; j++)
            {
                if (histogramm_red.ContainsKey((byte)imgRed[i, j].R))
                    histogramm_red[(byte)imgRed[i, j].R]++;
                else
                    histogramm_red[(byte)imgRed[i, j].R] = 1;
                imgRed[i, j] = new Rgba32 { A = 255, R = imgRed[i, j].R, G = 0, B = 0 };
            }
        }
        imgRed.SaveAsPng("automobile_red.jpg");


        var imgGreen = Image.Load<Rgba32>(default_name);
        for (int i = 0; i < imgGreen.Width; i++)
        {
            for (int j = 0; j < imgGreen.Height; j++)
            {
                if (histogramm_green.ContainsKey((byte)imgGreen[i, j].G))
                    histogramm_green[(byte)imgGreen[i, j].G]++;
                else
                    histogramm_green[(byte)imgGreen[i, j].G] = 1;
                imgGreen[i, j] = new Rgba32 { A = 255, R = 0, G = imgGreen[i, j].G, B = 0 };
            }
        }
        imgGreen.SaveAsPng("automobile_green.jpg");


        var imgBlue = Image.Load<Rgba32>(default_name);
        for (int i = 0; i < imgBlue.Width; i++)
        {
            for (int j = 0; j < imgBlue.Height; j++)
            {
                if (histogramm_blue.ContainsKey((byte)imgBlue[i, j].B))
                    histogramm_blue[(byte)imgBlue[i, j].B]++;
                else
                    histogramm_blue[(byte)imgBlue[i, j].B] = 1;
                imgBlue[i, j] = new Rgba32 { A = 255, R = 0, G = 0, B = imgBlue[i, j].B };
            }
        }
        imgBlue.SaveAsPng("automobile_blue.jpg");


        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        try
        {
            var default_name = "automobile.jpg";
            var args = Environment.GetCommandLineArgs();
            if (args.Length > 1)
            {
                default_name = args[1];
            }
            FileStream fs = new FileStream(default_name, FileMode.Open);
            image_automobile = Texture2D.FromStream(GraphicsDevice, fs);

            fs = new FileStream("automobile_red.jpg", FileMode.Open);
            image_automobile_red = Texture2D.FromStream(GraphicsDevice, fs);

            fs = new FileStream("automobile_green.jpg", FileMode.Open);
            image_automobile_green = Texture2D.FromStream(GraphicsDevice, fs);

            fs = new FileStream("automobile_blue.jpg", FileMode.Open);
            image_automobile_blue = Texture2D.FromStream(GraphicsDevice, fs);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.White);

        _spriteBatch.Begin();


        Vector2 position_auto = new Vector2(50, 50); 
        Vector2 scale_auto = new Vector2(0.4f, 0.4f); 
        _spriteBatch.Draw(image_automobile, position_auto, null, Color.White, 0f, Vector2.Zero, scale_auto, SpriteEffects.None, 0f);

        position_auto = new Vector2(500, 50); 
        scale_auto = new Vector2(0.4f, 0.4f); 
        _spriteBatch.Draw(image_automobile_red, position_auto, null, Color.White, 0f, Vector2.Zero, scale_auto, SpriteEffects.None, 0f);

        position_auto = new Vector2(950, 50);
        scale_auto = new Vector2(0.4f, 0.4f);
        _spriteBatch.Draw(image_automobile_green, position_auto, null, Color.White, 0f, Vector2.Zero, scale_auto, SpriteEffects.None, 0f);

        position_auto = new Vector2(1400, 50);
        scale_auto = new Vector2(0.4f, 0.4f);
        _spriteBatch.Draw(image_automobile_blue, position_auto, null, Color.White, 0f, Vector2.Zero, scale_auto, SpriteEffects.None, 0f);

        _spriteBatch.End(); 


        histogramRendererRed.DrawHistogram(_spriteBatch, histogramm_red, Color.Red, new Rectangle(500,300,400,300));
        histogramRendererGreen.DrawHistogram(_spriteBatch, histogramm_green, Color.Green, new Rectangle(950, 300, 400, 300));
        histogramRendererBlue.DrawHistogram(_spriteBatch, histogramm_blue, Color.Blue, new Rectangle(1400, 300, 400, 300));



        base.Draw(gameTime);
    }
}
