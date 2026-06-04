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

        public override void Apply(PlayerShip player) => player.Health = player.MaxHealth;
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

        public override void Apply(PlayerShip player) => GameManager.Instance.KillAllEnemies();
    }

    /// <summary>
    /// Звезда: даёт очки, притягивается к игроку в радиусе магнита, вращается.
    /// Адаптация BonusStar из CocosSharp.
    /// </summary>
    public class BonusStar : Bonus
    {
        private const int ScoreValue = 10;
        private const float MagnetDist = 250f;
        private const float TurnSpeed = 90f; // градусов/сек доворота
        private float _angleDeg = 180f;
        private readonly float _rotateSpeed;

        private static readonly Random Rnd = new Random();

        public BonusStar(Vector2 pos) : base(pos)
        {
            Type = BonusType.STAR;
            CurrentSpeed = 70f;
            LoadSprite("Images/bonusStar");
            SetDirection(_angleDeg);
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
                float dist = Vector2.Distance(Position, player.Position);
                float target;
                if (dist < MagnetDist)
                {
                    // в зоне магнита — плавно доворачиваем к игроку
                    Vector2 d = player.Position - Position;
                    target = MathHelper.ToDegrees((float)Math.Atan2(d.X, -d.Y));
                }
                else
                {
                    target = 180f; // иначе плавно возвращаемся к падению вниз
                }
                _angleDeg = ApproachAngle(_angleDeg, target, TurnSpeed * dt);
                SetDirection(_angleDeg);
            }

            Position += Velocity * dt;
            Rotation += _rotateSpeed * dt;

            var gm = GameManager.Instance;
            if (Position.Y > gm.ScreenHeight + 50 || Position.Y < -50)
                IsAlive = false;
        }

        private static float ApproachAngle(float current, float target, float maxStep)
        {
            float diff = target - current;
            if (Math.Abs(diff) <= maxStep)
                return target;
            return current + Math.Sign(diff) * maxStep;
        }

        public override void Apply(PlayerShip player) => player.Score += ScoreValue;
    }
}
