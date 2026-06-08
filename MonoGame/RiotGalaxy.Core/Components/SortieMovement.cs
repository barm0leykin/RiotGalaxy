using System;
using Microsoft.Xna.Framework;
using RiotGalaxy.GameObjects;
using RiotGalaxy.Managers;

namespace RiotGalaxy.Components
{
    /// <summary>Тактика пике при вылете из улья (задаётся списком в enemies.yaml).</summary>
    public enum SortieTactic
    {
        Random,  // прямой курс вниз с уклоном + отскок от боков
        Snake,   // змейкой (синус по X) с уходом вниз
        Ram,     // таран — прямо в точку, где был игрок на момент вылета
        Ellipse  // петля/дуга (Galaga-стиль) с уходом вниз
    }

    /// <summary>
    /// Вылет из улья (Galaga-стиль): враг пикирует вниз (стреляя) по выбранной тактике,
    /// уходит за нижнюю границу, появляется сверху и возвращается в свою ячейку улья
    /// (снова FormationMovement). Управляется ульем (см. <see cref="GameObjects.Hive"/>).
    /// </summary>
    public class SortieMovement : MovementComponent
    {
        private enum Phase { Dive, Return }

        private readonly Hive _hive;
        private readonly int _cx, _cy;
        private readonly SortieTactic _tactic;
        private Phase _phase = Phase.Dive;

        // Старт пике (для траекторных тактик) и накопленное время
        private readonly float _startX, _startY;
        private float _t;

        // Прямолинейные тактики (Random/Ram)
        private float _vx, _vy;
        // Параметры траекторных тактик (Snake/Ellipse)
        private float _amp, _omega, _radius, _descend, _dir = 1f;

        public SortieMovement(GameObject owner, float speed, Hive hive, int cx, int cy, SortieTactic tactic)
            : base(owner, speed)
        {
            _hive = hive;
            _cx = cx;
            _cy = cy;
            _tactic = tactic;
            _startX = owner.Position.X;
            _startY = owner.Position.Y;

            var rnd = (owner as Enemy)?.AiRandom;
            double R() => rnd?.NextDouble() ?? 0.5;

            switch (tactic)
            {
                case SortieTactic.Ram:
                {
                    // Курс на позицию игрока в момент вылета (таран)
                    var player = GameManager.Instance.Player;
                    Vector2 dir = (player != null) ? player.Position - owner.Position : new Vector2(0, speed);
                    if (dir.LengthSquared() < 0.001f) dir = new Vector2(0, speed);
                    dir.Normalize();
                    _vx = dir.X * speed;
                    _vy = dir.Y * speed;
                    break;
                }
                case SortieTactic.Snake:
                    // Линейная скорость ≈ speed: вниз 0.85·speed + боковое колебание ≤0.5·speed
                    _amp = 100f;
                    _descend = speed * 0.85f;
                    _omega = (speed * 0.5f) / _amp; // боковая макс. скорость = amp·omega ≈ 0.5·speed
                    break;
                case SortieTactic.Ellipse:
                    // Тангенциальная скорость орбиты = R·omega = speed; центр медленно сносится вниз
                    _radius = 110f;
                    _omega = speed / _radius;
                    _descend = speed * 0.3f;
                    _dir = R() < 0.5 ? -1f : 1f; // петля влево или вправо
                    break;
                default: // Random
                {
                    float deg = 155f + (float)(R() * 50f); // 155..205 — вниз с уклоном
                    float rad = MathHelper.ToRadians(deg);
                    _vx = (float)Math.Sin(rad) * speed;
                    _vy = -(float)Math.Cos(rad) * speed;
                    break;
                }
            }
        }

        public override void Update(GameTime gameTime)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            var gm = GameManager.Instance;

            if (_phase == Phase.Dive)
            {
                _t += dt;
                Vector2 p = _owner.Position;

                switch (_tactic)
                {
                    case SortieTactic.Snake:
                        p.X = _startX + _amp * (float)Math.Sin(_omega * _t);
                        p.Y = _startY + _descend * _t;
                        break;

                    case SortieTactic.Ellipse:
                    {
                        float theta = _omega * _t;
                        p.X = _startX + _dir * _radius * (float)Math.Sin(theta);
                        p.Y = _startY + _descend * _t - _radius * (float)Math.Cos(theta) + _radius;
                        break;
                    }

                    case SortieTactic.Ram:
                        p += new Vector2(_vx, _vy) * dt; // прямо, без отскока (таран)
                        break;

                    default: // Random — курс вниз с отскоком от боков
                        p += new Vector2(_vx, _vy) * dt;
                        float border = gm.ScreenWidth * 0.10f;
                        if (p.X < border) { _vx = -_vx; p.X = border + 1; }
                        else if (p.X > gm.ScreenWidth - border - _owner.Width)
                        {
                            _vx = -_vx;
                            p.X = gm.ScreenWidth - border - _owner.Width - 1;
                        }
                        break;
                }

                // Удержим в пределах экрана по X (для траекторных тактик)
                p.X = MathHelper.Clamp(p.X, 0f, gm.ScreenWidth - _owner.Width);

                // Пролетел за нижнюю границу → появляется сверху и заходит на возврат в улей
                if (p.Y > gm.ScreenHeight + 50f)
                {
                    p.Y = -50f;
                    _phase = Phase.Return;
                }

                _owner.Position = p;
            }
            else // Return — летим к своей ячейке
            {
                Vector2 target = _hive.CellWorldPos(_cx, _cy);
                Vector2 to = target - _owner.Position;
                float dist = to.Length();
                float step = _speed * dt;

                if (dist <= step || dist < 0.001f)
                {
                    _owner.Position = target;
                    _owner.Movement = new FormationMovement(_owner, _speed, _hive, _cx, _cy);
                    if (_owner is Enemy e)
                    {
                        e.ShootSafe = true; // снова в строю — не стреляем
                        _hive.NotifyReturned(e);
                    }
                }
                else
                {
                    _owner.Position += to / dist * step;
                }
            }
        }

        /// <summary>Разобрать строку тактики (из enemies.yaml). Неизвестное → Random.</summary>
        public static SortieTactic ParseTactic(string s)
        {
            switch (s?.Trim().ToLowerInvariant())
            {
                case "snake": return SortieTactic.Snake;
                case "ram": return SortieTactic.Ram;
                case "ellipse": return SortieTactic.Ellipse;
                default: return SortieTactic.Random;
            }
        }
    }
}
