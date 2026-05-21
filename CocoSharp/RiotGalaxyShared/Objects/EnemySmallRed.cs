using System;
using CocosSharp;
using RiotGalaxy;
using RiotGalaxy.Factories;
using RiotGalaxy.Objects.ObjBehavior;


namespace RiotGalaxy.Objects 
{
    class EnemySmallRed : Enemy
    {        
        public EnemySmallRed()
        {
            enemyType = EnemyType.RED;
            Hp = 20;
            Damage = 10;
            CurrentSpeed = 60;

            // Sprite
            draw.LoadGraphics("enemyRed.png");
            /*sprite = GameManager.sLoader.Load("enemyRed.png");
            sprite.AnchorPoint = CCPoint.AnchorMiddle;
            AddChild(sprite);*/
            //SetRect();

            shoot = new ObjBehShootAimToPlayer(this);
            move = new BehEnMoveStandartBounce(this);
            ai = new ObjBehAIEnemyRed(this);

            ShootInterval = 3;
        }
        public EnemySmallRed(float x, float y) : this()
        {           
            PositionX = x;
            PositionY = y;
        }
        override public void Activity(float time)
        {
            ai.AI(time);
            move.Move(time);

            // стрельбой управляет АИ
            /*ActionTime += time;
            if (ActionTime > ShootInterval)
            {
                ActionTime = 0;
                shoot.Shoot(time);
            }*/
        }
    }
}