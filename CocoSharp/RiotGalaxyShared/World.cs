using System;
using System.Collections.Generic;
using System.Text;
using CocosSharp;

namespace RiotGalaxy
{
    public class World
    {
        public int Cell_size { get; }
        public int num_cells_x { get; }
        public int num_cells_y  { get; }
        private Cell[,] cells;

        public World()
        {
            Cell_size = 90;
            num_cells_x = 16;
            num_cells_y = 10;
            System.Diagnostics.Debug.WriteLine("=== World === ");

            cells = new Cell[num_cells_x, num_cells_y];            

            //небольшие вычисления для центровки на экране
            int cell_x_shift = (Options.width - num_cells_x * Cell_size) / 2;
            int cell_y_shift = (Options.height - num_cells_y * Cell_size) / 2;
            
            int cell_y_coord = cell_y_shift;
            for (int y = 0; y < num_cells_y; y++)
            {
                int cell_x_coord = cell_x_shift;
                for (int x = 0; x < num_cells_x; x++)
                {
                    cells[x, y] = new Cell();
                    cells[x, y].point.X = cell_x_coord;
                    cells[x, y].point.Y = cell_y_coord;

                    cell_x_coord += Cell_size;
                    //System.Diagnostics.Debug.WriteLine("=== World fill cells === ");
                }
                cell_y_coord += Cell_size;
            }
        }
        public Cell GetCell(int cx, int cy)
        {
            return cells[cx, cy];
            /*if (cells[cx, cy] != null)
                return cells[cx, cy];
            else return null;*/
        }
        public CCPoint GetCellPoint(int cx, int cy)
        {
            //Городим велосипед на костылях для отрицателдьных значений
            /*CCPoint point;
            if (cx < 0)
            {                
                point.X = 0 - Cell_size;
            }else
            if (cx > num_cells_x)
            {
                point.X = num_cells_x * Cell_size + Cell_size;
            }

            if (cy < 0)
            {
                point.Y = 0 - Cell_size;
            }
            else
            if (cy > num_cells_y)
            {
                point.Y = num_cells_y * Cell_size + Cell_size;
            }
            return point;*/
            return cells[cx, cy].point;
        }

    }
}
