using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace RiotGalaxy.GameObjects
{
    /// <summary>
    /// Маршрут движения врага — последовательность точек (экранные координаты).
    /// Точки задаются в координатах ячеек World в Content/Routes/&lt;name&gt;.yaml.
    /// Аналог Route из CocoSharp (LoadRoute/GetNextPoint).
    /// Каждый враг получает свой экземпляр (свой индекс прогресса), но список точек общий (кэш).
    /// </summary>
    public class Route
    {
        private readonly List<Vector2> _points;
        private int _i;

        public Route(List<Vector2> points) => _points = points;

        public bool HasPoints => _points != null && _points.Count > 0;
        public Vector2 Current => _points[_i];
        public bool IsLast => _i >= _points.Count - 1;
        public void Advance() { if (!IsLast) _i++; }

        // ----- загрузка с кэшем (позиции детерминированы размером экрана) -----
        private static readonly Dictionary<string, List<Vector2>> _cache = new Dictionary<string, List<Vector2>>();

        public static Route Load(string name, World world)
        {
            if (!_cache.TryGetValue(name, out var pts))
            {
                pts = BuildPoints(name, world);
                _cache[name] = pts;
            }
            return new Route(pts);
        }

        private static List<Vector2> BuildPoints(string name, World world)
        {
            var pts = new List<Vector2>();
            var data = Utils.Yaml.LoadAsset<RouteYaml>("Content/Routes/" + name + ".yaml");
            if (data?.Points != null)
            {
                foreach (var p in data.Points)
                    if (p != null && p.Count >= 2)
                        pts.Add(world.GetCellPosition((int)p[0], (int)p[1]));
            }
            return pts;
        }

        private class RouteYaml
        {
            public List<List<float>> Points { get; set; }
        }
    }
}
