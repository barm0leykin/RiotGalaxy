using Microsoft.Xna.Framework;
using RiotGalaxy.GameObjects;

namespace RiotGalaxy.Components
{
    /// <summary>
    /// Движение врага в составе формации (улья): летит к своей ячейке, затем «прилипает»
    /// к ней и барражирует вместе со всем ульем (ячейка движется вместе с Hive.Offset).
    /// </summary>
    public class FormationMovement : MovementComponent
    {
        private readonly Hive _hive;
        private readonly int _cx, _cy;

        public FormationMovement(GameObject owner, float speed, Hive hive, int cx, int cy)
            : base(owner, speed)
        {
            _hive = hive;
            _cx = cx;
            _cy = cy;
        }

        public override void Update(GameTime gameTime)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Vector2 target = _hive.CellWorldPos(_cx, _cy);
            Vector2 to = target - _owner.Position;
            float dist = to.Length();
            float step = _speed * dt;

            if (dist <= step || dist < 0.001f)
                _owner.Position = target;            // в формации — следуем за ячейкой
            else
                _owner.Position += to / dist * step; // летим к своей ячейке
        }
    }
}
