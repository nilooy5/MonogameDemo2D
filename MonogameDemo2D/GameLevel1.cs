using Game1;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using RC_Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace Game1
{
    class GameLevel_1 : RC_GameStateParent
    {
        float bottomLimit = 0;

        float xx = 100;
        float yy = 600 / 2 - 50 / 2;

        int gameWindowWidth = 800;
        int gameWindowHeight = 600;
        Rectangle playArea = new Rectangle(1, 1, 799, 598);

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
        Texture2D texMissile;
        Texture2D texTruck;
        Texture2D texFailScreen;
        Texture2D texBoom;

        Sprite3 spaceship = null;
        Sprite3 missile = null;
        Sprite3 enemy_missile = null;
        Sprite3 failScreen = null;
        Sprite3 boom = null;

        SpriteList enemies;
        SpriteList enemy_missile_list;

        ScrollBackGround skyBack = null;

        SoundEffect boomSound;
        LimitSound limBoomSound;
        SpriteFont font1;
        int gameScore = 0;
        bool showBB = false;

        public override void LoadContent()
        {
            texBack = Util.texFromFile(graphicsDevice, Dir.dir + "scroll_back2.png");
            texSpaceShip = Util.texFromFile(graphicsDevice, Dir.dir + "Spaceship3a.png");
            texTruck = Util.texFromFile(graphicsDevice, Dir.dir + "Truck1.png");
            texMissile = Util.texFromFile(graphicsDevice, Dir.dir + "Missile.png");
            texFailScreen = Util.texFromFile(graphicsDevice, Dir.dir + "fail_screen.png");
            texBoom = Util.texFromFile(graphicsDevice, Dir.dir + "Boom6.png");

            font1 = Content.Load<SpriteFont>("SpriteFont1");

            boomSound = Content.Load<SoundEffect>("flack");

            enemies = new SpriteList();
            //Create 5 sprites and put them into our list of enemies
            for (int i = 0; i < 5; i++)
            {
                Sprite3 s = new Sprite3(true, texTruck, gameWindowWidth-100, 100 + (100 * i)); //create a sprite
                s.setHeight(s.getHeight() / 10);
                s.setWidth(s.getWidth() / 10);
                s.setDeltaSpeed(new Vector2(0.3f, -5));//Set sprite speed

                enemies.addSpriteReuse(s);//Add all sprites to the list                
            }

            limBoomSound = new LimitSound(boomSound, 3);

            skyBack = new ScrollBackGround(texBack, texBack.Bounds, new Rectangle(0, 0, gameWindowWidth, gameWindowHeight), -5f, 2);

            spaceship = new Sprite3(true, texSpaceShip, xx, yy);
            setupSpaceship(spaceship);

            missile = new Sprite3(true, texMissile, 0, 0); //535x83
            setupMissile(missile);

            boom = new Sprite3(true, texBoom, 0, 0); //535x83
            setupBoom(boom);

            failScreen = new Sprite3(false, texFailScreen, 0, 0);
        }

        public override void Update(GameTime gameTime)
        {
            if (Game1.keyState.IsKeyDown(Keys.P) && !Game1.prevKeyState.IsKeyDown(Keys.P))
            {
                gameStateManager.pushLevel(3);
            }
            skyBack.Update(gameTime);

            //Move all of the active enemies in the list
            enemies.moveDeltaXY();

            foreach (Sprite3 s in enemies)
            {
                if (s.active && !s.inside(playArea))//If the asteroid is not inside the playArea rectangle
                {
                    s.setDeltaSpeed(s.getDeltaSpeed() * -1);//Reverse the direction (mirroring)
                }
            }

            handleSpaceshipMovement(Game1.keyState);

            if (Game1.keyState.IsKeyDown(Keys.B) && !Game1.prevKeyState.IsKeyDown(Keys.B)) showBB = !showBB;

            // updating horizontal speed
            if (Game1.keyState.IsKeyDown(Keys.Left)) ssXSpeed = 3f;
            if (Game1.prevKeyState.IsKeyUp(Keys.Left)) ssXSpeed = defaultXspeed;

            missile.animationTick(gameTime);

            if (missile.state == 0)
            {
                missile.setVisible(false);
                missile.setPosY(spaceship.getPosY() + missileOffsetY);
            }

            if (Game1.keyState.IsKeyDown(Keys.F)) missile.state = 1;

            if (missile.state == 1)
            {
                missile.setVisible(true);
                moveMissile();
            }

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

        public override void Draw(GameTime gameTime)
        {
            graphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin();

            skyBack.Draw(spriteBatch);
            spaceship.Draw(spriteBatch);
            missile.Draw(spriteBatch);
            failScreen.Draw(spriteBatch);
            boom.Draw(spriteBatch);
            enemies.Draw(spriteBatch);
            if (showBB) renderBoundingBoxes();
            spriteBatch.DrawString(font1, "score: " + gameScore, new Vector2(10, 10), Color.White, 0, Vector2.Zero, 2.5f, SpriteEffects.None, 0);

            spriteBatch.End();
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
            boom.setHeight(boom.getHeight()/2);
            boom.setWidth(boom.getWidth()/2);

            for (int i = 0; i < boomYframes; i++)
            {
                for (int j = 0; j < boomXframes; j++)
                {
                    boomAnim[i * boomXframes + j].X = j;
                    boomAnim[i * boomXframes + j].Y = i;
                }
            }

            boom.setAnimationSequence(boomAnim, 20, 20, 5);
            boom.setPos(0,0);
            boom.animationStart();
        }

        private void setupSpaceship(Sprite3 spaceship)
        {
            spaceship.setHeight(50);
            spaceship.setWidth(100);
            spaceship.setBBToTexture();
            bottomLimit = gameWindowHeight - spaceship.getHeight();
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

        private void checkColilssions(GameTime gameTime, Vector2[] boomAnim)
        {

            foreach (Sprite3 e in enemies) 
            { 
                if (e.collision(missile) && e.active)
                {
                    playBoomAnimation(e.collisionRect(missile));
                    e.active = false;
                    e.visible = false;
                    missile.active = false;
                    missile.visible = false;
                    limBoomSound.playSound();
                    gameScore = gameScore + 10;
                }
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
            spaceship.drawBB(spriteBatch, Color.Green);
            spaceship.drawHS(spriteBatch, Color.Red);
            missile.drawBB(spriteBatch, Color.Green);
            missile.drawHS(spriteBatch, Color.Red);
            boom.drawBB(spriteBatch, Color.Green);
            boom.drawHS(spriteBatch, Color.Red);
            foreach (Sprite3 s in enemies)
            {
                s.drawBB(spriteBatch, Color.Green);
                s.drawHS(spriteBatch, Color.Red);
            }
        }

        private void playBoomAnimation(Rectangle? colRect)
        {
            boom.setPos(colRect.Value.X-boom.getWidth()/2, colRect.Value.Y - boom.getHeight()/2);
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


