using Microsoft.Xna.Framework;
using RiotGalaxy.Components;

namespace RiotGalaxy.GameObjects
{
    /// <summary>
    /// Красный малый враг. Самый живучий (Hp 20), медленный. Аналог EnemySmallRed из CocosSharp.
    /// (Прицельная стрельба по игроку появится на этапе столкновений/боёв.)
    /// </summary>
    public class EnemySmallRed : Enemy
    {
        public EnemySmallRed(Vector2 position) : base(position)
        {
            ApplyStats(EnemyType.RED);

            LoadSprite("Images/enemyRed");

            Move = new EnemyBounceMovement(this, CurrentSpeed);
            Move.SetDirection(180f); // вниз, медленно
            Movement = Move;
            Collision = new EnemyCollisionComponent(this);
        }

        // Красный целится прямо в игрока (аналог ObjBehShootAimToPlayer)
        protected override void Shoot() => ShootAimAtPlayer();
    }
}
