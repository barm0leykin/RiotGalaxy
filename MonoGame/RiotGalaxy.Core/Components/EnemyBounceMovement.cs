using System;
using Microsoft.Xna.Framework;
using RiotGalaxy.GameObjects;
using RiotGalaxy.Managers;

namespace RiotGalaxy.Components
{
    /// <summary>
    /// Движение врага с отскоком от боковых границ и телепортом снизу-вверх.
    /// Адаптация BehEnMoveStandartBounce из CocosSharp.
    /// Угол направления: 0 = вверх, 180 = вниз (с учётом инверсии оси Y в MonoGame).
    /// </summary>
    public class EnemyBounceMovement : MovementComponent
    {
        private float _vx;
        private float _vy;

        public EnemyBounceMovement(GameObject owner, float speed) : base(owner, speed)
        {
        }

        /// <summary>
        /// Задать направление движения (в градусах) со скоростью владельца.
        /// Аналог BehMove.SetDirection: вектор (0, speed) повёрнут на угол.
        /// </summary>
        public void SetDirection(float angleDeg)
        {
            float rad = MathHelper.ToRadians(angleDeg);
            float speed = (_owner is Enemy e) ? e.CurrentSpeed : _speed;
            _vx = (float)Math.Sin(rad) * speed;   // angle 0 -> (0, -speed) = вверх
            _vy = -(float)Math.Cos(rad) * speed;  // angle 180 -> (0, +speed) = вниз
        }

        public override void Update(GameTime gameTime)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            var gm = GameManager.Instance;
            float width = gm.ScreenWidth;
            float height = gm.ScreenHeight;

            Vector2 p = _owner.Position + new Vector2(_vx, _vy) * dt;

            // Отскок от боковых границ (поле минус 10% с каждой стороны)
            float border = width * 0.10f;
            if (p.X < border)
            {
                _vx = -_vx;
                p.X = border + 1;
            }
            else if (p.X > width - border - _owner.Width)
            {
                _vx = -_vx;
                p.X = width - border - _owner.Width - 1;
            }

            // Улетел вниз за экран — появляется сверху (аналог SlideUp)
            if (p.Y > height + 50)
                p.Y = -50;

            _owner.Position = p;
        }
    }
}
