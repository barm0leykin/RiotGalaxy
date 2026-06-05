using Microsoft.Xna.Framework;

namespace RiotGalaxy.GameObjects
{
    /// <summary>
    /// Ячейка координатной сетки мира: позиция на экране и (опц.) занявший её объект.
    /// Аналог Cell из CocosSharp.
    /// </summary>
    public class Cell
    {
        public Vector2 Position;
        public GameObject Obj;
    }

    /// <summary>
    /// Координатная сетка игрового поля (аналог World из CocoSharp).
    /// Матрица ячеек фиксированного размера, отцентрированная на экране —
    /// используется для расстановки врагов в формациях (Hive).
    /// </summary>
    public class World
    {
        public int CellSize { get; } = 90;
        public int NumX { get; } = 16;
        public int NumY { get; } = 10;

        private readonly Cell[,] _cells;

        public World(int screenWidth, int screenHeight)
        {
            _cells = new Cell[NumX, NumY];

            // Центрируем сетку на экране (как в оригинале)
            int shiftX = (screenWidth - NumX * CellSize) / 2;
            int shiftY = (screenHeight - NumY * CellSize) / 2;

            for (int y = 0; y < NumY; y++)
            {
                for (int x = 0; x < NumX; x++)
                {
                    _cells[x, y] = new Cell
                    {
                        Position = new Vector2(shiftX + x * CellSize, shiftY + y * CellSize)
                    };
                }
            }
        }

        public Cell GetCell(int cx, int cy) => _cells[Clamp(cx, NumX), Clamp(cy, NumY)];

        public Vector2 GetCellPosition(int cx, int cy) => GetCell(cx, cy).Position;

        private static int Clamp(int v, int max) => v < 0 ? 0 : (v >= max ? max - 1 : v);
    }
}
