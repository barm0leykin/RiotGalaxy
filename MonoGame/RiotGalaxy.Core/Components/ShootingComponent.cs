using Microsoft.Xna.Framework;
using RiotGalaxy.GameObjects;

namespace RiotGalaxy.Components
{
    /// <summary>
    /// Базовый класс компонентов стрельбы (паттерн Стратегия).
    /// Примечание: стрельба игрока теперь реализована через иерархию Weapon
    /// (PlayerShip.Gun). Эти компоненты — задел под стрельбу врагов (этап 7).
    /// </summary>
    public abstract class ShootingComponent
    {
        protected GameObject _owner;
        protected float _fireRate;
        protected float _fireTimer = 0f;

        public float FireRate
        {
            get => _fireRate;
            set => _fireRate = value <= 0 ? 0.1f : value;
        }

        public ShootingComponent(GameObject owner, float fireRate)
        {
            _owner = owner;
            _fireRate = fireRate;
        }

        public abstract void Update(GameTime gameTime);

        /// <summary>
        /// Попытка выстрелить с учётом темпа стрельбы.
        /// </summary>
        public virtual bool TryShoot()
        {
            if (_fireTimer <= 0f)
            {
                _fireTimer = 1f / _fireRate;
                return true;
            }
            return false;
        }
    }

    /// <summary>
    /// Компонент стрельбы игрока. Сейчас оружие игрока управляется через PlayerShip.Gun
    /// (иерархия Weapon), поэтому компонент оставлен как лёгкий задел и не управляет снарядами.
    /// </summary>
    public class PlayerShootingComponent : ShootingComponent
    {
        public PlayerShootingComponent(GameObject owner) : base(owner, 2.0f)
        {
        }

        public override void Update(GameTime gameTime)
        {
            if (_fireTimer > 0f)
                _fireTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
        }
    }

    /// <summary>
    /// Компонент стрельбы врагов. Заготовка под этап 7 (враги атакуют игрока).
    /// </summary>
    public class EnemyShootingComponent : ShootingComponent
    {
        public ShootingPattern Pattern { get; set; }
        public Vector2 TargetPosition { get; set; }

        public EnemyShootingComponent(GameObject owner, float fireRate, ShootingPattern pattern = ShootingPattern.Direct)
            : base(owner, fireRate)
        {
            Pattern = pattern;
            TargetPosition = Vector2.Zero;
        }

        public override void Update(GameTime gameTime)
        {
            if (_fireTimer > 0f)
                _fireTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Логика автострельбы врагов будет добавлена на этапе 7
        }
    }

    /// <summary>
    /// Паттерны стрельбы врагов.
    /// </summary>
    public enum ShootingPattern
    {
        Direct,    // прямо в цель
        Fixed,     // в фиксированном направлении
        Pattern    // по заданному паттерну
    }
}
