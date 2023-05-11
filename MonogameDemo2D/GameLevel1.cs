using Game1;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using RC_Framework;
using System;

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
        Texture2D texMissileEnemy;
        Texture2D texTruck;
        Texture2D texFailScreen;
        Texture2D texBoom;
        Texture2D playerLifeTex;

        Sprite3 spaceship = null;
        Sprite3 missile = null;
        Sprite3 failScreen = null;
        Sprite3 boom = null;

        SpriteList enemies;
        SpriteList enemy_missile_list;
        SpriteList playerHealth = null;

        ScrollBackGround skyBack = null;

        SoundEffect boomSound;
        LimitSound limBoomSound;
        SpriteFont font1;
        int gameScore = 0;
        int updateCounter = 0;
        int enemyAgression = 10;
        int baseHealth = 5;
        bool showBB = false;

        public override void LoadContent()
        {
            texBack = Util.texFromFile(graphicsDevice, Dir.dir + "scroll_back2.png");
            texSpaceShip = Util.texFromFile(graphicsDevice, Dir.dir + "playerShip2_blue_2.png");
            texTruck = Util.texFromFile(graphicsDevice, Dir.dir + "playerShip3_redL_Dark.png");
            texMissile = Util.texFromFile(graphicsDevice, Dir.dir + "Missile.png");
            texMissileEnemy = Util.texFromFile(graphicsDevice, Dir.dir + "missile2 - Copy.png");
            texFailScreen = Util.texFromFile(graphicsDevice, Dir.dir + "fail_screen.png");
            texBoom = Util.texFromFile(graphicsDevice, Dir.dir + "Boom6.png");
            playerLifeTex = Util.texFromFile(graphicsDevice, Dir.dir + "playerLife1_green.png");

            font1 = Content.Load<SpriteFont>("SpriteFont1");

            boomSound = Content.Load<SoundEffect>("flack");

            playerHealth = new SpriteList();
            for (int i = 0; i < baseHealth; i++)
            {
                Sprite3 h = new Sprite3(true, playerLifeTex, 10 + (i * 50), gameWindowHeight-40);
                //h.setWidth(50);
                //h.setHeight(50);
                playerHealth.addSpriteReuse(h);
            }

            enemies = new SpriteList();
            enemy_missile_list = new SpriteList();
            //Create 5 sprites and put them into our list of enemies
            for (int i = 0; i < 5; i++)
            {
                Sprite3 s = new Sprite3(true, texTruck, gameWindowWidth-100, 100 + (100 * i)); //create a sprite
                Sprite3 m = new Sprite3(true, texMissileEnemy, gameWindowWidth - 100, 100 + (100 * i)); //create a sprite
                s.setHeight(s.getHeight() / 2);
                s.setWidth(s.getWidth() / 2);
                s.setDeltaSpeed(new Vector2(0.3f, -5));//Set sprite speed

                m.setHeight(m.getHeight() / 10);
                m.setWidth(m.getWidth() / 10);
                m.state = 0;

                enemies.addSpriteReuse(s);//Add all sprites to the list
                enemy_missile_list.addSpriteReuse(m);//Add all sprites to the list
            }

            limBoomSound = new LimitSound(boomSound, 3);

            skyBack = new ScrollBackGround(texBack, texBack.Bounds, new Rectangle(0, 0, gameWindowWidth, gameWindowHeight), -5f, 2);

            spaceship = new Sprite3(true, texSpaceShip, xx, yy);
            setupSpaceship(spaceship);

            missile = new Sprite3(true, texMissile, 0, 0); //535x83
            missile = setupMissile(missile);

            boom = new Sprite3(true, texBoom, 0, 0); //535x83
            setupBoom(boom);

            failScreen = new Sprite3(false, texFailScreen, 0, 0);
        }

        public override void Update(GameTime gameTime)
        {
            updateCounter++;
            if (Game1.keyState.IsKeyDown(Keys.Q) && !Game1.prevKeyState.IsKeyDown(Keys.Q))
            {
                gameStateManager.pushLevel(3);
            }
            skyBack.Update(gameTime);

            //Move all of the active enemies in the list
            enemies.moveDeltaXY();

            for (int i = 0; i < enemies.count(); i++)
            {
                if (enemies[i].active && !enemies[i].inside(playArea))//If the asteroid is not inside the playArea rectangle
                {
                    enemies[i].setDeltaSpeed(enemies[i].getDeltaSpeed() * -1);//Reverse the direction (mirroring)
                }
                if (enemy_missile_list[i].state == 0)
                {
                    enemy_missile_list[i].setPosX(enemies[i].getPosX());
                    enemy_missile_list[i].setPosY(enemies[i].getPosY() + 18);
                    enemy_missile_list[i].active = enemies[i].active;
                    enemy_missile_list[i].setVisible(enemies[i].active);
                }
            }
            if (updateCounter % enemyAgression == 0)
            {
                // get a random number between 0 and 4
                Random random = new Random();
                int randomNumber = random.Next(0, 5);
                for (int i = 0; i < enemies.count(); i++)
                {
                    if (i == randomNumber)
                    {
                        enemy_missile_list[i].state = 1;
                    }
                }
            }

            // Move all the missiles of active enemies
            foreach (Sprite3 s in enemy_missile_list)
            {
                if (s.state == 1)
                {
                    s.setPosX(s.getPosX() - missileSpeedX+10);
                }
            }

            // check if any of the missiles have gone off the screen & reset them
            foreach (Sprite3 s in enemy_missile_list)
            {
                if (s.state == 1 && (s.getPosX() < 0 || !s.active))
                {
                    s.state = 0;
                    s.active = true;
                    s.setVisible(true);
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
            enemy_missile_list.Draw(spriteBatch);
            playerHealth.Draw(spriteBatch);
            if (showBB) renderBoundingBoxes();
            spriteBatch.DrawString(font1, "score: " + gameScore, new Vector2(10, 10), Color.White, 0, Vector2.Zero, 2f, SpriteEffects.None, 0);
            //spriteBatch.DrawString(font1, "update counter: " + updateCounter, new Vector2(10, 30), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
            //spriteBatch.DrawString(font1, "health point: " + spaceship.hitPoints, new Vector2(10, 60), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);

            spriteBatch.End();
        }

        private Sprite3 setupMissile(Sprite3 missile)
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
            return missile;
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
            spaceship.setHeight(spaceship.getHeight()/1.5f);
            spaceship.setWidth(spaceship.getWidth()/1.5f);
            spaceship.setBBToTexture();
            spaceship.hitPoints = baseHealth;
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

            // checking for collisions between enemy missiles and spaceship
            foreach (Sprite3 m in enemy_missile_list)
            {
                if (m.collision(spaceship) && m.active)
                {
                    spaceship.hitPoints--;
                    // updating health bar
                    for (int i= 0;i < playerHealth.count(); i++)
                    {
                        if (i<spaceship.hitPoints) playerHealth[i].visible = true;
                        else playerHealth[i].visible = false;
                    }

                    // setting missile to inactive
                    m.setActive(false);
                    m.setVisible(false);
                    m.state = 0;

                    playBoomAnimation(m.collisionRect(spaceship));

                    if (spaceship.hitPoints == 0)
                    {
                        spaceship.active = false;
                        spaceship.visible = false;
                        pauseMovement();
                        failScreen.setActive(true);
                        failScreen.setVisible(true);
                    }
                    limBoomSound.playSound();
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
            foreach (Sprite3 s in enemy_missile_list)
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
            missile.setPosX(spaceship.getPosX() + spaceship.getWidth() - missile.getWidth());
            missile.setPosY(spaceship.getPosY() + missileOffsetY);
            missile.setActive(true);
            missile.setVisible(false);
        }
    }
}


