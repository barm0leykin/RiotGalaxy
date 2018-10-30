using CocosSharp;
using RiotGalaxy.Objects;
using RiotGalaxy.Factories;

namespace RiotGalaxy.Objects
{
    public class ObjBehPlayerMove : BehMove
    {
        private float maxspeed = 450, acceleration = 1500, brakingSpeed = 800;
        private int moveDirection = 0; // <0 left, 0 stop, >0 right 
        //PlayerShip owner;

        public ObjBehPlayerMove(GameObject obj) : base(obj)
        {}

        public void SetMoveDirection(CCPoint touchPoint)
        {
            if (touchPoint.X < owner.PositionX - owner.Width / 2) //влево
            {
                if (VelocityX > 0) //для мгновенного разворота
                    VelocityX = 0;
                moveDirection = -1;
            }
            else if (touchPoint.X > owner.PositionX + owner.Width / 2)//вправо
            {
                if (VelocityX < 0)
                    VelocityX = 0;
                moveDirection = 1;
            }
            else
                moveDirection = 0;
        }
        public void MoveRight()
        {
            moveDirection = 1;
        }
        public void MoveLeft()
        {
            moveDirection = -1;
        }
        public void SetNoMove()
        {
            moveDirection = 0;
        }

        override public void Move(float time)
        {
            //float halfSecondsSquared = (time * time) / 2.0f;
            //ускоряемся
            if (moveDirection < 0)
                VelocityX -= acceleration  * time; //* halfSecondsSquared;
            else if (moveDirection > 0)
                VelocityX += acceleration * time;
            else // если не ускоряемся, то тормозим
            {
                /// тормозим
                if (VelocityX < 0) // двигаемся влево
                {
                    VelocityX += brakingSpeed * time; // торможение
                    if (VelocityX > 0) // если слишком затормозили, то стоп
                        VelocityX = 0;
                }
                else if (VelocityX > 0) // двигаемся вправо
                {
                    VelocityX -= brakingSpeed * time; 
                    if (VelocityX < 0)
                        VelocityX = 0;
                }
            }

            if (VelocityX > maxspeed) // ограничение скорости
                VelocityX = maxspeed;
            if (VelocityX < -maxspeed)
                VelocityX = -maxspeed;

            owner.PositionX += VelocityX * time;// двигаемся

            if (owner.PositionX < owner.Width / 2) // остановка на краях экрана
            {
                owner.PositionX = owner.Width / 2;
                VelocityX = 0;
            }
            if (owner.PositionX > Options.width - owner.Width / 2)
            {
                owner.PositionX = Options.width - owner.Width / 2;
                VelocityX = 0;
            }
        }
    }
}