using CocosSharp;
using RiotGalaxy.Factories;
using System.Threading;
using System.Threading.Tasks;

namespace RiotGalaxy.Objects.Weapons
{
    public class Weapon
    {
        public enum WeaponType : int { CANNON = 0, AUTOCANNON, MINIGUN, LASER }
        public int weaponType;
        public float GunFireRate = 1.8f;  // скорострельность
        protected int fireCount = 0;    // счетчик выстрелов
        protected int damage = 10;      // убойная сила
        public bool Safe { get; set; }  // предохранитель

        protected GameObject owner;        
        //protected GameObject shell;
        protected CCPoint shellPos;

        
        //protected float direction = 180; // 0 - 360
        protected float shellSpeed = 200;
        protected float aimAngle;
        protected CCVector2 aimVector;        

        public Weapon(GameObject obj)
        {
            owner = obj;
            Safe = false;
        }
        public void Aim(CCPoint target)
        {
            aimAngle = GameManager.vMath.AimAngle(owner.Position, target);
            this.Aim(aimAngle); 
        }
        public void Aim(float angle)
        {
            // исходная точка выстрела = позиция стрелка
            shellPos = owner.Position;

            // целимся - сначала вычесляем тоступ от стреляющего, чтобы пуля не появлялась внутри его
            float offset = owner.Width;//например по ширине самого юнита
            aimVector = new CCVector2(0, offset);

            // целимся - вычисляем угол поворота ствола 
            aimAngle = angle;
            GameManager.vMath.RotateVector(ref aimVector, aimAngle);
            // смещаем исходную точку снаряда относительно стрелка
            shellPos.X += aimVector.X;
            shellPos.Y += aimVector.Y;

            //окончательно целимся - задаем направление и скорость выстрела
            aimVector.X = 0; aimVector.Y = shellSpeed;
            GameManager.vMath.RotateVector(ref aimVector, aimAngle);
        }

        virtual public void Fire()
        {
            fireCount++;
        }
    }

    class WeaponCannon : Weapon
    {
        public WeaponCannon(GameObject obj) : base(obj)
        {
            weaponType = (int)WeaponType.CANNON;
            GameManager.player.last_used_gun = (WeaponType)weaponType;
        }
        override public void Fire()
        {
            if (Safe) // если оружие на предохранителе, то не стреляем
                return;
            System.Diagnostics.Debug.WriteLine("================FireOnce================= " + fireCount);
            GameObject shell = BulletFactory.Self.CreateNew(shellPos);//GameObject shell = new Bullet(shellPos);

            //окончательно целимся - задаем направление выстрела
            //velocity = new CCVector2(0, shellSpeed);
            //GameManager.vMath.RotateVector(ref velocity, aimAngle);

            shell.CurrentSpeed = shellSpeed;
            shell.move.DirectionAngle = aimAngle;
            //shell.move.VelocityY = aimVector.Y;
            //shell.move.VelocityX = aimVector.X;
            shell.playerSide = owner.playerSide;
            fireCount++;
        }
    }

    class WeaponAutoCannon : Weapon
    {
        int burst = 2;
        //int interval = 250; //0.25f;
        float interval = 0.25f;
        //int ActionTime = 0;

        public WeaponAutoCannon(GameObject obj) : base(obj)
        {
            weaponType = (int)WeaponType.AUTOCANNON;
            GameManager.player.last_used_gun = (WeaponType)weaponType;
        }
        override public void Fire()
        {
            if (Safe) // если оружие на предохранителе, то не стреляем
                return;
            
            owner.Schedule(FireOnce, interval, (uint)burst-1, 0);

            //ActionTime = 0;
            //Task task = new Task(FireBurst);
            //task.Start();
            //GameManager.gameplay.gameEventDirector.GamePause += task.
        }
        /*void FireBurst()
        {
            int i = 0; // 1!

            //await System.Threading.Tasks.Task.W
            while ( i < burst)
            {
                i++;
                Thread.Sleep(interval);
                FireOnce(ActionTime);
            }
        }*/
 
        void FireOnce(float time)
        {
            //System.Diagnostics.Debug.WriteLine("================FireOnce=================");
            Aim(aimAngle);//чтобы обновить начальные координаты второго снаряда

            GameObject shell = BulletFactory.Self.CreateNew(shellPos);

            shell.CurrentSpeed = shellSpeed;
            shell.move.DirectionAngle = aimAngle;
            //shell.move.VelocityY = aimVector.Y;
            //shell.move.VelocityX = aimVector.X;
            shell.playerSide = owner.playerSide;
            fireCount++;
        }
    }

    class WeaponMinigun : Weapon
    {
        int burst = 5;
        //int interval = 250; //0.25f;
        float interval = 0.1f;
        //int ActionTime = 0;

        public WeaponMinigun(GameObject obj) : base(obj)
        {
            weaponType = (int)WeaponType.MINIGUN;
            GameManager.player.last_used_gun = (WeaponType)weaponType;
            //GunFireRate = 1.3f;
        }
        override public void Fire()
        {
            if (Safe) // если оружие на предохранителе, то не стреляем
                return;
            //Aim(0);
            owner.Schedule(FireOnce, interval, (uint)burst - 1, 0);
        }
        void FireOnce(float time)
        {
            //System.Diagnostics.Debug.WriteLine("================FireOnce=================");
            int rnd = CCRandom.GetRandomInt(-5, 5);
            Aim(aimAngle + rnd);//

            GameObject shell = new Slug(shellPos);

            shell.CurrentSpeed = shellSpeed;
            shell.move.DirectionAngle = aimAngle;
            //shell.move.VelocityY = aimVector.Y;
            //shell.move.VelocityX = aimVector.X;
            shell.playerSide = owner.playerSide;
            fireCount++;
            GameManager.gameplay.allObjects.Add(shell);
        }
    }

    class WeaponLaser : Weapon
    {
        public WeaponLaser(GameObject obj) : base(obj)
        {
            weaponType = (int)WeaponType.LASER;
            GameManager.player.last_used_gun = (WeaponType)weaponType;
        }
        override public void Fire()
        {
            if (Safe) // если оружие на предохранителе, то не стреляем
                return;
            GameObject shell = new Laser(shellPos); //BulletFactory.Self.CreateNew(shellPos);//GameObject shell = new Bullet(shellPos);
            
            shell.CurrentSpeed = shellSpeed * 2;
            shell.move.DirectionAngle = 0; // aimAngle;
            shell.playerSide = owner.playerSide;
            fireCount++;
            shell.name += "_" + fireCount;
            GameManager.gameplay.allObjects.Add(shell);
            //System.Diagnostics.Debug.WriteLine("=== Fire: " + shell.name);
        }
    }

    class NoWeapon : Weapon
    {
        public NoWeapon(GameObject obj) : base(obj)
        { }
        override public void Fire()
        { }        
    }
}