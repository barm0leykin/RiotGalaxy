using RiotGalaxy.Interface;
using RiotGalaxy.Objects;
using RiotGalaxy.Objects.ObjBehavior;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace RiotGalaxy
{
    /// <summary>
    /// Список событий. Основной с trigger_id == 0, остальные дополнительные, включаются по событию TRIGGER_ON
    /// списки с одинаковым trigger_id выполняются одновременно
    /// </summary>
    public class LvlEventList
    {        
        public int trigger_id;
        public int curEvent;
        public float spawnInterval = 1; // интервал вызова следующего события в секундах
        public float elapsedTime = 0;
        public Route current_route;     // текущий маршрут для юнитов
        public ArrayList list;
        public LvlEventList()
        { }
    }

    public class LvlEventDirector
    {
        private LvlEvent ev;

        ArrayList SyncCommands;
        ArrayList events;

        public LvlEventList curEvList;
        int curTriggerID = 0;   // Основной список == 0
        int curListLock = 0;    // счетчик блокировок. Если он == 0, значит списки с curTriggerID закончились и нужно вернуться в основной

        public LvlEventDirector()
        {
            SyncCommands = new ArrayList();

            curEvList = (LvlEventList)GameManager.level.AllEventsList[0];
            curEvList.spawnInterval = 1;
            curEvList.curEvent = 0;
            events = curEvList.list;
        }

        public void Update(float time)
        {
            // перебираем список списков событий
            for (int i = 0; i < GameManager.level.AllEventsList.Count; i++)
            {
                LvlEventList tmp = (LvlEventList)GameManager.level.AllEventsList[i];
                if (tmp.trigger_id == curTriggerID)   //обработка текущих списков
                {
                    curEvList = tmp;
                    events = curEvList.list;
                    HandleEvents(time);
                }
            }
        }
        public void HandleEvents(float time)
        {
            if (curEvList.curEvent >= curEvList.list.Count)
                return;
            curEvList.elapsedTime += time;
            //ev = (LvlEvent)curEvList.list[curEvList.curEvent];
            ev = (LvlEvent)events[curEvList.curEvent];

            switch (ev.evClass)
            {
                case (int)Level.LvlEventClass.COMMAND:
                    {
                        HandleCommands(); 
                        break;
                    }
                case (int)Level.LvlEventClass.ENEMY:
                    {
                        if (GameManager.level.enemySpawned >= GameManager.level.total_num_enimies)
                            return;
                        if (curEvList.elapsedTime < curEvList.spawnInterval) //задержка для боработки событий требующих задержки (спавн юнитов и тд)
                            return;
                        curEvList.elapsedTime = 0;

                        HandleEnemyes(); // спавним юнитов
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
            curEvList.curEvent++;
        }//HandleEvents

        void HandleCommands()
        {
            switch (ev.ev)  //сначала обработка событий не требующих паузы перед вызовом
            {
                case (int)Level.LvlEventCmdType.INTERVAL: // меняем интервал вызова следующего события
                    {
                        curEvList.spawnInterval = ev.value; 
                        break;
                    }
                case (int)Level.LvlEventCmdType.ROUTE: // изменился маршрут для новых юнитов
                    {
                        curEvList.current_route = new Route();
                        curEvList.current_route.LoadRoute(ev.valstr);
                        break;
                    }
                case (int)Level.LvlEventCmdType.CUT_SC:  // показываем мультики
                    {
                        CutScene cut = new CutScene();
                        cut.AddMsg(GameManager.player.pers, "Azaza!");
                        cut.ShowNextMessage();
                        break;
                    }

                case (int)Level.LvlEventCmdType.TRIGGER_ON: // триггер на выполнение одновременных действий
                    {
                        curTriggerID = (int)ev.value;
                        //считаем число списков с нужным ИД и ставим соотв колво блокировок
                        for (int i = 0; i < GameManager.level.AllEventsList.Count; i++)
                        {
                            LvlEventList tmp = (LvlEventList)GameManager.level.AllEventsList[i];
                            if (tmp.trigger_id == curTriggerID)   
                            {
                                curListLock++;
                            }
                        }                                
                        break;
                    }
                case (int)Level.LvlEventCmdType.TRIGGER_OFF: // выполнение одновременных действий завершено
                    {
                        curListLock--;
                        if (curListLock == 0)   // если больше нет блокировок возвращаемся в основной список событий
                            curTriggerID = 0;
                        GameManager.level.AllEventsList.Remove(curEvList);//этот список событий закончился - удаляем
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
        }

        void HandleEnemyes()
        {
            switch (ev.ev)
            {
                case (int)Level.LvlEventEnemyType.EN_SM_SCOUT:
                    {
                        GameManager.gameplay.SpawnEnemy(Enemy.EnemyType.SM_SCOUT);
                        break;
                    }
                case (int)Level.LvlEventEnemyType.EN_BLUE:
                    {
                        GameManager.gameplay.SpawnEnemy(Enemy.EnemyType.BLUE);
                        break;
                    }
                case (int)Level.LvlEventEnemyType.EN_GREEN:
                    {
                        GameManager.gameplay.SpawnEnemy(Enemy.EnemyType.GREEN);
                        break;
                    }
                case (int)Level.LvlEventEnemyType.EN_RED:
                    {
                        GameManager.gameplay.SpawnEnemy(Enemy.EnemyType.RED);
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
        }
    }//Class
   
}
