using RiotGalaxy.Objects;
using CocosSharp;
using System.Collections.Generic;
using static RiotGalaxy.Objects.GameObject;

namespace RiotGalaxy.Interface
{
    public class InputHandler
    {
        //UnitCommand command;
        public List<MyButton> GuiButtons;
        public InputHandler()
        {
            GuiButtons = new List<MyButton>();
        }
        public bool HandleUserInput(CCPoint touchPoint)
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
    }
}