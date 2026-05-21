using CocosSharp;
using RiotGalaxy.Objects;
using RiotGalaxy.Objects.ObjBehavior;

namespace RiotGalaxy.Objects
{
    public class Bonus : GameObject
    {
        public enum BonusType : int { BULLET_UP = 0, HP_UP, NUKE_BOMB, STAR }
        public BonusType bonusType;

        public Bonus()
        {
            System.Diagnostics.Debug.WriteLine("=== ObjCreated: Bonus ===");
            objectType = ObjType.BONUS;
            playerSide = true;
            coll = new BehHardCollision(this);
            move = new BehMoveGravityFall(this);
            draw = new ObjBehDrawSprite(this);
            CurrentSpeed = 200;
            move.SetDirection(180);
            /*move = new BehMoveGravityFall(this)
            {
                VelocityX = 0,
                VelocityY = -200
            };*/
        }
        override public void Activity(float time)
        {
            move.Move(time);
            if (PositionY > Options.height || PositionY < 0)
                needToDelete = true;
        }
        public override bool Collision(GameObject obj)
        {
            if (obj.objectType == ObjType.PLAYER)
            {
                ApplyBonus(obj);
                needToDelete = true;
            }
            return true;
        }
        public override void Hit(int damage)
        {            
        }
        virtual public void ApplyBonus(GameObject obj)
        { }
    }    
}