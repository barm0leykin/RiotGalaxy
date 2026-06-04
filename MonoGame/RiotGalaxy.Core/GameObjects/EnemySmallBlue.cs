using Microsoft.Xna.Framework;
using RiotGalaxy.Components;

namespace RiotGalaxy.GameObjects
{
    /// <summary>
    /// Синий малый враг. Hp 10, движется прямо вниз. Аналог EnemySmallBlue из CocosSharp.
    /// </summary>
    public class EnemySmallBlue : Enemy
    {
        public EnemySmallBlue(Vector2 position) : base(position)
        {
            Type = EnemyType.BLUE;
            Hp = MaxHp = 10;
            Damage = 10;
            MaxSpeed = CurrentSpeed = 130f;

            LoadSprite("Images/enemyBlue");

            Move = new EnemyBounceMovement(this, CurrentSpeed);
            Move.SetDirection(180f); // строго вниз
            Movement = Move;
            Collision = new EnemyCollisionComponent(this);
        }

        // Синий стреляет строго вниз (аналог ObjBehShootDown)
        protected override void Shoot() => ShootDown();
    }
}
