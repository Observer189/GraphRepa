using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
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
    SpriteFont font;
    //Предполагаем, что spritebatch начал рисование
    public void DrawHistgram(SpriteBatch sb, Rectangle bound, List<(string name, float value)> tables, List<Color>? colors){
        if (tables.Count == 0) {return;}
        Color[] data = new Color[10 * 10];
        Texture2D rectTexture = new Texture2D(GraphicsDevice, 10, 10);
            for (int i = 0; i < data.Length; ++i)
                data[i] = Color.White;
            rectTexture.SetData(data);
        var max = tables.Select(x => x.value).Max();
        var diff = max;
        var offsetLeft = font.MeasureString(max.ToString()).X;
        var offsetBottom = font.MeasureString(tables.First().name).Y;
        var graphBounds = new Rectangle(bound.X + (int)offsetLeft, bound.Y, bound.Width - (int)offsetLeft, bound.Height - (int)offsetBottom);
        var partWidth = graphBounds.Width / tables.Count;
        for (int i = 0; i < tables.Count; i++)
        {
            var table = tables[i];
            var proportion = (table.value) / diff;
            var color = colors?[i] ?? Color.Black;
            var ySize = (int)(graphBounds.Height * proportion);
            var yMove = graphBounds.Height - ySize;
            var drawRect = new Rectangle(graphBounds.X + i*partWidth, graphBounds.Y + yMove, partWidth, ySize);
            sb.Draw(rectTexture, drawRect, color);
            sb.DrawString(font, table.name, new Vector2(graphBounds.Left + i*partWidth, graphBounds.Bottom), Color.Black);
        }
        sb.DrawString(font, max.ToString(), new Vector2(bound.Left, bound.Top), Color.Black);
    }
    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        var img = Image.Load<Rgba32>("XXXL.webp");
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
        font = Content.Load<SpriteFont>("defaultfont");
        
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
        var height = GraphicsDevice.PresentationParameters.BackBufferHeight;
        var width = GraphicsDevice.PresentationParameters.BackBufferWidth;
        _spriteBatch.Draw(image,new Rectangle(0,0,width/3,height / 3),Color.White);

        DrawHistgram(_spriteBatch, new Rectangle(0,0,width/3,height / 3), new List<(string name, float value)>() {("123", 255), ("451", 126), ("1241", 84)},
            new List<Color>() {Color.Aqua, Color.Olive, Color.Red}
        );

        _spriteBatch.End();
        // TODO: Add your drawing code here
        base.Draw(gameTime);
    }
}