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

        int missileHeight = 25;
        int missileWidth = 50;

        int missedCounter = 0;

        float defaultXspeed = 6;
        float ssXSpeed = 6;
        float ssYSpeed = 4f;


        float missileOffsetY = 20f;

        Texture2D texBack;
        Texture2D texSpaceShip;
        Texture2D texMountain;
        Texture2D texMissile;
        Texture2D texTruck;
        Texture2D texFailScreen;

        Sprite3 spaceship = null;
        Sprite3 mountain = null;
        Sprite3 missile = null;
        Sprite3 truck = null;
        Sprite3 failScreen = null;

        ImageBackground skyBack = null;

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

            texBack = Util.texFromFile(GraphicsDevice, dir + "Back1.png");
            texSpaceShip = Util.texFromFile(GraphicsDevice, dir + "Spaceship3a.png");
            texMountain = Util.texFromFile(GraphicsDevice, dir + "Mountain2.png");
            texMissile = Util.texFromFile(GraphicsDevice, dir + "Missile.png");
            texTruck = Util.texFromFile(GraphicsDevice, dir + "Truck1.png");
            texFailScreen = Util.texFromFile(GraphicsDevice, dir + "fail_screen.png");

            skyBack = new ImageBackground(texBack, Color.White, GraphicsDevice);

            spaceship = new Sprite3(true, texSpaceShip, xx, yy);
            setupSpaceship(spaceship);

            missile = new Sprite3(true, texMissile, 0,0); //535x83
            setupMissile(missile);

            mountain = new Sprite3(true, texMountain, 700, 0);
            setupMountains();

            truck = new Sprite3(true, texTruck, 0, 0);
            setupTruck(truck, 10, mountain.getPosX(), mountain.getPosY()-mountain.getHeight());

            failScreen = new Sprite3(false, texFailScreen, 0, 0);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            prevK = k;
            k = Keyboard.GetState();

            handleSpaceshipMovement(k);

            if (k.IsKeyDown(Keys.B) && prevK.IsKeyUp(Keys.B)) showBB = !showBB;

            // updating horizontal speed
            if (k.IsKeyDown(Keys.Left)) ssXSpeed = 3f;
            if (prevK.IsKeyUp(Keys.Left)) ssXSpeed = defaultXspeed;

            // updating obstacles position
            updateObstaclesPosition();

            missile.animationTick(gameTime);

            if (missile.getPosX() > gameWindowWidth)
            {
                missile.setPosX(spaceship.getPosX() + spaceship.getWidth());
                missile.setPosY(spaceship.getPosY() + missileOffsetY);
                missile.animationStart();
            } else missile.setPosX(missile.getPosX() + ssXSpeed);

            checkColilssions();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();

            skyBack.Draw(_spriteBatch);
            spaceship.Draw(_spriteBatch);
            mountain.Draw(_spriteBatch);
            missile.Draw(_spriteBatch);
            truck.Draw(_spriteBatch);
            failScreen.Draw(_spriteBatch);

            if (showBB)  renderBoundingBoxes();

            _spriteBatch.End();

            base.Draw(gameTime);
        }

        private void setupMissile(Sprite3 missile)
        {
            missile.setWidthHeightOfTex(535, 83);
            missile.setXframes(3);
            missile.setYframes(1);
            missile.setWidthHeight(535 / 3, 83);
            missile.setBBToWH();
            missile.setHeight(missileHeight);
            missile.setWidth(missileWidth);
            Vector2[] anim = new Vector2[8]; // arrays start at 0 REMEMBER
            anim[0].X = 0; anim[0].Y = 0;
            anim[1].X = 1; anim[1].Y = 0;
            anim[2].X = 2; anim[2].Y = 0;
            missile.setAnimationSequence(anim, 0, 2, 5);
            missile.setAnimFinished(0); // this is the default but - explicit for the tutorial
            missile.setPos(spaceship.getPosX() + spaceship.getWidth(), spaceship.getPosY() + missileOffsetY);
            missile.animationStart();
        }

        private void setupSpaceship(Sprite3 spaceship)
        {
            spaceship.setHeight(50);
            spaceship.setWidth(100);
            spaceship.setBBToTexture();
            bottomLimit = gameWindowHeight - spaceship.getHeight();
        }

        private void setupMountains()
        {
            mountain.setPosY(gameWindowHeight - mountain.getHeight());
        }

        private void setupTruck(Sprite3 truck, int shrinkFactor, float baseX, float baseY)
        {
            truck.setHeight(truck.getHeight() / shrinkFactor);
            truck.setWidth(truck.getWidth() / shrinkFactor);
            truck.setPosX(baseX);
            truck.setPosY(baseY+truck.getHeight()+10);
        }

        private void handleSpaceshipMovement(KeyboardState k)
        {
            if (k.IsKeyDown(Keys.Up))
            {
                if (spaceship.getPosY() >= 0) spaceship.setPosY(spaceship.getPosY() - ssYSpeed);
            }
            if (k.IsKeyDown(Keys.Down))
            {
                if (spaceship.getPosY() < bottomLimit) spaceship.setPosY(spaceship.getPosY() + ssYSpeed);
            }
        }

        private void updateObstaclesPosition()
        {
            if (mountain.getPosX() < -mountain.getWidth())
            {
                mountain.setPosX(gameWindowWidth);
            }
            else mountain.setPosX(mountain.getPosX() - ssXSpeed);

            if (truck.getPosX() < -truck.getWidth())
            {
                updateMissedCounter();
                truck.setPosX(gameWindowWidth);
            }
            else truck.setPosX(mountain.getPosX() - ssXSpeed);
        }


        /// <summary>
        /// counts the number of missed trucks and calculates scores based on it
        /// </summary>
        private void updateMissedCounter()
        {
            if (truck.getActive() && (truck.getPosX() < -mountain.getWidth()))
            {
                missedCounter++;
            }
        }

        private void checkColilssions()
        {

            bool ssCollidedWithMountain = mountain.collision(spaceship);
            bool ssCollidesWithTruck = truck.collision(spaceship);
            bool missileCollidesWithTruck = truck.collision(missile);

            if (ssCollidedWithMountain || (truck.getActive() && ssCollidesWithTruck))
            {
                spaceship.active = false;
                spaceship.visible = false;
                pauseMovement();
                failScreen.visible = true;
            }
            if (missileCollidesWithTruck)
            {
                truck.active = false;
                truck.visible = false;
                missile.visible = false;
                missile.active = false;
            }
        }

        private void pauseMovement()
        {
            defaultXspeed = 0;
            ssXSpeed = 0;
            ssYSpeed = 0;
        }

        private void renderBoundingBoxes()
        {
            spaceship.drawBB(_spriteBatch, Color.Green);
            spaceship.drawHS(_spriteBatch, Color.Red);
            mountain.drawBB(_spriteBatch, Color.Green);
            mountain.drawHS(_spriteBatch, Color.Red);
            missile.drawBB(_spriteBatch, Color.Green);
            missile.drawHS(_spriteBatch, Color.Red);
            truck.drawBB(_spriteBatch, Color.Green);
            truck.drawHS(_spriteBatch, Color.Red);
        }
    }
}
