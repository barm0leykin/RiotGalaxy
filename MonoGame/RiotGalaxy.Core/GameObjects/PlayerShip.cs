using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using RiotGalaxy.Components;
using RiotGalaxy.Weapons;

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
                int oldHealth = _health;
                _health = Math.Max(0, Math.Min(value, MaxHealth));
                
                // Вызываем событие об изменении здоровья
                OnHealthChanged(oldHealth, _health);
                
                // Проверяем состояние игрока
                if (_health <= 0 && IsAlive)
                {
                    Die();
                }
            }
        }
        public int MaxHealth { get; set; }

        // Параметры движения
        public float Speed { get; set; }

        // Параметры оружия
        public WeaponType CurrentWeapon { get; set; }
        public float FireRate { get; set; }
        private float _timeSinceLastShot = 0f;

        // Текущее оружие (аналог playerShip.gun из CocosSharp)
        public Weapon Gun { get; private set; }

        // Параметры неуязвимости (аналог GodMode в CocosSharp)
        public bool IsInvulnerable { get; private set; }
        private float _invulnerabilityTime = 0f;
        private const float DEFAULT_INVULNERABILITY_TIME = 2000f; // 2 секунды по умолчанию
        
        // Состояние игрока (переопределяем базовый)
        public new bool IsAlive { get; private set; } = true;
        
        // События
        public event Action<int, int> HealthChanged; // oldHealth, newHealth
        public event Action PlayerDied;
        public event Action PlayerRespawned;

        public PlayerShip(Vector2 position) : base(position, new Vector2(60, 60))
        {
            MaxHealth = 100; // По умолчанию 100 HP, как в CocosSharp
            Health = MaxHealth;
            Speed = 300f; // пикселей в секунду
            CurrentWeapon = WeaponType.Cannon;
            FireRate = 2f; // выстрелов в секунду
            IsAlive = true;

            // Инициализируем компоненты
            Movement = new PlayerMovementComponent(this, Speed);
            Shooting = new PlayerShootingComponent(this);
            Collision = new PlayerCollisionComponent(this);

            // Оружие по умолчанию — пушка (аналог CocosSharp)
            Gun = new WeaponCannon(this);
            
            // Убедимся, что компоненты правильно инициализированы
            Console.WriteLine($"=== PlayerShip initialized with MovementComponent at position {Position} ===");
            Console.WriteLine($"=== Player health initialized: {Health}/{MaxHealth} ===");

            // Текстура загружается в LoadContent (спрайт "Images/ship")
            Texture = null;
        }

        /// <summary>
        /// Имя спрайта корабля в Content Pipeline (атлас CocosSharp -> Images/ship)
        /// </summary>
        public const string ShipSpriteAsset = "Images/ship";

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

            // Обновляем состояние неуязвимости
            if (IsInvulnerable)
            {
                _invulnerabilityTime -= deltaTime;
                if (_invulnerabilityTime <= 0)
                {
                    DeactivateInvulnerability();
                }
            }

            // Обновляем оружие (очереди/перезарядка)
            Gun?.Update(gameTime);

            // Вызываем базовый метод (обновляет компоненты)
            base.Update(gameTime);
        }

        /// <summary>
        /// Выстрел из текущего оружия. Аналог вызова playerShip.gun.Fire().
        /// </summary>
        public void Fire()
        {
            if (!IsAlive)
                return;
            Gun?.Fire();
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
        /// Аналог Hit из CocosSharp
        /// </summary>
        public void TakeDamage(int damage)
        {
            if (!IsAlive || IsInvulnerable)
                return;

            Console.WriteLine($"=== Player taking damage: {damage}, current HP: {Health} ===");
            
            // Наносим урон
            Health -= damage;

            // Если еще живы, активируем временную неуязвимость (щит)
            if (IsAlive)
            {
                ActivateInvulnerability(DEFAULT_INVULNERABILITY_TIME);
            }
        }

        /// <summary>
        /// Восстановление здоровья
        /// Аналог HpUp из CocosSharp
        /// </summary>
        public void Heal(int amount)
        {
            Console.WriteLine($"=== Player healing: {amount}, current HP: {Health} ===");
            Health += amount;
        }

        /// <summary>
        /// Активация неуязвимости на указанное время
        /// Аналог AddMyshield из CocosSharp
        /// </summary>
        public void ActivateInvulnerability(float duration)
        {
            IsInvulnerable = true;
            _invulnerabilityTime = duration;
            Console.WriteLine($"=== Shield activated for {duration}ms ===");
        }

        /// <summary>
        /// Деактивация неуязвимости
        /// </summary>
        private void DeactivateInvulnerability()
        {
            IsInvulnerable = false;
            _invulnerabilityTime = 0;
            Console.WriteLine("=== Shield deactivated ===");
        }

        /// <summary>
        /// Смерть игрока
        /// </summary>
        private void Die()
        {
            if (!IsAlive) return;
            
            Console.WriteLine("=== Player died! ===");
            IsAlive = false;
            
            // Вызываем событие смерти
            PlayerDied?.Invoke();
            
            // Здесь будет событие смерти игрока
            // Например: GameManager.Instance.ChangeGameState(GameManager.GameState.GameOver);
        }

        /// <summary>
        /// Воскрешение игрока
        /// </summary>
        public void Respawn()
        {
            Console.WriteLine("=== Player respawning ===");
            Health = MaxHealth;
            IsAlive = true;
            IsInvulnerable = false;
            
            // Сброс позиции (может быть настроен в потомках)
            // Position = new Vector2(GameManager.Instance.ScreenWidth / 2, GameManager.Instance.ScreenHeight - 100);
            
            // Даем временную неуязвимость после воскрешения
            ActivateInvulnerability(DEFAULT_INVULNERABILITY_TIME);
            
            // Сброс других параметров
            _timeSinceLastShot = 0;
            
            // Вызываем событие воскрешения
            PlayerRespawned?.Invoke();
        }

        /// <summary>
        /// Смена оружия
        /// </summary>
        public void ChangeWeapon(WeaponType weaponType)
        {
            CurrentWeapon = weaponType;

            // Пересоздаём оружие соответствующего типа (аналог ChangeWeapon из CocosSharp)
            switch (weaponType)
            {
                case WeaponType.Cannon:
                    Gun = new WeaponCannon(this);
                    break;
                case WeaponType.MachineGun:
                    Gun = new WeaponMinigun(this);
                    break;
                case WeaponType.Laser:
                    Gun = new WeaponLaser(this);
                    break;
            }
            Console.WriteLine($"=== Weapon changed to {weaponType} ===");
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
            // Если неуязвим, рисуем щит (аналог AddMyshield из CocosSharp)
            if (IsInvulnerable)
            {
                // Создаем простую текстуру для щита (если еще не создана)
                // Это просто пример, в реальном приложении текстуры лучше кэшировать
                if (_graphicsDevice == null) return; // Пропускаем если не установлен GraphicsDevice
                Texture2D shieldTexture = CreateSimpleShieldTexture();
                
                // Рисуем прозрачный внешний слой щита
                spriteBatch.Draw(
                    shieldTexture, 
                    Position, 
                    null, 
                    new Color(100, 150, 255, 40), // Очень прозрачный синий
                    Rotation, 
                    new Vector2(32, 32), 
                    1.2f, 
                    SpriteEffects.None, 
                    0f
                );
                
                // Рисуем основной слой щита
                spriteBatch.Draw(
                    shieldTexture, 
                    Position, 
                    null, 
                    new Color(0, 100, 255, 80), // Полупрозрачный синий
                    Rotation, 
                    new Vector2(32, 32), 
                    1.0f, 
                    SpriteEffects.None, 
                    0f
                );
            }
        }
        
        /// <summary>
        /// Создание простой текстуры для щита
        /// </summary>
        private Texture2D CreateSimpleShieldTexture()
        {
            // Создаем текстуру 64x64 в форме круга для щита
            Texture2D texture = new Texture2D(_graphicsDevice, 64, 64);
            Color[] data = new Color[64 * 64];
            
            int centerX = 32;
            int centerY = 32;
            int radius = 30;
            
            for (int y = 0; y < 64; y++)
            {
                for (int x = 0; x < 64; x++)
                {
                    int index = y * 64 + x;
                    // Создаем круг для щита
                    float distance = (float)Math.Sqrt(Math.Pow(x - centerX, 2) + Math.Pow(y - centerY, 2));
                    if (distance <= radius)
                    {
                        // Градиент от центра к краям
                        float alpha = 1.0f - (distance / radius);
                        byte alphaByte = (byte)(alpha * 255);
                        data[index] = new Color((byte)0, (byte)150, (byte)255, alphaByte);
                    }
                    else
                    {
                        // Прозрачные пиксели вне круга
                        data[index] = Color.Transparent;
                    }
                }
            }
            
            texture.SetData(data);
            return texture;
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
            // GraphicsDevice нужен для генерации текстуры щита (CreateSimpleShieldTexture)
            // и для фолбэк-заглушки, если спрайт не загрузится.
            _graphicsDevice = graphicsDevice;
        }

        /// <summary>
        /// Загрузка спрайта корабля из Content Pipeline.
        /// Заменяет прежнюю заглушку (зелёный квадрат) реальным спрайтом "Images/ship".
        /// </summary>
        public void LoadContent(ContentManager content)
        {
            try
            {
                Texture = content.Load<Texture2D>(ShipSpriteAsset);
                Console.WriteLine($"=== PlayerShip sprite '{ShipSpriteAsset}' loaded ({Texture.Width}x{Texture.Height}) ===");
            }
            catch (Exception ex)
            {
                // Фолбэк на заглушку, чтобы игра не падала, если ассет недоступен
                Console.WriteLine($"=== Failed to load '{ShipSpriteAsset}', falling back to placeholder: {ex.Message} ===");
                if (_graphicsDevice != null)
                    Texture = CreateSimpleTexture(Color.Lime);
            }
        }
        
        /// <summary>
        /// Обработчик изменения здоровья
        /// </summary>
        private void OnHealthChanged(int oldHealth, int newHealth)
        {
            Console.WriteLine($"=== Player health changed: {oldHealth} -> {newHealth} ===");
            
            // Вызываем событие об изменении здоровья
            HealthChanged?.Invoke(oldHealth, newHealth);
            
            // Здесь можно добавить дополнительную логику, например:
            // - Визуальные эффекты при получении урона
            // - Звуковые эффекты
            // - Обновление UI
        }
    }
}