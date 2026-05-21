using CocosSharp;
using System.Collections.Generic;

namespace RiotGalaxy
{
    public class SceneNextLevel : CCScene
    {
        CCLayer layer;
        public SceneNextLevel(CCGameView gameView) : base(gameView)
        {
            layer = new CCLayer();            
            this.AddChild(layer);

            CreateText();
            //var grid = new Grid();
            CreateTouchListener();
        }

        private void CreateText()
        {
            //var text = System.IO.File.ReadAllText("l01tmp.txt");/////

            string lab_lvl = "-= Galaxy Riot!!!! =-";
            var label = new CCLabel(lab_lvl, "Arial", 50, CCLabelFormat.SystemFont);
            label.Color = CCColor3B.Orange;
            label.PositionX = layer.ContentSize.Width / 2;
            label.PositionY = layer.ContentSize.Height / 2;
            layer.AddChild(label);

            lab_lvl = "Level " + GameManager.cur_level;
            var label_level = new CCLabel(lab_lvl, "Arial", 50, CCLabelFormat.SystemFont);
            label_level.Color = CCColor3B.White;
            label_level.PositionX = layer.ContentSize.Width / 2;
            label_level.PositionY = layer.ContentSize.Height / 3;
            layer.AddChild(label_level);

            //test
            /*CCMenu menu;
            CCMenuItem menuitem1;

            menuitem1 = new CCMenuItemLabel(new CCLabel("blue", "arial", 40f), delegate (object obj)
            {
                //Console.WriteLine("button clicked!");
            });
            menuitem1.Position = new CCPoint(200f, 200f);
            menu = new CCMenu(menuitem1);

            AddChild(menu);*/
        }

        public void CreateTouchListener()
        {
            GameDelegate.gameManager.userInputHandler.touchListener.OnTouchesBegan += HandleTouchesBegan;
            layer.AddEventListener(GameDelegate.gameManager.userInputHandler.touchListener);
        }
        public void RemoveTouchListener()
        {
            GameDelegate.gameManager.userInputHandler.touchListener.OnTouchesBegan -= HandleTouchesBegan;
            layer.RemoveEventListener(GameDelegate.gameManager.userInputHandler.touchListener);
        }

        private void HandleTouchesBegan(List<CCTouch> arg1, CCEvent arg2)
        {
            ICommand cmd = new CommandStartNextLevel();
            cmd.Execute();
        }
        public void CloseScene()
        {
            RemoveTouchListener();
        }
    }
}