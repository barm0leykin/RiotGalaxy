using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
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

        // Кросс-платформенный указатель (мышь на desktop, палец на Android).
        // Позиция — в ВИРТУАЛЬНЫХ координатах (1280x768).
        private bool _ptrDown, _ptrPrevDown;
        private Point _ptrPos;

        protected int ScreenW => GameManager.Instance.ScreenWidth;
        protected int ScreenH => GameManager.Instance.ScreenHeight;
        protected SpriteFont Font => GameManager.Instance.Font;

        protected Screen()
        {
            // Захватываем стартовое состояние, чтобы клавиша/клик, которыми нас открыли,
            // не «прокликались» на первом же кадре.
            Kb = PrevKb = Keyboard.GetState();
            Ms = PrevMs = Mouse.GetState();
#if ANDROID
            _ptrDown = _ptrPrevDown = TouchPanel.GetState().Count > 0;
#else
            _ptrDown = _ptrPrevDown = Ms.LeftButton == ButtonState.Pressed;
#endif
        }

        public virtual void Update(GameTime gameTime)
        {
            PrevKb = Kb;
            Kb = Keyboard.GetState();
            PrevMs = Ms;
            Ms = Mouse.GetState();

            // Кросс-платформенный указатель
            _ptrPrevDown = _ptrDown;
#if ANDROID
            var touches = TouchPanel.GetState();
            if (touches.Count > 0)
            {
                _ptrDown = true;
                _ptrPos = GameManager.Instance.ScreenToVirtual(touches[0].Position).ToPoint();
            }
            else
            {
                _ptrDown = false; // позицию сохраняем последней
            }
#else
            _ptrDown = Ms.LeftButton == ButtonState.Pressed;
            _ptrPos = GameManager.Instance.ScreenToVirtual(new Vector2(Ms.X, Ms.Y)).ToPoint();
#endif
        }

        public abstract void Draw(SpriteBatch spriteBatch);

        protected bool KeyPressed(Keys key) => Kb.IsKeyDown(key) && PrevKb.IsKeyUp(key);

        /// <summary>Клик = момент нажатия (edge) указателя (ЛКМ или касание), как и у клавиш —
        /// чтобы клик/тап, которым открыли экран, не «прокликивал» его на отпускании.</summary>
        protected bool MouseClicked() => _ptrDown && !_ptrPrevDown;

        /// <summary>Позиция указателя в виртуальных координатах (1280x768).</summary>
        protected Point MousePoint => _ptrPos;

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
