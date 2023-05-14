using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using RC_Framework;
using System;

namespace Game1
{
    class GameLevel_4_Boss : RC_GameStateParent
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
        Sprite3 shield = null;

        SpriteList playerHealth = null;
        SpriteList bossBody = null;
        SpriteList bossMissileList = null;

        ScrollBackGround skyBack = null;

        SoundEffect boomSound;
        SoundEffect music;
        SoundEffect shootSound;

        int shieldDuration = 15;
        int shieldInputCounter = 0;

        LimitSound limBoomSound;
        LimitSound limShootSound;
        LimitSound musicBackground;

        SpriteFont font1;
        int updateCounter = 0;
        int enemyAgression = 10;
        int baseHealth = 5;
        bool showBB = false;

        ParticleSystem particleSys1;
        ParticleSystem particleSys2;
        ParticleSystem particleSys3;
        ParticleSystem particleSys4;
        ParticleSystem particleSys5;
        Rectangle rectangle = new Rectangle(0, 0, 800, 600);

        Texture2D particleTex;

        public override void LoadContent()
        {
            texBack = Util.texFromFile(graphicsDevice, Dir.dir + "scroll_back2.png");
            Texture2D texShield = Util.texFromFile(graphicsDevice, Dir.dir + "spr_shield.png");
            texSpaceShip = Util.texFromFile(graphicsDevice, Dir.dir + "playerShip2_blue_2.png");
            texTruck = Util.texFromFile(graphicsDevice, Dir.dir + "playerShip3_redL_Dark.png");
            texMissile = Util.texFromFile(graphicsDevice, Dir.dir + "Missile.png");
            texMissileEnemy = Util.texFromFile(graphicsDevice, Dir.dir + "missile2 - Copy.png");
            texFailScreen = Util.texFromFile(graphicsDevice, Dir.dir + "fail_screen.png");
            texBoom = Util.texFromFile(graphicsDevice, Dir.dir + "Boom6.png");
            playerLifeTex = Util.texFromFile(graphicsDevice, Dir.dir + "playerLife1_green.png");
            Texture2D texEnemyHead = Util.texFromFile(graphicsDevice, Dir.dir + "enemyBlack2.png");
            Texture2D texEnemyChest = Util.texFromFile(graphicsDevice, Dir.dir + "enemyBlack5.png");
            Texture2D texEnemyLegs = Util.texFromFile(graphicsDevice, Dir.dir + "enemyBlack3.png");
            particleTex = Util.texFromFile(graphicsDevice, Dir.dir + "Particle2.png");

            font1 = Content.Load<SpriteFont>("SpriteFont1");

            boomSound = Content.Load<SoundEffect>("flack");
            music = Content.Load<SoundEffect>("chopsticks");
            shootSound = Content.Load<SoundEffect>("shoot");

            bossBody = new SpriteList();
            Sprite3 head = new Sprite3(true, texEnemyHead, gameWindowWidth - 200, 200 + (100 * 0));
            Sprite3 chest = new Sprite3(true, texEnemyChest, gameWindowWidth - 200, 200 + (100 * 1));
            Sprite3 legs = new Sprite3(true, texEnemyLegs, gameWindowWidth - 200, 200 + (100 * 2));
            head.setHeight(head.getHeight() * 1.5f);
            head.setWidth(head.getWidth() * 1.5f);
            chest.setHeight(chest.getHeight() * 1.5f);
            chest.setWidth(chest.getWidth() * 1.5f);
            legs.setHeight(legs.getHeight() * 1.5f);
            legs.setWidth(legs.getWidth() * 1.5f);
            head.hitPoints = baseHealth + 2;
            chest.hitPoints = baseHealth + 2;
            legs.hitPoints = baseHealth + 2;
            bossBody.addSpriteReuse(head);
            bossBody.addSpriteReuse(chest);
            bossBody.addSpriteReuse(legs);

            bossMissileList = new SpriteList();
            for (int i = 0; i < 5; i++)
            {
                Sprite3 m = new Sprite3(true, texMissileEnemy, gameWindowWidth - 100, 100 + (100 * i)); //create a sprite

                m.setHeight(m.getHeight() / 10);
                m.setWidth(m.getWidth() / 10);
                m.state = 0;
                bossMissileList.addSpriteReuse(m);//Add all sprites to the list
            }


            playerHealth = new SpriteList();
            for (int i = 0; i < baseHealth; i++)
            {
                Sprite3 h = new Sprite3(true, playerLifeTex, 10 + (i * 50), gameWindowHeight - 40);
                //h.setWidth(50);
                //h.setHeight(50);
                playerHealth.addSpriteReuse(h);
            }

            limBoomSound = new LimitSound(boomSound, 10);
            limShootSound = new LimitSound(shootSound, 10);
            musicBackground = new LimitSound(music, 1);
            //musicBackground.playSound();

            skyBack = new ScrollBackGround(texBack, texBack.Bounds, new Rectangle(0, 0, gameWindowWidth, gameWindowHeight), -5f, 2);

            spaceship = new Sprite3(true, texSpaceShip, xx, yy);
            setupSpaceship(spaceship);

            shield = new Sprite3(false, texShield, spaceship.getPosX(), spaceship.getPosY());
            shield.setWidth(100);
            shield.setHeight(100);
            shield.setActive(false);

            missile = new Sprite3(true, texMissile, 0, 0); //535x83
            missile = setupMissile(missile);

            boom = new Sprite3(true, texBoom, 0, 0); //535x83
            setupBoom(boom);

            failScreen = new Sprite3(false, texFailScreen, 0, 0);

            setParticleSystems();
        }

        public override void Update(GameTime gameTime)
        {
            updateCounter++;
            if (Game1.keyState.IsKeyDown(Keys.Q) && !Game1.prevKeyState.IsKeyDown(Keys.Q))
            {
                gameStateManager.pushLevel(3);
            }
            skyBack.Update(gameTime);

            //bossMissileList.moveDeltaXY();
            for (int i = 0; i < bossMissileList.count(); i++)
            {
                if (!bossMissileList[i].inside(playArea) || !bossMissileList[i].getActive())
                {
                    bossMissileList[i].setPosX(gameWindowWidth - 100);
                    bossMissileList[i].setPosY(100 + (100 * i));
                    bossMissileList[i].state = 0;
                }
                if (updateCounter % 200 == 0)
                {
                    bossMissileList[i].state = 1;
                }
                if (bossMissileList[i].state == 1)
                {
                    bossMissileList[i].setActive(true);
                    bossMissileList[i].setVisible(true);
                    bossMissileList[i].moveTo(new Vector2(spaceship.getPosX() + spaceship.getWidth() / 2, spaceship.getPosY() + spaceship.getHeight() / 2), 5f, false);
                }
            }

            // shield logic
            activateDeactivateShield();

            handleSpaceshipMovement(Game1.keyState);
            shield.setPos(spaceship.getPosX()-20, spaceship.getPosY() - 15);

            checkColilssions(gameTime, boomAnim);

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

            if (Game1.keyState.IsKeyDown(Keys.F))
            {
                limShootSound.playSound();
                missile.state = 1;
            }

            if (missile.state == 1)
            {
                missile.setVisible(true);
                moveMissile();
            }

            boom.animationTick(gameTime);

            if (missedCounter > 3)
            {
                pauseMovement();
                failScreen.setActive(true);
                failScreen.setVisible(true);
            }
            if(bossBody.countActive() == 0) 
            {
                gameStateManager.pushLevel(5);
            }

            particleSys1.sysPos = new Vector2(bossMissileList[0].getPosX() + bossMissileList[0].getWidth(), bossMissileList[0].getPosY() + bossMissileList[0].getHeight() / 2);
            particleSys2.sysPos = new Vector2(bossMissileList[1].getPosX() + bossMissileList[1].getWidth(), bossMissileList[1].getPosY() + bossMissileList[1].getHeight() / 2);
            particleSys3.sysPos = new Vector2(bossMissileList[2].getPosX() + bossMissileList[2].getWidth(), bossMissileList[2].getPosY() + bossMissileList[2].getHeight() / 2);
            particleSys4.sysPos = new Vector2(bossMissileList[3].getPosX() + bossMissileList[3].getWidth(), bossMissileList[3].getPosY() + bossMissileList[3].getHeight() / 2);
            particleSys5.sysPos = new Vector2(bossMissileList[4].getPosX() + bossMissileList[4].getWidth(), bossMissileList[4].getPosY() + bossMissileList[4].getHeight() / 2);


            particleSys1.Update(gameTime);
            particleSys2.Update(gameTime);
            particleSys3.Update(gameTime);
            particleSys4.Update(gameTime);
            particleSys5.Update(gameTime);


            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            graphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin();

            skyBack.Draw(spriteBatch);
            spaceship.Draw(spriteBatch);
            shield.Draw(spriteBatch);
            missile.Draw(spriteBatch);
            failScreen.Draw(spriteBatch);
            boom.Draw(spriteBatch);
            bossBody.Draw(spriteBatch);
            bossMissileList.Draw(spriteBatch);
            playerHealth.Draw(spriteBatch);
            if (showBB) renderBoundingBoxes();
            spriteBatch.DrawString(font1, "score: " + Game1.gameScore, new Vector2(10, 10), Color.White, 0, Vector2.Zero, 2f, SpriteEffects.None, 0);

            particleSys1.Draw(spriteBatch);
            particleSys2.Draw(spriteBatch);
            particleSys3.Draw(spriteBatch);
            particleSys4.Draw(spriteBatch);
            particleSys5.Draw(spriteBatch);

            spriteBatch.End();
        }

        private void activateDeactivateShield()
        {
            if (Game1.keyState.IsKeyDown(Keys.A) && shieldInputCounter < shieldDuration)
            {
                shield.setActive(true);
                shield.setVisible(true);
                shieldInputCounter++;
            }
            else if (Game1.keyState.IsKeyUp(Keys.A) && Game1.prevKeyState.IsKeyDown(Keys.A) || shieldInputCounter == shieldDuration)
            {
                shield.setActive(false);
                shield.setVisible(false);
            }
            if (!Game1.keyState.IsKeyDown(Keys.A))
            {
                if (shieldInputCounter > 0) shieldInputCounter--;
            }
        }

        private void checkColilssions(GameTime gameTime, Vector2[] boomAnim)
        {

            foreach (Sprite3 e in bossBody)
            {
                if (e.collision(missile) && e.active)
                {
                    missile.active = false;
                    missile.visible = false;
                    e.hitPoints--;
                    if (e.hitPoints == 0)
                    {
                        e.active = false;
                        e.visible = false;
                        Game1.gameScore = Game1.gameScore + 100;
                    }
                    playBoomAnimation(e.collisionRect(missile));
                    limBoomSound.playSound();
                    Game1.gameScore = Game1.gameScore + 10;
                }
            }
            // checking for collisions between enemy missiles and spaceship
            foreach (Sprite3 m in bossMissileList)
            {
                if (m.collision(spaceship) && m.active)
                {
                    spaceship.hitPoints--;
                    // updating health bar
                    for (int i = 0; i < playerHealth.count(); i++)
                    {
                        if (i < spaceship.hitPoints) playerHealth[i].visible = true;
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

                // collision between missile and shield
                if (m.collision(shield) && m.active && shield.active)
                {
                    m.setActive(false);
                    m.setVisible(false);
                    m.state = 0;
                    playBoomAnimation(m.collisionRect(shield));
                    limBoomSound.playSound();
                }
            }
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
            boom.setHeight(boom.getHeight() / 2);
            boom.setWidth(boom.getWidth() / 2);

            for (int i = 0; i < boomYframes; i++)
            {
                for (int j = 0; j < boomXframes; j++)
                {
                    boomAnim[i * boomXframes + j].X = j;
                    boomAnim[i * boomXframes + j].Y = i;
                }
            }

            boom.setAnimationSequence(boomAnim, 20, 20, 5);
            boom.setPos(0, 0);
            boom.animationStart();
        }

        private void setupSpaceship(Sprite3 spaceship)
        {
            spaceship.setHeight(spaceship.getHeight() / 1.5f);
            spaceship.setWidth(spaceship.getWidth() / 1.5f);
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
            shield.drawBB(spriteBatch, Color.Green);
            shield.drawHS(spriteBatch, Color.Red);
            spriteBatch.DrawString(font1, "update counter: " + updateCounter, new Vector2(10, 30), Color.White, 0, Vector2.Zero, 2f, SpriteEffects.None, 0);
            spriteBatch.DrawString(font1, "shield pressed: " + shieldInputCounter, new Vector2(10, 40), Color.White, 0, Vector2.Zero, 2f, SpriteEffects.None, 0);
            foreach (Sprite3 s in bossBody)
            {
                s.drawBB(spriteBatch, Color.Green);
                s.drawHS(spriteBatch, Color.Red);
            }
        }

        private void playBoomAnimation(Rectangle? colRect)
        {
            boom.setPos(colRect.Value.X - boom.getWidth() / 2, colRect.Value.Y - boom.getHeight() / 2);
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

        void setParticleSystems()
        {
            // ----------------- particle system 1 -----------------

            rectangle = new Rectangle(0, 0, 800, 600);
            particleSys1 = new ParticleSystem(new Vector2(bossMissileList[0].getPosX() + bossMissileList[0].getWidth(), bossMissileList[0].getPosY() + bossMissileList[0].getHeight() / 2), 40, 900, 118);
            particleSys1.setMandatory1(particleTex, new Vector2(5, 5), new Vector2(32, 32), Color.Red, Color.Yellow);
            particleSys1.setMandatory2(-1, 1, 1, 5, 0);
            particleSys1.setMandatory3(120, rectangle);
            particleSys1.setMandatory4(new Vector2(0, 0), new Vector2(1, 0), new Vector2(0, 0));
            particleSys1.activate();

            // ----------------- particle system 2 -----------------

            particleSys2 = new ParticleSystem(new Vector2(bossMissileList[1].getPosX() + bossMissileList[1].getWidth(), bossMissileList[1].getPosY() + bossMissileList[1].getHeight() / 2), 40, 900, 118);
            particleSys2.setMandatory1(particleTex, new Vector2(5, 5), new Vector2(32, 32), Color.Red, Color.Yellow);
            particleSys2.setMandatory2(-1, 1, 1, 5, 0);
            particleSys2.setMandatory3(120, rectangle);
            particleSys2.setMandatory4(new Vector2(0, 0), new Vector2(1, 0), new Vector2(0, 0));
            particleSys1.activate();

            // ----------------- particle system 3 -----------------

            particleSys3 = new ParticleSystem(new Vector2(bossMissileList[2].getPosX() + bossMissileList[2].getWidth(), bossMissileList[2].getPosY() + bossMissileList[2].getHeight() / 2), 40, 900, 118);
            particleSys3.setMandatory1(particleTex, new Vector2(5, 5), new Vector2(32, 32), Color.Red, Color.Yellow);
            particleSys3.setMandatory2(-1, 1, 1, 5, 0);
            particleSys3.setMandatory3(120, rectangle);
            particleSys3.setMandatory4(new Vector2(0, 0), new Vector2(1, 0), new Vector2(0, 0));
            particleSys2.activate();

            // ----------------- particle system 4 -----------------

            particleSys4 = new ParticleSystem(new Vector2(bossMissileList[3].getPosX() + bossMissileList[3].getWidth(), bossMissileList[3].getPosY() + bossMissileList[3].getHeight() / 2), 40, 900, 118);
            particleSys4.setMandatory1(particleTex, new Vector2(5, 5), new Vector2(32, 32), Color.Red, Color.Yellow);
            particleSys4.setMandatory2(-1, 1, 1, 5, 0);
            particleSys4.setMandatory3(120, rectangle);
            particleSys4.setMandatory4(new Vector2(0, 0), new Vector2(1, 0), new Vector2(0, 0));
            particleSys3.activate();

            // ----------------- particle system 5 -----------------

            particleSys5 = new ParticleSystem(new Vector2(bossMissileList[4].getPosX() + bossMissileList[4].getWidth(), bossMissileList[4].getPosY() + bossMissileList[4].getHeight() / 2), 40, 900, 118);
            particleSys5.setMandatory1(particleTex, new Vector2(5, 5), new Vector2(32, 32), Color.Red, Color.Yellow);
            particleSys5.setMandatory2(-1, 1, 1, 5, 0);
            particleSys5.setMandatory3(120, rectangle);
            particleSys5.setMandatory4(new Vector2(0, 0), new Vector2(1, 0), new Vector2(0, 0));
            particleSys4.activate();
        }
    }
}
