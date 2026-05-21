using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RiotGalaxy.Components;

namespace RiotGalaxy.GameObjects
{
    /// <summary>
    /// Класс снаряда
    /// </summary>
    public class Bullet : GameObject
    {
        // Свойства снаряда
        public float Speed { get; set; } = 10f;
        public int Damage { get; set; } = 5;
        public Vector2 Direction { get; set; }
        public bool IsEnemyBullet { get; set; }
        public bool IsPiercing { get; set; }
        public bool IsActive { get; set; }
        public GameObject Owner { get; set; }
        
        // Таймер жизни снаряда (для удаления пуль, которые не попали в цель)
        private float _lifetime;
        private float _maxLifetime = 5f;
        
        public Bullet(Vector2 position, Vector2 direction, GameObject owner) 
            : base(position, new Vector2(8, 16))
        {
            Direction = direction;
            Owner = owner;
            IsActive = true;
            _lifetime = 0f;
            
            // Устанавливаем направление вращения в направлении движения
            float angle = (float)Math.Atan2(direction.Y, direction.X) + MathHelper.PiOver2;
            Rotation = angle;
            
            // Компоненты для снаряда
            Collision = new BulletCollisionComponent(this);
        }
        
        public override void Update(GameTime gameTime)
        {
            if (!IsActive)
                return;
                
            base.Update(gameTime);
            
            // Движение снаряда
            Position += Direction * Speed;
            
            // Обновление времени жизни
            _lifetime += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (_lifetime > _maxLifetime)
            {
                IsActive = false;
            }
            
            // Проверка выхода за пределы экрана
            float screenWidth = 1280;
            float screenHeight = 768;
            
            if (Position.X < 0 || Position.X > screenWidth || 
                Position.Y < 0 || Position.Y > screenHeight)
            {
                IsActive = false;
            }
        }
        
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (!IsActive || Texture == null)
                return;
                
            // Используем разные цвета для вражеских и дружественных пуль
            Color color = IsEnemyBullet ? Color.Red : Color.Yellow;
            
            var origin = new Vector2(Texture.Width / 2, Texture.Height / 2);
            
            spriteBatch.Draw(
                Texture,
                Position,
                null,
                color * Opacity,
                Rotation,
                origin,
                Scale,
                SpriteEffects.None,
                0f
            );
        }
    }
}
