using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using RC_Framework;
//using SharpDX.Direct2D1;
using System.Reflection.Metadata;
// using SharpDX.Direct3D9;

namespace MonogameDemo2D
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

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
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            texBack = Util.texFromFile(GraphicsDevice, dir + "Back1.png"); //***
            texSpaceShip = Util.texFromFile(GraphicsDevice, dir + "Spaceship3a.png"); //***
            spaceship = new Sprite3(true, texSpaceShip, 0, 0);
            spaceship.setBBToTexture();
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            prevK = k;
            k = Keyboard.GetState();
            if (k.IsKeyDown(Keys.B) && prevK.IsKeyUp(Keys.B)) // ***
            {
                showBB = !showBB;
            }


            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();

            _spriteBatch.Draw(texBack, new Vector2(0, 0), Color.White);
            _spriteBatch.Draw(texSpaceShip, new Vector2(0, 0), Color.White);

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