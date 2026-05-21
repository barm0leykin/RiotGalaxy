using CocosSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace RiotGalaxy.Objects
{
    class BonusStar : Bonus
    {
        int score = 10;
        //bool magnetOn = false;
        int magnetDist = 250;   // расстояние на к-м звезда притягивается к игроку
        float angleToPlayer;    // угол на игрока
        float turnSpeed = 45;   // для изменения направления полета к игроку или вниз
        int rotationAngl = 0, rotateSpeed = 0; //для кручения вокруг своей оси

        public BonusStar(CCPoint pos)
        {
            bonusType = BonusType.STAR;

            // Sprite
            draw.LoadGraphics("bonusStar.png");

            Position = pos;

            CurrentSpeed = 70;
            move = new BehLinearMove(this);
            move.SetDirection(180); //изначально летим вниз

            rotateSpeed = CCRandom.GetRandomInt(-5, 5); // скорость вращения вокруг своей оси
            /*
            CCAction action;
            action = new CCRotateBy(7, CCRandom.GetRandomInt(-359, 360));
            AddAction(action);
            */
        }
        override public void Activity(float time)
        {
            move.Move(time);
            rotationAngl += rotateSpeed;
            Rotation = rotationAngl;

            float dist = GameManager.vMath.GetVectorLength(Position, GameManager.gameplay.playerShip.Position);
            if(dist < magnetDist)
            {
                // Если игрок ближе чем дистанция действия магнита, бонус постепенно меняет траекторию на сближение
                angleToPlayer = GameManager.vMath.AimAngle(Position, GameManager.gameplay.playerShip.Position);
                if(move.DirectionAngle != angleToPlayer)
                {
                    if (move.DirectionAngle - angleToPlayer < 0)
                    {
                        move.DirectionAngle += turnSpeed * time;
                    }
                    else
                    {
                        move.DirectionAngle -= turnSpeed * time;
                    }
                }

                //move.SetDirectionTo(GameManager.gameplay.playerShip.Position);                
            }

            // если магнит перестал дейтвовать, постепенно меняем траекторию на вертикально вниз
            if(dist > magnetDist && move.DirectionAngle != 180)
            {
                if (move.DirectionAngle > 180)
                {
                    move.DirectionAngle -= turnSpeed * time;
                    if (move.DirectionAngle < 180)//перестарались
                        move.DirectionAngle = 180;
                }
                if (move.DirectionAngle < 180)
                {
                    move.DirectionAngle += turnSpeed * time;
                    if (move.DirectionAngle >180)//перестарались
                        move.DirectionAngle = 180;
                }
            }

            if (PositionY > Options.height || PositionY < 0)
                needToDelete = true;
        }
        public override void ApplyBonus(GameObject obj)
        {
            //base.ApplyBonus(obj);
            GameManager.player.Score += score;
        }
    }
}
