using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using RC_Framework;
using System;
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
        float missileSpeedX = 20f;

        Vector2[] boomAnim = new Vector2[21];

        float missileOffsetY = 20f;

        Texture2D texBack;
        Texture2D texSpaceShip;
        Texture2D texMountain;
        Texture2D texMissile;
        Texture2D texTruck;
        Texture2D texFailScreen;
        Texture2D texBoom;

        Sprite3 spaceship = null;
        Sprite3 mountain = null;
        Sprite3 missile = null;
        Sprite3 truck = null;
        Sprite3 failScreen = null;
        Sprite3 boom = null;

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
            texBoom = Util.texFromFile(GraphicsDevice, dir + "Boom6.png");

            skyBack = new ImageBackground(texBack, Color.White, GraphicsDevice);

            spaceship = new Sprite3(true, texSpaceShip, xx, yy);
            setupSpaceship(spaceship);

            missile = new Sprite3(true, texMissile, 0,0); //535x83
            setupMissile(missile); 

            mountain = new Sprite3(true, texMountain, 700, 0);
            setupMountains();

            truck = new Sprite3(true, texTruck, 0, 0);
            setupTruck(truck, 10, mountain.getPosX(), mountain.getPosY()-mountain.getHeight());

            boom = new Sprite3(true, texBoom, 0, 0); //535x83
            setupBoom(boom);

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

            if (missile.state == 0)
            {
                missile.setVisible(false);
                missile.setPosY(spaceship.getPosY() + missileOffsetY);
            }
            if (k.IsKeyDown(Keys.F) && prevK.IsKeyUp(Keys.F)) missile.state = 1;
            if (missile.state == 1)
            {
                missile.setVisible(true);
                moveMissile();
            }

            boom.setPosX(truck.getPosX());

            boom.animationTick(gameTime);

            checkColilssions(gameTime, boomAnim);

            if (missedCounter > 3) 
            {
                pauseMovement();
                failScreen.setActive(true);
                failScreen.setVisible(true);
            }

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
            boom.Draw(_spriteBatch);

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
            missile.setPos(spaceship.getPosX() + spaceship.getWidth() - missile.getWidth(), spaceship.getPosY() + missileOffsetY);
            missile.animationStart();
        }

        private void setupBoom(Sprite3 boom)
        {
            int boomWidth = 896;
            int boomHeight = 384;
            int boomXframes = 7;
            int boomYframes = 3;
            boom.setWidthHeightOfTex(boomWidth, boomHeight);
            boom.setXframes(boomXframes);
            boom.setYframes(boomYframes);
            boom.setWidthHeight(boomWidth / boomXframes, boomHeight / boomYframes);
            boom.setBBToWH();

            for (int i = 0; i < boomYframes; i++)
            {
                for (int j = 0; j < boomXframes; j++)
                {
                    boomAnim[i * boomXframes + j].X = j;
                    boomAnim[i * boomXframes + j].Y = i;
                }
            }

            boom.setAnimationSequence(boomAnim, 20, 20, 5);
            // boom.setAnimFinished(1);
            boom.setPos(truck.getPosX(), mountain.getPosY()-100);
            boom.animationStart();
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
                // updateMissedCounter();
                if (!truck.active)
                {
                    truck.setActive(true);
                    truck.visible = true;
                    boom.active = true;
                }
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

        private void checkColilssions(GameTime gameTime, Vector2[] boomAnim)
        {

            bool ssCollidedWithMountain = mountain.collision(spaceship);
            bool ssCollidesWithTruck = truck.collision(spaceship);
            bool missileCollidesWithTruck = truck.collision(missile) && truck.active;

            if (ssCollidedWithMountain || (truck.getActive() && ssCollidesWithTruck))
            {
                spaceship.active = false;
                spaceship.visible = false;
                pauseMovement();
                failScreen.visible = true;
            }
            if (missileCollidesWithTruck)
            {
                playBoomAnimation();
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
            missileSpeedX = 0;
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
            boom.drawBB(_spriteBatch, Color.Green);
            boom.drawHS(_spriteBatch, Color.Red);
        }

        private void playBoomAnimation()
        {
            boom.setVisible(true);
            boom.setAnimationSequence(boomAnim, 0, 20, 5);
            boom.setAnimFinished(1);
            boom.animationStart();
        }

        private void moveMissile()
        {
            if (missile.getPosX() > gameWindowWidth) resetMissilePosition();
            else missile.setPosX(missile.getPosX() + missileSpeedX);
        }

        private void resetMissilePosition()
        {
            missile.state = 0;
            missile.setPosX(spaceship.getPosX() + missile.getWidth());
            missile.setPosY(spaceship.getPosY() + missileOffsetY);
            missile.setActive(true);
            missile.setVisible(false);
        }
    }
}
