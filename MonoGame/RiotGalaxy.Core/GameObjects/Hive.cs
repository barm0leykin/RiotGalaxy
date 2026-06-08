using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace RiotGalaxy.GameObjects
{
    /// <summary>
    /// Улей — формация поверх ячеек World. Враги занимают свои ячейки и синхронно
    /// барражируют (весь улей плавно качается из стороны в сторону). Аналог Hive из CocoSharp.
    ///
    /// Дополнительно умеет «вылеты» (sortie, Galaga-стиль): периодически отправляет
    /// осевших юнитов в пике-атаку; отпикировав, юнит возвращается в свою ячейку
    /// (см. <see cref="Components.SortieMovement"/>). Включается из описания уровня.
    /// </summary>
    public class Hive
    {
        private readonly World _world;
        private readonly int _startX, _startY, _cols, _rows;
        private readonly bool[,] _taken;

        // Члены улья и их ячейки (для координации вылетов)
        private sealed class Member
        {
            public Enemy Enemy;
            public int Cx, Cy;
            public bool OnSortie; // сейчас в вылете (не трогаем до возврата)
        }
        private readonly List<Member> _members = new List<Member>();

        // Параметры вылетов
        private bool _sortieEnabled;
        private float _sortieInterval = 4f;
        private int _sortieCount = 1;
        private float _sortieTimer;
        private static readonly Random _rnd = new Random();

        // Дрейф всего улья (синхронное барражирование)
        public Vector2 Offset { get; private set; }
        private float _time;
        private const float SwayAmplitude = 70f; // пикселей по X
        private const float SwaySpeed = 1.0f;     // рад/сек

        public Hive(World world, int startX, int startY, int cols, int rows)
        {
            _world = world;
            _startX = startX;
            _startY = startY;
            _cols = cols;
            _rows = rows;
            _taken = new bool[cols, rows];
        }

        /// <summary>Включить вылеты из улья: раз в interval секунд по count юнитов.</summary>
        public void EnableSortie(float interval, int count)
        {
            _sortieEnabled = true;
            _sortieInterval = interval > 0f ? interval : 4f;
            _sortieCount = count > 0 ? count : 1;
            _sortieTimer = 0f;
        }

        /// <summary>Зарегистрировать врага как члена улья (его ячейка cx,cy).</summary>
        public void Register(Enemy enemy, int cx, int cy)
        {
            _members.Add(new Member { Enemy = enemy, Cx = cx, Cy = cy });
        }

        /// <summary>Враг вернулся в строй после вылета — снова доступен для следующих вылетов.</summary>
        public void NotifyReturned(Enemy enemy)
        {
            var m = _members.Find(x => x.Enemy == enemy);
            if (m != null) m.OnSortie = false;
        }

        public void Update(float dt)
        {
            _time += dt;
            Offset = new Vector2((float)Math.Sin(_time * SwaySpeed) * SwayAmplitude, 0f);

            // Убираем погибших из учёта
            _members.RemoveAll(m => m.Enemy == null || !m.Enemy.IsAlive);

            if (!_sortieEnabled || _members.Count == 0)
                return;

            _sortieTimer += dt;
            if (_sortieTimer >= _sortieInterval)
            {
                _sortieTimer = 0f;
                LaunchSortie();
            }
        }

        /// <summary>Отправить в вылет до _sortieCount осевших в улье юнитов (случайно).</summary>
        private void LaunchSortie()
        {
            // Кандидаты: в строю, живые, уже «осевшие» возле своей ячейки
            var ready = new List<Member>();
            foreach (var m in _members)
            {
                if (m.OnSortie || m.Enemy == null || !m.Enemy.IsAlive)
                    continue;
                if (Vector2.Distance(m.Enemy.Position, CellWorldPos(m.Cx, m.Cy)) < 50f)
                    ready.Add(m);
            }

            int n = Math.Min(_sortieCount, ready.Count);
            for (int i = 0; i < n; i++)
            {
                int idx = _rnd.Next(ready.Count);
                var m = ready[idx];
                ready.RemoveAt(idx);
                m.OnSortie = true;
                m.Enemy.StartSortie(this, m.Cx, m.Cy);
            }
        }

        /// <summary>Занять следующую свободную ячейку. false — улей заполнен.</summary>
        public bool TryTakeCell(out int cx, out int cy)
        {
            for (int y = 0; y < _rows; y++)
            {
                for (int x = 0; x < _cols; x++)
                {
                    if (!_taken[x, y])
                    {
                        _taken[x, y] = true;
                        cx = x; cy = y;
                        return true;
                    }
                }
            }
            cx = cy = -1;
            return false;
        }

        /// <summary>Мировая позиция ячейки улья с учётом текущего дрейфа.</summary>
        public Vector2 CellWorldPos(int cx, int cy) =>
            _world.GetCellPosition(_startX + cx, _startY + cy) + Offset;
    }
}
