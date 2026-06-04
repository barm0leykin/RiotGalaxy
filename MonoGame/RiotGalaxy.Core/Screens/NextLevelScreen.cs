using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using RiotGalaxy.Managers;

namespace RiotGalaxy.Screens
{
    /// <summary>
    /// Экран между уровнями: номер уровня, описание, статистика. Продолжить — в игру.
    /// </summary>
    public class NextLevelScreen : Screen
    {
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (KeyPressed(Keys.Space) || KeyPressed(Keys.Enter) || MouseClicked())
                GameManager.Instance.ChangeGameState(GameManager.GameState.Playing);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            var gm = GameManager.Instance;
            DrawCentered(spriteBatch, $"Уровень {gm.CurrentLevel} из {gm.TotalLevels}", ScreenH * 0.3f, Color.Orange, 1.5f);

            if (!string.IsNullOrWhiteSpace(gm.CurrentLevelDescription))
                DrawCentered(spriteBatch, gm.CurrentLevelDescription, ScreenH * 0.45f, Color.White);

            if (gm.Player != null)
                DrawCentered(spriteBatch, $"Очки: {gm.Player.Score}", ScreenH * 0.55f, Color.Yellow);

            DrawCentered(spriteBatch, "Пробел / Enter — продолжить", ScreenH * 0.75f, Color.Gray);
        }
    }
}
