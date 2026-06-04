using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using RiotGalaxy.Managers;

namespace RiotGalaxy.Screens
{
    /// <summary>
    /// Базовый класс экрана меню (заставка, главное меню, настройки).
    /// Сам читает ввод (клавиатура/мышь) с защитой от ложного срабатывания на первом кадре.
    /// </summary>
    public abstract class Screen
    {
        protected KeyboardState Kb, PrevKb;
        protected MouseState Ms, PrevMs;

        protected int ScreenW => GameManager.Instance.ScreenWidth;
        protected int ScreenH => GameManager.Instance.ScreenHeight;
        protected SpriteFont Font => GameManager.Instance.Font;

        protected Screen()
        {
            // Захватываем стартовое состояние, чтобы клавиша, которой нас открыли,
            // не «прокликалась» на первом же кадре.
            Kb = PrevKb = Keyboard.GetState();
            Ms = PrevMs = Mouse.GetState();
        }

        public virtual void Update(GameTime gameTime)
        {
            PrevKb = Kb;
            Kb = Keyboard.GetState();
            PrevMs = Ms;
            Ms = Mouse.GetState();
        }

        public abstract void Draw(SpriteBatch spriteBatch);

        protected bool KeyPressed(Keys key) => Kb.IsKeyDown(key) && PrevKb.IsKeyUp(key);

        /// <summary>Клик мыши = отпускание ЛКМ в этом кадре.</summary>
        protected bool MouseClicked() =>
            Ms.LeftButton == ButtonState.Released && PrevMs.LeftButton == ButtonState.Pressed;

        protected Point MousePoint => new Point(Ms.X, Ms.Y);

        /// <summary>Текст по центру по горизонтали.</summary>
        protected void DrawCentered(SpriteBatch sb, string text, float y, Color color, float scale = 1f)
        {
            if (Font == null) return;
            Vector2 size = Font.MeasureString(text) * scale;
            sb.DrawString(Font, text, new Vector2(ScreenW / 2f - size.X / 2f, y),
                color, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
        }
    }
}
