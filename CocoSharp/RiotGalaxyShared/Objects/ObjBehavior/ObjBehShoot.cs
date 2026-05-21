using CocosSharp;
using RiotGalaxy.Factories;

namespace RiotGalaxy.Objects //.ObjBehavior
{
    public class ObjBehShoot // стреляет, напимер, строго вниз, в зависимости от скорости или прицельно
    {
        protected GameObject owner;
        public ObjBehShoot(GameObject obj)
        {
            owner = obj;
        }
        virtual public void Shoot(float unusedValue)
        { }
    }


    class ObjBehShootDown : ObjBehShoot // стреляет строго вниз
    {
        public ObjBehShootDown(GameObject obj) : base(obj)
        {
        }
        override public void Shoot(float unusedValue)
        {
            if (owner.gun.Safe)
                return;
            owner.gun.Aim(180f);
            owner.gun.Fire();
        }
    }
    class ObjBehShootUp : ObjBehShoot // стреляет строго вниз
    {
        public ObjBehShootUp(GameObject obj) : base(obj)
        {
        }
        override public void Shoot(float time)
        {
            if (owner.gun.Safe)
                return;
            owner.gun.Aim(0f);
            owner.gun.Fire();

            if (owner.GetType() == typeof(PlayerShip))
            {
                GameManager.gameplay.playerShip.rounds++;
            }
        }
    }
    
    class ObjBehShootSlide : ObjBehShoot
    {
        public ObjBehShootSlide(GameObject obj) : base(obj)
        {
        }
        override public void Shoot(float unusedValue)
        {
            if (owner.gun.Safe)
                return;
            //целимся в направлении движения
            CCPoint targetPos = owner.Position;
            //targetPos.X += owner.move.VelocityX;
            //targetPos.Y += owner.move.VelocityY * 1.5f;
            //owner.gun.Aim(targetPos);

            float aimAngle = owner.move.DirectionAngle;
            if (aimAngle < 180)
                aimAngle += (180 - aimAngle) / 2;
            if (aimAngle > 180)
                aimAngle -= (aimAngle-180) / 2;
            owner.gun.Aim(aimAngle);
        }
    }

    class ObjBehShootAimToPlayer : ObjBehShoot
    {
        public ObjBehShootAimToPlayer(GameObject obj) : base(obj)
        {
        }
        override public void Shoot(float unusedValue)
        {
            if (owner.gun.Safe)
                return;
            owner.gun.Aim(GameManager.gameplay.playerShip.Position);
            owner.gun.Fire();
        }
    }

    class ObjBehNoShoot : ObjBehShoot
    {
        public ObjBehNoShoot(GameObject obj) : base(obj)
        {
        }
        override public void Shoot(float unusedValue)
        {
        }
    }

}