using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MLEM.Input;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Lab5_1
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        public SpriteFont font;
        public List<(Vector2, Vector2, Color, float thickness)> Edges = new();
        public InputHandler inputHandler;
        public int currentParser = 0;
        public List<Parser> parsers = new List<Parser>
        {
            new ("""
                F 60 270
                F -> F-F++F-F
                """),
            new ("""
                F++F++F 60 180
                F -> F-F++F-F
                """),
            new ("""
                F+F+F+F 90 0
                F -> F+F-F-FF+F+F-F
                """),
            new ("""
                FXF--FF--FF 60 120
                F -> FF
                X -> --FXF++FXF++FXF--
                """),
            new ("""
                YF 60 0
                F -> F
                X -> YF+XF+Y
                Y -> XF-YF-X
                """),
            new ("""
                X 90 0
                F -> F
                X -> −YF+XFX+FY−
                Y -> +XF−YFY−FX+
                """),
            new ("""
                X 90 0
                F -> F
                X -> X+YF+
                Y -> -FX-Y
                """),
            new ("""
                XF 60 0
                F -> F
                X -> X+YF++YF−FX−−FXFX−YF+
                Y -> −FX+YFYF++YF+FX−−FX−Y
                """),
            new ("""
                F 22 180
                F -> FF-[-F+F+F]+[+F-F-F]
                """),
            new ("""
                X 20 180
                F -> FF
                X -> F[+X]F[-X]+X
                """),
            new ("""
                X 22.5 180
                F -> FF
                X -> F−[[X]+X]+F[+FX]−X
                """),
            new ("""
                X 45 180
                X -> F[@[-X]+X]
                """) { StartingColor = Color.Brown, EndingColor = Color.Green, VectorForward = new Vector2(0, 30)}
            
        };
        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            Window.AllowUserResizing = true;
            // TODO: Add your initialization logic here
            inputHandler = new InputHandler(this);
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            font = Content.Load<SpriteFont>("defaultfont");

            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            inputHandler.Update(gameTime);
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            //Console.WriteLine($"{GraphicsDevice.PresentationParameters.Bounds}");

            if (Edges.Count == 0)
            {
                Edges = parsers[currentParser].GetLines();
                Edges = ScaleToFit(Edges, GraphicsDevice.PresentationParameters.Bounds);
            }
            if (inputHandler.IsPressed(Keys.A)) {
                parsers[currentParser].Iterate();
                Edges = parsers[currentParser].GetLines();
                Edges = ScaleToFit(Edges, GraphicsDevice.PresentationParameters.Bounds);
            }
            if (inputHandler.IsPressed(Keys.S))
            {
                parsers[currentParser].Randomness = !parsers[currentParser].Randomness;
                Edges = parsers[currentParser].GetLines();
                Edges = ScaleToFit(Edges, GraphicsDevice.PresentationParameters.Bounds);
            }
            if (inputHandler.IsPressed(Keys.R))
            {
                parsers[currentParser].Reset();
                Edges = parsers[currentParser].GetLines();
                Edges = ScaleToFit(Edges, GraphicsDevice.PresentationParameters.Bounds);
            }
            if (inputHandler.IsPressed(Keys.Z))
            {
                currentParser = currentParser - 1 < 0 ? parsers.Count - 1 : currentParser - 1;
                Edges = parsers[currentParser].GetLines();
                Edges = ScaleToFit(Edges, GraphicsDevice.PresentationParameters.Bounds);
            }
            if (inputHandler.IsPressed(Keys.X))
            {
                currentParser = (currentParser + 1) % parsers.Count;
                Edges = parsers[currentParser].GetLines();
                Edges = ScaleToFit(Edges, GraphicsDevice.PresentationParameters.Bounds);
            }
            // TODO: Add your update logic here


            base.Update(gameTime);
        }
        private List<(Vector2, Vector2, Color, float thickness)> ScaleToFit(List<(Vector2, Vector2, Color, float thickness)> values, Rectangle rect)
        {
            if (values.Count == 0) return values;
            var dots = values.SelectMany(x => new[] { x.Item1, x.Item2 }).ToList();
            var left = dots.Select(x => x.X).Min();
            var rigth = dots.Select(x => x.X).Max();
            var top = dots.Select(x => x.Y).Min();
            var down = dots.Select(x => x.Y).Max();
            var height = down - top;
            var width = rigth - left;
            var shift = new Vector2(left, top);
            var l = values.Select(x => (x.Item1 - shift, x.Item2 - shift, x.Item3, x.thickness)).ToList();
            if (height > rect.Height || width > rect.Width)
            {
                var scale = MathF.Max(height / rect.Height, width / rect.Width);
                l = l.Select(x => (x.Item1 / scale, x.Item2 / scale, x.Item3, x.thickness * scale)).ToList();
            }

            return l;

        }
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            _spriteBatch.Begin();
            foreach (var edge in Edges)
            {
                //if (edge.Item1.)
                //Console.WriteLine($"{edge.Item1}   {edge.Item2}");
                _spriteBatch.DrawLine(edge.Item1, edge.Item2, edge.Item3, thickness: edge.thickness);
            }
            //foreach( var t in parsers[currentParser].Expression.Where(x => ((short)x) > 128))
            //{
            //    Console.WriteLine($"ANOMAL SYMBOL {t} (code:{((short)t)})");
            //}
            _spriteBatch.DrawString(font, new string(parsers[currentParser].Expression.Where(x => (ushort)x < 128).ToArray()), new Vector2(0, 300), Color.Black);
            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}