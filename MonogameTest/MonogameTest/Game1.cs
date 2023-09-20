using System;
using System.Diagnostics;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using Color = Microsoft.Xna.Framework.Color;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace MonogameTest;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    Texture2D image;
    Texture2D image2;
    Texture2D image3;
    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected void TransformToGray(PixelAccessor<Rgba32> accessor, float rCoef, float gCoef,float bCoef)
    {
        for (int y = 0; y < accessor.Height; y++)
        {
            Span<Rgba32> pixelRow = accessor.GetRowSpan(y);

            // pixelRow.Length has the same value as accessor.Width,
            // but using pixelRow.Length allows the JIT to optimize away bounds checks:
            for (int x = 0; x < pixelRow.Length; x++)
            {
                // Get a reference to the pixel at position x
                ref Rgba32 pixel = ref pixelRow[x];
                var vect = pixel.ToVector4();
                var sum = vect.X*rCoef + vect.Y*gCoef + vect.Z*bCoef;
                pixel = new Rgba32 (sum,sum,sum);
            }
        }
    }
    protected void TransformToGrayNTSC(PixelAccessor<Rgba32> accessor)
    {
        TransformToGray(accessor,0.299f,0.587f,0.114f);
    }
    
    protected void TransformToGrayHDTV(PixelAccessor<Rgba32> accessor)
    {
        TransformToGray(accessor,0.2126f,0.7152f,0.0722f);
    }

    protected void SubtractAbs(PixelAccessor<Rgba32> target, 
       PixelAccessor<Rgba32> accessor1, PixelAccessor<Rgba32> accessor2)
    {
        for (int y = 0; y < target.Height; y++)
        {
            Span<Rgba32> pixelRowTarget = target.GetRowSpan(y);
            Span<Rgba32> pixelRow1 = accessor1.GetRowSpan(y);
            Span<Rgba32> pixelRow2 = accessor2.GetRowSpan(y);
            // pixelRow.Length has the same value as accessor.Width,
            // but using pixelRow.Length allows the JIT to optimize away bounds checks:
            for (int x = 0; x < pixelRowTarget.Length; x++)
            {
                // Get a reference to the pixel at position x
                ref Rgba32 pixel = ref pixelRowTarget[x];
                ref Rgba32 pixel1 = ref pixelRow1[x];
                ref Rgba32 pixel2 = ref pixelRow2[x];
                pixel = new Rgba32 {R = (byte)(Math.Abs(pixel1.R - pixel2.R)),G = (byte)(Math.Abs(pixel1.G - pixel2.G))
                    ,B = (byte)(Math.Abs(pixel1.B - pixel2.B)),A = 255};
            }
        }
    }

    protected override void Initialize()
    {
        var img = Image.Load<Rgba32>("Гладиатор2.jpg");
        /*for (int i = 0; i < img.Width; i++)
        {
            for (int j = 0; j < img.Height; j++)
            {
                var t = img[1, 1];
                var sum = img[i, j].R + img[i, j].B + img[i, j].G;
                img[i, j] = new Rgba32{A = 255, B = (byte)(sum/3), R =  (byte)(sum/3), G = (byte)(sum/3)};
            }
        }*/
        var copy = img.Clone();
        var diff = new Image<Rgba32>(img.Width,img.Height);
        img.ProcessPixelRows(TransformToGrayHDTV);
        img.SaveAsPng("ImgProcessed.png");
        copy.ProcessPixelRows(TransformToGrayNTSC);
        copy.SaveAsPng("ImgProcessed2.png");
        diff.ProcessPixelRows(img,copy,SubtractAbs);
        diff.SaveAsPng("ImgProcessed3.png");
        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        
        try
        {
            FileStream fs = new FileStream("ImgProcessed.png",FileMode.Open);
            image = Texture2D.FromStream(GraphicsDevice,fs);
            
            fs = new FileStream("ImgProcessed2.png",FileMode.Open);
            image2 = Texture2D.FromStream(GraphicsDevice,fs);
            
            fs = new FileStream("ImgProcessed3.png",FileMode.Open);
            image3 = Texture2D.FromStream(GraphicsDevice,fs);
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
        _spriteBatch.Draw(image2,new Rectangle(150,0,100,100),Color.White);
        _spriteBatch.Draw(image3,new Rectangle(300,0,100,100),Color.White);
        _spriteBatch.End();
        // TODO: Add your drawing code here
        base.Draw(gameTime);
    }
}