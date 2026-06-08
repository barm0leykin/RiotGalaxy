using RiotGalaxy.GameObjects;
using RiotGalaxy.Managers;

namespace RiotGalaxy.AI
{
    /// <summary>
    /// Контроллер ИИ врага — машина состояний. Порт BehAI из CocosSharp
    /// (Objects/ObjBehavior/BehAI.cs): держит текущее состояние, меняет его и раз в ~3 c
    /// генерирует «новую идею». Конкретные типы решают условия переходов.
    /// </summary>
    public abstract class EnemyAI
    {
        protected readonly Enemy owner;
        protected AIState state;
        protected float ideaTime;

        // Граница «зоны роения»: когда враг влетел в верхние 30% экрана.
        // (В оригинале swarmingPosition = 0.7*height при Y-вверх; в MonoGame Y-вниз → 0.3*height.)
        protected float SwarmY => GameManager.Instance.ScreenHeight * 0.3f;

        protected const float IdeaInterval = 3f;

        protected EnemyAI(Enemy owner) => this.owner = owner;

        public void ChangeState(AIState newState)
        {
            state?.Exit();
            state = newState;
            state.Enter();
        }

        public abstract void Update(float dt);
    }

    /// <summary>ИИ-«пустышка»: ничего не делает (порт ObjBehAIDumd).</summary>
    public class EnemyAIDumb : EnemyAI
    {
        public EnemyAIDumb(Enemy owner) : base(owner) { }
        public override void Update(float dt) { }
    }

    /// <summary>Красный: влетает (TakeOff) → роится и стреляет раз в 6 c. Порт ObjBehAIEnemyRed.</summary>
    public class EnemyAIRed : EnemyAI
    {
        public EnemyAIRed(Enemy owner) : base(owner) => ChangeState(new AIStateTakeOff(owner));

        public override void Update(float dt)
        {
            state.Update(dt);

            ideaTime += dt;
            if (ideaTime > IdeaInterval)
            {
                state.NewIdea(dt);
                ideaTime = 0f;
            }

            // Влетел в зону роения → Swarming (стрельба реже)
            if (state is AIStateTakeOff && owner.Position.Y > SwarmY)
            {
                ChangeState(new AIStateSwarming(owner));
                owner.SetShootInterval(6f);
            }
        }
    }

    /// <summary>
    /// Синий: TakeOff → Swarming (стрельба раз в 8 c) → иногда Attack (раз в 3 c);
    /// улетел за верх — снова TakeOff. Порт ObjBehAIEnemyBlue.
    /// </summary>
    public class EnemyAIBlue : EnemyAI
    {
        public EnemyAIBlue(Enemy owner) : base(owner) => ChangeState(new AIStateTakeOff(owner));

        public override void Update(float dt)
        {
            state.Update(dt);

            ideaTime += dt;
            if (ideaTime > IdeaInterval)
            {
                state.NewIdea(dt);
                ideaTime = 0f;

                // Из роения иногда срываемся в атаку (стреляем чаще)
                if (state is AIStateSwarming && owner.AiRandom.Next(1, 6) == 1)
                {
                    ChangeState(new AIStateAttack(owner));
                    owner.SetShootInterval(3f);
                }
            }

            // Влетел в зону роения → Swarming
            if (state is AIStateTakeOff && owner.Position.Y > SwarmY)
            {
                ChangeState(new AIStateSwarming(owner));
                owner.SetShootInterval(8f);
            }

            // Атакуя, улетел за верхнюю границу → снова заходим на взлёт
            if (state is AIStateAttack && owner.Position.Y < 0)
                ChangeState(new AIStateTakeOff(owner));
        }
    }
}
