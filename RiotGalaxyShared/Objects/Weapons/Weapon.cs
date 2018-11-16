using CocosSharp;
using RiotGalaxy.Factories;
using RiotGalaxy.Interface;
using System.Threading;
using System.Threading.Tasks;

public class WeaponOptions
{
    public int burst;          // колво выстрелов в очереди
    public float burstInterval;   // промежуток между выстрелами в очереди
    public float reloadSpeed;  // время перезарядки (очередями)
    public float damage;       // убойная сила
    public float shellSpeed;   // скорость снаряда
}

namespace RiotGalaxy.Objects.Weapons
{
    public class Weapon
    {
        public enum WeaponType : int { CANNON = 0, MINIGUN, LASER }
        public int weaponType;

        public WeaponOptions wpOptions;

        protected int fireCount = 0;    // счетчик выстрелов
        //protected int damage = 10;      // убойная сила
        public bool Safe { get; set; }  // предохранитель

        protected GameObject owner;
        protected GameObject shell;
        protected CCPoint shellPos;

        //protected float shellSpeed = 200;
        protected float aimAngle;
        protected CCVector2 aimVector;        

        public Weapon(GameObject obj, int lvl = 0)
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
            aimVector.X = 0; aimVector.Y = wpOptions.shellSpeed;
            GameManager.vMath.RotateVector(ref aimVector, aimAngle);
        }

        public void Fire()
        {
            if (Safe) // если оружие на предохранителе, то не стреляем
                return;
            owner.Schedule(FireOnce, wpOptions.burstInterval, (uint)wpOptions.burst - 1, 0);
        }
        virtual public void FireOnce(float time)
        {
            if (Safe) // если оружие на предохранителе, то не стреляем
                return;
            if (shell == null)
                return;
            //System.Diagnostics.Debug.WriteLine("================FireOnce=================");
            shell.CurrentSpeed  = wpOptions.shellSpeed;
            shell.Damage        = wpOptions.damage;
            shell.move.DirectionAngle = aimAngle;
            shell.playerSide    = owner.playerSide;
            fireCount++;
            shell.name += "_" + fireCount;
            GameManager.gameplay.allObjects.Add(shell);
        }

        virtual public void Upgrade()
        {
        }
        public void LoadWeaponOptions(WeaponOptions opt)
        {
            wpOptions = opt;
        }
    }

    class WeaponCannon : Weapon
    {        
        public WeaponCannon(GameObject obj, int lvl = 0) : base(obj, lvl)
        {
            weaponType = (int)WeaponType.CANNON;
            //GameManager.player.last_used_gun = (WeaponType)weaponType;
            wpOptions = Options.weapons.cannons[lvl];
        }
        override public void Upgrade()
        {
            if (GameManager.player.upgrades.cannon +1 < Options.maxWeaponLevel)
            {
                GameManager.player.upgrades.cannon++;
                GameManager.gameplay.iface.Message("Gun level " + GameManager.player.upgrades.cannon, (int)GUI.MsgType.GAME_CONSOL);
                LoadWeaponOptions(Options.weapons.cannons[GameManager.player.upgrades.cannon]);
            }
        }
        override public void FireOnce(float time)
        {
            Aim(aimAngle);//чтобы обновить начальные координаты второго снаряда
            //GameObject shell = BulletFactory.Self.CreateNew(shellPos);
            shell = new Bullet(shellPos);
            base.FireOnce(time);
        }
    }

    class WeaponMinigun : Weapon
    {
        public WeaponMinigun(GameObject obj, int lvl = 0) : base(obj, lvl)
        {
            weaponType = (int)WeaponType.MINIGUN;
            //GameManager.player.last_used_gun = (WeaponType)weaponType;
            wpOptions = Options.weapons.miniguns[lvl];
        }
        override public void Upgrade()
        {
            if (GameManager.player.upgrades.minigun + 1 < Options.maxWeaponLevel)
            {
                GameManager.player.upgrades.minigun++;
                GameManager.gameplay.iface.Message("Minigun level " + GameManager.player.upgrades.minigun, (int)GUI.MsgType.GAME_CONSOL);
                LoadWeaponOptions(Options.weapons.miniguns[GameManager.player.upgrades.minigun]);
            }
        }
        override public void FireOnce(float time)
        {
            int accuracy = CCRandom.GetRandomInt(-5, 5);
            Aim(aimAngle + accuracy);//
            shell = new Slug(shellPos);
            base.FireOnce(time);
        }
    }

    class WeaponLaser : Weapon
    {
        public WeaponLaser(GameObject obj, int lvl = 0) : base(obj, lvl)
        {
            weaponType = (int)WeaponType.LASER;
            //GameManager.player.last_used_gun = (WeaponType)weaponType;
            wpOptions = Options.weapons.lasers[lvl];
        }
        override public void Upgrade()
        {
            if (GameManager.player.upgrades.laser + 1 < Options.maxWeaponLevel)
            {
                GameManager.player.upgrades.laser++;
                GameManager.gameplay.iface.Message("Laser level " + GameManager.player.upgrades.laser, (int)GUI.MsgType.GAME_CONSOL);
                LoadWeaponOptions(Options.weapons.lasers[GameManager.player.upgrades.laser]);
            }
        }
        override public void FireOnce(float time)
        {
            shell = new Laser(shellPos); 
            base.FireOnce(time);
        }
    }

    class NoWeapon : Weapon
    {
        public NoWeapon(GameObject obj) : base(obj)
        { }
    }
}