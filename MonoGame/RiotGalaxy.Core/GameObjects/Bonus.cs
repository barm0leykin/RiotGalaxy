using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RiotGalaxy.Components;

namespace RiotGalaxy.GameObjects
{
    /// <summary>
    /// Класс бонуса, который может подбирать игрок
    /// </summary>
    public class Bonus : GameObject
    {
        // Тип бонуса
        public BonusType Type { get; set; }
        
        // Скорость падения бонуса
        public float FallSpeed { get; set; } = 1.5f;
        
        // Магнитное притяжение к игроку
        public float MagnetRange { get; set; } = 100f;
        public float MagnetSpeed { get; set; } = 8f;
        
        // Состояние бонуса
        public bool IsCollected { get; private set; }
        
        public Bonus(Vector2 position, BonusType type) : base(position, new Vector2(20, 20))
        {
            Type = type;
            IsCollected = false;
            
            // Компоненты для бонуса
            Movement = new BonusMovementComponent(this, FallSpeed);
            Collision = new BonusCollisionComponent(this);
        }
        
        public override void Update(GameTime gameTime)
        {
            if (IsCollected)
                return;
                
            base.Update(gameTime);
        }
        
        /// <summary>
        /// Подбор бонуса игроком
        /// </summary>
        public void Collect()
        {
            IsCollected = true;
            IsAlive = false;
            
            // Логика применения эффекта бонуса к игроку будет в GameManager
        }
    }
    
    /// <summary>
    /// Типы бонусов
    /// </summary>
    public enum BonusType
    {
        Health,         // Восстановление здоровья
        WeaponUpgrade,  // Улучшение оружия
        Shield,         // Временный щит
        ScoreMultiplier // Множитель очков
    }
    
    /// <summary>
    /// Компонент движения для бонусов
    /// </summary>
    public class BonusMovementComponent : MovementComponent
    {
        private float _fallSpeed;
        private float _magnetRange;
        private float _magnetSpeed;
        private PlayerShip _player;
        
        public BonusMovementComponent(GameObject owner, float fallSpeed) : base(owner, 0)
        {
            _fallSpeed = fallSpeed;
        }
        
        public void SetMagnetParams(float range, float speed, PlayerShip player)
        {
            _magnetRange = range;
            _magnetSpeed = speed;
            _player = player;
        }
        
        public override void Update(GameTime gameTime)
        {
            if (_owner is Bonus && _player != null)
            {
                // Вычисляем расстояние до игрока
                float distance = Vector2.Distance(_owner.Position, _player.Position);
                
                // Если игрок в зоне притяжения, движемся к нему
                if (distance < _magnetRange && distance > 10f)
                {
                    Vector2 direction = Vector2.Normalize(_player.Position - _owner.Position);
                    Vector2 newPosition = _owner.Position + direction * _magnetSpeed;
                    _owner.Position = newPosition;
                }
                else
                {
                    // Обычное падение вниз
                    Vector2 newPosition = _owner.Position + Vector2.UnitY * _fallSpeed;
                    _owner.Position = newPosition;
                }
            }
        }
    }
}
