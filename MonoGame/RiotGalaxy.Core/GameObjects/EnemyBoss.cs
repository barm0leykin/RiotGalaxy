using Microsoft.Xna.Framework;
using RiotGalaxy.Components;

namespace RiotGalaxy.GameObjects
{
    /// <summary>
    /// Босс: живучий и крупный враг с прицельной стрельбой. Уникальное поведение —
    /// медленное барражирование по горизонтали и частые залпы в игрока.
    /// (Отдельного спрайта боса в атласе нет — используем enemyRed увеличенным.)
    /// </summary>
    public class EnemyBoss : Enemy
    {
        public EnemyBoss(Vector2 position) : base(position)
        {
            ApplyStats(EnemyType.BOSS); // hp/damage/скорость/интервал стрельбы из конфига

            LoadSprite("Images/enemyRed");
            Scale = new Vector2(2.5f, 2.5f); // визуально крупнее
            Size = new Vector2(Size.X * 2.5f, Size.Y * 2.5f); // и хитбокс крупнее

            Move = new EnemyBounceMovement(this, CurrentSpeed);
            Move.SetDirection(135f); // медленно вниз-вбок, отскакивает от краёв
            Movement = Move;
            Collision = new EnemyCollisionComponent(this);
        }

        protected override void Shoot() => ShootAimAtPlayer();
    }
}
