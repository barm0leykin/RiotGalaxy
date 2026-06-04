using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RiotGalaxy.Managers;

namespace RiotGalaxy.GameObjects
{
    /// <summary>
    /// Базовый класс снаряда. Аналог Shell из CocosSharp.
    /// Летит по прямой в направлении Direction со скоростью Speed,
    /// самоуничтожается при выходе за пределы экрана.
    /// </summary>
    public class Shell : GameObject
    {
        public float Speed { get; set; } = 200f;
        public Vector2 Direction { get; set; } = -Vector2.UnitY; // по умолчанию вверх по экрану
        public int Damage { get; set; } = 10;
        public int Hp { get; set; } = 1;
        public bool PlayerSide { get; set; } = true;   // true = выпущен игроком (для отключения friendly-fire)
        public bool IsPiercing { get; protected set; } = false; // летит насквозь (лазер)

        protected Shell(Vector2 position) : base(position, new Vector2(8, 16))
        {
        }

        /// <summary>
        /// Загрузка спрайта снаряда из Content Pipeline (аналог draw.LoadGraphics).
        /// </summary>
        protected void LoadSprite(string asset)
        {
            try
            {
                Texture = GameManager.Instance.Content.Load<Texture2D>(asset);
                if (Texture != null)
                    Size = new Vector2(Texture.Width, Texture.Height);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"=== Shell sprite '{asset}' load failed: {ex.Message} ===");
            }
        }

        /// <summary>
        /// Движение и проверка выхода за экран. Аналог Shell.Activity из CocosSharp.
        /// </summary>
        public override void Update(GameTime gameTime)
        {
            if (!IsAlive)
                return;

            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Position += Direction * Speed * dt;

            var gm = GameManager.Instance;
            if (Position.Y < -Height || Position.Y > gm.ScreenHeight + Height ||
                Position.X < -Width || Position.X > gm.ScreenWidth + Width)
            {
                IsAlive = false; // GameManager удалит объект из списка
            }
        }
    }
}
