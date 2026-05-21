using CocosSharp;
using RiotGalaxy.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace RiotGalaxy.Objects.ObjBehavior
{
    public class ObjBehDraw
    {
        protected GameObject owner;
        public ObjBehDraw(GameObject obj)
        {
            owner = obj;
        }
        public void Draw()
        {
        }
        virtual public bool LoadGraphics(string filename)
        {
            return false;
        }
        virtual public CCRect GetRect()
        {
            return new CCRect();
        }
    }
    public class ObjBehNoDraw : ObjBehDraw
    {
        public ObjBehNoDraw(GameObject obj) : base(obj)
        { }
    }

    public class ObjBehDrawSprite : ObjBehDraw
    {
        Sprite sprite;
        public ObjBehDrawSprite(GameObject obj) : base(obj)
        { }
        override public bool LoadGraphics(string filename)
        {
            sprite = new Sprite();
            if (sprite.LoadSprite(owner, filename))
            {
                owner.Width = sprite.GetCCSprite().ContentSize.Width;
                owner.Height = sprite.GetCCSprite().ContentSize.Height;
                return true;
            }
            return false;
        }
        override public CCRect GetRect()
        {
            /*float w = sprite.GetCCSprite().ContentSize.Width * sprite.GetCCSprite().ScaleX, 
                h = sprite.GetCCSprite().ContentSize.Height * sprite.GetCCSprite().ScaleY;
            CCRect cr = new CCRect(owner.PositionX - w / 2, owner.PositionY - h / 2, w, h);*/
            CCRect cr = new CCRect(owner.PositionX - owner.Width / 2, owner.PositionY - owner.Height / 2, owner.Width, owner.Height);
            return cr;
        }
    }

}
