using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MLEM.Font;
using MLEM.Input;
using MLEM.Misc;
using MLEM.Ui;
using MLEM.Ui.Elements;
using MLEM.Ui.Style;
using MonoGame.Extended;
using RectangleF = MLEM.Misc.RectangleF;

namespace Lab5_3
{
    public enum State
    {
        Adding,
        Deleting,
        Editing,
    }
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        public UiSystem uiSystem;
        public InputHandler inputHandler;
        public SpriteFont font;
        public State state;
        private Panel root;

        private List<BezierPoint> mainPoints = new List<BezierPoint>();

        private float interactionRadius = 3f;
        private MouseState lastFrameState;
        private MouseState curMouseState;
        private BezierPoint dragPoint = null;
        private int dragInd;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            Window.AllowUserResizing = true;
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            font = Content.Load<SpriteFont>("defaultfont");

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
            var button0 = new Button(Anchor.AutoLeft, size: new Vector2(150, 30), text: "Add point") 
            { OnPressed = (elem) =>
                {
                    this.state = State.Adding;
                }
            };
            panel.AddChild(button0);
            var button1 = new Button(Anchor.AutoLeft, size: new Vector2(150, 30), text: "Edit") { OnPressed = (elem) => { this.state = State.Editing; } };
            panel.AddChild(button1);
            var button2 = new Button(Anchor.AutoLeft, size: new Vector2(95, 30), text: "Delete") { OnPressed = (elem) => { this.state = State.Deleting; }, 
                PositionOffset = new Vector2(0, 5)};
            panel.AddChild(button2);
            var button3 = new Button(Anchor.AutoLeft, size: new Vector2(150, 30), text: "Clear All") { OnPressed = (elem) => { mainPoints.Clear(); },
                PositionOffset = new Vector2(0, 5)};
            panel.AddChild(button3);
            var text =  new Paragraph(Anchor.AutoLeft, 100, (x) => { return this.state.ToString(); }, true);
            panel.AddChild(text);
            uiSystem.Add("panel", panel);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            inputHandler.Update();
            root.Size = new Vector2(150, Window.ClientBounds.Size.Y);
            var field = GraphicsDevice.PresentationParameters.Bounds;
            field.Width -= 150;
            lastFrameState = curMouseState;
            curMouseState = Mouse.GetState();
            switch (state)
            {
                case State.Adding:
                {
                    if (inputHandler.IsPressed(MouseButton.Left) && field.Contains(curMouseState.Position))
                    {
                        mainPoints.Add(new BezierPoint(curMouseState.Position.ToVector2()));
                    }
                }
                    break;
                case State.Editing:
                {
                    if (dragPoint != null)
                    {
                        if (curMouseState.LeftButton == ButtonState.Released)
                        {
                            dragPoint = null;
                            Mouse.SetCursor(MouseCursor.Arrow);
                        }
                        else
                        {
                            dragPoint[dragInd] = curMouseState.Position.ToVector2();
                        }
                    }
                    else
                    {
                        int ind = -1;
                        int jind = -1;
                        for (int i = 0; i < mainPoints.Count; i++)
                        {
                            for (int j = 0; j < 3; j++)
                            {
                                if (Vector2.Distance(mainPoints[i][j], curMouseState.Position.ToVector2()) < interactionRadius)
                                {
                                    ind = i;
                                    jind = j;
                                    break;
                                }
                            }
                        }
                        if (ind != -1)
                        {
                            Mouse.SetCursor(MouseCursor.Hand);
                        }
                        else
                        {
                            Mouse.SetCursor(MouseCursor.Arrow);
                        }

                        if (ind != -1 && inputHandler.IsPressed(MouseButton.Left))
                        {
                            dragPoint = mainPoints[ind];
                            dragInd = jind;
                        }
                    }

                }
                    break;
                case State.Deleting:
                {
                    int ind = -1;
                    for (int i = 0; i < mainPoints.Count; i++)
                    {
                        if (Vector2.Distance(mainPoints[i][1], curMouseState.Position.ToVector2()) < interactionRadius)
                        {
                            ind = i;
                            break;
                        }
                    }

                    if (ind != -1)
                    {
                        Mouse.SetCursor(MouseCursor.Crosshair);
                    }
                    else
                    {
                        Mouse.SetCursor(MouseCursor.Arrow);
                    }

                    if (inputHandler.IsPressed(MouseButton.Left) && field.Contains(curMouseState.Position))
                    {
                        if (ind != -1)
                        {
                            mainPoints.RemoveAt(ind);
                        }
                    }
                } 
                    break;
            }
            this.uiSystem.Update(gameTime);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);
            _spriteBatch.Begin();

            if (mainPoints.Count > 1)
            {
                for (int i = 0; i < mainPoints.Count - 1; i++)
                {
                    Vector2[] points = new Vector2[4];
                    points[0] = mainPoints[i][1];
                    points[1] = mainPoints[i][2];
                    points[2] = mainPoints[i+1][0];
                    points[3] = mainPoints[i+1][1];
                    float step = 1f / (Vector2.Distance(points[0], points[1])+Vector2.Distance(points[1], points[2])+Vector2.Distance(points[2], points[3]));
                    for (float t = 0f; t <=1f; t+=step)
                    {
                      DrawCurvePoint(points,t,_spriteBatch);                        
                    }
                }
            }
            
            foreach (var point in mainPoints)
            {
                if (state == State.Editing)
                {
                    _spriteBatch.DrawLine(point[0], point[1], Color.Black);
                    _spriteBatch.DrawLine(point[1], point[2], Color.Black);
                    _spriteBatch.DrawPoint(point[0], Color.Blue, 5f);
                    _spriteBatch.DrawPoint(point[2], Color.Blue, 5f);
                }
                _spriteBatch.DrawPoint(point[1],Color.LawnGreen,5f);
            }

            _spriteBatch.End();
            // TODO: Add your drawing code here
            uiSystem.Draw(gameTime,_spriteBatch);
            base.Draw(gameTime);
        }
        
        public void DrawCurvePoint(Vector2[] points, float t, SpriteBatch batch)
        {
            if (points.Length == 1)
            {
                batch.DrawPoint(points[0], Color.Black,2f);
            }
            else
            {
                var newPoints = new Vector2[points.Length - 1];
                for (int i = 0; i < newPoints.Length; i++)
                {
                    newPoints[i] = (1 - t) * points[i] + t * points[i + 1];
                }
                DrawCurvePoint(newPoints, t, batch);
            }
        }
    }
    
}