using System;
using Microsoft.Xna.Framework;
using RiotGalaxy.GameObjects;

namespace RiotGalaxy.Components
{
    /// <summary>
    /// Базовый класс для компонентов движения
    /// Реализует паттерн Стратегия
    /// </summary>
    public abstract class MovementComponent
    {
        protected GameObject _owner;
        protected float _speed;
        
        public float Speed 
        { 
            get => _speed; 
            set 
            { 
                _speed = value < 0 ? 0 : value; 
            } 
        }
        
        public MovementComponent(GameObject owner, float speed)
        {
            _owner = owner;
            _speed = speed;
        }
        
        public abstract void Update(GameTime gameTime);
    }
    
    /// <summary>
    /// Компонент движения для игрока
    /// </summary>
    public class PlayerMovementComponent : MovementComponent
    {
        private float _minX, _maxX, _minY, _maxY;
        
        public PlayerMovementComponent(GameObject owner, float speed) : base(owner, speed)
        {
            // Устанавливаем границы движения (будут обновлены при инициализации GameManager)
            _minX = 0;
            _maxX = 1280;
            _minY = 0;
            _maxY = 768;
        }
        
        public void SetBounds(float minX, float maxX, float minY, float maxY)
        {
            _minX = minX;
            _maxX = maxX;
            _minY = minY;
            _maxY = maxY;
        }
        
        public override void Update(GameTime gameTime)
        {
            // Движение игрока управляется через InputManager
            // Этот метод может содержать дополнительную логику, если необходимо
        }
        
        /// <summary>
        /// Перемещение игрока в указанном направлении
        /// </summary>
        public void Move(Vector2 direction)
        {
            Vector2 newPosition = _owner.Position + direction * _speed;
            
            // Ограничиваем движение в пределах экрана
            newPosition.X = MathHelper.Clamp(newPosition.X, _minX, _maxX);
            newPosition.Y = MathHelper.Clamp(newPosition.Y, _minY, _maxY);
            
            _owner.Position = newPosition;
        }
    }
    
    /// <summary>
    /// Компонент движения для врагов
    /// </summary>
    public class EnemyMovementComponent : MovementComponent
    {
        protected Vector2 _direction;
        protected MovementPattern _pattern;
        protected float _timer = 0f;
        protected float[] _patternParameters;
        
        public Vector2 Direction 
        { 
            get => _direction; 
            set => _direction = Vector2.Normalize(value); 
        }
        
        public EnemyMovementComponent(GameObject owner, float speed, MovementPattern pattern, float[] parameters = null) 
            : base(owner, speed)
        {
            _pattern = pattern;
            _patternParameters = parameters ?? new float[0];
            _direction = Vector2.UnitY; // По умолчанию двигаемся вниз
        }
        
        public override void Update(GameTime gameTime)
        {
            _timer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            
            switch (_pattern)
            {
                case MovementPattern.Linear:
                    MoveLinear();
                    break;
                case MovementPattern.SineWave:
                    MoveSineWave();
                    break;
                case MovementPattern.Circle:
                    MoveCircle();
                    break;
                case MovementPattern.Formation:
                    MoveInFormation();
                    break;
            }
        }
        
        protected virtual void MoveLinear()
        {
            Vector2 newPosition = _owner.Position + _direction * _speed;
            _owner.Position = newPosition;
        }
        
        protected virtual void MoveSineWave()
        {
            float amplitude = _patternParameters.Length > 0 ? _patternParameters[0] : 50f;
            float frequency = _patternParameters.Length > 1 ? _patternParameters[1] : 2f;
            
            float xOffset = amplitude * (float)Math.Sin(_timer * frequency);
            Vector2 newPosition = _owner.Position + _direction * _speed;
            newPosition.X += xOffset;
            _owner.Position = newPosition;
        }
        
        protected virtual void MoveCircle()
        {
            float radius = _patternParameters.Length > 0 ? _patternParameters[0] : 100f;
            float centerX = _patternParameters.Length > 1 ? _patternParameters[1] : _owner.Position.X;
            float centerY = _patternParameters.Length > 2 ? _patternParameters[2] : _owner.Position.Y;
            
            float angle = _timer * _speed * 0.05f;
            Vector2 newPosition = new Vector2(
                centerX + radius * (float)Math.Cos(angle),
                centerY + radius * (float)Math.Sin(angle)
            );
            
            _owner.Position = newPosition;
        }
        
        protected virtual void MoveInFormation()
        {
            // Движение в строю будет реализовано с учетом положения в формации
            // Этот метод будет переопределен в конкретных реализациях
            MoveLinear();
        }
    }
    
    /// <summary>
    /// Паттерны движения врагов
    /// </summary>
    public enum MovementPattern
    {
        Linear,
        SineWave,
        Circle,
        Formation
    }
}
