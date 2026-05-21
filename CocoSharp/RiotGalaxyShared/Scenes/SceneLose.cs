using CocosSharp;
using System.Collections.Generic;

namespace RiotGalaxy
{
    public class SceneLose : CCScene
    {
        CCLayer layer;
        string message;
        public SceneLose(CCGameView gameView, string msg = "") : base(gameView)
        {
            layer = new CCLayer();            
            this.AddChild(layer);

            message = msg;
            CreateText();
            CreateTouchListener();
        }

        private void CreateText()
        {
            var label = new CCLabel("-= You lose! =-", "Arial", 50, CCLabelFormat.SystemFont);
            label.Color = CCColor3B.Red;
            label.PositionX = layer.ContentSize.Width / 2.0f;
            label.PositionY = layer.ContentSize.Height / 2.0f;
            layer.AddChild(label);

            
            var msg_label = new CCLabel(message, "Arial", 40, CCLabelFormat.SystemFont);
            msg_label.Color = CCColor3B.White;
            msg_label.PositionX = layer.ContentSize.Width / 2;
            msg_label.PositionY = layer.ContentSize.Height / 3;
            layer.AddChild(msg_label);
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