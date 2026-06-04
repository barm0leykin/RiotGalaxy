using Microsoft.Xna.Framework;
using RiotGalaxy.Components;

namespace RiotGalaxy.GameObjects
{
    /// <summary>
    /// Зелёный малый враг. Hp 10, движется вниз, периодически меняя направление
    /// и скорость (NewIdea каждые 2 сек). Аналог EnemySmallGreen из CocosSharp.
    /// </summary>
    public class EnemySmallGreen : Enemy
    {
        private float _ideaTime = 0f;
        private const float IdeaInterval = 2f;

        public EnemySmallGreen(Vector2 position) : base(position)
        {
            Type = EnemyType.GREEN;
            Hp = MaxHp = 10;
            Damage = 10;

            LoadSprite("Images/enemyGreen");

            Move = new EnemyBounceMovement(this, CurrentSpeed);
            Movement = Move;
            Collision = new EnemyCollisionComponent(this);

            NewIdea();
        }

        public override void Update(GameTime gameTime)
        {
            if (!IsAlive)
                return;

            _ideaTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (_ideaTime > IdeaInterval)
            {
                _ideaTime = 0f;
                NewIdea();
            }

            base.Update(gameTime); // обновит движение через компонент
        }

        // Зелёный стреляет вниз
        protected override void Shoot() => ShootDown();

        /// <summary>Случайно меняет скорость и направление (135..225° — вниз с уклоном).</summary>
        private void NewIdea()
        {
            CurrentSpeed = 100f + (float)Rnd.NextDouble() * 50f; // 100..150
            Move.SetDirection(135f + (float)Rnd.NextDouble() * 90f);
        }
    }
}
