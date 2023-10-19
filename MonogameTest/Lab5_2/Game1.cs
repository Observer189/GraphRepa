using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System;
using System.Diagnostics;
using System.Threading;

namespace Lab5_2
{
   
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private int size = 1080;
        //private int size = 600;
        //private int size = 150;
        private int roughness = 10; // Насыщенность ландшафта
        private Dictionary<string, float> data;

        //private int seed = 10;
        private int seed = 12345;
        Texture2D whiteTexture;

        private int TerrainWidth = 1080; 
        private int TerrainHeight = 1080;

        private float[,] heightMap;


        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferredBackBufferWidth = TerrainWidth;
            _graphics.PreferredBackBufferHeight = TerrainHeight;
            data = new Dictionary<string, float>();
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            heightMap = new float[TerrainWidth, TerrainHeight];
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            whiteTexture = new Texture2D(GraphicsDevice, 1, 1);
            Color[] colorData = new Color[1];
            colorData[0] = Color.White;
            whiteTexture.SetData(colorData);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            base.Update(gameTime);
        }

        public float Val(int x, int y, float? v = null)
        {
            if (v.HasValue)
            {
                data[x + "_" + y] = MathHelper.Clamp(v.Value, 0.0f, 1.0f);
            }
            else
            {
                if (x <= 0 || x >= size || y <= 0 || y >= size)
                {
                    return 0.0f;
                }
                if (!data.ContainsKey(x + "_" + y))
                {
                    int baseSize = 1;
                    while ((x & baseSize) == 0 && (y & baseSize) == 0)
                    {
                        baseSize <<= 1;
                    }

                    if ((x & baseSize) != 0 && (y & baseSize) != 0)
                    {
                        SquareStep(x, y, baseSize);
                    }
                    else
                    {
                        DiamondStep(x, y, baseSize);
                    }
                }
            }

            return data[x + "_" + y];
        }
        public float Displace(float v, int blockSize, int x, int y)
        {
            return v + (RandFromPair(x, y) - 0.5f) * blockSize * 2.0f / size * roughness;
        }
        private float RandFromPair(int x, int y)
        {
            int xm7 = 0, xm13 = 0, xm1301081 = 0, ym8461 = 0, ym105467 = 0, ym105943 = 0;

            for (int i = 0; i < 80; i++)
            {
                xm7 = x % 7;
                xm13 = x % 13;
                xm1301081 = x % 1301081;
                ym8461 = y % 8461;
                ym105467 = y % 105467;
                ym105943 = y % 105943;
                y = x + seed;
                x += (xm7 + xm13 + xm1301081 + ym8461 + ym105467 + ym105943);
            }
            return (xm7 + xm13 + xm1301081 + ym8461 + ym105467 + ym105943) / 1520972.0f;
        }

        private void SquareStep(int x, int y, int blockSize)
        {
            if (!data.ContainsKey(x + "_" + y))
            {
                float displacedValue = Displace(
                    (Val(x - blockSize, y - blockSize) +
                     Val(x + blockSize, y - blockSize) +
                     Val(x - blockSize, y + blockSize) +
                     Val(x + blockSize, y + blockSize)) / 4, blockSize, x, y);

                heightMap[x, y] = displacedValue; // Сохраняем высоту в буфер данных
                Val(x, y, displacedValue);

            }
        }

        private void DiamondStep(int x, int y, int blockSize)
        {
            if (!data.ContainsKey(x + "_" + y))
            {
                float displacedValue = Displace(
                    (Val(x - blockSize, y) +
                     Val(x + blockSize, y) +
                     Val(x, y - blockSize) +
                     Val(x, y + blockSize)) / 4, blockSize, x, y);

                heightMap[x, y] = displacedValue; // Сохраняем высоту в буфер данных
                Val(x, y, displacedValue);

            }
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();

            for (int x = 0; x < TerrainWidth; x++)
            {
                for (int y = 0; y < TerrainHeight; y++)
                {
                    Val(x, y);
                    float terrainValue = heightMap[x, y]; // Используем данные из буфера

                    //Debug.WriteLine($"Val at ({x}, {y}): {Val(x, y)}");
                    //Debug.WriteLine($"terrainValue at ({x}, {y}): {terrainValue}");
                    Color terrainColor = DetermineTerrainColor(terrainValue);
                    Rectangle rect = new Rectangle(x, y, 1, 1);
                    _spriteBatch.Draw(whiteTexture, rect, terrainColor);
                }

            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }

        private Color DetermineTerrainColor(float terrainValue)
        {
            Color fromColor = Color.Gray;
            Color toColor = Color.White;

            if (terrainValue < 0.2f)
            {
                return Color.Lerp(fromColor, toColor, terrainValue / 0.2f);
            }
            else if (terrainValue < 0.4f)
            {
                return Color.Lerp(fromColor, toColor, (terrainValue - 0.2f) / 0.2f);
            }
            else if (terrainValue < 0.85f)
            {
                return Color.Lerp(fromColor, toColor, (terrainValue - 0.4f) / 0.4f);
            }
            else
            {
                return toColor; 
            }
        }

    }
}
