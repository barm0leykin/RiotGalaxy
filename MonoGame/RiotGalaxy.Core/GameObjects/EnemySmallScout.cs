using Microsoft.Xna.Framework;
using RiotGalaxy.Components;

namespace RiotGalaxy.GameObjects
{
    /// <summary>
    /// Малый разведчик. Hp 10, слабый урон, движется вниз со случайным уклоном.
    /// Аналог EnemySmallScout из CocosSharp.
    /// </summary>
    public class EnemySmallScout : Enemy
    {
        public EnemySmallScout(Vector2 position) : base(position)
        {
            ApplyStats(EnemyType.SM_SCOUT); // hp/damage/скорость (рандом из конфига)

            LoadSprite("Images/enemySmallScout");

            Move = new EnemyBounceMovement(this, CurrentSpeed);
            Movement = Move;
            Collision = new EnemyCollisionComponent(this);

            // случайное направление (155..205° — вниз с лёгким уклоном)
            Move.SetDirection(155f + (float)Rnd.NextDouble() * 50f);
        }
    }
}
