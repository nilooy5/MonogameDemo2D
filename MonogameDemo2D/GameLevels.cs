using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using RC_Framework;

namespace Game1
{
    class Dir
    {
        public static string dir = @"C:\Users\fazal_ix0ll8n\source\repos\MonogameDemo2D\MonogameDemo2D\images\";
    }

    class GameLevel_0 : RC_GameStateParent
    {
        Texture2D texBack = null;

        public override void LoadContent()
        {
            font1 = Content.Load<SpriteFont>("spritefont1");
            texBack = Util.texFromFile(graphicsDevice, Dir.dir + "galaxy_title_small.png");
        }

        public override void Update(GameTime gameTime)
        {

            if (Game1.keyState.IsKeyDown(Keys.S) && !Game1.prevKeyState.IsKeyDown(Keys.S))
            {
                gameStateManager.setLevel(1);
            }
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
