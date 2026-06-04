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
            Type = EnemyType.SM_SCOUT;
            Hp = MaxHp = 10;
            Damage = 5;

            LoadSprite("Images/enemySmallScout");

            Move = new EnemyBounceMovement(this, CurrentSpeed);
            Movement = Move;
            Collision = new EnemyCollisionComponent(this);

            // случайные скорость (60..100) и направление (155..205° — вниз с лёгким уклоном)
            CurrentSpeed = 60f + (float)Rnd.NextDouble() * 40f;
            Move.SetDirection(155f + (float)Rnd.NextDouble() * 50f);
        }
    }
}
