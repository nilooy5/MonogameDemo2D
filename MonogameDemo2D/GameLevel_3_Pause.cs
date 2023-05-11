using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using RC_Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1
{
    class GameLevel_3_Pause : RC_GameStateParent
    {
        Texture2D texBack = null;

        RC_RenderableList ren = new RC_RenderableList();
        Texture2D texG; //         
        Texture2D texP; // 
        RC_Renderable f2 = null;

        public override void LoadContent()
        {
            font1 = Content.Load<SpriteFont>("SpriteFont1");
            //texG = Util.texFromFile(graphicsDevice, Dir.dir + "Galaxy800x600.png");  //         
            //texP = Util.texFromFile(graphicsDevice, Dir.dir + "Policebox2Transparent.png");  // 
            texBack = Util.texFromFile(graphicsDevice, Dir.dir + "galaxy_paused.png");


        }

        public override void Update(GameTime gameTime)
        {

            if (Game1.keyState.IsKeyDown(Keys.R) && !Game1.prevKeyState.IsKeyDown(Keys.R))
            {
                gameStateManager.popLevel();
            }
            if (Game1.keyState.IsKeyDown(Keys.Space) && !Game1.prevKeyState.IsKeyDown(Keys.Space))
            {
                setUpRendarables();
            }
            if (Game1.keyState.IsKeyDown(Keys.F2) && !Game1.prevKeyState.IsKeyDown(Keys.F2))
            {
                setUpRendarableF2();
            }


            ren.Update(gameTime);
            if (f2 != null) f2.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            graphicsDevice.Clear(Color.Aqua);
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);
            if (f2 != null) f2.Draw(spriteBatch);
            spriteBatch.DrawString(font1, "level 3 - Pause Screen - press r to return", new Vector2(100, 100), Color.Brown);
            spriteBatch.DrawString(font1, "level 3 - Press Space for Renderables Test", new Vector2(100, 120), Color.Brown);
            spriteBatch.DrawString(font1, "level 3 - Press F2 for Background Test", new Vector2(100, 140), Color.Brown);
            spriteBatch.Draw(texBack, new Vector2(-150, 0), Color.White);

            ren.Draw(spriteBatch);
            spriteBatch.End();
        }

        public void setUpRendarables()
        {
            RC_Renderable r = new TextRenderableFade("fade this", new Vector2(10, 160), font1, Color.DarkGreen, Color.Transparent, 120);
            ren.addReuse(r);
            //r = new TextureFade(texP, new Rectangle(200, 200, 50, 50), new Rectangle(250, 250, 120, 120),
            //                    Color.White, Color.Transparent, 160);
            ren.addReuse(r);
        }

        public void setUpRendarableF2()
        {
            if (f2 != null) return;
            //f2 = new ScrollBackGround(texG, new Rectangle(0, 0, 800, 600), new Rectangle(0, 0, 800, 600), 1, 1);
            //ren.addReuse(f2);
        }


    }
}
