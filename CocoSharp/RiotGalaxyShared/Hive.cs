using System;
using System.Collections.Generic;
using System.Text;
using CocosSharp;
using RiotGalaxy;
using RiotGalaxy.Objects;

namespace RiotGalaxy
{
    public class Cell
    {
        //public int x { get; set; }
        //public int y { get; set; }
        public CCPoint point;
        public GameObject Obj { get; set; }

        CCSprite sprite;
        public Cell()
        {
            //System.Diagnostics.Debug.WriteLine("=== Cell === ");
            point = new CCPoint();
            Obj = null;
        }
        public void TakeCell(GameObject newobj) // занять ячейку
        {
            Obj = newobj;
        }
        public void FreeCell() // освободить ячейку
        {
            Obj = null;
        } 
        public void Mark()
        {
            sprite = GameManager.sLoader.Load("red_cross.png");
            sprite.AnchorPoint = CCPoint.AnchorMiddle;
            sprite.Position = point;
            GameManager.ScGame.gameplayLayer.AddChild(sprite);
        }
    }
    public class Hive
    {
        /// <summary>
        /// Это улей, он состоит из ячеек (Cell), таких же по размеру как и мир (World)
        /// каждая (наверное) вражина, когда прилетает должна занять свое место в улье и тусить там до тех пор пока не решиться атокавать
        /// пока они в улье, они могут синхронно барражировать
        /// так же враги после атаки могут возвращаться на свое место
        /// </summary>
        int num_cells_x = 8;
        int num_cells_y = 2;
        Cell[,] cells;

        public Hive()
        {
            System.Diagnostics.Debug.WriteLine("=== Hive === ");
            cells = new Cell[num_cells_x, num_cells_y];
            
            //указываем левый нижний угол улья в координатах ячеек World
            int start_x = 5, start_y = 6;

            //сопоставляем ячейки улья ячейкам мира
            for (int y = 0; y < num_cells_y; y++)    //(int y = num_cells_y-1; y >= 0; y--)
            {
                for (int x = 0; x < num_cells_x; x++)   //for (int x = 0; x < num_cells_x; x++)
                {
                    //cells[x, y] = GameManager.gameplay.world.cells[start_x + x, start_y + y];
                    cells[x, y] = GameManager.gameplay.world.GetCell(start_x + x, start_y + y);
                    //System.Diagnostics.Debug.WriteLine("=== GetCell NULL === ");
                }
            }
        }
        public Cell GetCell(int cx, int cy)
        {
            return cells[cx, cy];
        }
        public Cell GetNextFreeCell() // запросить незанятую ячейку
        {
            for (int y = 0; y < num_cells_y; y++)
            {
                for (int x = 0; x < num_cells_x; x++)
                {
                    if (cells[x, y].Obj == null)
                        return cells[x, y];
                }
            }
            return null;
        }
        public void Marc()
        {
            for (int y = 0; y < num_cells_y; y++)
            {
                for (int x = 0; x < num_cells_x; x++) 
                {
                    cells[x, y].Mark();                    
                }
            }
        }
    }
}
