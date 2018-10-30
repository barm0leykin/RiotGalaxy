using CocosSharp;
using System.Collections;

namespace RiotGalaxy.Interface
{
    /// <summary>
    /// Класс будет содержать все элементы интерфейса в одном месте (кнопки и тд)
    /// слово Interface было занято, поэтому HUD :)
    /// </summary>
    public class HUD
    {
        ArrayList buttons;
        Console console;
        //int num_cons_msg = 3;
        bool debug_msg_to_cons = false;
        public enum MsgType : int { DEBUG = 0, GAME_CONSOL, MESSAGE }

        public HUD()
        {
            buttons = new ArrayList();
            console = new Console();
        }

        public void AddButton(MyButton btn)
        {
            buttons.Add(btn);
            GameManager.ScGame.hudLayer.AddChild(btn);
            GameManager.gameplay.userInputHandler.AddButnHandler(btn);
        }
        public void DelButton(MyButton btn)
        {
            if (btn == null)
                return;
            buttons.Remove(btn);    // удаляем из списка
            btn.RemoveFromParent(); // удаляем со сцены
            GameManager.gameplay.userInputHandler.DelBtnHandler(btn);   //удаляем хэндлер ввода
            btn = null;
        }
        public MyButton FindByName(string name)
        {
            foreach (MyButton btn in buttons)
            {
                if (btn.name == name)
                    return btn;
            }
            return null;
        }

        public void Message(string msg, int type = 0)
        {
            switch (type)
            {
                case (int)MsgType.DEBUG:
                    {
                        System.Diagnostics.Debug.WriteLine(msg);
                        if (debug_msg_to_cons)
                            Message(msg, (int)MsgType.GAME_CONSOL);
                        break;
                    }
                case (int)MsgType.GAME_CONSOL:
                    {
                        //System.Diagnostics.Debug.WriteLine(msg);
                        console.AddConsoleMsg(msg);
                        break;
                    }
                case (int)MsgType.MESSAGE:
                    {
                        GameManager.gameplay.Message(msg, 500);
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
        }
    } // class HUD
}
