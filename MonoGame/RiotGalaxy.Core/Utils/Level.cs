using System;
using System.Collections.Generic;
using System.IO;
using RiotGalaxy.GameObjects;

namespace RiotGalaxy.Utils
{
    /// <summary>
    /// Уровень: загружается из Content/Levels/level{N}.yaml, разворачивает события
    /// в очередь спавна и выдаёт врагов по таймеру (Tick). Аналог Level из CocoSharp.
    /// </summary>
    public class Level
    {
        public int Number { get; private set; }
        public string Description { get; private set; } = "";
        public int TotalEnemies { get; private set; }
        public bool AllSpawned => _queue.Count == 0;

        /// <summary>Запрос на спавн врага из таймлайна уровня.</summary>
        public struct SpawnInfo
        {
            public EnemyType Type;
            public bool Formation;  // спавнить в формацию (улей)
            public string Route;    // имя маршрута (null/пусто — без маршрута)
            public string After;    // поведение после маршрута: formation/scatter/bounce
        }

        private enum ActionKind { Spawn, SetInterval, Wait }
        private struct Action
        {
            public ActionKind Kind;
            public EnemyType Enemy;
            public bool Formation;
            public string Route;
            public string After;
            public float Value; // интервал или пауза
        }

        private readonly Queue<Action> _queue = new Queue<Action>();
        private float _interval = 1f;
        private float _timer;

        public static string LevelPath(int n) =>
            Path.Combine(AppContext.BaseDirectory, "Content", "Levels", $"level{n}.yaml");

        /// <summary>Сколько уровней доступно (по наличию файлов level1.yaml, level2.yaml, ...).</summary>
        public static int CountLevels()
        {
            int n = 0;
            while (File.Exists(LevelPath(n + 1)))
                n++;
            return n;
        }

        public bool Load(int number)
        {
            Number = number;
            _queue.Clear();
            _interval = 1f;
            _timer = 0f;
            TotalEnemies = 0;

            var data = Yaml.LoadFile<LevelYaml>(LevelPath(number));
            if (data == null)
                return false;

            Description = data.Description ?? "";
            if (data.SpawnInterval > 0)
                _interval = data.SpawnInterval;

            if (data.Events != null)
            {
                foreach (var ev in data.Events)
                {
                    if (!string.IsNullOrWhiteSpace(ev.Enemy))
                    {
                        int count = ev.Count > 0 ? ev.Count : 1;
                        EnemyType type = ParseEnemy(ev.Enemy);
                        for (int i = 0; i < count; i++)
                            _queue.Enqueue(new Action { Kind = ActionKind.Spawn, Enemy = type, Formation = ev.Formation, Route = ev.Route, After = ev.After });
                        TotalEnemies += count;
                    }
                    else if (ev.Interval.HasValue)
                    {
                        _queue.Enqueue(new Action { Kind = ActionKind.SetInterval, Value = ev.Interval.Value });
                    }
                    else if (ev.Wait.HasValue)
                    {
                        _queue.Enqueue(new Action { Kind = ActionKind.Wait, Value = ev.Wait.Value });
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Продвинуть таймлайн. Возвращает типы врагов, которых нужно заспавнить в этом кадре.
        /// </summary>
        public List<SpawnInfo> Tick(float dt)
        {
            var spawn = new List<SpawnInfo>();
            if (_queue.Count == 0)
                return spawn;

            _timer -= dt;
            while (_timer <= 0f && _queue.Count > 0)
            {
                Action a = _queue.Dequeue();
                switch (a.Kind)
                {
                    case ActionKind.SetInterval:
                        _interval = a.Value; // без задержки — сразу к следующему действию
                        break;
                    case ActionKind.Wait:
                        _timer += a.Value;
                        break;
                    case ActionKind.Spawn:
                        spawn.Add(new SpawnInfo { Type = a.Enemy, Formation = a.Formation, Route = a.Route, After = a.After });
                        _timer += _interval;
                        break;
                }
            }
            return spawn;
        }

        private static EnemyType ParseEnemy(string name)
        {
            switch (name.Trim().ToLowerInvariant())
            {
                case "blue": return EnemyType.BLUE;
                case "green": return EnemyType.GREEN;
                case "red": return EnemyType.RED;
                case "scout":
                case "smscout": return EnemyType.SM_SCOUT;
                case "boss": return EnemyType.BOSS;
                default: return EnemyType.SM_SCOUT;
            }
        }

        // POCO под level{N}.yaml
        private class LevelYaml
        {
            public string Description { get; set; }
            public float SpawnInterval { get; set; } = 1f;
            public List<EventYaml> Events { get; set; }
        }
        private class EventYaml
        {
            public string Enemy { get; set; }
            public int Count { get; set; }
            public bool Formation { get; set; }
            public string Route { get; set; }
            public string After { get; set; }
            public float? Interval { get; set; }
            public float? Wait { get; set; }
        }
    }
}
