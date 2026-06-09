using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RiotGalaxy.Managers;

namespace RiotGalaxy.GameObjects
{
    /// <summary>
    /// Типы бонусов (как в CocosSharp).
    /// </summary>
    public enum BonusType { BULLET_UP = 0, HP_UP, NUKE_BOMB, STAR }

    /// <summary>
    /// Базовый бонус. Падает вниз, при подборе игроком применяет эффект.
    /// Адаптация Bonus из CocosSharp.
    /// </summary>
    public class Bonus : GameObject
    {
        public BonusType Type { get; protected set; }
        protected float CurrentSpeed = 200f;
        protected Vector2 Velocity;
        public bool IsCollected { get; private set; }

        protected Bonus(Vector2 position) : base(position, new Vector2(20, 20))
        {
            SetDirection(180f); // по умолчанию падает вниз
        }

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
                Console.WriteLine($"=== Bonus sprite '{asset}' load failed: {ex.Message} ===");
            }
        }

        /// <summary>Направление движения (градусы): 0 = вверх, 180 = вниз.</summary>
        protected void SetDirection(float angleDeg)
        {
            float rad = MathHelper.ToRadians(angleDeg);
            Velocity = new Vector2((float)Math.Sin(rad), -(float)Math.Cos(rad)) * CurrentSpeed;
        }

        public override void Update(GameTime gameTime)
        {
            if (!IsAlive)
                return;

            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Position += Velocity * dt;

            var gm = GameManager.Instance;
            if (Position.Y > gm.ScreenHeight + 50 || Position.Y < -50)
                IsAlive = false;
        }

        /// <summary>Подбор бонуса игроком.</summary>
        public void Collect()
        {
            IsCollected = true;
            IsAlive = false;
        }

        /// <summary>Применить эффект бонуса к игроку (переопределяется типами).</summary>
        public virtual void Apply(PlayerShip player) { }
    }

    /// <summary>Восстановление здоровья до максимума.</summary>
    public class BonusHpUp : Bonus
    {
        public BonusHpUp(Vector2 pos) : base(pos)
        {
            Type = BonusType.HP_UP;
            LoadSprite("Images/bonusHPUp");
        }

        public override void Apply(PlayerShip player)
        {
            int before = player.Health;
            player.Heal(Utils.BonusConfig.Current.HpUpAmount);
            int gained = player.Health - before;
            Managers.MessageLog.Add(gained > 0 ? $"+{gained} HP" : "HP полное", Color.Lime);
        }
    }

    /// <summary>Улучшение текущего оружия.</summary>
    public class BonusBulletUp : Bonus
    {
        public BonusBulletUp(Vector2 pos) : base(pos)
        {
            Type = BonusType.BULLET_UP;
            LoadSprite("Images/bonusBulletUp");
        }

        public override void Apply(PlayerShip player) => player.UpgradeWeapon();
    }

    /// <summary>Уничтожение всех врагов на экране.</summary>
    public class BonusNukeBomb : Bonus
    {
        public BonusNukeBomb(Vector2 pos) : base(pos)
        {
            Type = BonusType.NUKE_BOMB;
            LoadSprite("Images/bonusNukeBomb");
        }

        public override void Apply(PlayerShip player)
        {
            GameManager.Instance.KillAllEnemies();
            Managers.MessageLog.Add("Бомба! Всех в труху", Color.Orange);
        }
    }

    /// <summary>
    /// Звезда: даёт очки, притягивается к игроку в радиусе магнита, вращается.
    /// Адаптация BonusStar из CocosSharp.
    /// </summary>
    public class BonusStar : Bonus
    {
        private const float FallSpeed = 70f;    // скорость падения вне зоны магнита
        private float _angleDeg = 180f;          // текущий курс (град); 180 = вниз
        private readonly float _rotateSpeed;

        private static readonly Random Rnd = new Random();

        public BonusStar(Vector2 pos) : base(pos)
        {
            Type = BonusType.STAR;
            CurrentSpeed = FallSpeed;
            LoadSprite("Images/bonusStar");
            SetDirection(_angleDeg); // по умолчанию падает вниз
            _rotateSpeed = (float)(Rnd.NextDouble() * 4.0 - 2.0); // вращение вокруг своей оси
        }

        public override void Update(GameTime gameTime)
        {
            if (!IsAlive)
                return;

            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            var player = GameManager.Instance.Player;
            if (player != null)
            {
                var magnet = Weapons.WeaponConfig.Magnet; // оборудование корабля (weapons.yaml)
                Vector2 d = player.Position - Position;
                float target;
                if (d.LengthSquared() < magnet.Radius * magnet.Radius)
                {
                    // в зоне магнита — плавно доворачиваем КУРС на игрока и ускоряемся
                    CurrentSpeed = magnet.PullSpeed;
                    target = MathHelper.ToDegrees((float)Math.Atan2(d.X, -d.Y));
                }
                else
                {
                    // вне зоны — плавно возвращаемся к падению вниз
                    CurrentSpeed = FallSpeed;
                    target = 180f;
                }
                _angleDeg = ApproachAngle(_angleDeg, target, magnet.TurnSpeed * dt);
                SetDirection(_angleDeg);
            }

            Position += Velocity * dt;
            Rotation += _rotateSpeed * dt;

            var gm = GameManager.Instance;
            if (Position.Y > gm.ScreenHeight + 50 || Position.Y < -50)
                IsAlive = false;
        }

        /// <summary>Плавный доворот current→target за шаг maxStep (град), кратчайшим путём.</summary>
        private static float ApproachAngle(float current, float target, float maxStep)
        {
            // Кратчайшая разница в диапазоне (-180, 180] — иначе на границе ±180° доворот «длинным путём»
            float diff = Mod360(target - current + 180f) - 180f;
            if (Math.Abs(diff) <= maxStep)
                return Mod360(target);
            return Mod360(current + Math.Sign(diff) * maxStep);
        }

        private static float Mod360(float a)
        {
            a %= 360f;
            return a < 0 ? a + 360f : a;
        }

        public override void Apply(PlayerShip player)
        {
            int score = Utils.BonusConfig.Current.StarScore;
            player.Score += score;
            Managers.MessageLog.Add($"+{score} очк.", Color.Gold);
        }
    }
}
