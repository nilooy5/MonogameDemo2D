using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using RC_Framework;
using Microsoft.Xna.Framework.Input;

namespace Game1
{
    class GameLevel_5_Finished : RC_GameStateParent
    {
        Texture2D texBack = null;

        public override void LoadContent()
        {
            font1 = Content.Load<SpriteFont>("SpriteFont1");
            texBack = Util.texFromFile(graphicsDevice, Dir.dir + "galaxy_completed.png");
        }

        public override void Update(GameTime gameTime)
        {

            //if (Game1.keyState.IsKeyDown(Keys.R) && !Game1.prevKeyState.IsKeyDown(Keys.R))
            //{
            //    gameStateManager.popLevel();
            //}

            //if (Game1.keyState.IsKeyDown(Keys.S) && !Game1.prevKeyState.IsKeyDown(Keys.S))
            //{
            //    gameStateManager.popLevel();
            //    gameStateManager.setLevel(4);
            //}
        }

        public override void Draw(GameTime gameTime)
        {
            graphicsDevice.Clear(Color.Aqua);
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);

            spriteBatch.Draw(texBack, new Vector2(-150, 0), Color.White);

            spriteBatch.End();
        }


    }
}