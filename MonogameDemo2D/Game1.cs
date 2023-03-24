using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using RC_Framework;
using System.IO;

namespace MonogameDemo2D
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        int gameWindowWidth = 800;
        int gameWindowHeight = 600;
        float bottomLimit = 0;

        float xx = 100;
        float yy = 600/2 - 50/2;
        int paddleSpeed = 3;
        int lhs = 236;
        int rhs = 564;
        int bot = 543;

        Texture2D texBack;
        Texture2D texSpaceShip;

        Sprite3 spaceship = null;

        ImageBackground back1 = null;

        bool showBB = false;

        KeyboardState prevK;
        KeyboardState k;

        string dir = @"C:\Users\fazal_ix0ll8n\source\repos\MonogameDemo2D\MonogameDemo2D\images\";

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            _graphics.PreferredBackBufferHeight = gameWindowHeight;
            _graphics.PreferredBackBufferWidth = gameWindowWidth;
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            LineBatch.init(GraphicsDevice);

            texBack = Util.texFromFile(GraphicsDevice, dir + "Back1.png"); //***
            texSpaceShip = Util.texFromFile(GraphicsDevice, dir + "Spaceship3a.png"); //***

            back1 = new ImageBackground(texBack, Color.White, GraphicsDevice);

            spaceship = new Sprite3(true, texSpaceShip, xx, yy);
            spaceship.setHeight(50);
            spaceship.setWidth(100);
            spaceship.setBBToTexture();
            bottomLimit = gameWindowHeight - spaceship.getHeight();
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            prevK = k;
            k = Keyboard.GetState();
            if (k.IsKeyDown(Keys.Up))
            {
                if (spaceship.getPosY() >=0) spaceship.setPosY(spaceship.getPosY() - paddleSpeed);
            }

            if (k.IsKeyDown(Keys.Down))
            {
                if (spaceship.getPosY() < bottomLimit) spaceship.setPosY(spaceship.getPosY() + paddleSpeed);
            }

            if (k.IsKeyDown(Keys.B) && prevK.IsKeyUp(Keys.B)) showBB = !showBB;

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();
            back1.Draw(_spriteBatch);
            spaceship.Draw(_spriteBatch);

            if (showBB)
            {
                spaceship.drawBB(_spriteBatch, Color.Black);
                spaceship.drawHS(_spriteBatch, Color.Green);
                // LineBatch.drawLineRectangle(_spriteBatch, playArea, Color.Blue);
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
