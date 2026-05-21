using System;
using CocosSharp;


namespace RiotGalaxy.Objects
{
    public class Object : CCNode
    {
        public enum objType : int{ OBJECT = 0, BULLET, UNIT, PLAYER, ENEMY }
        public objType objectType;
        public bool playerSide;
        public CCRect collisionRect;
        protected CCSprite sprite;

        //sprite.contentSize.width* sprite.scaleX
        public bool needToDelete;

        public Object()
        {
            needToDelete = false;
        }
        void Activity(float time)
        {
        }
        virtual public bool Collision(Object obj)
        {
            return false;
        }

        public CCRect GetRect()
        {
            float w = sprite.ContentSize.Width * sprite.ScaleX, h = sprite.ContentSize.Height * sprite.ScaleY;
            CCRect cr = new CCRect(PositionX - w/2, PositionY - h/2, w,h);
            return cr;
        }
    }
}