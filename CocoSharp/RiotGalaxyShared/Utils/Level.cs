using Microsoft.Xna.Framework;
using System;
using System.IO;
using System.Collections;
//using RiotGalaxyAndroid;
using RiotGalaxy;

namespace RiotGalaxy
{
    public struct LvlEvent
    {
        public int evClass;
        public int ev;
        public float value;
        public string valstr;
    }

    public class Level
    {
        public enum LvlEventClass : int { COMMAND = 0, ENEMY, BONUS }
        public enum LvlEventEnemyType : int {  EN_SM_SCOUT, EN_BLUE, EN_GREEN, EN_RED }
        public enum LvlEventCmdType : int { WAIT = 0, INTERVAL, ROUTE, END, TRIGGER_ON, TRIGGER_OFF, CUT_SC }        

        ArrayList CurrentEventsList;    // основной (0) список событий
        public ArrayList AllEventsList; // список списков событий

        public int cur_level_num = 1;
        public int totalNumberOfLevels;

        public string levelDescription = "";
        public int total_num_enimies = 1;
        public int enemySpawned =0;
        public int enemyKilled =0;
        public int enemyRemain;

        public Level()
        {            
            DetermineAvailableLevels(); //определяем сколько всего файлов с уровнями
        }
        public bool LoadLevel(int level_num)
        {
            cur_level_num = level_num;  
            enemySpawned = 0;   
            enemyKilled = 0; 

            AllEventsList = new ArrayList();

            LvlEventList mainEvList = new LvlEventList();
            mainEvList.trigger_id = 0;
            mainEvList.curEvent = 0;
            mainEvList.list = new ArrayList();
            
            AllEventsList.Add(mainEvList);
                        
            CurrentEventsList = mainEvList.list;

            string filename = "Content/Levels/level" + level_num + ".txt";
            if (GameManager.fUtils.OpenTextFile(filename))
            {
                ArrayList file = GameManager.fUtils.GetFileContent();
                foreach (object str in file)
                {                    
                    ParceLine(str.ToString());
                }

                DetermineTotalEnemies(); // Определяем сколько на уровне врагов                            
                enemyRemain = total_num_enimies;
                return true; // Уровень успешно загружен
            }else
                return false;            
        }
        private void ParceLine(string line)
        {
            //bool sync_cmd_now = false;
            int i = 0;
            LvlEvent newEv;
            newEv.evClass = 0;
            newEv.ev = 0;
            newEv.value = 0;
            newEv.valstr = "";

            if (line.Contains("#")) // комментарий обрезаем
            {
                line = line.Remove(line.IndexOf("#"));
                if (line.Length == 0) //если в строке кроме комментария ничего не было, то выходим
                    return;
            }

            if (line.Contains("description:"))
            {
                line = line.Substring(line.IndexOf(":") + 1);
                levelDescription = line; //(int)tmpline;
                System.Diagnostics.Debug.WriteLine("Parce: levelDescription: " + levelDescription);
            }
            else if (line.Contains("<sync_cmd:"))
            {
                LvlEventList syncCmd = new LvlEventList();
                line = line.Substring(line.IndexOf(":") + 1);
                line = line.Trim('>');

                LvlEventList PrevEvList = (LvlEventList)GameManager.level.AllEventsList[0];//скопируем из основного листа дефолтные настройки
                syncCmd.trigger_id = Int32.Parse(line);
                syncCmd.curEvent = 0;
                syncCmd.spawnInterval = 1; // PrevEvList.spawnInterval;
                syncCmd.current_route = PrevEvList.current_route;
                syncCmd.list = new ArrayList();
                AllEventsList.Add(syncCmd);

                CurrentEventsList = syncCmd.list;
            }
            else if (line.Contains("</sync_cmd>"))
            {
                newEv.evClass = (int)LvlEventClass.COMMAND;
                newEv.ev = (int)LvlEventCmdType.TRIGGER_OFF;
                CurrentEventsList.Add(newEv); ////////////

                // делаем основной список событий [0] снова текущим
                LvlEventList t = (LvlEventList)AllEventsList[0];
                CurrentEventsList = t.list;//(EventList)t.list;
            }
            else if (line.Contains("trigger:"))
            {
                line = line.Substring(line.IndexOf(":") + 1);
                newEv.evClass = (int)LvlEventClass.COMMAND;
                newEv.ev = (int)LvlEventCmdType.TRIGGER_ON;
                newEv.value = Int32.Parse(line);//Convert.ToSingle(line);
                System.Diagnostics.Debug.WriteLine("trigger: " + newEv.value);
                CurrentEventsList.Add(newEv);
            }
            else if (line.Contains("script:"))
            {
                line = line.Substring(line.IndexOf(":") + 1);
                line = line.Trim();
                newEv.evClass = (int)LvlEventClass.COMMAND;
                newEv.ev = (int)LvlEventCmdType.ROUTE;
                System.Diagnostics.Debug.WriteLine("script: " + line);
                newEv.valstr = line;
                CurrentEventsList.Add(newEv);
            }
            else if (line.Contains("cut_sc:"))
            {
                line = line.Substring(line.IndexOf(":") + 1);
                line = line.Trim();
                newEv.evClass = (int)LvlEventClass.COMMAND;
                newEv.ev = (int)LvlEventCmdType.CUT_SC;
                System.Diagnostics.Debug.WriteLine("cut_sc: " + line);
                newEv.value = float.Parse(line);
                CurrentEventsList.Add(newEv);
            }

            else if (line.Contains("wait:"))
            {
                line = line.Substring(line.IndexOf(":") + 1);
                System.Diagnostics.Debug.WriteLine("Parce: wait: " + line);
                newEv.evClass = (int)LvlEventClass.COMMAND;
                newEv.ev = (int)LvlEventCmdType.WAIT;
                newEv.value = float.Parse(line);                
                CurrentEventsList.Add(newEv);
            }
            else if (line.Contains("interval:"))
            {
                line = line.Substring(line.IndexOf(":") + 1);
                newEv.evClass = (int)LvlEventClass.COMMAND;
                newEv.ev = (int)LvlEventCmdType.INTERVAL;
                newEv.value = float.Parse(line);//Convert.ToSingle(line);
                System.Diagnostics.Debug.WriteLine("interval value: " + newEv.value);
                CurrentEventsList.Add(newEv);
            }

            else if (line.Contains("enemysmscout:"))
            {
                line = line.Substring(line.IndexOf(":") + 1);
                System.Diagnostics.Debug.WriteLine("Parce: enemysmscout: " + line);
                newEv.evClass = (int)LvlEventClass.ENEMY;
                newEv.ev = (int)LvlEventEnemyType.EN_SM_SCOUT;
                int num = Int32.Parse(line);
                while (i < num)
                {
                    CurrentEventsList.Add(newEv);
                    i++;
                }
            }
            else if (line.Contains("enemyblue:"))
            {
                line = line.Substring(line.IndexOf(":") + 1);
                System.Diagnostics.Debug.WriteLine("Parce: enemyblue: " + line);
                newEv.evClass = (int)LvlEventClass.ENEMY;
                newEv.ev = (int)LvlEventEnemyType.EN_BLUE;
                int num = Int32.Parse(line);
                while (i < num)
                {
                    CurrentEventsList.Add(newEv);
                    i++;
                }
            }
            else if (line.Contains("enemygreen:"))
            {
                line = line.Substring(line.IndexOf(":") + 1);
                System.Diagnostics.Debug.WriteLine("Parce: enemygreen: " + line);
                newEv.evClass = (int)LvlEventClass.ENEMY;
                newEv.ev = (int)LvlEventEnemyType.EN_GREEN;
                int num = Int32.Parse(line);
                while (i < num)
                {
                    CurrentEventsList.Add(newEv);
                    i++;
                }
            }
            else if (line.Contains("enemyred:"))
            {
                line = line.Substring(line.IndexOf(":") + 1);
                System.Diagnostics.Debug.WriteLine("Parce: enemyred: " + line);

                newEv.evClass = (int)LvlEventClass.ENEMY;
                newEv.ev = (int)LvlEventEnemyType.EN_RED;
                int num = Int32.Parse(line);
                while(i < num)
                {
                    CurrentEventsList.Add(newEv);
                    i ++;
                }                
            }
            /*else if (line.Contains("endlevel:"))
            {
                //line = line.Substring(line.IndexOf(":") + 1);
                newEv.evClass = (int)LvlEventClass.EVENT;
                newEv.ev = (int)LvlEventType.END;
                System.Diagnostics.Debug.WriteLine("The end");
                events.Add(newEv);
            }*/
        }
        private int DetermineTotalEnemies()
        {
            total_num_enimies = 0;

            for (int i = 0; i < AllEventsList.Count; i++)//смотрим во всех списках списков :)
            {
                LvlEventList t = (LvlEventList)AllEventsList[i];
                ArrayList events = t.list;

                foreach (LvlEvent indexEv in events)
                {
                    if (indexEv.evClass == (int)LvlEventClass.ENEMY)
                    {
                        total_num_enimies++; //total_num_enimies += (int)indexEv.value;
                    }
                }
            }
            System.Diagnostics.Debug.WriteLine("total_num_enimies: " + total_num_enimies);
            return total_num_enimies;
        }
        private void DetermineAvailableLevels()
        {
            // This game relies on levels being named "levelx.tmx" where x is an integer beginning with
            // 1. We have to rely on MonoGame's TitleContainer which doesn't give us a GetFiles method - we simply
            // have to check if a file exists, and if we get an exception on the call then we know the file doesn't
            // exist. 
            totalNumberOfLevels = 1;
            while (true)
            {
                bool fileExists = false;
                try
                {
                    using (var stream = TitleContainer.OpenStream("Content/Levels/level" + totalNumberOfLevels + ".txt"))
                    {
                    }
                    // if we got here then the file exists!
                    fileExists = true;
                }
                catch
                {
                    // do nothing, fileExists will remain false
                }
                if (!fileExists)
                {
                    break;
                }
                else
                {
                    totalNumberOfLevels++;
                }
            }
            totalNumberOfLevels--;//
        }
    }//class
}//namespace
 