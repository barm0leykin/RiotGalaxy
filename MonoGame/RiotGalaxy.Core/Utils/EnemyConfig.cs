using System;
using System.Collections.Generic;
using RiotGalaxy.GameObjects;

namespace RiotGalaxy.Utils
{
    /// <summary>
    /// Параметры врагов из Content/Config/enemies.yaml. Скорость и интервал стрельбы
    /// поддерживают рандомизацию (min/max). Дефолты в коде = фолбэк, игра работает без файла.
    /// </summary>
    public static class EnemyConfig
    {
        public class Stats
        {
            public float Hp { get; set; } = 10;
            public float Damage { get; set; } = 10;
            public float Speed { get; set; } = 100;
            public float SpeedMin { get; set; }
            public float SpeedMax { get; set; }
            public float ShootInterval { get; set; } = 3f;
            public float ShootIntervalMin { get; set; }
            public float ShootIntervalMax { get; set; }

            // Скорость во время вылета/атаки из улья (0 — использовать обычную Speed).
            public float AttackSpeed { get; set; }
            public float AttackSpeedMin { get; set; }
            public float AttackSpeedMax { get; set; }

            /// <summary>Тактики пике при вылете из улья (random/snake/ram/ellipse). Пусто — random.</summary>
            public List<string> Tactics { get; set; }

            public float PickSpeed(Random r) => Pick(Speed, SpeedMin, SpeedMax, r);
            public float PickShootInterval(Random r) => Pick(ShootInterval, ShootIntervalMin, ShootIntervalMax, r);
            /// <summary>Скорость атаки; 0 — не задана (вызывающий берёт обычную скорость).</summary>
            public float PickAttackSpeed(Random r) => Pick(AttackSpeed, AttackSpeedMin, AttackSpeedMax, r);

            private static float Pick(float fixedValue, float min, float max, Random r) =>
                (max > min) ? min + (float)r.NextDouble() * (max - min) : fixedValue;
        }

        // Дефолтные параметры (совпадают с прежними хардкодами)
        private static readonly Dictionary<EnemyType, Stats> _defaults = new Dictionary<EnemyType, Stats>
        {
            [EnemyType.BLUE]     = new Stats { Hp = 10, Damage = 10, Speed = 130, AttackSpeed = 200, ShootInterval = 3, Tactics = new List<string> { "snake", "ram" } },
            [EnemyType.GREEN]    = new Stats { Hp = 10, Damage = 10, SpeedMin = 100, SpeedMax = 150, AttackSpeed = 200, ShootInterval = 3, Tactics = new List<string> { "random", "snake" } },
            [EnemyType.RED]      = new Stats { Hp = 20, Damage = 10, Speed = 60, AttackSpeed = 160, ShootInterval = 3, Tactics = new List<string> { "ram", "ellipse" } },
            [EnemyType.SM_SCOUT] = new Stats { Hp = 10, Damage = 5, SpeedMin = 60, SpeedMax = 100, AttackSpeed = 150, ShootInterval = 0, Tactics = new List<string> { "random" } },
            [EnemyType.BOSS]     = new Stats { Hp = 200, Damage = 20, Speed = 50, ShootInterval = 1.2f },
        };

        private static Dictionary<EnemyType, Stats> _stats;

        public static Stats Get(EnemyType type)
        {
            if (_stats != null && _stats.TryGetValue(type, out var s))
                return s;
            return _defaults.TryGetValue(type, out var d) ? d : new Stats();
        }

        public static void Load()
        {
            var data = Yaml.LoadAsset<Dictionary<string, Stats>>(Yaml.ConfigAsset("enemies.yaml"));
            if (data == null)
                return;

            _stats = new Dictionary<EnemyType, Stats>(_defaults);
            foreach (var kv in data)
            {
                if (TryParseType(kv.Key, out var type) && kv.Value != null)
                    _stats[type] = kv.Value;
            }
        }

        private static bool TryParseType(string name, out EnemyType type)
        {
            switch (name.Trim().ToLowerInvariant())
            {
                case "blue": type = EnemyType.BLUE; return true;
                case "green": type = EnemyType.GREEN; return true;
                case "red": type = EnemyType.RED; return true;
                case "scout":
                case "smscout": type = EnemyType.SM_SCOUT; return true;
                case "boss": type = EnemyType.BOSS; return true;
                default: type = EnemyType.RND; return false;
            }
        }
    }
}
