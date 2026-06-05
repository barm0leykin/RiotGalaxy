using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RiotGalaxy.Components;
using RiotGalaxy.Managers;
using RiotGalaxy.Weapons;

namespace RiotGalaxy.GameObjects
{
    /// <summary>
    /// Типы врагов (как в CocosSharp).
    /// </summary>
    public enum EnemyType { RND = 0, SM_SCOUT, BLUE, GREEN, RED, BOSS }

    /// <summary>
    /// Базовый класс врага. Адаптация Enemy из CocosSharp.
    /// Конкретные параметры (Hp, урон, скорость, спрайт, траектория) задают наследники.
    /// </summary>
    public class Enemy : GameObject
    {
        public EnemyType Type { get; protected set; } = EnemyType.RND;
        public int Hp { get; set; } = 10;
        public int MaxHp { get; set; } = 10;
        public int Damage { get; set; } = 10;
        public float MaxSpeed { get; set; } = 100f;
        public float CurrentSpeed { get; set; } = 100f;

        // Типизированный доступ к компоненту движения с отскоком
        protected EnemyBounceMovement Move;

        // Оружие врага (аналог Enemy.gun из CocosSharp)
        public Weapon Gun { get; protected set; }
        protected float ShootInterval = 3f; // сек между выстрелами
        private float _actionTime;

        protected static readonly Random Rnd = new Random();

        public Enemy(Vector2 position) : base(position, new Vector2(45, 45))
        {
            Gun = new WeaponCannon(this);
            _actionTime = (float)Rnd.NextDouble() * ShootInterval; // разнобой старта стрельбы
        }

        public override void Update(GameTime gameTime)
        {
            if (!IsAlive)
                return;

            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Оружие (очереди/перезарядка)
            Gun?.Update(gameTime);

            // Решение о стрельбе по таймеру (базовый AI). ShootInterval<=0 — не стреляет.
            if (ShootInterval > 0f)
            {
                _actionTime += dt;
                if (_actionTime >= ShootInterval)
                {
                    _actionTime = 0f;
                    Shoot();
                }
            }

            base.Update(gameTime); // движение через компонент
        }

        /// <summary>
        /// Поставить врага в формацию (улей): движение сменяется на полёт к ячейке + барражирование.
        /// </summary>
        public void JoinFormation(Hive hive, int cx, int cy)
        {
            Movement = new FormationMovement(this, CurrentSpeed, hive, cx, cy);
            Move = null;
        }

        /// <summary>Пустить врага по маршруту; end — поведение после маршрута (отскок/разлёт/формация).</summary>
        public void SetRoute(Route route, RouteEndBehavior end = RouteEndBehavior.Bounce, Hive hive = null)
        {
            Movement = new RouteMovement(this, CurrentSpeed, route, end, hive);
            Move = null;
        }

        /// <summary>
        /// Применить параметры из конфига (enemies.yaml) к врагу заданного типа,
        /// включая рандомизацию скорости/интервала стрельбы. Вызывается в конструкторах типов.
        /// </summary>
        protected void ApplyStats(EnemyType type)
        {
            Type = type;
            var s = Utils.EnemyConfig.Get(type);
            Hp = MaxHp = (int)s.Hp;
            Damage = (int)s.Damage;
            MaxSpeed = CurrentSpeed = s.PickSpeed(Rnd);
            ShootInterval = s.PickShootInterval(Rnd);
            _actionTime = (float)Rnd.NextDouble() * (ShootInterval > 0 ? ShootInterval : 1f); // разнобой старта
        }

        /// <summary>Поведение стрельбы. По умолчанию враг не стреляет (переопределяется типами).</summary>
        protected virtual void Shoot() { }

        /// <summary>Выстрел строго вниз (аналог ObjBehShootDown).</summary>
        protected void ShootDown()
        {
            Gun.Aim(MathHelper.Pi); // 180° = вниз
            Gun.Fire();
        }

        /// <summary>Прицельный выстрел в игрока (аналог ObjBehShootAimToPlayer).</summary>
        protected void ShootAimAtPlayer()
        {
            var player = GameManager.Instance.Player;
            if (player == null)
                return;
            Vector2 d = player.Position - Position;
            float angle = (float)Math.Atan2(d.X, -d.Y); // конвенция Weapon.Aim: 0=вверх, π=вниз
            Gun.Aim(angle);
            Gun.Fire();
        }

        /// <summary>
        /// Загрузка спрайта врага из Content Pipeline (аналог draw.LoadGraphics).
        /// </summary>
        protected void LoadSprite(string asset)
        {
            try
            {
                Texture = GameManager.Instance.Content.Load<Texture2D>(asset);
                if (Texture != null)
                    Size = new Vector2(Texture.Width, Texture.Height);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"=== Enemy sprite '{asset}' load failed: {ex.Message} ===");
            }
        }

        /// <summary>
        /// Получение урона. Аналог Enemy.Hit из CocosSharp.
        /// </summary>
        public void TakeDamage(int damage)
        {
            Hp -= damage;
            if (Hp <= 0)
            {
                Hp = 0;
                Die();
            }
        }

        /// <summary>
        /// Уничтожение врага.
        /// </summary>
        protected virtual void Die()
        {
            IsAlive = false; // GameManager удалит объект и обновит счётчики
            AudioManager.Instance.PlayEffect("explode1");
        }
    }
}
