using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using RiotGalaxy.Managers;
using RiotGalaxy.Utils;

namespace RiotGalaxy.Screens
{
    /// <summary>
    /// Экран настроек: громкость звуковых эффектов с сохранением в файл.
    /// Управление: стрелки влево/вправо или кнопки [-]/[+]; «Назад»/Esc — сохранить и в меню.
    /// </summary>
    public class SettingsScreen : Screen
    {
        private const float Step = 0.1f;

        private Rectangle MinusRect => new Rectangle((int)(ScreenW / 2f - 160), (int)(ScreenH * 0.45f), 48, 40);
        private Rectangle PlusRect => new Rectangle((int)(ScreenW / 2f + 112), (int)(ScreenH * 0.45f), 48, 40);
        private Rectangle BackRect
        {
            get
            {
                string t = "Назад";
                Vector2 s = Font != null ? Font.MeasureString(t) : new Vector2(100, 30);
                return new Rectangle((int)(ScreenW / 2f - s.X / 2f) - 12, (int)(ScreenH * 0.7f) - 6,
                    (int)s.X + 24, (int)s.Y + 12);
            }
        }

        private void ChangeVolume(float delta)
        {
            var audio = AudioManager.Instance;
            audio.EffectsVolume = MathHelper.Clamp(audio.EffectsVolume + delta, 0f, 1f);
        }

        private void Back()
        {
            GameSettings.Save();
            GameManager.Instance.ChangeGameState(GameManager.GameState.MainMenu);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (KeyPressed(Keys.Left) || KeyPressed(Keys.A))
                ChangeVolume(-Step);
            if (KeyPressed(Keys.Right) || KeyPressed(Keys.D))
                ChangeVolume(Step);

            if (MouseClicked())
            {
                if (MinusRect.Contains(MousePoint)) ChangeVolume(-Step);
                else if (PlusRect.Contains(MousePoint)) ChangeVolume(Step);
                else if (BackRect.Contains(MousePoint)) Back();
            }

            if (KeyPressed(Keys.Escape) || KeyPressed(Keys.Enter))
                Back();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            DrawCentered(spriteBatch, "Настройки", ScreenH * 0.22f, Color.Orange);

            int percent = (int)System.Math.Round(AudioManager.Instance.EffectsVolume * 100);
            DrawCentered(spriteBatch, $"Громкость эффектов: {percent}%", ScreenH * 0.35f, Color.White);

            if (Font != null)
            {
                bool minusHover = MinusRect.Contains(MousePoint);
                bool plusHover = PlusRect.Contains(MousePoint);
                spriteBatch.DrawString(Font, "[-]", new Vector2(MinusRect.X, MinusRect.Y),
                    minusHover ? Color.Yellow : Color.White);
                spriteBatch.DrawString(Font, "[+]", new Vector2(PlusRect.X, PlusRect.Y),
                    plusHover ? Color.Yellow : Color.White);
            }

            bool backHover = BackRect.Contains(MousePoint);
            DrawCentered(spriteBatch, (backHover ? "> " : "  ") + "Назад", ScreenH * 0.7f,
                backHover ? Color.Yellow : Color.White);

            DrawCentered(spriteBatch, "Стрелки ←/→ — громкость, Esc — назад", ScreenH * 0.9f, Color.Gray);
        }
    }
}
