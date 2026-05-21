using CocosSharp;
using System.Collections.Generic;
using RiotGalaxy.Interface;

namespace RiotGalaxy
{
    public class SceneWin : CCScene
    {
        CCLayer layer;
        public SceneWin(CCGameView gameView) : base(gameView)
        {
            layer = new CCLayer();            
            this.AddChild(layer);

            CreateText();
            CreateTouchListener();
        }
        private void CreateText()
        {
            var label = new CCLabel("-= Победа! =-", "Arial", 50, CCLabelFormat.SystemFont);
            label.Color = CCColor3B.Blue;
            label.PositionX = layer.ContentSize.Width / 2.0f;
            label.PositionY = layer.ContentSize.Height / 2.0f;
            layer.AddChild(label);

            int prev_lvl = GameManager.level.cur_level_num;
            var label_lvl = new CCLabel("Уровень " + prev_lvl.ToString() + " пройден" + 
               "\n врагов убито: " + GameManager.level.total_num_enimies, "Arial", 40, CCLabelFormat.SystemFont);
            label_lvl.Color = CCColor3B.Red;
            label_lvl.PositionX = layer.ContentSize.Width / 2;
            label_lvl.PositionY = layer.ContentSize.Height / 3;
            layer.AddChild(label_lvl);
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
            ICommand cmd = new CommandNextLevelSplash();
            cmd.Execute();
        }
        public void CloseScene()
        {
            RemoveTouchListener();
        }
    }    
}