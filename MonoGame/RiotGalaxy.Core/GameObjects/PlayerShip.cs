using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RiotGalaxy.Components;

namespace RiotGalaxy.GameObjects
{
    /// <summary>
    /// Типы оружия
    /// </summary>
    public enum WeaponType
    {
        Cannon,
        MachineGun,
        Laser
    }

    /// <summary>
    /// Класс корабля игрока
    /// Аналог PlayerShip из CocosSharp для MonoGame
    /// </summary>
    public class PlayerShip : GameObject
    {
        // Параметры здоровья
        private int _health;
        public int Health
        {
            get { return _health; }
            set
            {
                _health = Math.Max(0, Math.Min(value, MaxHealth));
                // Здесь будет событие об обновлении здоровья
            }
        }
        public int MaxHealth { get; set; }

        // Параметры движения
        public float Speed { get; set; }

        // Параметры оружия
        public WeaponType CurrentWeapon { get; set; }
        public float FireRate { get; set; }
        private float _timeSinceLastShot = 0f;

        // Параметры неуязвимости
        public bool IsInvulnerable { get; private set; }
        private float _invulnerabilityTime = 0f;

        public PlayerShip(Vector2 position) : base(position, new Vector2(60, 60))
        {
            MaxHealth = 100;
            Health = MaxHealth;
            Speed = 300f; // пикселей в секунду
            CurrentWeapon = WeaponType.Cannon;
            FireRate = 2f; // выстрелов в секунду

            // Инициализируем компоненты
            Movement = new PlayerMovementComponent(this, Speed);
            Shooting = new PlayerShootingComponent(this);
            Collision = new PlayerCollisionComponent(this);
            
            // Убедимся, что компоненты правильно инициализированы
            Console.WriteLine($"=== PlayerShip initialized with MovementComponent at position {Position} ===");

            // Текстура будет создана позже после установки GraphicsDevice
            Texture = null;
        }

        /// <summary>
        /// Обновление состояния корабля
        /// </summary>
        public override void Update(GameTime gameTime)
        {
            if (!IsAlive)
                return;

            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Обновляем таймеры
            _timeSinceLastShot += deltaTime;

            if (IsInvulnerable)
            {
                _invulnerabilityTime -= deltaTime;
                if (_invulnerabilityTime <= 0)
                {
                    IsInvulnerable = false;
                }
            }

            // Вызываем базовый метод (обновляет компоненты)
            base.Update(gameTime);
        }

        /// <summary>
        /// Отрисовка корабля
        /// </summary>
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            // Отладочное сообщение
            Console.WriteLine($"=== Drawing PlayerShip at ({Position.X}, {Position.Y}), IsAlive: {IsAlive} ===");
            
            if (!IsAlive)
                return;

            // Если неуязвим, делаем мигание
            if (IsInvulnerable && (int)(_invulnerabilityTime * 10) % 2 == 0)
            {
                return; // Пропускаем отрисовку каждый второй кадр
            }

            // Вызываем базовый метод отрисовки
            base.Draw(gameTime, spriteBatch);

            // Рисуем дополнительную информацию (если нужно)
            DrawAdditionalInfo(spriteBatch);
        }

        /// <summary>
        /// Попытка выстрелить
        /// </summary>
        public bool TryShoot()
        {
            if (!IsAlive || _timeSinceLastShot < 1f / FireRate)
            {
                return false;
            }

            _timeSinceLastShot = 0f;
            return true;
        }

        /// <summary>
        /// Получение урона
        /// </summary>
        public void TakeDamage(int damage)
        {
            if (!IsAlive || IsInvulnerable)
                return;

            Health -= damage;

            if (Health <= 0)
            {
                Health = 0;
                IsAlive = false;
            }
            else
            {
                // Активируем временную неуязвимость
                ActivateInvulnerability(2f);
            }
        }

        /// <summary>
        /// Восстановление здоровья
        /// </summary>
        public void Heal(int amount)
        {
            Health += amount;
        }

        /// <summary>
        /// Активация неуязвимости на указанное время
        /// </summary>
        public void ActivateInvulnerability(float duration)
        {
            IsInvulnerable = true;
            _invulnerabilityTime = duration;
        }

        /// <summary>
        /// Смена оружия
        /// </summary>
        public void ChangeWeapon(WeaponType weaponType)
        {
            CurrentWeapon = weaponType;

            switch (weaponType)
            {
                case WeaponType.Cannon:
                    FireRate = 2f;
                    break;
                case WeaponType.MachineGun:
                    FireRate = 10f;
                    break;
                case WeaponType.Laser:
                    FireRate = 1f;
                    break;
            }
        }

        /// <summary>
        /// Сброс состояния корабля (например, при рестарте уровня)
        /// </summary>
        public void Reset()
        {
            Health = MaxHealth;
            IsAlive = true;
            IsInvulnerable = false;
            _timeSinceLastShot = 0f;

            // Сброс позиции (пока заглушка)
            // Position = new Vector2(GameManager.Instance.ScreenWidth / 2, GameManager.Instance.ScreenHeight - 100);
            Position = new Vector2(640, 668); // 1280/2, 768-100
        }

        /// <summary>
        /// Создание простой текстуры для корабля
        /// </summary>
        private Texture2D CreateSimpleTexture(Color color)
        {
            // Создаем текстуру 64x64 (квадрат)
            Texture2D texture = new Texture2D(_graphicsDevice ?? throw new Exception("GraphicsDevice not set"), 64, 64);
            Color[] data = new Color[64 * 64];
            
            // Создаем простой корабль как набор пикселей
            for (int y = 0; y < 64; y++)
            {
                for (int x = 0; x < 64; x++)
                {
                    int index = y * 64 + x;
                    
                    // Создаем простую форму корабля
                    if (y >= 40) // Нижняя часть (корпус)
                    {
                        if (x >= 20 && x <= 43)
                            data[index] = color;
                    }
                    else if (y >= 30) // Средняя часть
                    {
                        if (x >= 25 && x <= 38)
                            data[index] = color;
                    }
                    else if (y >= 20) // Верхняя часть (кабина)
                    {
                        if (x >= 28 && x <= 35)
                            data[index] = color;
                    }
                    else // Верхушка
                    {
                        if (x >= 30 && x <= 33)
                            data[index] = color;
                    }
                }
            }
            
            texture.SetData(data);
            return texture;
        }

        /// <summary>
        /// Дополнительная отрисовка (например, для здоровья или эффектов)
        /// </summary>
        private void DrawAdditionalInfo(SpriteBatch spriteBatch)
        {
            // Если неуязвим, рисуем щит
            if (IsInvulnerable)
            {
                // Создаем простую текстуру для щита (если еще не создана)
                // Это просто пример, в реальном приложении текстуры лучше кэшировать
                if (_graphicsDevice == null) return; // Пропускаем если не установлен GraphicsDevice
                Texture2D shieldTexture = CreateSimpleTexture(Color.Blue);
                
                // Рисуем щит немного больше размера корабля
                spriteBatch.Draw(
                    shieldTexture, 
                    Position, 
                    null, 
                    new Color(0, 100, 255, 100), // Полупрозрачный синий
                    Rotation, 
                    new Vector2(32, 32), 
                    1.2f, 
                    SpriteEffects.None, 
                    0f
                );
            }
        }

        /// <summary>
        /// Получение GraphicsDevice (нужно для создания текстур)
        /// </summary>
        // private GraphicsDevice GraphicsDevice => GameManager.Instance.GraphicsDevice;
        // Используем заглушку вместо GameManager пока не реализован
        private GraphicsDevice _graphicsDevice;
        
        /// <summary>
        /// Установка GraphicsDevice
        /// </summary>
        public void SetGraphicsDevice(GraphicsDevice graphicsDevice)
        {
            _graphicsDevice = graphicsDevice;
            
            // Создаем текстуру для корабля после установки GraphicsDevice
            if (Texture == null)
            {
                // Используем ярко-зеленый цвет, чтобы корабль был хорошо виден на черном фоне
                Texture = CreateSimpleTexture(Color.Lime);
                Console.WriteLine("=== PlayerShip texture created with Lime color ===");
            }
        }
    }
}
