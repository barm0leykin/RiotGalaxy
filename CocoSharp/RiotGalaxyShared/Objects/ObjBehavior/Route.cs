using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using CocosSharp;
using RiotGalaxy;
using RiotGalaxy.Utils;

namespace RiotGalaxy.Objects.ObjBehavior
{
    public struct RoutePoint
    {
        public CCPoint point;
        public int action;
    }
    public class Route : ICloneable
    {
        int cur_point = 0;
        int total_points = 0;
        public List<RoutePoint> r_points;
        //private FileUtils fUtils;
        public Route()
        {
            r_points = new List<RoutePoint>();
        }
        public void AddPoint(CCPoint point, int action = 0)
        {
            RoutePoint p;
            p.point = point;
            p.action = action;
            r_points.Add(p);
            total_points++;
        }
        public void AddPointWorld(int wx, int wy, int action = 0)
        {
            CCPoint point;
            if (wx < 0)
            {
                point = GameManager.gameplay.world.GetCellPoint(0, wy);
                point.X = 0 -GameManager.gameplay.world.Cell_size;
            }
            if (wx > GameManager.gameplay.world.num_cells_x)
            {
                point = GameManager.gameplay.world.GetCellPoint(0, wy);
                point.X = GameManager.gameplay.world.num_cells_x * GameManager.gameplay.world.Cell_size + GameManager.gameplay.world.Cell_size;//ширина +1
            }
            point = GameManager.gameplay.world.GetCellPoint(wx, wy);
            AddPoint(point, action);
        }


        public CCPoint GetPoint(int i)
        {
            if (i < total_points)
                return r_points[i].point;
            else
                return r_points[total_points - 1].point; //если не получается возвращаем крайнюю точку
        }
        public CCPoint GetCurrentPoint()
        {
            return r_points[cur_point].point;
        }
        public CCPoint GetNextPoint()
        {
            if (!IsLast())
            {
                cur_point++;
                return r_points[cur_point].point;
            }
            return GetLast();
        }
        public CCPoint GetFirst()
        {
            return r_points[0].point;
        }
        public CCPoint GetLast()
        {
            return r_points[total_points - 1].point;
        }
        public bool IsLast()
        {
            if (cur_point == total_points - 1)
                return true;
            else return false;
        }
        public bool LoadRoute(string routename)
        {
            string filename = "Content/Scripts/" + routename + ".txt";
            if (GameManager.fUtils.OpenTextFile(filename))
            {
                ArrayList file = GameManager.fUtils.GetFileContent();
                foreach (object str in file)
                {
                    ParceLine(str.ToString());
                }
                return true; // Маршрут загружен
            }
            else
                return false;

        }
        private void ParceLine(string line)
        {
            int x, y;
            if (line.Contains("point:"))
            {
                line = line.Substring(line.IndexOf(":") + 1);
                System.Diagnostics.Debug.WriteLine("Parce: point:" + line);

                var stringArray = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                string str = stringArray[0];
                /*if (str.Contains("L") || str.Contains("R") || str.Contains("T") || str.Contains("D"))
                {
                    x = ParceWord(str);
                }else*/
                    x = Int32.Parse(stringArray[0]);

                /*str = stringArray[1];
                if (str.Contains("L") || str.Contains("R") || str.Contains("T") || str.Contains("D"))
                {
                    y = ParceWord(str);
                }else*/
                    y = Int32.Parse(stringArray[1]);
                
                System.Diagnostics.Debug.WriteLine("x: " + x + " y: " + y);                

                AddPointWorld(x, y);
            }
        }
        private int ParceWord(string str)
        {
            int res = 0;
            return res;
        }
        public object Clone()
        {
            Route copy = new Route();//(Route)this.MemberwiseClone();

            copy.r_points = new List<RoutePoint>(r_points.Count);
            for (int i=0; i < r_points.Count; i++)
            {
                RoutePoint copy_point = new RoutePoint();
                copy_point.point.X = r_points[i].point.X;
                copy_point.point.Y = r_points[i].point.Y;
                copy_point.action = r_points[i].action;

                copy.AddPoint(copy_point.point, copy_point.action); /// #$@!
                //copy.r_points[i] = copy_point;     //copy.r_points[i].point = r_points[i].point;
            }

            //copy.r_points = System.Copy(copy.r_points, copy.r_points);
            //r_points.CopyTo(copy.r_points);
            return copy;
        }
        public static T DeepClone<T>(T obj)
        {
            T objResult;
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(ms, obj);
                ms.Position = 0;
                objResult = (T)bf.Deserialize(ms);
            }
            return objResult;
        }
    }
}
