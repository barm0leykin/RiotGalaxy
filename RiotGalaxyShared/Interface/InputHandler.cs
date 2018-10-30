using RiotGalaxy.Objects;
using CocosSharp;
using System.Collections.Generic;
using static RiotGalaxy.Objects.GameObject;
using RiotGalaxy.Commands;

namespace RiotGalaxy.Interface
{
    public class InputHandler
    {
        //UnitCommand command;
        private bool isTouch = false;
        private bool isTouchBegan = false;
        CCPoint locationOnScreen;
        public CCEventListenerTouchAllAtOnce touchListener;

        public List<MyButton> GuiButtons;
        public InputHandler()
        {
            GuiButtons = new List<MyButton>();
            CreateTouchListener();
        }
        public bool HandlePressButtons(CCPoint touchPoint)
        {
            //System.Diagnostics.Debug.WriteLine("=== HandleUserInput === X: " + touchPoint.X +" Y: " + touchPoint.Y);
            foreach (MyButton b in GuiButtons)
            {
                if (b == null)
                    continue;
                if (b.CheckCollision(touchPoint))
                {
                    //System.Diagnostics.Debug.WriteLine("=== Press Button ===");
                    b.Press();
                    return true;
                }
            }
            return false;
        }//HandleUserInput()

        public void AddButnHandler(MyButton btn)
        {
            System.Diagnostics.Debug.WriteLine("=== AddButton ===");
            GuiButtons.Add(btn);
        }
        public void DelBtnHandler(MyButton btn)
        {
            GuiButtons.Remove(btn);
        }


        /// <summary>
        /// 
        /// </summary>

        public void HandleScGameInput()
        {
            if (isTouchBegan)   // это первое нажатие?
            {
                isTouchBegan = false;

                if (!GameDelegate.gameManager.userInputHandler.HandlePressButtons(locationOnScreen))   // есть ли касание элементов интерфейса?
                {
                    if (GameManager.gameplay.Pause == true) // если нет, то мб игра на паузе?
                    {
                        //GameManager.gameplay.Pause = false; ////!!!!
                        CommandPauseWeaponMenu cmd;
                        cmd = new CommandPauseWeaponMenu();
                        cmd.Execute();
                        //GameManager.gameplay.Pause = false;
                    }
                    GameManager.gameplay.playerShip.move.SetMoveDirection(locationOnScreen);    // двигаем playerShip
                }
            }
            else if (isTouch)   // нажатие не первое, а касание продолжается - управляем кораблем
            {
                GameManager.gameplay.playerShip.move.SetMoveDirection(locationOnScreen);    // двигаем playerShip            
            }
            else
                GameManager.gameplay.playerShip.move.SetNoMove();   // касаний экрана нет, ускорение кораблю больше не предаем
        }

        public void CreateTouchListener()
        {
            touchListener = new CCEventListenerTouchAllAtOnce
            {
                OnTouchesBegan = HandleTouchesBegan,
                OnTouchesMoved = HandleTouchesMoved,
                OnTouchesEnded = HandleTouchesEnded,
                OnTouchesCancelled = HandleTouchesCanceled
            };
            //gameplayLayer.AddEventListener(touchListener);
        }
        private void HandleTouchesBegan(System.Collections.Generic.List<CCTouch> touches, CCEvent touchEvent)
        {
            isTouch = true;
            isTouchBegan = true;
            locationOnScreen = touches[0].Location;
        }
        private void HandleTouchesEnded(System.Collections.Generic.List<CCTouch> touches, CCEvent touchEvent)
        {
            isTouch = false;
        }
        private void HandleTouchesCanceled(System.Collections.Generic.List<CCTouch> touches, CCEvent touchEvent)
        {
            isTouch = false;
        }
        private void HandleTouchesMoved(System.Collections.Generic.List<CCTouch> touches, CCEvent touchEvent)
        {
            isTouch = true;
            locationOnScreen = touches[0].Location;
        }
    }
}