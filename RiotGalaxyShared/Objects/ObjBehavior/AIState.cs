using CocosSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace RiotGalaxy.Objects.ObjBehavior
{
    public class AIState
    {
        protected Enemy owner;
        public bool ReadyToChange { get; set; }
        public AIState(Enemy obj)
        {
            owner = obj;
            ReadyToChange = true;
        }
        virtual public void Enter()
        {
        }
        virtual public void Exit()
        {            
        }
        virtual public void NewIdea(float time)
        { }
        virtual public void Update(float time)
        { }

    }

    class AIStateTakeOff : AIState
    {
        //float destinationY;
        public AIStateTakeOff(Enemy obj) : base(obj)
        {            
        }

        override public void Enter()
        {
            System.Diagnostics.Debug.WriteLine(owner.name + "=== AIStateTakeOff === ");
            //1 вычисляем куда нужно прилететь
            //destinationY = GameOptions.height * 0.7f;
            ReadyToChange = false;

            owner.CurrentSpeed = owner.MaxSpeed;    // полный вперед
            owner.gun.Safe = true;                  // не стреляем

            NewIdea(0);
        }
        override public void NewIdea(float time)
        {
            owner.move.SetDirection(CCRandom.GetRandomFloat(135, 225));
        }
    }



    class AIStateSwarming : AIState
    {
        BehMove oldmoveStyle;
        public AIStateSwarming(Enemy obj) : base(obj)
        {
        }

        override public void Enter()
        {
            System.Diagnostics.Debug.WriteLine(owner.name + "=== AIStateTakeSwarming === ");
            
            //1 начинаем летать медленее
            oldmoveStyle = owner.move;
            owner.move = new BehEnMoveSwarm(owner);
            owner.CurrentSpeed = owner.MaxSpeed / 5;
            owner.gun.Safe = false; // можно стрелять
            NewIdea(0);
        }
        public override void Exit()
        {
            owner.move = oldmoveStyle;
            base.Exit();
        }

        override public void NewIdea(float time)
        {
            //2 потихоньку тусим
            //owner.move = new ObjBehLinearMove(owner); //swarming
            owner.move.SetDirection(CCRandom.GetRandomFloat(0, 360));

            //owner.move.VelocityX = CCRandom.GetRandomFloat(-swarmingSpeed, swarmingSpeed); // рандомно меняем скорость и направление
            //owner.move.VelocityY = CCRandom.GetRandomFloat(-swarmingSpeed, swarmingSpeed);

            ReadyToChange = true;
        }
    }// AIStateTakeSwarming



    class AIStateAttack : AIState
    {
        public AIStateAttack(Enemy obj) : base(obj)
        {
        }
        override public void Enter()
        {
            System.Diagnostics.Debug.WriteLine(owner.name + "=== AIStateAttack === ");
            owner.move = new BehEnMoveStandartBounce(owner);
            owner.CurrentSpeed = owner.MaxSpeed;
            owner.gun.Safe = false; // можно стрелять
            NewIdea(0);
        }
        override public void NewIdea(float time)
        {
            owner.move.SetDirection(CCRandom.GetRandomFloat(155, 205));
            ReadyToChange = true;
        }
    }// AIStateAttack



    class AIStateOnRoute : AIState
    {
        TriggerObject trigger;
        Cell hive_cell;
        public AIStateOnRoute(Enemy obj) : base(obj)
        {
            trigger = new TriggerObject();
            trigger.coll = new BehHardCollision(trigger);
        }
        override public void Enter()
        {
            System.Diagnostics.Debug.WriteLine("=== AIStateOnRoute === ");

            /*owner.route = new Route();
            owner.route.LoadRoute("zmeyka1-left");*/
            owner.move = new BehLinearMove(owner);
            owner.route = (Route)GameManager.gameplay.lvlEventDirector.curEvList.current_route.Clone(); //current_route.Clone();
            owner.move.SetDirectionTo(owner.route.GetCurrentPoint());         //летим к первой точке
            trigger.Position = owner.route.GetCurrentPoint();                 //ставим на этой точке триггер

            hive_cell = GameManager.gameplay.hive.GetNextFreeCell();
            hive_cell.TakeCell(owner);                                  //забил пустую ячейку в улье
            owner.route.AddPoint(hive_cell.point);                            //последняя точка - улей

            System.Diagnostics.Debug.WriteLine(owner.name + "=== AIStateOnRoute Enter=== ");
            owner.CurrentSpeed = owner.MaxSpeed;
            owner.gun.Safe = true; //
            NewIdea(0);
        }
        override public void NewIdea(float time)
        {
        }
        override public void Update(float time)
        {
            if (owner.coll.CheckCollisionPoint(trigger))
            {
                if (owner.route.IsLast())
                {                    
                    System.Diagnostics.Debug.WriteLine(owner.name + "=== достиг ПОСЛЕДНЕЙ точки маршрута === ");
                    //hive_cell.FreeCell();
                    AIState state = new AIStateSwarming(owner);
                    owner.ai.ChangeState(state);
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine(owner.name + "=== достиг точки маршрута === ");
                    owner.move.SetDirectionTo(owner.route.GetNextPoint());//меняем точку назначения
                    trigger.Position = owner.route.GetCurrentPoint();//переносим триггер
                }
            }
        }
    }// AIStateOnRoute

}