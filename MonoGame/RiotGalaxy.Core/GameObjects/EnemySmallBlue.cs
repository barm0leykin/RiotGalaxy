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
            ApplyStats(EnemyType.BLUE);

            LoadSprite("Images/enemyBlue");

            Collision = new EnemyCollisionComponent(this);

            // ИИ-машина состояний (порт ObjBehAIEnemyBlue): взлёт → роение ↔ атака.
            // Сама задаёт движение/курс. Формация/маршрут из YAML её отключают.
            Ai = new AI.EnemyAIBlue(this);
        }

        // Синий стреляет строго вниз (аналог ObjBehShootDown)
        protected override void Shoot() => ShootDown();
    }
}
