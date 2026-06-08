using System;
using System.Collections.Generic;
using RiotGalaxy.GameObjects;

namespace RiotGalaxy.Utils
{
    /// <summary>
    /// Уровень: загружается из Content/Levels/level{N}.yaml, разворачивает события в таймлайн
    /// и выдаёт врагов по таймеру (Tick). Поддерживает параллельные волны (событие `parallel`):
    /// несколько групп спавнятся одновременно, основной таймлайн ждёт их завершения.
    /// Аналог Level + LvlEventDirector (trigger/sync_cmd) из CocoSharp.
    /// </summary>
    public class Level
    {
        public int Number { get; private set; }
        public string Description { get; private set; } = "";
        public int TotalEnemies { get; private set; }
        public bool AllSpawned => (_main == null || _main.Done) && _active.Count == 0;

        // Вылеты из улья (Galaga): включаются на уровне, темп — в YAML
        public bool Sortie { get; private set; }
        public float SortieInterval { get; private set; } = 4f;
        public int SortieCount { get; private set; } = 1;

        /// <summary>Запрос на спавн врага из таймлайна уровня.</summary>
        public struct SpawnInfo
        {
            public EnemyType Type;
            public bool Formation;  // спавнить в формацию (улей)
            public string Route;    // имя маршрута (null/пусто — без маршрута)
            public string After;    // поведение после маршрута: formation/scatter/bounce
        }

        private enum ActionKind { Spawn, SetInterval, Wait, Parallel }

        private class Action
        {
            public ActionKind Kind;
            public EnemyType Enemy;
            public bool Formation;
            public string Route;
            public string After;
            public float Value;                 // интервал или пауза
            public List<List<Action>> Groups;   // для Parallel: группы под-таймлайнов
        }

        /// <summary>Под-таймлайн: своя очередь действий, свой интервал и таймер.</summary>
        private class Timeline
        {
            public readonly Queue<Action> Queue;
            public float Interval;
            public float Timer;
            public Timeline(IEnumerable<Action> actions, float interval)
            {
                Queue = new Queue<Action>(actions);
                Interval = interval;
            }
            public bool Done => Queue.Count == 0;
        }

        private Timeline _main;
        private readonly List<Timeline> _active = new List<Timeline>(); // активные параллельные волны
        private float _defaultInterval = 1f;

        public static string LevelAsset(int n) => $"Content/Levels/level{n}.yaml";

        /// <summary>Сколько уровней доступно (по наличию файлов level1.yaml, level2.yaml, ...).</summary>
        public static int CountLevels()
        {
            int n = 0;
            while (Yaml.AssetExists(LevelAsset(n + 1)))
                n++;
            return n;
        }

        public bool Load(int number)
        {
            Number = number;
            _active.Clear();
            _main = null;
            TotalEnemies = 0;

            var data = Yaml.LoadAsset<LevelYaml>(LevelAsset(number));
            if (data == null)
                return false;

            Description = data.Description ?? "";
            _defaultInterval = data.SpawnInterval > 0 ? data.SpawnInterval : 1f;

            Sortie = data.Sortie;
            SortieInterval = data.SortieInterval > 0 ? data.SortieInterval : 4f;
            SortieCount = data.SortieCount > 0 ? data.SortieCount : 1;

            int total = 0;
            var actions = BuildActions(data.Events, ref total);
            TotalEnemies = total;
            _main = new Timeline(actions, _defaultInterval);
            return true;
        }

        private List<Action> BuildActions(List<EventYaml> events, ref int total)
        {
            var list = new List<Action>();
            if (events == null)
                return list;

            foreach (var ev in events)
            {
                if (ev.Parallel != null && ev.Parallel.Count > 0)
                {
                    var groups = new List<List<Action>>();
                    foreach (var group in ev.Parallel)
                        groups.Add(BuildActions(group, ref total));
                    list.Add(new Action { Kind = ActionKind.Parallel, Groups = groups });
                }
                else if (!string.IsNullOrWhiteSpace(ev.Enemy))
                {
                    int count = ev.Count > 0 ? ev.Count : 1;
                    EnemyType type = ParseEnemy(ev.Enemy);
                    for (int i = 0; i < count; i++)
                        list.Add(new Action { Kind = ActionKind.Spawn, Enemy = type, Formation = ev.Formation, Route = ev.Route, After = ev.After });
                    total += count;
                }
                else if (ev.Interval.HasValue)
                {
                    list.Add(new Action { Kind = ActionKind.SetInterval, Value = ev.Interval.Value });
                }
                else if (ev.Wait.HasValue)
                {
                    list.Add(new Action { Kind = ActionKind.Wait, Value = ev.Wait.Value });
                }
            }
            return list;
        }

        /// <summary>Продвинуть таймлайн. Возвращает врагов на спавн в этом кадре.</summary>
        public List<SpawnInfo> Tick(float dt)
        {
            var output = new List<SpawnInfo>();

            // Пока идут параллельные волны — основной таймлайн ждёт
            if (_active.Count > 0)
            {
                foreach (var t in _active)
                    AdvanceSegment(t, dt, output);
                _active.RemoveAll(t => t.Done);
                return output;
            }

            if (_main != null)
                AdvanceMain(dt, output);
            return output;
        }

        private void AdvanceMain(float dt, List<SpawnInfo> output)
        {
            if (_main.Queue.Count == 0)
                return;

            _main.Timer -= dt;
            while (_main.Timer <= 0f && _main.Queue.Count > 0)
            {
                Action a = _main.Queue.Dequeue();
                if (a.Kind == ActionKind.Parallel)
                {
                    foreach (var group in a.Groups)
                        _active.Add(new Timeline(group, _main.Interval));
                    _main.Timer = 0f; // основной продолжится после завершения параллельных
                    break;
                }
                ApplyAction(_main, a, output);
            }
        }

        private static void AdvanceSegment(Timeline t, float dt, List<SpawnInfo> output)
        {
            if (t.Queue.Count == 0)
                return;
            t.Timer -= dt;
            while (t.Timer <= 0f && t.Queue.Count > 0)
                ApplyAction(t, t.Queue.Dequeue(), output);
        }

        private static void ApplyAction(Timeline t, Action a, List<SpawnInfo> output)
        {
            switch (a.Kind)
            {
                case ActionKind.SetInterval:
                    t.Interval = a.Value;
                    break;
                case ActionKind.Wait:
                    t.Timer += a.Value;
                    break;
                case ActionKind.Spawn:
                    output.Add(new SpawnInfo { Type = a.Enemy, Formation = a.Formation, Route = a.Route, After = a.After });
                    t.Timer += t.Interval;
                    break;
            }
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
            // Вылеты из улья (Galaga): sortie/sortieInterval/sortieCount
            public bool Sortie { get; set; }
            public float SortieInterval { get; set; } = 4f;
            public int SortieCount { get; set; } = 1;
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
            public List<List<EventYaml>> Parallel { get; set; } // параллельные группы
        }
    }
}
