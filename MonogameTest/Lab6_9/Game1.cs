﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MLEM.Cameras;
using MLEM.Extensions;
using MLEM.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using static MLEM.Graphics.StaticSpriteBatch;

namespace Lab6_9
{
    public enum CurrentCamera
    {
        Axonometric,
        Perspective
    }
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        int ScreenWidth = 1280;
        int ScreenHeight = 720;
        List<Object3D> objects = new List<Object3D>() {  };
        List<PrimitiveShape> shapes = new List<PrimitiveShape>() { };
        PrimitiveShape OXYZLines = PrimitiveShape.OXYZLines(4);
        CurrentCamera CurrentCamera = CurrentCamera.Axonometric;
        //Индекс текущего объекта, -1 если никакой объект не выбран
        int objectsIndex = -1;
        int shapesIndex = -1;
        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            _graphics.PreferredBackBufferWidth = ScreenWidth;
            _graphics.PreferredBackBufferHeight = ScreenHeight;
        }
        Texture2D screenTexture = null;
        protected override void Initialize()
        {
            //Добавлять объекты и их сдвигать можно тут
            var cubeObj = Object3D.Cube();
            cubeObj.transformationMatrix *= Matrix.CreateTranslation(3, 0, 0);
            objects.Add(cubeObj);

            var cubeShape = PrimitiveShape.Cube();
            shapes.Add(cubeShape);


            // TODO: Add your initialization logic here
            screenTexture = new Texture2D(GraphicsDevice, ScreenWidth, ScreenHeight);
            inputHandler = new InputHandler(this);
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
        }

        


        /// <summary>
        /// 
        /// </summary>
        /// <param name="buffer">Ожидается первая координата Y, вторая - X</param>
        /// <param name="line"></param>
        /// <param name="scale"></param>
        /// <param name="offset"></param>
        /// <param name="color"></param>
        static void DrawLine(Color[,] buffer, (Vector2 begin, Vector2 end) line, float scale, Vector2 offset, Color color)
        {
            (var x1, var y1) = (line.begin * scale + offset).ToPoint();
            (var x2, var y2) = (line.end * scale + offset).ToPoint();

            int dx = Math.Abs(x2 - x1);
            int dy = Math.Abs(y2 - y1);
            int sx = (x1 < x2) ? 1 : -1;
            int sy = (y1 < y2) ? 1 : -1;
            int err = dx - dy;
            var bufferlimX = buffer.GetLength(1);
            var bufferlimY = buffer.GetLength(0);

            while (true)
            {
                if (x1 >= 0 && x1 < bufferlimX && y1 >= 0 && y1 < bufferlimY)
                    buffer[y1, x1] = color;
                //Console.WriteLine($"{x1}, {y1}");
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
        static void DrawWireFrame(Color[,] buffer, List<(Vector2 begin, Vector2 end)> lines, float scale, Vector2 offset, Color color)
        {
            foreach (var line in lines)
            {
                DrawLine(buffer, line, scale, offset, color);
            }
        }
        List<(Vector2 begin, Vector2 end)> ProjectWireFrameWithMatrix(Matrix projectionMatrix, PrimitiveShape shape)
        {
            var wf = new List<(Vector2 begin, Vector2 end)>();
            var vertixes = shape.vertixes;
            var resMatrix = shape.transformationMatrix * projectionMatrix;
            foreach (var polygon in shape.polygons)
            {
                for (int i = 0; i < polygon.Length; i++)
                {
                    var i1 = (i + 1 == polygon.Length) ? 0 : i + 1;
                    var v1 = new Vector4(vertixes[polygon[i]], 1);
                    var v2 = new Vector4(vertixes[polygon[i1]], 1);
                    var begin = Vector4.Transform(v1, resMatrix);
                    var end = Vector4.Transform(v2, resMatrix);
                    var vec2begin = new Vector2 { X = begin.X / begin.W, Y = -begin.Y / begin.W };
                    var vec2end = new Vector2 { X = end.X / end.W, Y = -end.Y / end.W };

                    wf.Add((vec2begin, vec2end));
                }
            }
            return wf;
        }
        List<(Vector2 begin, Vector2 end)> ProjectWireFrameWithMatrix(Matrix projectionMatrix, Object3D shape)
        {
            var wf = new List<(Vector2 begin, Vector2 end)>();
            var vertixes = shape.vertixes;
            var resMatrix = shape.transformationMatrix * projectionMatrix;
            foreach (var polygon in shape.triangles)
            {
                
                var v1 = new Vector4(vertixes[polygon.v1], 1);
                var v2 = new Vector4(vertixes[polygon.v2], 1);
                var v3 = new Vector4(vertixes[polygon.v3], 1);
                var v1Tr = Vector4.Transform(v1, resMatrix);
                var v2Tr = Vector4.Transform(v2, resMatrix);
                var v3Tr = Vector4.Transform(v3, resMatrix);

                var v1v2 = new Vector2 { X = v1Tr.X / v1Tr.W, Y = -v1Tr.Y / v1Tr.W };
                var v2v2 = new Vector2 { X = v2Tr.X / v2Tr.W, Y = -v2Tr.Y / v2Tr.W };
                var v3v2 = new Vector2 { X = v3Tr.X / v3Tr.W, Y = -v3Tr.Y / v3Tr.W };

                
                wf.Add((v1v2, v2v2));
                wf.Add((v2v2, v3v2));
                wf.Add((v3v2, v1v2));


            }
            return wf;
        }
        (float phi, float psi) AxonometricProjectionAngles = (float.Pi / 4, float.Pi / 4);
        //(float phi, float psi) AxonometricProjectionAngles = (0, 0);
        //(float phi, float psi) AxonometricProjectionAngles = (float.Pi / 4, 0);

        static Matrix GetAxonometric(float phi, float psi)
        {
            var cospsi = MathF.Cos(psi);
            var cosphi = MathF.Cos(phi);
            var sinpsi = MathF.Sin(psi);
            var sinphi = MathF.Sin(phi);
            var matrix = new Matrix()
            {
                M11 = cospsi, M12 = sinphi * sinpsi,
                M21 = 0, M22 = cosphi,
                M31 = sinpsi, M32 = -sinphi * cospsi,
                M44 = 1,
            };
            return matrix;
        }
        static Matrix GetPerspective(float scale)
        {
            var matrix = new Matrix()
            {
                M11 = 1, 
                M22 = 1, 
                M33 = 0, M34 = -1/scale,
                M44 = 1,
            };
            return matrix;
        }
        InputHandler inputHandler = null;
        protected override void Update(GameTime gameTime)
        {
            inputHandler.Update(gameTime);
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            //var k = Keyboard.GetState();
            if (inputHandler.IsDown(Keys.W))
            {
                AxonometricProjectionAngles.phi -= 0.1f;
            }
            if (inputHandler.IsDown(Keys.S))
            {
                AxonometricProjectionAngles.phi += 0.1f;
            }
            if (inputHandler.IsDown(Keys.D))
            {
                AxonometricProjectionAngles.psi += 0.1f;
            }
            if (inputHandler.IsDown(Keys.A))
            {
                AxonometricProjectionAngles.psi -= 0.1f;
            }
            if (inputHandler.IsPressed(Keys.C))
            {
                CurrentCamera = CurrentCamera switch
                {
                    CurrentCamera.Axonometric => CurrentCamera.Perspective,
                    CurrentCamera.Perspective => CurrentCamera.Axonometric,
                    _ => throw new NotImplementedException()
                };
            }
            if (inputHandler.IsPressed(Keys.Tab))
            {
                if (shapesIndex == -1)
                {
                    if (objectsIndex < objects.Count - 1) { objectsIndex++; } else { objectsIndex = -1; shapesIndex++; }
                }
                else if(objectsIndex  == -1) {
                    if (shapesIndex < shapes.Count - 1) { shapesIndex++; } else { shapesIndex = -1; objectsIndex = -1; }

                }
            }
            base.Update(gameTime);
        }
        
        protected override void Draw(GameTime gameTime)
        {
            //Первая координата - Y, вторая - X. Так быстрее.
            var buffer = new Color[ScreenHeight, ScreenWidth];
            Vector2 center = new Vector2(ScreenWidth / 2, ScreenHeight / 2);
            float scale = CurrentCamera switch {

                CurrentCamera.Axonometric => 50f,
                CurrentCamera.Perspective => 200f,
                _ => throw new NotImplementedException()

            };
            var camera = CurrentCamera switch
            {
                CurrentCamera.Axonometric => GetAxonometric(AxonometricProjectionAngles.phi, AxonometricProjectionAngles.psi),
                CurrentCamera.Perspective => Matrix.CreateRotationZ(-AxonometricProjectionAngles.psi) * Matrix.CreateRotationX(-AxonometricProjectionAngles.phi) * Matrix.CreateTranslation(0, 0, -5) * GetPerspective(1.3f),
                _ => throw new NotImplementedException()

            };
            //var camera = GetAxonometric(AxonometricProjectionAngles.phi, AxonometricProjectionAngles.psi);
            //var camera = Matrix.CreateRotationZ(-AxonometricProjectionAngles.psi) * Matrix.CreateRotationX(-AxonometricProjectionAngles.phi) * Matrix.CreateTranslation(0, 0, -5) * GetPerspective(1.3f);
            //TODO: Добавить перспективную матричную камеру


            for (int i = 0; i < shapes.Count; i++)
            {
                var t = ProjectWireFrameWithMatrix(camera, shapes[i]);
                if (i == shapesIndex) {
                    DrawWireFrame(buffer, t, scale, center, Color.Green);
                }
                else
                {
                    DrawWireFrame(buffer, t, scale, center, Color.Black);
                }
            }
            for (int i = 0; i < objects.Count; i++)
            {
                var t = ProjectWireFrameWithMatrix(camera, objects[i]);
                if (i == objectsIndex)
                {
                    DrawWireFrame(buffer, t, scale, center, Color.Green);
                }
                else
                {
                    DrawWireFrame(buffer, t, scale, center, Color.Black);

                }
            }


            //Линия координат
            DrawWireFrame(buffer, ProjectWireFrameWithMatrix(camera, OXYZLines), scale, center, Color.Blue);
            
            
            var textureBuffer = new Color[ScreenHeight * ScreenWidth];
            for (int y = 0; y < buffer.GetLength(0); y++)
            {
                for (int x = 0; x < buffer.GetLength(1); x++)
                {
                    textureBuffer[buffer.GetLength(1) * y + x] = buffer[y, x];
                }

            }
            screenTexture.SetData(textureBuffer);
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();
            _spriteBatch.Draw(screenTexture, new Vector2(0), Color.White);
            _spriteBatch.End();

            

            base.Draw(gameTime);
        }
    }
}