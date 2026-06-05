using System;
using Microsoft.Xna.Framework;
using RiotGalaxy.GameObjects;
using RiotGalaxy.Managers;

namespace RiotGalaxy.Weapons
{
    /// <summary>
    /// Базовый класс оружия. Адаптация Weapon из CocosSharp.
    /// Стреляет очередями (burst) с интервалом burstInterval, между очередями —
    /// перезарядка reloadSpeed. Создаёт снаряды и добавляет их в список объектов игры.
    /// </summary>
    public class Weapon
    {
        public enum WeaponType { CANNON = 0, MINIGUN, LASER }

        public int WeaponTypeId;
        public WeaponOptions Options;
        public bool Safe { get; set; } // предохранитель

        // Уровень прокачки и таблица параметров по уровням
        public int Level { get; protected set; }
        protected WeaponOptions[] _levels;

        protected GameObject _owner;
        protected int _fireCount = 0;

        // Прицеливание
        protected float _aimAngle = 0f;            // базовый угол прицеливания, рад (0 = строго вверх)

        // Очередь и перезарядка
        private int _burstRemaining = 0;
        private float _burstTimer = 0f;
        private float _reloadTimer = 0f;

        public Weapon(GameObject owner)
        {
            _owner = owner;
            Safe = false;
        }

        public void LoadWeaponOptions(WeaponOptions opt) => Options = opt;

        /// <summary>Привязать таблицу уровней и выставить стартовый уровень.</summary>
        protected void InitLevel(WeaponOptions[] levels, int lvl)
        {
            _levels = levels;
            Level = Math.Clamp(lvl, 0, levels.Length - 1);
            Options = levels[Level];
        }

        /// <summary>
        /// Задать БАЗОВЫЙ угол прицеливания (рад, 0 = вверх). Разброс (minigun) добавляется
        /// поверх него на каждый выстрел, не меняя базу.
        /// </summary>
        public void Aim(float angleRad)
        {
            _aimAngle = angleRad;
        }

        /// <summary>Единичный вектор направления для угла (0 = вверх; в MonoGame Y вниз).</summary>
        private static Vector2 DirFromAngle(float angleRad) =>
            new Vector2((float)Math.Sin(angleRad), -(float)Math.Cos(angleRad));

        /// <summary>
        /// Покадровое обновление: проигрывание очереди и отсчёт перезарядки.
        /// </summary>
        public void Update(GameTime gameTime)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (_reloadTimer > 0f)
                _reloadTimer -= dt;

            if (_burstRemaining > 0)
            {
                _burstTimer -= dt;
                if (_burstTimer <= 0f)
                {
                    FireOnce();
                    _burstRemaining--;
                    _burstTimer = Options.burstInterval;
                }
            }
        }

        /// <summary>
        /// Запрос на выстрел. Начинает новую очередь, если оружие не на перезарядке
        /// и предыдущая очередь завершена. Аналог Weapon.Fire из CocosSharp.
        /// </summary>
        public void Fire()
        {
            if (Safe || Options == null)
                return;
            if (_reloadTimer > 0f || _burstRemaining > 0)
                return; // идёт перезарядка или ещё стреляет текущая очередь

            _burstRemaining = Math.Max(1, Options.burst);
            _reloadTimer = Options.reloadSpeed;
            if (_owner is PlayerShip)
                AudioManager.Instance.PlayEffect("fire1"); // звук только у игрока

            // первый выстрел очереди — сразу
            FireOnce();
            _burstRemaining--;
            _burstTimer = Options.burstInterval;
        }

        /// <summary>Угол текущего выстрела (минигатлинг переопределяет для разброса).</summary>
        protected virtual float GetFireAngle() => _aimAngle;

        /// <summary>Создание конкретного снаряда (переопределяется в наследниках).</summary>
        protected virtual Shell CreateShell(Vector2 position) => new Bullet(position);

        /// <summary>
        /// Одиночный выстрел: создаёт снаряд, задаёт ему параметры и добавляет в игру.
        /// Аналог Weapon.FireOnce из CocosSharp.
        /// </summary>
        protected void FireOnce()
        {
            float angle = GetFireAngle();             // база + разброс (для конкретного выстрела)
            Vector2 dir = DirFromAngle(angle);
            Vector2 spawn = _owner.Position + dir * (_owner.Height * 0.5f); // не появляться внутри стрелка

            Shell shell = CreateShell(spawn);
            shell.Speed = Options.shellSpeed;
            shell.Damage = (int)Options.damage;
            shell.Direction = dir;
            shell.Rotation = angle;
            shell.PlayerSide = (_owner is PlayerShip); // сторона снаряда = сторона стрелка

            _fireCount++;
            GameManager.Instance.GameObjects.Add(shell);
        }

        /// <summary>Повысить уровень оружия (перезагрузить параметры следующего уровня).</summary>
        public virtual void Upgrade()
        {
            if (_levels != null && Level + 1 < _levels.Length)
            {
                Level++;
                Options = _levels[Level];
            }
        }
    }

    /// <summary>Пушка: одиночные мощные снаряды (Bullet).</summary>
    public class WeaponCannon : Weapon
    {
        public WeaponCannon(GameObject owner, int lvl = 0) : base(owner)
        {
            WeaponTypeId = (int)WeaponType.CANNON;
            InitLevel(WeaponConfig.Cannons, lvl);
        }

        protected override Shell CreateShell(Vector2 position) => new Bullet(position);
    }

    /// <summary>Пулемёт: быстрые очереди слабых снарядов (Slug) с разбросом ±5°.</summary>
    public class WeaponMinigun : Weapon
    {
        private static readonly Random _rnd = new Random();

        public WeaponMinigun(GameObject owner, int lvl = 0) : base(owner)
        {
            WeaponTypeId = (int)WeaponType.MINIGUN;
            InitLevel(WeaponConfig.Miniguns, lvl);
        }

        protected override float GetFireAngle()
        {
            // разброс ±5 градусов, как в оригинале (CCRandom.GetRandomInt(-5,5))
            float spreadDeg = _rnd.Next(-5, 6);
            return _aimAngle + MathHelper.ToRadians(spreadDeg);
        }

        protected override Shell CreateShell(Vector2 position) => new Slug(position);
    }

    /// <summary>Лазер: быстрые пробивающие снаряды (Laser).</summary>
    public class WeaponLaser : Weapon
    {
        public WeaponLaser(GameObject owner, int lvl = 0) : base(owner)
        {
            WeaponTypeId = (int)WeaponType.LASER;
            InitLevel(WeaponConfig.Lasers, lvl);
        }

        protected override Shell CreateShell(Vector2 position) => new Laser(position);
    }

    /// <summary>Отсутствие оружия (Null Object). Аналог NoWeapon из CocosSharp.</summary>
    public class NoWeapon : Weapon
    {
        public NoWeapon(GameObject owner) : base(owner)
        {
            Safe = true;
        }
    }
}
