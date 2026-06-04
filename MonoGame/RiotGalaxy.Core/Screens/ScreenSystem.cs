using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RiotGalaxy.Screens
{
    /// <summary>
    /// Простой менеджер экранов меню: хранит активный Screen и проксирует Update/Draw.
    /// Аналог ScreenSystem из плана миграции.
    /// </summary>
    public class ScreenSystem
    {
        public Screen Current { get; private set; }

        public void Change(Screen screen) => Current = screen;

        public void Update(GameTime gameTime) => Current?.Update(gameTime);

        public void Draw(SpriteBatch spriteBatch) => Current?.Draw(spriteBatch);
    }
}
