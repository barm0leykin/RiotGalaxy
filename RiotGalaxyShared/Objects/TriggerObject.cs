using System;
using System.Collections.Generic;
using System.Text;
using CocosSharp;

namespace RiotGalaxy.Objects
{
    class TriggerObject : GameObject
    {
        public TriggerObject()
        {
        }
        public TriggerObject(CCPoint pos) : this()
        {
            Position = pos;            
        }
        public override CCRect GetRect()
        {
            int w = 2;
            int h = 2;
            CCRect cr = new CCRect(PositionX - w / 2, PositionY - h / 2, w, h);
            return cr;
        }
    }
}
