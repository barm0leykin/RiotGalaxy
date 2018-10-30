using CocosSharp;
using System.Collections.Generic;

namespace RiotGalaxy
{
    public class SceneMainMenu : CCScene
    {
        CCLayer layer;
        public SceneMainMenu(CCGameView gameView) : base(gameView)
        {
            layer = new CCLayer();
            layer.Color = new CCColor3B(CCColor4B.Blue);////?
            this.AddChild(layer);

            CreateText();
            //CreateTouchListener();
            //GameDelegate.gameManager.userInputHandler.touchListener.OnTouchesBegan = HandleTouchesBegan;
            //layer.AddEventListener(GameDelegate.gameManager.userInputHandler.touchListener);
            CreateTouchListener();
        }
        private void CreateText()
        {
            //var label = new CCLabel("-= Galaxy Riot!!!! =-", "SCConvoy.fnt", 100, CCLabelFormat.BitMapFont)
            var label = new CCLabel("-= Galaxy Riot!!!! =-", "Arial", 100, CCLabelFormat.SystemFont)
            {
                //Scale = 2,
                Color = CCColor3B.Orange,
                PositionX = layer.ContentSize.Width / 2,
                PositionY = layer.ContentSize.Height / 2
            }; 
            layer.AddChild(label);

            var label_level = new CCLabel("нажми чтобы начать подавлять аццкий всегалактический бунт", "Arial", 20, CCLabelFormat.SystemFont);
            //label_level.Scale = 0.5f;
            label_level.Color = CCColor3B.White;
            label_level.PositionX = layer.ContentSize.Width / 2;
            label_level.PositionY = layer.ContentSize.Height / 3;
            layer.AddChild(label_level);
        }

        public void CreateTouchListener()
        {
            //layer.AddEventListener(GameDelegate.gameManager.userInputHandler.touchListener);
            GameDelegate.gameManager.userInputHandler.touchListener.OnTouchesBegan += HandleTouchesBegan;
            layer.AddEventListener(GameDelegate.gameManager.userInputHandler.touchListener);
        }
        public void RemoveTouchListener()
        {
            GameDelegate.gameManager.userInputHandler.touchListener.OnTouchesBegan -= HandleTouchesBegan;
            layer.RemoveEventListener(GameDelegate.gameManager.userInputHandler.touchListener);
        }
        public void HandleTouchesBegan(List<CCTouch> arg1, CCEvent arg2)
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