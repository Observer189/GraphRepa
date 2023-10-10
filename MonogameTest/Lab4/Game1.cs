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
using MLEM.Extensions;

public enum State
{
    Editing,
    Transforming,
    Checking,
    
    Rotation90
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
        //private List<List<Vector2>> polygons = new List<List<Vector2>>() { new List<Vector2>() } ;
        private List<DirPolygon> polygons = new List<DirPolygon>();
        private int chosenPolygon = -1;
        private Vector2 CenterOfCoordinates = new Vector2();
        private Matrix2 TransformationMatrix = Matrix2.Identity;
        private List<Vector2> CrossingDots = new List<Vector2>();
        private bool rotateLine = false;
        
        private bool hasRotated90 = false;

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
            var panel = new Panel(Anchor.AutoRight, size: new Vector2(150, 500), positionOffset: Vector2.Zero);
            this.root = panel;
            var button0 = new Button(Anchor.AutoLeft, size: new Vector2(150, 30), text: "Add poligon") 
            { OnPressed = (elem) => 
                {
                  this.polygons.Add(new DirPolygon());
                }
            };
            panel.AddChild(button0);
            var button1 = new Button(Anchor.AutoLeft, size: new Vector2(150, 30), text: "Editing") { OnPressed = (elem) => { this.state = State.Editing; } };
            panel.AddChild(button1);
            //var button2 = new Button(Anchor.AutoLeft, size: new Vector2(95, 30), text: "Transforming") { OnPressed = (elem) => { this.state = State.Transforming; }, 
            //    PositionOffset = new Vector2(0, 5)};
            //panel.AddChild(button2);
            var button3 = new Button(Anchor.AutoLeft, size: new Vector2(150, 30), text: "Checking") { OnPressed = (elem) => { this.state = State.Checking; },
                PositionOffset = new Vector2(0, 5)};
            panel.AddChild(button3);
            var button4 = new Button(Anchor.AutoLeft, size: new Vector2(150, 30), text: "Clean scene") { OnPressed = (elem) => { this.polygons = new List<DirPolygon>(); 
                this.TransformationMatrix = Matrix2.Identity;
                chosenPolygon = -1;
            },
                PositionOffset = new Vector2(0, 5)};
            panel.AddChild(button4);
            var text =  new Paragraph(Anchor.AutoLeft, 100, (x) => { return this.state.ToString(); }, true);
            panel.AddChild(text);
            uiSystem.Add("panel", panel);
            // TODO: use this.Content to load your game content here
            var button2 = new Button(Anchor.AutoLeft, size: new Vector2(150, 60), text: "Transform selected polygon(s)")
            {
                OnPressed = (elem) =>
                {
                    this.state = State.Transforming;
                },
            };

            panel.AddChild(button2);
            //var button5 = new Button(Anchor.AutoLeft, size: new Vector2(150, 60), text: "Scaling relative to center(w, s)")
            //{
            //    OnPressed = (elem) =>
            //    {
            //        this.state = State.Scaling;
            //    },
            //};
            //panel.AddChild(button5);

            var rotation90 = new Button(Anchor.AutoLeft, size: new Vector2(150, 60), text: "Rotation90")
            {
                OnPressed = (elem) =>
                {
                    this.state = State.Rotation90;
                    hasRotated90 = false;
                },
            };
            panel.AddChild(rotation90);

        }

        protected override void Update(GameTime gameTime)
        {
            inputHandler.Update();
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            var field = GraphicsDevice.PresentationParameters.Bounds;
            field.Width -= 150;
            CenterOfCoordinates = field.Center.ToVector2();
            var mouseState = Mouse.GetState();
            if (inputHandler.IsPressed(MouseButton.Right) && field.Contains(mouseState.Position))
            {
                for (int i = 0; i < polygons.Count; i++)
                {
                    if (polygons[i].GetTransformedCopy().IsInside(mouseState.Position.ToVector2()-CenterOfCoordinates))
                    {
                        chosenPolygon = i;
                    }
                }   
            }
            //if (state != State.Rotation90) hasRotated90 = false;
            switch (this.state) {
                case State.Editing:
                    {
                        //if ( inputHandler.IsPressed(MouseButton.Left) && field.Contains(mouseState.Position) )
                        //{
                        //    this.polygons.Last().Add(mouseState.Position.ToVector2() - CenterOfCoordinates);
                        //}
                        if (inputHandler.IsPressed(MouseButton.Left) && field.Contains(mouseState.Position))
                        {
                            var newPoint = mouseState.Position.ToVector2() - CenterOfCoordinates;
                            if (polygons.Count > 0)
                            {
                                var currentPolygon = this.polygons.Last();
                                currentPolygon.AddPoint(newPoint);
                            }
                        }

                    }
                    break;
                case State.Checking:
                    CrossingDots = new List<Vector2>();
                    var l = polygons.Where(x => x.vertices.Count == 2).Select(x => x.vertices.Select(y => Vector2.Transform(y, x.LocalTransformations)).ToList()).ToList();
                    for (int i = 0; i < l.Count; i++)
                    {
                        for (int j = i + 1; j < l.Count; j++)
                        {
                            var a = l[i][0];
                            var b = l[i][1];
                            var c = l[j][0];
                            var d = l[j][1];
                            var t = Utilities.CrossingCoordinates(a, b, c, d);

                            if (t.HasValue)
                            {
                                CrossingDots.Add(t.Value);
                            }
                        }
                    }
                    break;
                case State.Transforming:
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
                        var pols = chosenPolygon == -1 ? polygons : new List<DirPolygon>() { polygons[chosenPolygon] };
                        // центр масс фигуры
                        Vector2 center = Vector2.Zero;
                        foreach (var polygon in pols)
                        {
                            foreach (var point in polygon.GetTransformedCopy().vertices)
                            {
                                center += point;
                            }
                        }
                        center /= pols.Sum(polygon => polygon.vertices.Count);

                        float cosAngle = (float)Math.Cos(rotationAngle);
                        float sinAngle = (float)Math.Sin(rotationAngle);
                        for (int i = 0; i < pols.Count; i++)
                        {
                        var shift_matrix = new Matrix2() { M11 = 1,         M12 = 0, 
                                                           M21 = 0,         M22 = 1, 
                                                           M31 = -center.X, M32 = -center.Y};
                        var rotation_matrix = new Matrix2() {M11 = cosAngle, M12 = -sinAngle,
                                                             M21 = sinAngle, M22 = cosAngle,
                                                             M31 = 0,        M32 = 0};
                        var back_shift_matrix = new Matrix2() { M11 = 1,         M12 = 0, 
                                                                M21 = 0,         M22 = 1, 
                                                                M31 = center.X, M32 = center.Y};
                        pols[i].LocalTransformations *= shift_matrix * rotation_matrix * back_shift_matrix;
                        //for (int j = 0; j < polygons[i].vertices.Count; j++)
                        //    {
                        //        // Перенос фигуры в начало координат
                        //        var translatedPoint = polygons[i].vertices[j] - center;

                        //        // Применение поворота
                        //        var rotatedX = translatedPoint.X * cosAngle - translatedPoint.Y * sinAngle;
                        //        var rotatedY = translatedPoint.X * sinAngle + translatedPoint.Y * cosAngle;

                        //        // Обратный перенос обратно в исходное положение
                        //        var finalPoint = new Vector2(rotatedX, rotatedY) + center;

                        //        polygons[i].vertices[j] = finalPoint;
                        //    }
                        
                        Vector2 localCenter = Vector2.Zero;
                        var pols_scaling = chosenPolygon == -1 ? polygons : new List<DirPolygon>() { polygons[chosenPolygon] };

                        foreach (var polygon in pols_scaling)
                        {
                            foreach (var point in polygon.vertices)
                            {
                                localCenter += point;
                            }
                        }

                        localCenter /= pols_scaling.Sum(polygon => polygon.vertices.Count);

                        KeyboardState keyboard_state = Keyboard.GetState();
                        float scaleFactor = 1.0f;

                        if (keyboard_state.IsKeyDown(Keys.W)) 
                        {
                            scaleFactor = 1.1f; 
                        }
                        else if (keyboard_state.IsKeyDown(Keys.S)) 
                        {
                            scaleFactor = 0.9f; 
                        }
                        Matrix2 translationMatrix = Matrix2.CreateTranslation(-localCenter.X, -localCenter.Y);

                        Matrix2 scaleMatrix = Matrix2.CreateScale(scaleFactor, scaleFactor);

                        Matrix2 backTransformationMatrix = Matrix2.CreateTranslation(localCenter.X, localCenter.Y);
                        // матрица трансляции для перемещения в начало координат

                        // итоговая матрицу преобразования (сначала трансляция, затем масштабирование)
                        Matrix2 transformationMatrix = translationMatrix * scaleMatrix * backTransformationMatrix;

                        float shift_X = 0;
                        float shift_Y = 0;
                        const float shiftAmount = 4f;
                        if (keyboard_state.IsKeyDown(Keys.F))
                        {
                            shift_X -= shiftAmount;
                        }
                        if (keyboard_state.IsKeyDown(Keys.H))
                        {
                            shift_X += shiftAmount;
                        }
                        if (keyboard_state.IsKeyDown(Keys.T))
                        {
                            shift_Y -= shiftAmount;
                        }
                        if (keyboard_state.IsKeyDown(Keys.G))
                        {
                            shift_Y += shiftAmount;
                        }
                        var finalTranslationMatrix = new Matrix2()
                        {
                            M11 = 1, M12 = 0,
                            M21 = 0, M22 = 1,
                            M31 = shift_X, M32 = shift_Y,
                        };
                        transformationMatrix *= finalTranslationMatrix;
                        foreach (var polygon in pols_scaling)
                        {
                            polygon.LocalTransformations *= transformationMatrix;
                            //for (int i = 0; i < polygon.vertices.Count; i++)
                            //{
                            //    Vector3 transformedPoint = Vector3.Transform(new Vector3(polygon.vertices[i], 0), transformationMatrix);
                            //    polygon.vertices[i] = new Vector2(transformedPoint.X, transformedPoint.Y);
                            //}
                        }
                    }
                    break;
                case State.Rotation90:
                    if (!hasRotated90)
                    {
                        var edges = polygons.Where(x => x.vertices.Count == 2).ToList();
                        foreach (var edge in edges)
                        {
                            var middle = (edge.vertices[0] + edge.vertices[1]) / 2;
                            var shift_center = Matrix2.CreateTranslation(-middle);
                            var rotate_matrix = Matrix2.CreateRotationZ(MathF.PI / 2);
                            var unshift_center = Matrix2.CreateTranslation(middle);
                            edge.LocalTransformations *= shift_center * rotate_matrix * unshift_center;
                            hasRotated90 = true;
                        }
                        //if (edges.Count > 0)
                        //{
                        //    var firstEdge = edges[0];

                        //    var edgeCenter = Vector2.Zero;
                        //    foreach (var point in firstEdge)
                        //    {
                        //        edgeCenter += point;
                        //    }
                        //    edgeCenter /= firstEdge.Count();

                        //    // Угол поворота на 90 градусов
                        //    float rotationAngle = MathHelper.PiOver2;

                        //    // Вычисляем новые координаты для точек отрезка после поворота
                        //    for (int i = 0; i < firstEdge.Count(); i++)
                        //    {
                        //        var point = firstEdge[i];

                        //        // Перенос точки к началу координат
                        //        var translatedPoint = point - edgeCenter;

                        //        // Применение матрицы поворота
                        //        var rotatedX = translatedPoint.X * (float)Math.Cos(rotationAngle) - translatedPoint.Y * (float)Math.Sin(rotationAngle);
                        //        var rotatedY = translatedPoint.X * (float)Math.Sin(rotationAngle) + translatedPoint.Y * (float)Math.Cos(rotationAngle);

                        //        // Обратный перенос обратно в исходное положение
                        //        var finalPoint = new Vector2(rotatedX, rotatedY) + edgeCenter;

                        //        firstEdge[i] = finalPoint;
                        //    }

                        //    // Обновление фигуры polygons после поворота ребра
                        //    foreach (var polygon in polygons)
                        //    {
                        //        for (int i = 0; i < polygon.vertices.Count(); i++)
                        //        {
                        //            // Перенос точки к началу координат
                        //            var translatedPoint = polygon.vertices[i] - edgeCenter;

                        //            // Применение матрицы поворота
                        //            var rotatedX = translatedPoint.X * (float)Math.Cos(rotationAngle) - translatedPoint.Y * (float)Math.Sin(rotationAngle);
                        //            var rotatedY = translatedPoint.X * (float)Math.Sin(rotationAngle) + translatedPoint.Y * (float)Math.Cos(rotationAngle);

                        //            // Обратный перенос обратно в исходное положение
                        //            var finalPoint = new Vector2(rotatedX, rotatedY) + edgeCenter;

                        //            polygon[i] = finalPoint;
                        //        }
                        //    }

                        //    hasRotated90 = true;
                        //}
                    }
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
            foreach (var polygon in polygons)
            {
                if (polygon.vertices.Count == 1)
                {
                    if (state != State.Editing)
                        _spriteBatch.DrawPoint(Vector3.Transform(new Vector3(polygon.vertices[0], 0), polygon.LocalTransformations).ToVector2() + this.CenterOfCoordinates, Color.Black, 2.5f);
                    else
                        _spriteBatch.DrawPoint(polygon.vertices[0] + CenterOfCoordinates, Color.Black, 2.5f);
                }
                else
                {
                    foreach (var edge in polygon.edges)
                    {
                        if (state != State.Editing)
                            _spriteBatch.DrawLine(
                            Vector3.Transform(new Vector3(polygon.vertices[edge.Item1], 0)
                                ,polygon.LocalTransformations).ToVector2()+CenterOfCoordinates,
                            Vector3.Transform(new Vector3(polygon.vertices[edge.Item2], 0),
                                polygon.LocalTransformations).ToVector2()+CenterOfCoordinates,Color.Black);
                        else
                            _spriteBatch.DrawLine(polygon.vertices[edge.Item1] + CenterOfCoordinates,
                            polygon.vertices[edge.Item2] + CenterOfCoordinates, Color.Black);
                    }
                }
            }
            if (state == State.Checking)
            {
                foreach (var item in CrossingDots)
                {
                    _spriteBatch.DrawPoint(item + CenterOfCoordinates, Color.Red, size:5.0f);
                    
                }
            }
            if (chosenPolygon != -1)
            {
                float minX = float.MaxValue;
                float minY = float.MaxValue;
                float maxX = float.MinValue;
                float maxY = float.MinValue;
                foreach (var vert in polygons[chosenPolygon].GetTransformedCopy().vertices)
                {
                    if (vert.X > maxX)
                    {
                        maxX = vert.X;
                    }
                    if (vert.Y > maxY)
                    {
                        maxY = vert.Y;
                    }
                    if (vert.X < minX)
                    {
                        minX = vert.X;
                    }
                    if (vert.Y < minY)
                    {
                        minY = vert.Y;
                    }
                }
                _spriteBatch.DrawLine(minX+CenterOfCoordinates.X,minY+CenterOfCoordinates.Y,maxX+CenterOfCoordinates.X,minY+CenterOfCoordinates.Y,Color.Green);
                _spriteBatch.DrawLine(maxX+CenterOfCoordinates.X,minY+CenterOfCoordinates.Y,maxX+CenterOfCoordinates.X,maxY+CenterOfCoordinates.Y,Color.Green);
                _spriteBatch.DrawLine(maxX+CenterOfCoordinates.X,maxY+CenterOfCoordinates.Y,minX+CenterOfCoordinates.X,maxY+CenterOfCoordinates.Y,Color.Green);
                _spriteBatch.DrawLine(minX+CenterOfCoordinates.X,maxY+CenterOfCoordinates.Y,minX+CenterOfCoordinates.X,minY+CenterOfCoordinates.Y,Color.Green);
            }
            /*switch (this.state)
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
                case State.Scaling:
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
                    break;
                case State.Rotation90:
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
                    break;

            }*/
            if (state == State.Transforming)
            {
                _spriteBatch.DrawString(font, "WASD to rotate and scale polygon.\n" +
                    "R to stop rotation.\n" +
                    "TFGH to move polygon.\n", new Vector2(0, 0), Color.Black);
            }
            _spriteBatch.End();
            // TODO: Add your drawing code here
            this.uiSystem.Draw(gameTime, this._spriteBatch);
            
            base.Draw(gameTime);
        }
    }
}