using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace RiotGalaxy
{
    public delegate void GameEventHandler();

    public class GameEventDirector
    {
        public enum EventsID : int { HP_UPD = 0, SCORE_UPD, G_PAUSE, G_RESUME }
        ArrayList EvList;
        //ArrayList Subscribers;
        
        //GameEventHandler _del;
        //GameEventHandler points_upd;

        public event GameEventHandler PointsUpd;//?
        public event GameEventHandler HpUpd;
        public event GameEventHandler GamePause, GameResume;

        public GameEventDirector()
        {
            EvList = new ArrayList();
            //Subscribers = new ArrayList();
        }
        // Регистрируем делегат
        /*public void RegisterHandler(GameEventHandler del)
        {
            _del += del; // добавляем делегат
        }
        // Отмена регистрации делегата
        public void UnregisterHandler(GameEventHandler del)
        {
            _del -= del; // удаляем делегат
        }

        //test
        public void RegisterScoreHandler(GameEventHandler del)
        {
            points_upd += del; // добавляем делегат
        }*/

        public void AddEvent(int id)
        {
            EvList.Add(id);
        }
        public void Update()
        {
            foreach(int i in EvList)
            {
                switch (i)
                {
                    case (int)EventsID.HP_UPD:
                        {
                            HpUpd?.Invoke();
                            break;
                        }
                    case (int)EventsID.SCORE_UPD:
                        {
                            PointsUpd?.Invoke();
                            break;
                        }
                    case (int)EventsID.G_PAUSE:
                        {
                            GamePause?.Invoke();
                            break;
                        }
                    case (int)EventsID.G_RESUME:
                        {
                            GameResume?.Invoke();
                            break;
                        }
                    default:
                        break;
                }
            }
            EvList.Clear();
        }//Update()
    }
}
