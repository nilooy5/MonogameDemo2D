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

        public override void LoadContent()
        {
            font1 = Content.Load<SpriteFont>("spritefont1");
        }

        public override void Update(GameTime gameTime)
        {

            if (Game1.keyState.IsKeyDown(Keys.N) && !Game1.prevKeyState.IsKeyDown(Keys.N))
            {
                gameStateManager.setLevel(1);
            }
        }

        public override void Draw(GameTime gameTime)
        {
            graphicsDevice.Clear(Color.Aqua);
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);
            spriteBatch.DrawString(font1, "level 0 - press n to go to next level", new Vector2(100, 100), Color.Brown);
            spriteBatch.End();
        }

    }
}
