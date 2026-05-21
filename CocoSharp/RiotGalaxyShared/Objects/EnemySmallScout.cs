using CocosSharp;

namespace RiotGalaxy.Objects
{
    class EnemySmallScout : Enemy
    {
        //
        public EnemySmallScout()
        {
            enemyType = EnemyType.SM_SCOUT;           

            Hp = 10;
            Damage = 5;

            // Sprite
            draw.LoadGraphics("enemySmallScout.png");

            shoot = new ObjBehShootSlide(this);
            move = new BehEnMoveStandartBounce(this);

            //Schedule(Activity);
            NewIdea(0);
            //Schedule(NewIdea, 3);

        }
        public EnemySmallScout(float x, float y) : this()
        {
            PositionX = x;
            PositionY = y;
        }
        override public void Activity(float time)
        {
            //Move(time);
            move.Move(time);
            //FacePoint(GameManager.gameplay.playerShip.PositionX, GameManager.gameplay.playerShip.PositionY);
        }
        void NewIdea(float time)
        {
            //move.VelocityX = CCRandom.GetRandomFloat(-50, 50); // рандомно меняем скорость и направление
            //move.VelocityY = CCRandom.GetRandomFloat(-100, -120);

            CurrentSpeed = CCRandom.GetRandomFloat(60, 100);
            move.SetDirection(CCRandom.GetRandomFloat(155, 205));
        }

    }
}