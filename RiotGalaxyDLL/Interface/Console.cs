using CocosSharp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace RiotGalaxy.Interface
{
    class Console
    {
        CCLabel cons_label_line1, cons_label_line2, cons_label_line3;
        ArrayList console_messages;

        public Console()
        {
            console_messages = new ArrayList();

            cons_label_line3 = new CCLabel(">", "Arial", 20, CCLabelFormat.SystemFont)
            {
                Color = CCColor3B.Gray,
                PositionX = 10,
                PositionY = Options.height - 5,
                AnchorPoint = CCPoint.AnchorUpperLeft
            };
            GameManager.ScGame.hudLayer.AddChild(cons_label_line3);
            cons_label_line2 = new CCLabel(">", "Arial", 20, CCLabelFormat.SystemFont)
            {
                Color = CCColor3B.Gray,
                PositionX = 10,
                PositionY = Options.height - 25,
                AnchorPoint = CCPoint.AnchorUpperLeft
            };
            GameManager.ScGame.hudLayer.AddChild(cons_label_line2);
            cons_label_line1 = new CCLabel(">", "Arial", 20, CCLabelFormat.SystemFont)
            {
                Color = CCColor3B.White,
                PositionX = 10,
                PositionY = Options.height - 45,
                AnchorPoint = CCPoint.AnchorUpperLeft
            };
            GameManager.ScGame.hudLayer.AddChild(cons_label_line1);
        }
        public void AddConsoleMsg(string msg)
        {
            console_messages.Add("> " + msg);
            DrawConsoleMsg();
        }
        void DrawConsoleMsg()
        {
            string msg = console_messages[console_messages.Count - 1].ToString();
            cons_label_line1.Text = msg;
            if (console_messages.Count >= 2)
            {
                msg = console_messages[console_messages.Count - 2].ToString();
                cons_label_line2.Text = msg;
            }
            if (console_messages.Count >= 3)
            {
                msg = console_messages[console_messages.Count - 3].ToString();
                cons_label_line3.Text = msg;
            }
        }
    }// class Console
}
