﻿using Microsoft.Xna.Framework;
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
    Scaling,
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
                },
            };

            panel.AddChild(button2);
            var button5 = new Button(Anchor.AutoLeft, size: new Vector2(150, 60), text: "Scaling relative to center(w, s)")
            {
                OnPressed = (elem) =>
                {
                    this.state = State.Scaling;
                },
            };
            panel.AddChild(button5);

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
            CenterOfCoordinates = field.Center.ToVector2();
            field.Width -= 100;
            var mouseState = Mouse.GetState();
            if (inputHandler.IsPressed(MouseButton.Right) && field.Contains(mouseState.Position))
            {
                for (int i = 0; i < polygons.Count; i++)
                {
                    if (polygons[i].IsInside(mouseState.Position.ToVector2()-CenterOfCoordinates))
                    {
                        chosenPolygon = i;
                    }
                }   
            }
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

                        // центр масс фигуры
                        Vector2 center = Vector2.Zero;
                        foreach (var polygon in polygons)
                        {
                            foreach (var point in polygon.vertices)
                            {
                                center += point;
                            }
                        }
                        center /= polygons.Sum(polygon => polygon.vertices.Count);

                        float cosAngle = (float)Math.Cos(rotationAngle);
                        float sinAngle = (float)Math.Sin(rotationAngle);

                        for (int i = 0; i < polygons.Count; i++)
                        {
                            for (int j = 0; j < polygons[i].vertices.Count; j++)
                            {
                                // Перенос фигуры в начало координат
                                var translatedPoint = polygons[i].vertices[j] - center;

                                // Применение поворота
                                var rotatedX = translatedPoint.X * cosAngle - translatedPoint.Y * sinAngle;
                                var rotatedY = translatedPoint.X * sinAngle + translatedPoint.Y * cosAngle;

                                // Обратный перенос обратно в исходное положение
                                var finalPoint = new Vector2(rotatedX, rotatedY) + center;

                                polygons[i].vertices[j] = finalPoint;
                            }
                        }
                    break;
                case State.Scaling:
                    {
                        Vector2 localCenter = Vector2.Zero;

                        foreach (var polygon in polygons)
                        {
                            foreach (var point in polygon.vertices)
                            {
                                localCenter += point;
                            }
                        }

                        localCenter /= polygons.Sum(polygon => polygon.vertices.Count);

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

                        Matrix scaleMatrix = Matrix.CreateScale(scaleFactor, scaleFactor, 1f);

                        // матрица трансляции для перемещения в начало координат
                        Matrix translationMatrix = Matrix.CreateTranslation(-localCenter.X, -localCenter.Y, 0f);

                        // итоговая матрицу преобразования (сначала трансляция, затем масштабирование)
                        Matrix transformationMatrix = scaleMatrix * translationMatrix;

                        foreach (var polygon in polygons)
                        {
                            for (int i = 0; i < polygon.vertices.Count; i++)
                            {
                                Vector3 transformedPoint = Vector3.Transform(new Vector3(polygon.vertices[i], 0), transformationMatrix);
                                polygon.vertices[i] = new Vector2(transformedPoint.X, transformedPoint.Y);
                            }
                        }
                    }
                    break;
                /*case State.Rotation90:
                    if (!hasRotated90)
                    {
                        if (edges.Count > 0)
                        {
                            var firstEdge = edges[0];

                            var edgeCenter = Vector2.Zero;
                            foreach (var point in firstEdge)
                            {
                                edgeCenter += point;
                            }
                            edgeCenter /= firstEdge.Count;

                            // Угол поворота на 90 градусов
                            float rotationAngle = MathHelper.PiOver2;

                            // Вычисляем новые координаты для точек отрезка после поворота
                            for (int i = 0; i < firstEdge.Count; i++)
                            {
                                var point = firstEdge[i];

                                // Перенос точки к началу координат
                                var translatedPoint = point - edgeCenter;

                                // Применение матрицы поворота
                                var rotatedX = translatedPoint.X * (float)Math.Cos(rotationAngle) - translatedPoint.Y * (float)Math.Sin(rotationAngle);
                                var rotatedY = translatedPoint.X * (float)Math.Sin(rotationAngle) + translatedPoint.Y * (float)Math.Cos(rotationAngle);

                                // Обратный перенос обратно в исходное положение
                                var finalPoint = new Vector2(rotatedX, rotatedY) + edgeCenter;

                                firstEdge[i] = finalPoint;
                            }

                            // Обновление фигуры polygons после поворота ребра
                            foreach (var polygon in polygons)
                            {
                                for (int i = 0; i < polygon.Count; i++)
                                {
                                    // Перенос точки к началу координат
                                    var translatedPoint = polygon[i] - edgeCenter;

                                    // Применение матрицы поворота
                                    var rotatedX = translatedPoint.X * (float)Math.Cos(rotationAngle) - translatedPoint.Y * (float)Math.Sin(rotationAngle);
                                    var rotatedY = translatedPoint.X * (float)Math.Sin(rotationAngle) + translatedPoint.Y * (float)Math.Cos(rotationAngle);

                                    // Обратный перенос обратно в исходное положение
                                    var finalPoint = new Vector2(rotatedX, rotatedY) + edgeCenter;

                                    polygon[i] = finalPoint;
                                }
                            }

                            hasRotated90 = true;
                        }
                    }
                    break;*/
                   

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
                    _spriteBatch.DrawPoint(Vector3.Transform(new Vector3(polygon.vertices[0], 0),polygon.LocalTransformations).ToVector2() + this.CenterOfCoordinates, Color.Black, 2.5f);
                }
                else
                {
                    foreach (var edge in polygon.edges)
                    {
                        _spriteBatch.DrawLine(
                            Vector3.Transform(new Vector3(polygon.vertices[edge.Item1], 0)
                                ,polygon.LocalTransformations).ToVector2()+CenterOfCoordinates,
                            Vector3.Transform(new Vector3(polygon.vertices[edge.Item2], 0),
                                polygon.LocalTransformations).ToVector2()+CenterOfCoordinates,Color.Black);
                    }
                }
            }

            if (chosenPolygon != -1)
            {
                float minX = float.MaxValue;
                float minY = float.MaxValue;
                float maxX = float.MinValue;
                float maxY = float.MinValue;
                foreach (var vert in polygons[chosenPolygon].vertices)
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
                _spriteBatch.DrawLine(minX+CenterOfCoordinates.X,minY+CenterOfCoordinates.Y,maxX+CenterOfCoordinates.X,minY+CenterOfCoordinates.Y,Color.Yellow);
                _spriteBatch.DrawLine(maxX+CenterOfCoordinates.X,minY+CenterOfCoordinates.Y,maxX+CenterOfCoordinates.X,maxY+CenterOfCoordinates.Y,Color.Yellow);
                _spriteBatch.DrawLine(maxX+CenterOfCoordinates.X,maxY+CenterOfCoordinates.Y,minX+CenterOfCoordinates.X,maxY+CenterOfCoordinates.Y,Color.Yellow);
                _spriteBatch.DrawLine(minX+CenterOfCoordinates.X,maxY+CenterOfCoordinates.Y,minX+CenterOfCoordinates.X,minY+CenterOfCoordinates.Y,Color.Yellow);
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
            
            _spriteBatch.End();
            // TODO: Add your drawing code here
            this.uiSystem.Draw(gameTime, this._spriteBatch);
            
            base.Draw(gameTime);
        }
    }
}