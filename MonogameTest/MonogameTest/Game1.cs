using System;
using System.Diagnostics;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using Color = Microsoft.Xna.Framework.Color;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace MonogameTest;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    Texture2D image;
    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        var img = Image.Load<Rgba32>("Гладиатор2.jpg");
        for (int i = 0; i < img.Width; i++)
        {
            for (int j = 0; j < img.Height; j++)
            {
                var t = img[1, 1];
                var sum = img[i, j].R + img[i, j].B + img[i, j].G;
                img[i, j] = new Rgba32{A = 255, B = (byte)(sum/3), R =  (byte)(sum/3), G = (byte)(sum/3)};/*Rgba32.ParseHex("#0000ff");*/
            }
        }
        img.SaveAsPng("ImgProcessed.png");
        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        
        try
        {
            FileStream fs = new FileStream("ImgProcessed.png",FileMode.Open);
            image = Texture2D.FromStream(GraphicsDevice,fs);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
       
        
        // TODO: use this.Content to load your game content here
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();
        // TODO: Add your update logic here

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);
        _spriteBatch.Begin();
        _spriteBatch.Draw(image,new Rectangle(0,0,100,100),Color.White);
        _spriteBatch.End();
        // TODO: Add your drawing code here
        base.Draw(gameTime);
    }
}