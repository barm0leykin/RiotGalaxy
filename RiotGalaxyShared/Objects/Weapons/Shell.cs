using CocosSharp;
using RiotGalaxy.Objects.ObjBehavior;
using System;
using System.Collections.Generic;
using System.Text;

namespace RiotGalaxy.Objects.Weapons
{
    class Shell : GameObject
    {
        //public float speed, direction;
        public Shell(CCPoint point)
        {
            Position = point;
            objectType = ObjType.SHELL;
            name = this.objectType.ToString();
            coll = new BehHardCollision(this);
            draw = new ObjBehDrawSprite(this);
            //move = new ObjBehLinearMove(this);

            Hp = 1;
            Damage = 10;

            move = new BehLinearMove(this);
        }
        public override bool Collision(GameObject obj)
        {
            if (obj.objectType == ObjType.BONUS)
                return false;
            if (playerSide == obj.playerSide) // откл фрэндли-файр
                return false;
            obj.Hit((int)Damage);////////////////!!!!!!!!
            Hit(obj);
            return true;
        }

        virtual public void Hit(GameObject obj)
        {
            if (this.playerSide && !obj.playerSide)
            {
                GameManager.player.Score++; // одно очко за попадание
            }
            needToDelete = true;
        }
        override public void Activity(float time)
        {
            move.Move(time);

            if (PositionY > Options.height || PositionY < 0 || PositionX > Options.width || PositionX < 0)
            {
                needToDelete = true;
                //RemoveChild(sprite);
            }
        }
    }
}
