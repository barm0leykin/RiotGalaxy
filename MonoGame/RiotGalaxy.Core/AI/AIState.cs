using RiotGalaxy.GameObjects;

namespace RiotGalaxy.AI
{
    /// <summary>
    /// Базовое состояние ИИ врага. Порт AIState из CocosSharp (Objects/ObjBehavior/AIState.cs).
    /// Состояние управляет движением/скоростью/стрельбой владельца; переходы решает <see cref="EnemyAI"/>.
    /// </summary>
    public abstract class AIState
    {
        protected readonly Enemy owner;

        /// <summary>Готовность состояния к смене (как ReadyToChange в оригинале).</summary>
        public bool ReadyToChange { get; protected set; } = true;

        protected AIState(Enemy owner) => this.owner = owner;

        public virtual void Enter() { }
        public virtual void Exit() { }
        public virtual void NewIdea(float dt) { }
        public virtual void Update(float dt) { }
    }

    /// <summary>Взлёт/влёт на экран: полный ход, не стреляет, курс вниз (135..225°). Порт AIStateTakeOff.</summary>
    public class AIStateTakeOff : AIState
    {
        public AIStateTakeOff(Enemy owner) : base(owner) { }

        public override void Enter()
        {
            ReadyToChange = false;
            owner.CurrentSpeed = owner.MaxSpeed; // полный вперёд
            owner.ShootSafe = true;              // не стреляем
            owner.UseBounceMovement();
            NewIdea(0);
        }

        public override void NewIdea(float dt)
        {
            owner.SetMoveDirection(135f + (float)owner.AiRandom.NextDouble() * 90f); // 135..225 — вниз
        }
    }

    /// <summary>Роение: медленно (1/5 скорости), стреляет, случайный курс. Порт AIStateSwarming.</summary>
    public class AIStateSwarming : AIState
    {
        public AIStateSwarming(Enemy owner) : base(owner) { }

        public override void Enter()
        {
            owner.CurrentSpeed = owner.MaxSpeed / 5f; // летаем медленнее
            owner.ShootSafe = false;                  // можно стрелять
            owner.UseBounceMovement();
            NewIdea(0);
        }

        public override void NewIdea(float dt)
        {
            owner.SetMoveDirection((float)owner.AiRandom.NextDouble() * 360f); // куда угодно
            ReadyToChange = true;
        }
    }

    /// <summary>Атака: полный ход с отскоком, стреляет, курс вниз (155..205°). Порт AIStateAttack.</summary>
    public class AIStateAttack : AIState
    {
        public AIStateAttack(Enemy owner) : base(owner) { }

        public override void Enter()
        {
            owner.CurrentSpeed = owner.MaxSpeed;
            owner.ShootSafe = false;
            owner.UseBounceMovement();
            NewIdea(0);
        }

        public override void NewIdea(float dt)
        {
            owner.SetMoveDirection(155f + (float)owner.AiRandom.NextDouble() * 50f); // 155..205 — вниз
            ReadyToChange = true;
        }
    }
}
