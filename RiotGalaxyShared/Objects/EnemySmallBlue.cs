using System;
using CocosSharp;
using RiotGalaxy;
using RiotGalaxy.Factories;
using RiotGalaxy.Objects.ObjBehavior;

namespace RiotGalaxy.Objects
{
    class EnemySmallBlue : Enemy
    {
        public EnemySmallBlue()
        {
            enemyType = EnemyType.BLUE;
            Hp = 10;
            Damage = 10;
            MaxSpeed = 130;
            CurrentSpeed = 130;

            shoot = new ObjBehShootDown(this);
            move = new BehEnMoveStandartBounce(this);
            ai = new ObjBehAIEnemyBlue(this);
            //draw = new ObjBehDrawSprite(this);
            draw.LoadGraphics("enemyBlue.png");

            ShootInterval = 3;
        }
        public EnemySmallBlue(float x, float y) : this()
        {            
            PositionX = x;
            PositionY = y;
        }
        override public void Activity(float time)
        {
            //Move(time);  
            ai.AI(time);
            move.Move(time);

            ActionTime += time;
            if (ActionTime > ShootInterval)
            {
                ActionTime = 0;
                shoot.Shoot(time);
            }
        }
        /*void NewIdea(float time)
        {
            move.VelocityX = CCRandom.GetRandomFloat(-100, 100); // рандомно меняем скорость и направление
            move.VelocityY = CCRandom.GetRandomFloat(-100, -200);
        }*/
    }
}