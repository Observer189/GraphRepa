using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MLEM.Font;
using MLEM.Input;
using MLEM.Ui;
using MLEM.Ui.Style;
using MonoGame.Extended;
using System.Reflection.PortableExecutable;

namespace Lab9Horizon
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        public SpriteFont font;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            inputHandler = new InputHandler(this);
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            font = Content.Load<SpriteFont>("defaultfont");
            uiSystem = new UiSystem(this, new UntexturedStyle(this._spriteBatch) { Font = new GenericSpriteFont(font) });
            // TODO: use this.Content to load your game content here
        }
        public UiSystem uiSystem;
        InputHandler inputHandler;
        protected override void Update(GameTime gameTime)
        {
            inputHandler.Update(gameTime);
            if (inputHandler.IsDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);


            //Удали, когда начнёшь что-то делать
            _spriteBatch.Begin();
            _spriteBatch.DrawLine(new Vector2(200, 200), new Vector2(250, 250), Color.Black, 3);
            _spriteBatch.DrawLine(new Vector2(250, 250), new Vector2(300, 250), Color.Black, 3);
            _spriteBatch.DrawLine(new Vector2(300, 250), new Vector2(350, 200), Color.Black, 3);
            _spriteBatch.DrawCircle(new Vector2(220, 100), 30, 100, Color.Black, 3);
            _spriteBatch.DrawCircle(new Vector2(330, 100), 30, 100, Color.Black, 3);

            _spriteBatch.End();
            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
    }
}