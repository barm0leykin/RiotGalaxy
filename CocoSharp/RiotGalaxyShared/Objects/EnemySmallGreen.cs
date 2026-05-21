using CocosSharp;

namespace RiotGalaxy.Objects
{
    class EnemySmallGreen : Enemy
    {
        float IdeaTime = 0;
        public EnemySmallGreen()
        {
            enemyType = EnemyType.GREEN;
            Hp = 10;
            Damage = 10;

            // Sprite
            draw.LoadGraphics("enemyGreen.png");

            shoot = new ObjBehShootSlide(this);
            move = new BehEnMoveStandartBounce(this);

            ShootInterval = 3;
            NewIdea(0);
        }
        public EnemySmallGreen(float x, float y) : this()
        {
            PositionX = x;
            PositionY = y;
        }
        override public void Activity(float time)
        {
            ai.AI(time);
            move.Move(time);

            ActionTime += time;
            IdeaTime += time;
            if (ActionTime > ShootInterval) // пришло время стрелять
            {
                ActionTime = 0;
                shoot.Shoot(time);
            }
            if (IdeaTime > 2)   // пришло время подумать новую мысль)
            {
                IdeaTime = 0;
                NewIdea(time);
            }
        }
        void NewIdea(float time)
        {
            //move.VelocityX = CCRandom.GetRandomFloat(-100, 100); // рандомно меняем скорость и направление
            //move.VelocityY = CCRandom.GetRandomFloat(-100, -200);
            CurrentSpeed = CCRandom.GetRandomFloat(100, 150);
            move.SetDirection(CCRandom.GetRandomFloat(135, 225));
        }
    }
}