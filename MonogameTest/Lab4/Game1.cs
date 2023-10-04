using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MLEM.Font;
using MLEM.Input;
using MLEM.Ui;
using MLEM.Ui.Elements;
using MLEM.Ui.Style;
using MonoGame.Extended;
using MonoGame.Extended.Shapes;
using System;
using System.Collections.Generic;
using System.Linq;

public enum State
{
    Editing,
    Transforming,
    Checking,
    Scaling
}
namespace Lab4
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        //https://mlem.ellpeck.de/articles/ui.html - документация по UI
        public UiSystem uiSystem;
        public InputHandler inputHandler;
        public SpriteFont font;
        public State state;

        private Panel root;
        private List<List<Vector2>> polygons = new List<List<Vector2>>() { new List<Vector2>() } ;
        private Vector2 CenterOfCoordinates = new Vector2();
        private Matrix2 TransformationMatrix = Matrix2.Identity;
        private bool rotateLine = false;

        private bool isTransforming = false;
        private bool isScaling = false;

        private float rotationAngle;


        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            _graphics.PreferredBackBufferWidth = 1080;
            _graphics.PreferredBackBufferHeight = 720;

        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            inputHandler = new InputHandler(this);
            font = Content.Load<SpriteFont>("defaultfont");
            this.uiSystem = new UiSystem(this, new UntexturedStyle(this._spriteBatch) { Font = new GenericSpriteFont(font)});
            var panel = new Panel(Anchor.AutoRight, size: new Vector2(150, 300), positionOffset: Vector2.Zero);
            this.root = panel;
            var button0 = new Button(Anchor.AutoLeft, size: new Vector2(150, 30), text: "Add poligon") { OnPressed = (elem) => { this.polygons.Add(new List<Vector2>()); } };
            panel.AddChild(button0);
            var button1 = new Button(Anchor.AutoLeft, size: new Vector2(150, 30), text: "Editing") { OnPressed = (elem) => { this.state = State.Editing; } };
            panel.AddChild(button1);
            //var button2 = new Button(Anchor.AutoLeft, size: new Vector2(95, 30), text: "Transforming") { OnPressed = (elem) => { this.state = State.Transforming; }, 
            //    PositionOffset = new Vector2(0, 5)};
            //panel.AddChild(button2);
            var button3 = new Button(Anchor.AutoLeft, size: new Vector2(150, 30), text: "Checking") { OnPressed = (elem) => { this.state = State.Checking; },
                PositionOffset = new Vector2(0, 5)};
            panel.AddChild(button3);
            var button4 = new Button(Anchor.AutoLeft, size: new Vector2(150, 30), text: "Clean scene") { OnPressed = (elem) => { this.polygons = new List<List<Vector2>>() { new List<Vector2>() }; 
                this.TransformationMatrix = Matrix2.Identity; },
                PositionOffset = new Vector2(0, 5)};
            panel.AddChild(button4);
            var text =  new Paragraph(Anchor.AutoLeft, 100, (x) => { return this.state.ToString(); }, true);
            panel.AddChild(text);
            uiSystem.Add("panel", panel);
            // TODO: use this.Content to load your game content here
            var button2 = new Button(Anchor.AutoLeft, size: new Vector2(150, 60), text: "Rotate around its center(a,d,r)")
            {
                OnPressed = (elem) =>
                {
                    this.state = State.Transforming;
                    isTransforming = true; // Включаем режим "Transforming"
                },
            };
            panel.AddChild(button2);
            var button5 = new Button(Anchor.AutoLeft, size: new Vector2(150, 60), text: "Scaling relative to center")
            {
                OnPressed = (elem) =>
                {
                    this.state = State.Scaling;
                    isScaling = true; // Включаем режим "Scaling"
                },
            };
            panel.AddChild(button5);

        }

        protected override void Update(GameTime gameTime)
        {
            inputHandler.Update();
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            var field = GraphicsDevice.PresentationParameters.Bounds;
            CenterOfCoordinates = field.Center.ToVector2();
            field.Width -= 100;
            var mouseState = Mouse.GetState();
            switch (this.state) {
                case State.Editing:
                    {
                        if ( inputHandler.IsPressed(MouseButton.Left) && field.Contains(mouseState.Position) )
                        {
                            this.polygons.Last().Add(mouseState.Position.ToVector2() - CenterOfCoordinates);
                        }
                    }
                    break;
                case State.Checking:
                    break;
                case State.Transforming:
                    if (isTransforming)
                    {
                        KeyboardState keyboardState = Keyboard.GetState();
                        if (keyboardState.IsKeyDown(Keys.A))
                        {
                            rotationAngle -= 0.01f; 
                        }
                        if (keyboardState.IsKeyDown(Keys.D))
                        {
                            rotationAngle += 0.01f; 
                        }
                        if (keyboardState.IsKeyDown(Keys.R))
                        {
                            rotationAngle = 0f;
                        }

                        // Найдите центр масс фигуры
                        Vector2 center = Vector2.Zero;
                        foreach (var polygon in polygons)
                        {
                            foreach (var point in polygon)
                            {
                                center += point;
                            }
                        }
                        center /= polygons.Sum(polygon => polygon.Count);

                        for (int i = 0; i < polygons.Count; i++)
                        {
                            for (int j = 0; j < polygons[i].Count; j++)
                            {
                                // Перенос фигуры в начало координат
                                var translatedPoint = polygons[i][j] - center;

                                // Применение матрицы поворота
                                var rotatedPoint = Vector2.Transform(translatedPoint, Matrix.CreateRotationZ(rotationAngle));

                                // Обратный перенос обратно в исходное положение
                                var finalPoint = rotatedPoint + center;

                                polygons[i][j] = finalPoint;
                            }
                        }
                    }
                    break;
                case State.Scaling:
                    break;
            }
            // TODO: Add your update logic here
            this.uiSystem.Update(gameTime);
            
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);
            _spriteBatch.Begin();
            switch (this.state)
            {
                case State.Editing:
                    {
                        foreach (var poligon in polygons)
                        {
                            if (poligon.Count == 1)
                            {
                                _spriteBatch.DrawPoint(poligon[0] + this.CenterOfCoordinates, Color.Black, 2.5f);
                            }
                            else
                            {
                                var p = new Polygon(poligon);

                                _spriteBatch.DrawPolygon(CenterOfCoordinates, p, Color.Black, thickness: 1);
                            }
                        }
                        
                    }
                    break;
                case State.Checking:
                    //Перебрать все полигоны из двух точек, найти точки пересечения, нарисовать их.
                    //Определить, является ли точка под мышкой внутри или вне как минимум первого полигона в списке.
                    //Определить, является ли точка под мышкой слева или справа первого отрезка в списке.
                    break;
                case State.Transforming:
                    {
                        foreach (var polygon in polygons)
                        {
                            if (polygon.Count == 1)
                            {
                                _spriteBatch.DrawPoint(polygon[0] + CenterOfCoordinates, Color.Black, 2.5f);
                            }
                            else
                            {
                                var p = new Polygon(polygon);
                                _spriteBatch.DrawPolygon(CenterOfCoordinates, p, Color.Black, thickness: 1);
                            }
                        }
                    }
                    break;
            }
            
            _spriteBatch.End();
            // TODO: Add your drawing code here
            this.uiSystem.Draw(gameTime, this._spriteBatch);
            
            base.Draw(gameTime);
        }
    }
}