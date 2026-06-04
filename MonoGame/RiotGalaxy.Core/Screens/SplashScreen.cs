using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using RiotGalaxy.Managers;

namespace RiotGalaxy.Screens
{
    /// <summary>
    /// Экран заставки: текстовый логотип и через несколько секунд переход в меню.
    /// Можно пропустить любой клавишей/кликом.
    /// </summary>
    public class SplashScreen : Screen
    {
        private const float Duration = 2.5f; // сек
        private float _time;

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            _time += (float)gameTime.ElapsedGameTime.TotalSeconds;

            bool skip = KeyPressed(Keys.Space) || KeyPressed(Keys.Enter) || KeyPressed(Keys.Escape) || MouseClicked();
            if (_time >= Duration || skip)
                GameManager.Instance.ChangeGameState(GameManager.GameState.MainMenu);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            DrawCentered(spriteBatch, "RIOT GALAXY", ScreenH * 0.4f, Color.Orange, 2.5f);
            DrawCentered(spriteBatch, "загрузка...", ScreenH * 0.6f, Color.Gray);
        }
    }
}
