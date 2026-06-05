using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RiotGalaxy.Managers
{
    /// <summary>
    /// Короткие всплывающие сообщения (тост-лог) — «+25 HP», «Оружие: лазер» и т.п.
    /// Показываются над тестовыми кнопками и затухают. Аналог GUI.Message из CocoSharp.
    /// </summary>
    public static class MessageLog
    {
        private class Entry
        {
            public string Text;
            public float Time;   // оставшееся время показа
            public Color Color;
        }

        private const float Lifetime = 2.5f; // сек
        private const int MaxShown = 5;
        private static readonly List<Entry> _entries = new List<Entry>();

        public static void Add(string text, Color? color = null)
        {
            _entries.Add(new Entry { Text = text, Time = Lifetime, Color = color ?? Color.White });
            if (_entries.Count > MaxShown)
                _entries.RemoveAt(0);
        }

        public static void Clear() => _entries.Clear();

        public static void Update(float dt)
        {
            for (int i = _entries.Count - 1; i >= 0; i--)
            {
                _entries[i].Time -= dt;
                if (_entries[i].Time <= 0f)
                    _entries.RemoveAt(i);
            }
        }

        /// <summary>Рисует сообщения снизу-вверх над панелью кнопок.</summary>
        public static void Draw(SpriteBatch spriteBatch, SpriteFont font, int screenWidth, int screenHeight)
        {
            if (font == null || _entries.Count == 0)
                return;

            // Кнопки занимают низ (top ~ screenHeight-58). Сообщения — выше них, столбиком вверх.
            float y = screenHeight - 96f;
            for (int i = _entries.Count - 1; i >= 0; i--)
            {
                var e = _entries[i];
                float alpha = MathHelper.Clamp(e.Time, 0f, 1f); // плавное затухание в последнюю секунду
                spriteBatch.DrawString(font, e.Text, new Vector2(12f, y), e.Color * alpha);
                y -= 24f;
            }
        }
    }
}
