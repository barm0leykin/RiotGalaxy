using System;
using Microsoft.Xna.Framework;
using RiotGalaxy.GameObjects;

namespace RiotGalaxy.Components
{
    /// <summary>Что делает враг после прохождения маршрута.</summary>
    public enum RouteEndBehavior
    {
        Bounce,     // вниз с отскоком (как было)
        Scatter,    // случайное направление с отскоком (разлёт)
        Formation   // занять ячейку улья и встать в строй
    }

    /// <summary>
    /// Движение врага по маршруту: летит от точки к точке. После последней точки переключается
    /// на выбранную стратегию (отскок / разлёт / формация). Аналог AIStateOnRoute из CocoSharp.
    /// </summary>
    public class RouteMovement : MovementComponent
    {
        private readonly Route _route;
        private readonly RouteEndBehavior _end;
        private readonly Hive _hive;
        private MovementComponent _after; // движение после маршрута
        private const float Threshold = 8f;

        private static readonly Random _rnd = new Random();

        public RouteMovement(GameObject owner, float speed, Route route,
            RouteEndBehavior end = RouteEndBehavior.Bounce, Hive hive = null) : base(owner, speed)
        {
            _route = route;
            _end = end;
            _hive = hive;
        }

        public override void Update(GameTime gameTime)
        {
            if (_after != null)
            {
                _after.Update(gameTime);
                return;
            }
            if (_route == null || !_route.HasPoints)
            {
                FinishRoute();
                return;
            }

            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            float speed = (_owner is Enemy e) ? e.CurrentSpeed : _speed;
            float step = speed * dt;

            Vector2 to = _route.Current - _owner.Position;
            float dist = to.Length();

            if (dist <= step || dist < Threshold)
            {
                _owner.Position = _route.Current;
                if (_route.IsLast)
                    FinishRoute();
                else
                    _route.Advance();
            }
            else
            {
                _owner.Position += to / dist * step;
            }
        }

        /// <summary>Выбор поведения после маршрута.</summary>
        private void FinishRoute()
        {
            if (_end == RouteEndBehavior.Formation && _hive != null && _hive.TryTakeCell(out int cx, out int cy))
            {
                _after = new FormationMovement(_owner, _speed, _hive, cx, cy);
                return;
            }

            var bounce = new EnemyBounceMovement(_owner, _speed);
            bounce.SetDirection(_end == RouteEndBehavior.Scatter
                ? 135f + (float)_rnd.NextDouble() * 90f // 135..225 — вниз с разлётом
                : 180f);                                // строго вниз
            _after = bounce;
        }
    }
}
