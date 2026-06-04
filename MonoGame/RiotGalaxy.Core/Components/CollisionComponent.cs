using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using RiotGalaxy.GameObjects;

namespace RiotGalaxy.Components
{
    /// <summary>
    /// Базовый класс для компонентов столкновений
    /// Реализует паттерн Стратегия
    /// </summary>
    public abstract class CollisionComponent
    {
        protected GameObject _owner;
        protected List<CollisionLayer> _collisionLayers;
        protected float _collisionRadius;
        protected bool _isCollisionEnabled;
        
        public float CollisionRadius 
        { 
            get => _collisionRadius; 
            set => _collisionRadius = Math.Max(0, value); 
        }
        
        public bool IsCollisionEnabled 
        { 
            get => _isCollisionEnabled; 
            set => _isCollisionEnabled = value; 
        }
        
        public List<CollisionLayer> CollisionLayers 
        { 
            get => _collisionLayers; 
            set => _collisionLayers = value; 
        }
        
        public CollisionComponent(GameObject owner, float collisionRadius = 32f)
        {
            _owner = owner;
            _collisionRadius = collisionRadius;
            _isCollisionEnabled = true;
            _collisionLayers = new List<CollisionLayer>();
        }
        
        public virtual void Update(GameTime gameTime)
        {
            if (!_isCollisionEnabled)
                return;
                
            // Проверка столкновений будет handled в GameManager
        }
        
        /// <summary>
        /// Проверка столкновения с другим объектом
        /// </summary>
        public virtual bool CheckCollision(GameObject other)
        {
            if (!_isCollisionEnabled || other == null || !other.IsAlive)
                return false;
                
            // Используем простую проверку по расстоянию
            float distance = Vector2.Distance(_owner.Position, other.Position);
            return distance < (_collisionRadius + other.Size.X / 2);
        }
        
        /// <summary>
        /// Обработка столкновения
        /// </summary>
        public abstract void OnCollide(GameObject other);
        
        /// <summary>
        /// Обработка столкновения с пулей
        /// </summary>
        public virtual void OnBulletHit(Bullet bullet)
        {
            // Реализация по умолчанию
            if (!_owner.IsAlive)
                return;
                
            // Если у владельца пули и этого объекта разные "стороны", наносим урон
            bool isEnemyCollision = (_owner is Enemy && bullet.PlayerSide) ||
                                   (_owner is PlayerShip && !bullet.PlayerSide);
                                   
            if (isEnemyCollision)
            {
                if (_owner is PlayerShip playerShip)
                {
                    playerShip.TakeDamage(bullet.Damage);
                }
                else if (_owner is Enemy enemy)
                {
                    enemy.TakeDamage(bullet.Damage);
                }
            }
        }
    }
    
    /// <summary>
    /// Компонент столкновений для игрока
    /// </summary>
    public class PlayerCollisionComponent : CollisionComponent
    {
        public PlayerCollisionComponent(GameObject owner) : base(owner, 30f)
        {
            _collisionLayers.Add(CollisionLayer.Player);
            _collisionLayers.Add(CollisionLayer.PlayerBullet);
        }
        
        public override void OnCollide(GameObject other)
        {
            if (other is Enemy)
            {
                // Столкновение с врагом - урон игроку и врагу
                PlayerShip playerShip = _owner as PlayerShip;
                Enemy enemy = other as Enemy;
                if (playerShip != null)
                    playerShip.TakeDamage(20);
                if (enemy != null)
                    enemy.TakeDamage(50);
            }
            else if (other is Bonus bonus)
            {
                // Подбор бонуса
                bonus.Collect();
            }
        }
    }
    
    /// <summary>
    /// Компонент столкновений для врага
    /// </summary>
    public class EnemyCollisionComponent : CollisionComponent
    {
        public EnemyCollisionComponent(GameObject owner) : base(owner, 25f)
        {
            _collisionLayers.Add(CollisionLayer.Enemy);
            _collisionLayers.Add(CollisionLayer.EnemyBullet);
        }
        
        public override void OnCollide(GameObject other)
        {
            if (other is PlayerShip playerShip)
            {
                // Столкновение с игроком
                Enemy enemy = _owner as Enemy;
                if (playerShip != null)
                    playerShip.TakeDamage(20);
                if (enemy != null)
                    enemy.TakeDamage(50);
            }
        }
    }
    
    /// <summary>
    /// Компонент столкновений для пули
    /// </summary>
    public class BulletCollisionComponent : CollisionComponent
    {
        private Bullet _bullet;
        
        public BulletCollisionComponent(Bullet bullet) : base(bullet, 5f)
        {
            _bullet = bullet;

            if (!_bullet.PlayerSide)
            {
                _collisionLayers.Add(CollisionLayer.EnemyBullet);
            }
            else
            {
                _collisionLayers.Add(CollisionLayer.PlayerBullet);
            }
        }

        public override void OnCollide(GameObject other)
        {
            // Продолжительность пули после столкновения
            if (!_bullet.PlayerSide && other is PlayerShip)
            {
                _bullet.IsAlive = false;
            }
            else if (_bullet.PlayerSide && other is Enemy)
            {
                if (!_bullet.IsPiercing) // Обычные пули исчезают при попадании
                    _bullet.IsAlive = false;
            }
        }
    }
    
    /// <summary>
    /// Компонент столкновений для бонуса
    /// </summary>
    public class BonusCollisionComponent : CollisionComponent
    {
        public BonusCollisionComponent(Bonus bonus) : base(bonus, 20f)
        {
            _collisionLayers.Add(CollisionLayer.Bonus);
            _collisionLayers.Add(CollisionLayer.Player);
        }
        
        public override void OnCollide(GameObject other)
        {
            if (other is PlayerShip)
            {
                Bonus bonus = _owner as Bonus;
                bonus.Collect();
            }
        }
    }
    
    /// <summary>
    /// Слои столкновений для оптимизации проверки
    /// </summary>
    public enum CollisionLayer
    {
        Player,
        Enemy,
        PlayerBullet,
        EnemyBullet,
        Bonus,
        Background
    }
}
