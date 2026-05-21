using CocosSharp;
using RiotGalaxy.Objects;
using RiotGalaxy.Factories;

namespace RiotGalaxy.Objects
{
    public class ObjBehCollision
    {
        protected GameObject owner;
        public ObjBehCollision(GameObject obj)
        {
            owner = obj;
        }
        virtual public bool isHasBody()
        {
            return false;
        }
        virtual public bool CheckCollision(GameObject obj)
        {
            return false;
        }
        virtual public bool CheckCollisionPoint(GameObject obj)
        {
            return false;
        }
    }
    class BehHardCollision : ObjBehCollision
    {
        public BehHardCollision(GameObject obj) : base(obj)
        { }
        override public bool isHasBody()
        {
            return true;
        }
        override public bool CheckCollision(GameObject obj)
        {
            if(obj.coll.isHasBody())
            {
                bool coll = false;
                CCRect r1 = owner.GetRect(), r2 = obj.GetRect();
                coll = CCRect.IntersetsRect(ref r1, ref r2);

                /*if (coll)
                    obj.Collision(owner); // obj1 стукает собой по obj2 :)    */                     

                //проверка на коллизии
                return coll;
            }
            return false;
        }
        override public bool CheckCollisionPoint(GameObject obj)
        {
            if (obj.coll.isHasBody())
            {
                bool coll = false;
                CCRect r1 = owner.GetRect();
                CCRect r2 = new CCRect(obj.PositionX, obj.PositionY, 1, 1);
                coll = CCRect.IntersetsRect(ref r1, ref r2);
                return coll;
            }
            return false;
        }

    }
    class ObjBehNoCollision : ObjBehCollision
    {
        public ObjBehNoCollision(GameObject obj) : base(obj)
        { }
        override public bool isHasBody()
        {
            return false;
        }
        override public bool CheckCollision(GameObject obj)
        {
            return false;
        }
    }
}