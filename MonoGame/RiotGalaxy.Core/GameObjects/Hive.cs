using System;
using Microsoft.Xna.Framework;

namespace RiotGalaxy.GameObjects
{
    /// <summary>
    /// Улей — формация поверх ячеек World. Враги занимают свои ячейки и синхронно
    /// барражируют (весь улей плавно качается из стороны в сторону). Аналог Hive из CocoSharp.
    /// </summary>
    public class Hive
    {
        private readonly World _world;
        private readonly int _startX, _startY, _cols, _rows;
        private readonly bool[,] _taken;

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

        public void Update(float dt)
        {
            _time += dt;
            Offset = new Vector2((float)Math.Sin(_time * SwaySpeed) * SwayAmplitude, 0f);
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
