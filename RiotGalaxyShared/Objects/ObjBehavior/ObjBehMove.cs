using CocosSharp;
using RiotGalaxy.Objects;
using RiotGalaxy.Factories;
using System;

namespace RiotGalaxy.Objects //.ObjBehavior
{
    public abstract class BehMove
    {
        protected GameObject owner;
        protected float VelocityX =0, VelocityY=0;
        float dirAngle = 0;
        public float DirectionAngle
        {
            get
            {
                return dirAngle;
            }
            set
            {
                dirAngle = value;
                SetDirection(value);
            }
        }
        protected CCVector2 DirectionVector;

        public BehMove(GameObject obj)
        {
            owner = obj;
        }
        abstract public void Move(float time);

        public void SetDirectionTo(CCPoint target)
        {
            DirectionAngle = GameManager.vMath.AimAngle(owner.Position, target);
            //this.SetDirection(DirectionAngle);
        }
        public void SetDirection(float angle)
        {
            //задаем направление и скорость движения
            //скорость = длина вектора
            DirectionVector.X = 0; DirectionVector.Y = owner.CurrentSpeed;
            GameManager.vMath.RotateVector(ref DirectionVector, angle);

            owner.move.VelocityY = DirectionVector.Y;
            owner.move.VelocityX = DirectionVector.X;
            dirAngle = angle;//?
        }
        protected void MovePosition(float time)
        {
            owner.PositionX += VelocityX * time;
            owner.PositionY += VelocityY * time;
        }        
        protected void BounceSideBorders()
        {
            /*if (owner.PositionX < GameOptions.width +30)
            {
                VelocityX = -VelocityX;
                owner.PositionX = GameOptions.width + 32;
            }
            else if (owner.PositionX > GameOptions.width -30)
            {
                VelocityX = -VelocityX;
                owner.PositionX = GameOptions.width -32;
            }*/

            // отскок от границ экрана (минус 10%)
            float border = Options.width * 0.10f;
            if (owner.PositionX < border)
            {
                VelocityX = -VelocityX;
                owner.PositionX = border + 1;
            }
            else if (owner.PositionX > Options.width - border - owner.Width)
            {
                VelocityX = -VelocityX;
                owner.PositionX = Options.width - border - owner.Width - 1;
            }
        }
        protected void SlideUp()
        {
            if (owner.PositionY < -50)        // улетели вниз экрана, выскакиваем сверху
                owner.PositionY = Options.height + 50;//needToDelete = true;            
        }
    }

    class BehEnMoveStandartBounce : BehMove
    {
        public BehEnMoveStandartBounce(GameObject obj) : base(obj)
        { }
        override public void Move(float time)
        {
            MovePosition(time);
            BounceSideBorders();
            SlideUp();
        }
    }

    class BehEnMoveSwarm : BehMove
    {
        float swarmingDownSide = Options.height / 2f;
        public BehEnMoveSwarm(GameObject obj) : base(obj)
        {
            System.Diagnostics.Debug.WriteLine("=== ObjBehEnMoveSwarm ===");
        }
        override public void Move(float time)
        {
            MovePosition(time);
            BounceSwarmBorders();
        }
        protected void BounceSwarmBorders()
        {
            BounceSideBorders();
            if (owner.PositionY < swarmingDownSide)        // улетели вниз экрана, выскакиваем сверху
            {
                owner.PositionY += 3;
                VelocityY = -VelocityY;
            }

            if (owner.PositionY > Options.height - owner.Width)        // улетели вниз экрана, выскакиваем сверху
            {
                owner.PositionY -= 3;
                VelocityY = -VelocityY;
            }
        }

    }
    
    class BehMoveGravityFall : BehMove
    {
        public BehMoveGravityFall(GameObject obj) : base(obj)
        {
            //VelocityY = 0;
            SetDirection(180);
        }
        override public void Move(float time)
        {
            VelocityY -= 9.8f * time;/////
            if (VelocityY < -200)
                VelocityY = -200;
            MovePosition(time);
            //owner.PositionY += VelocityY;// * time;
        }
    }

    class BehLinearMove : BehMove
    {
        public BehLinearMove(GameObject obj) : base(obj)
        { }
        override public void Move(float time)
        {
            MovePosition(time);
        }
    }

    class BehMoveNone : BehMove
    {
        public BehMoveNone(GameObject obj) : base(obj)
        { }
        override public void Move(float time)
        {
        }
    }

}