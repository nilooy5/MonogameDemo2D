using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using RC_Framework;
using System;
using System.IO;

namespace Game1
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        int gameWindowWidth = 800;
        int gameWindowHeight = 600;

        RC_GameStateManager levelManager;

        public static KeyboardState prevKeyState;
        public static KeyboardState keyState;

        string dir = @"C:\Users\fazal_ix0ll8n\source\repos\MonogameDemo2D\MonogameDemo2D\images\";

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferHeight = gameWindowHeight;
            graphics.PreferredBackBufferWidth = gameWindowWidth;
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            LineBatch.init(GraphicsDevice);

            levelManager = new RC_GameStateManager();

            levelManager.AddLevel(0, new GameLevel_0());
            levelManager.getLevel(0).InitializeLevel(GraphicsDevice, spriteBatch, Content, levelManager);
            levelManager.getLevel(0).LoadContent();
            //levelManager.setLevel(0);

            levelManager.AddLevel(1, new GameLevel_1());
            levelManager.getLevel(1).InitializeLevel(GraphicsDevice, spriteBatch, Content, levelManager);
            levelManager.getLevel(1).LoadContent();
            levelManager.setLevel(1);

        }

        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            prevKeyState = keyState;
            keyState = Keyboard.GetState();

            // previousMouseState = currentMouseState;
            // currentMouseState = Mouse.GetState();

            if (keyState.IsKeyDown(Keys.Escape)) this.Exit();

            levelManager.getCurrentLevel().Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            levelManager.getCurrentLevel().Draw(gameTime);

            base.Draw(gameTime);
        }
    }
}
