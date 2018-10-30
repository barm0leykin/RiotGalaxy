using CocosSharp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace RiotGalaxy.Interface
{
    public class Pers
    {
        public string Name { get; set; }
        public CCSprite avatar;

        public Pers(string n)
        {
            Name = n;
            // Sprite
            //sprite = GameManager.sLoader.Load(name + ".png"); //new CCSprite("ship"); //
            //sprite.AnchorPoint = CCPoint.AnchorMiddle;
        }
    }


    class CutScene
    {
        ArrayList msg;  // Что говорят
        ArrayList pers; // Кто говорит (аватарка персонажа)
        int msgNum = 0;
        //string mesage;

        CCDrawNode drawNode;
        //CCLabel label;
        //Pers pers;
        CCEventListenerTouchAllAtOnce touchListener;

        public CutScene()
        {
            msg = new ArrayList();
            pers = new ArrayList();
            // регистрируем боработчик событий
            touchListener = new CCEventListenerTouchAllAtOnce();
            touchListener.OnTouchesBegan = HandleTouchesBegan;
            GameManager.ScGame.gameplayLayer.AddEventListener(touchListener);
        }

        private void HandleTouchesBegan(System.Collections.Generic.List<CCTouch> touches, CCEvent touchEvent)
        {
            //bool isTouch = true;
            CCPoint locationOnScreen = touches[0].Location;

            if (drawNode.BoundingBoxTransformedToWorld.ContainsPoint(locationOnScreen))
            {
                //It's a click! // следующее сообщение
                Delete();
            }
        }

        public void AddMsg(Pers p, string text)
        {
            pers.Add(p);
            msg.Add(text);
            //
        }
        public void ShowNextMessage()
        {
            if (msgNum >= msg.Count)
                return;
            RenderMsgBox();
            msgNum++;
        }
        void RenderMsgBox()
        {
            var shape = new CCRect(200, 200, Options.width - 400, Options.height - 400);
            drawNode = new CCDrawNode();           

            drawNode.DrawRect(shape,
                fillColor: CCColor4B.Blue,
                borderWidth: 4,
                borderColor: CCColor4B.AliceBlue);

            Pers p = (Pers)pers[msgNum];
            //p.sprite.Position = new CCPoint(40, 20);//
            //drawNode.AddChild(p.sprite);

            CCLabel labelName = new CCLabel(p.Name, "Arial", 30, CCLabelFormat.SystemFont)
            {
                AnchorPoint = CCPoint.AnchorLowerLeft,
                Color = CCColor3B.Red,
                PositionX = 280,
                PositionY = 220
            };
            drawNode.AddChild(labelName);
            
            string text = (string)msg[msgNum];      
            
            CCLabel labelText = new CCLabel(text, "Arial", 30, CCLabelFormat.SystemFont)
            {
                AnchorPoint = CCPoint.AnchorLowerLeft,
                Color = CCColor3B.Red,
                PositionX = 740,
                PositionY = 230
            };
            drawNode.AddChild(labelText);

            GameManager.ScGame.gameplayLayer.AddChild(drawNode);
        }
        public void Delete()
        {
            GameManager.ScGame.RemoveEventListener(touchListener);
            drawNode.RemoveFromParent();
        }
    }
}
