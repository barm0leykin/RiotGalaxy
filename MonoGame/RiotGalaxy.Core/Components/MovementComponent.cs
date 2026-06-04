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
    /// Адаптация ObjBehPlayerMove из CocosSharp
    /// </summary>
    public class PlayerMovementComponent : MovementComponent
    {
        private float _minX, _maxX, _minY, _maxY;
        private float _maxSpeed = 450, _acceleration = 1500, _brakingSpeed = 800;
        private int _moveDirection = 0; // <0 left, 0 stop, >0 right
        private float _velocityX = 0;
        
        public float MaxSpeed 
        { 
            get => _maxSpeed; 
            set => _maxSpeed = value; 
        }
        
        public float Acceleration 
        { 
            get => _acceleration; 
            set => _acceleration = value; 
        }
        
        public float BrakingSpeed 
        { 
            get => _brakingSpeed; 
            set => _brakingSpeed = value; 
        }
        
        public PlayerMovementComponent(GameObject owner, float speed) : base(owner, speed)
        {
            // Устанавливаем границы движения (будут обновлены при инициализации GameManager)
            _minX = 0;
            _maxX = 1280;
            _minY = 0;
            _maxY = 768;
            
            // Параметры движения по умолчанию
            _maxSpeed = speed;
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
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Move(deltaTime);
        }
        
        /// <summary>
        /// Установка направления движения по точке касания
        /// Аналог SetMoveDirection из CocosSharp
        /// </summary>
        public void SetMoveDirection(Vector2 touchPoint)
        {
            float playerLeft = _owner.Position.X - _owner.Width / 2;
            float playerRight = _owner.Position.X + _owner.Width / 2;
            
            if (touchPoint.X < playerLeft) // влево
            {
                if (_velocityX > 0) // для мгновенного разворота
                    _velocityX = 0;
                _moveDirection = -1;
            }
            else if (touchPoint.X > playerRight) // вправо
            {
                if (_velocityX < 0)
                    _velocityX = 0;
                _moveDirection = 1;
            }
            else
                _moveDirection = 0;
        }
        
        /// <summary>
        /// Движение вправо
        /// </summary>
        public void MoveRight()
        {
            _moveDirection = 1;
        }
        
        /// <summary>
        /// Движение влево
        /// </summary>
        public void MoveLeft()
        {
            _moveDirection = -1;
        }
        
        /// <summary>
        /// Остановка движения
        /// Аналог MoveStop из CocosSharp
        /// </summary>
        public void MoveStop()
        {
            _moveDirection = 0;
        }
        
        /// <summary>
        /// Основной метод движения
        /// Аналог Move из CocosSharp
        /// </summary>
        private void Move(float deltaTime)
        {
            bool moved = false;
            
            // Ускоряемся
            if (_moveDirection < 0)
            {
                _velocityX -= _acceleration * deltaTime;
                moved = true;
            }
            else if (_moveDirection > 0)
            {
                _velocityX += _acceleration * deltaTime;
                moved = true;
            }
            else // если не ускоряемся, то тормозим
            {
                // Тормозим
                if (_velocityX < 0) // двигаемся влево
                {
                    _velocityX += _brakingSpeed * deltaTime; // торможение
                    if (_velocityX > 0) // если слишком затормозили, то стоп
                        _velocityX = 0;
                    moved = true;
                }
                else if (_velocityX > 0) // двигаемся вправо
                {
                    _velocityX -= _brakingSpeed * deltaTime;
                    if (_velocityX < 0)
                        _velocityX = 0;
                    moved = true;
                }
            }

            // Ограничение скорости
            if (_velocityX > _maxSpeed)
                _velocityX = _maxSpeed;
            if (_velocityX < -_maxSpeed)
                _velocityX = -_maxSpeed;

            // Двигаемся
            Vector2 newPosition = new Vector2(_owner.Position.X + _velocityX * deltaTime, _owner.Position.Y);
            
            // Ограничение движения по горизонтали
            float newX = MathHelper.Clamp(newPosition.X, _owner.Width / 2, _maxX - _owner.Width / 2);
            
            // Если достигли границы, останавливаемся
            if (newX <= _owner.Width / 2 || newX >= _maxX - _owner.Width / 2)
            {
                _velocityX = 0;
            }
            
            // Применяем новую позицию, только если она изменилась
            if (Math.Abs(_owner.Position.X - newX) > 0.01f)
            {
                _owner.Position = new Vector2(newX, _owner.Position.Y);
            }
        }
        
        /// <summary>
        /// Перемещение игрока в указанном направлении (legacy метод)
        /// </summary>
        public void MoveDirect(Vector2 direction)
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
