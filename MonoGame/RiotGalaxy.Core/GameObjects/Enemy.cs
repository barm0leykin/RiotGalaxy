using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RiotGalaxy.Components;

namespace RiotGalaxy.GameObjects
{
    /// <summary>
    /// Класс врага
    /// </summary>
    public class Enemy : GameObject
    {
        // Параметры врага
        public float Speed { get; set; } = 2f;
        public int Health { get; set; } = 30;
        public int MaxHealth { get; set; } = 30;
        public int Damage { get; set; } = 10;
        public int ScoreValue { get; set; } = 100;
        
        public Enemy(Vector2 position, EnemyType type) : base(position, GetSizeForType(type))
        {
            // Инициализация компонентов
            Movement = new EnemyMovementComponent(this, Speed, MovementPattern.Linear);
            Shooting = new EnemyShootingComponent(this, 1f, ShootingPattern.Direct);
            Collision = new EnemyCollisionComponent(this);
        }
        
        private static Vector2 GetSizeForType(EnemyType type)
        {
            switch (type)
            {
                case EnemyType.Small:
                    return new Vector2(45, 45);
                case EnemyType.Medium:
                    return new Vector2(55, 55);
                case EnemyType.Large:
                    return new Vector2(60, 60);
                default:
                    return new Vector2(50, 50);
            }
        }
        
        public override void Update(GameTime gameTime)
        {
            if (!IsAlive)
                return;
                
            base.Update(gameTime);
        }
        
        /// <summary>
        /// Получение урона
        /// </summary>
        public void TakeDamage(int damage)
        {
            Health -= damage;
            if (Health <= 0)
            {
                Health = 0;
                IsAlive = false;
            }
        }
    }
    
    /// <summary>
    /// Типы врагов
    /// </summary>
    public enum EnemyType
    {
        Small,
        Medium,
        Large,
        Boss
    }
}
