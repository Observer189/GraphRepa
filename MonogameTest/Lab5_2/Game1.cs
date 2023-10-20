using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System;
using System.Diagnostics;
using System.Threading;
using MLEM.Input;
using MonoGame.Extended;
using static System.Net.Mime.MediaTypeNames;

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
        private int InitialScale = 90;
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
            inputHandler = new InputHandler(this);
            var r = new Random();
            heightMap = new float[TerrainWidth/InitialScale, TerrainHeight/InitialScale];
            for (int x = 0; x < heightMap.GetLength(0); x++)
            {
                for (int y = 0; y < heightMap.GetLength(1); y++)
                {
                    heightMap[x, y] = r.NextSingle();
                }

            }
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
        public InputHandler inputHandler;
        

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
        private float[,] ExtendStep(float [,] heightMap)
        {
            var newWidth = 2 * heightMap.GetLength(0) - 1;
            var newHeight = 2 * heightMap.GetLength(1) - 1;
            var newHeightMap = new float[newWidth, newHeight];
            for (int x = 0; x < heightMap.GetLength(0); x++)
            {
                for (int y = 0; y < heightMap.GetLength(1); y++)
                {
                    var tX = 2*x;
                    var tY = 2*y;
                    newHeightMap[tX, tY] = heightMap[x, y];
                }

            }
            return newHeightMap;
        }
        private void SquareStep(float[,] heightmap)
        {
            for (int x = 1;x < heightmap.GetLength(0); x += 2)
            {
                for (int y = 1; y < heightmap.GetLength(1); y += 2)
                {
                    heightMap[x,y] = (heightmap[x + 1, y + 1] + heightmap[x + 1, y - 1] + heightmap[x - 1, y + 1] + heightmap[x - 1, y - 1]) / 4;
                }

            }
        }
        private void DiamondStep(float[,] heightmap)
        {
            for (int x = 0; x < heightmap.GetLength(0);x += 2)
            {
                for (int y = 1; y < heightmap.GetLength(1); y += 2)
                {
                    if (x == 0)
                    {
                        heightMap[x, y] = (heightmap[x + 1, y] + heightmap[x, y + 1] + heightmap[x, y - 1]) / 3;
                    }
                    else if (x >= heightMap.GetLength(0) - 2){
                        heightMap[x, y] = (heightmap[x - 1, y] + heightmap[x, y + 1] + heightmap[x, y - 1]) / 4;
                    }
                    else
                    {
                        heightMap[x, y] = (heightmap[x + 1, y] + heightmap[x - 1, y] + heightmap[x, y + 1] + heightmap[x, y - 1]) / 4;
                    }
                }
            }
            for (int x = 1; x < heightmap.GetLength(0); x += 2)
            {
                for (int y = 0; y < heightmap.GetLength(1); y += 2)
                {
                    if (y == 0)
                    {
                        heightMap[x, y] = (heightmap[x + 1, y] + heightmap[x, y + 1] + heightmap[x - 1, y]) / 3;
                    }
                    else if (y >= heightMap.GetLength(1) - 2)
                    {
                        heightMap[x, y] = (heightmap[x - 1, y] + heightmap[x + 1, y] + heightmap[x, y - 1]) / 4;
                    }
                    else
                    {
                        heightMap[x, y] = (heightmap[x + 1, y] + heightmap[x - 1, y] + heightmap[x, y + 1] + heightmap[x, y - 1]) / 4;
                    }
                }
            }
        }
        int state = 0;
        bool isAltHeightMap = false;
        protected override void Update(GameTime gameTime)
        {
            inputHandler.Update(gameTime);

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            if (inputHandler.IsPressed(Keys.S))
            {
                if (state == 0)
                {
                    heightMap = ExtendStep(heightMap);
                }
                if (state == 1)
                {
                    SquareStep(heightMap);
                }
                if (state == 2)
                {
                    DiamondStep(heightMap);
                }
                state = (state + 1) % 3;
                heightTexture = null;
            }
            if (inputHandler.IsPressed(Keys.A))
            {
                isAltHeightMap = !isAltHeightMap;
                heightTexture = null;
            }

                base.Update(gameTime);
        }
        Texture2D HeighmapToTexture(float[,] heightmap)
        {
            var t = new Texture2D(GraphicsDevice, TerrainWidth, TerrainHeight);
            var c = new Color[TerrainWidth * TerrainHeight];
            double xArr = heightMap.GetLength(0);
            double yArr = heightMap.GetLength(1);
            for (int y = 0; y < TerrainHeight; y++)
                for (int x = 0; x < TerrainWidth; x++)
                {
                    var hX = (int)Math.Floor(Math.Clamp(xArr / TerrainWidth * x, 0, xArr - 1));
                    var hY = (int)Math.Floor(Math.Clamp(yArr / TerrainHeight * y, 0, yArr - 1));
                    c[TerrainWidth * y + x] = DetermineTerrainColor(heightMap[hX, hY]);
                }
            t.SetData(c);
            return t;
        }
        Texture2D HeighmapToTextureAlt(float[,] heightmap)
        {
            var t = new Texture2D(GraphicsDevice, TerrainWidth, TerrainHeight);
            var c = new Color[TerrainWidth * TerrainHeight];
            double xArr = heightMap.GetLength(0);
            double yArr = heightMap.GetLength(1);
            for (int y = 0; y < TerrainHeight; y++)
                for (int x = 0; x < TerrainWidth; x++)
                {
                    var hX = (int)Math.Floor(Math.Clamp(xArr / TerrainWidth * x, 0, xArr - 1));
                    var hY = (int)Math.Floor(Math.Clamp(yArr / TerrainHeight * y, 0, yArr - 1));
                    c[TerrainWidth * y + x] = DetermineTerrainColorAlt(heightMap[hX, hY]);
                }
            t.SetData(c);
            return t;
        }
        Texture2D heightTexture = null;
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            if (heightTexture == null) { heightTexture = isAltHeightMap ?  HeighmapToTextureAlt(heightMap) : HeighmapToTexture(heightMap); }
            _spriteBatch.Begin();
            double xArr = heightMap.GetLength(0);
            double yArr = heightMap.GetLength(1);
            
            _spriteBatch.Draw(heightTexture, new Rectangle(0, 0, TerrainWidth, TerrainHeight), Color.White);

            _spriteBatch.End();

            base.Draw(gameTime);
        }
        private Color DetermineTerrainColorAlt(float terrainValue)
        {
            Color fromColor = Color.Black;
            Color toColor = Color.White;
            return Color.Lerp(fromColor, toColor, terrainValue);
        }
        private Color DetermineTerrainColor(float terrainValue)
        {
            Color fromColor = Color.DarkGray;
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
