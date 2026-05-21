using CocosSharp;

namespace RiotGalaxy.Objects.ObjBehavior
{
    public abstract class BehAI
    {
        //public enum AIState : int { TAKEOFF = 0, SWARMING, ATTACK }
        //protected int state = (int)AIState.TAKEOFF;
        protected AIState state;
        protected Enemy owner;
        protected float ideaTime = 0;
        protected float swarmingPosition = Options.height * 0.7f;

        public BehAI(Enemy obj)
        {
            owner = obj;
            //state = new AIState(owner);            
        }
        public void ChangeState(AIState newState)
        {
            if (state != null)
                state.Exit();
            state = newState;
            state.Enter();
        }
        abstract public void AI(float time);
    }
    class ObjBehAIDumd : BehAI // AI - тупой даун, ничего не делает)
    {
        public ObjBehAIDumd(Enemy obj) : base(obj)
        { }
        override public void AI(float time)
        {        }
    }

    class ObjBehAIEnemyRed : BehAI
    {
        public ObjBehAIEnemyRed(Enemy obj) : base(obj)
        {
            state = new AIStateTakeOff(owner);
            ChangeState(state); 
        }
        override public void AI(float time)
        {
            state.Update(time);

            ideaTime += time;
            if (ideaTime > 3)
            {
                System.Diagnostics.Debug.WriteLine("=== EnemyRed - new idea! === ----------------------------------------------");
                state.NewIdea(time);
                ideaTime = 0;
            }
            // Условия изменения состояний
            if (state.GetType() == typeof(AIStateTakeOff) && owner.PositionY < swarmingPosition && owner.PositionY > 0)
            {
                AIState state = new AIStateSwarming(owner);
                owner.ai.ChangeState(state);
                owner.Schedule(owner.shoot.Shoot, interval: 6f);///перенести в AIStateSwarming ?
            }
        }
    }

    class ObjBehAIEnemyBlue : BehAI
    {
        public ObjBehAIEnemyBlue(Enemy obj) : base(obj)
        {
            state = new AIStateOnRoute(owner); //
            //state = new AIStateTakeOff(owner);
            ChangeState(state);
        }
        override public void AI(float time)
        {
            state.Update(time);
            
            ideaTime += time;
            if (ideaTime > 3)
            {
                //System.Diagnostics.Debug.WriteLine("=== EnemyBlue - new idea! === ----------------------------------------------");
                state.NewIdea(time);
                ideaTime = 0;

                // Условия изменения состояний раз в N секунд
                if (state.GetType() == typeof(AIStateSwarming) && CCRandom.GetRandomInt(1, 5) == 1)
                {
                    AIState state = new AIStateAttack(owner);
                    owner.ai.ChangeState(state);
                    owner.Schedule(owner.shoot.Shoot, interval: 3f); //во время атаки стреляем чаще
                }
            }

            // Условия изменения состояний
            if (state.GetType() == typeof(AIStateTakeOff) && owner.PositionY < swarmingPosition && owner.PositionY > 0)
            {
                AIState state = new AIStateSwarming(owner);
                owner.ai.ChangeState(state);
                owner.Schedule(owner.shoot.Shoot, interval: 8f);
            }
            if (state.GetType() == typeof(AIStateAttack) && owner.PositionY < 0)
            {
                AIState state = new AIStateTakeOff(owner);
                owner.ai.ChangeState(state);
                owner.Unschedule(owner.shoot.Shoot);
            }

        }
    }


}