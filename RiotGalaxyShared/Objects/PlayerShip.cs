using CocosSharp;
using RiotGalaxy.Factories;
using System.Threading.Tasks;
using RiotGalaxy.Objects.Weapons;
using RiotGalaxy.Interface;
using RiotGalaxy.Objects.ObjBehavior;

namespace RiotGalaxy.Objects
{
    public class PlayerShip : GameObject
    {
        public int rounds = 0;
        public bool GodMode { get; set; }
        CCLabel label;
        public new ObjBehPlayerMove move; // !!! тк класс ObjBehPlayerMove специфичен дл€ игрока, пишем new
        private ButtonPlayerPause btn_pl_pause; // по нажатию на корабль игра ставитс€ на паузу, по€вл€етс€ меню выбора оружи€
        protected float ActionTime = 0;

        int hp;
        new public int Hp
        {
            get
            {
                return hp;
            }
            set
            {
                hp = value;
                // √енерируем событие о том, что жизн€ изменилась. ƒл€ HUD и тд
                if (GameManager.gameplay != null)
                    GameManager.gameplay.gameEventDirector.AddEvent((int)GameEventDirector.EventsID.HP_UPD);
            }
        }

        public PlayerShip(CCPoint pos)
        {
            objectType = ObjType.PLAYER;
            playerSide = true;
            Position = pos;

            // задаем параметры поведени€ кораблика (паттерн —тратеги€)
            move = new ObjBehPlayerMove(this);
            coll = new BehHardCollision(this);            
            shoot = new ObjBehShootUp(this);
            //gun = new WeaponCannon(this);
            draw = new ObjBehDrawSprite(this);
            PrepareWeapon();

            //HpMax = 100;
            Hp = GameManager.player.MaxHp;
            Damage = 100;   // сила тарана своей тушкой
            //GunFireRate = GameManager.player.GunFireRate; // 1f;

            // Sprite
            
            //sprite = GameManager.sLoader.Load("ship.png"); //new CCSprite("ship"); //
            //sprite.AnchorPoint = CCPoint.AnchorMiddle;
            //AddChild(sprite);
            draw.LoadGraphics("ship.png");

            // прикрепл€ем к кораблю невидимую кнопку паузы
            btn_pl_pause = new ButtonPlayerPause(new CCPoint(0, 0), this);
            btn_pl_pause.AnchorPoint = CCPoint.AnchorMiddle;
            AddChild(btn_pl_pause);
            GameManager.gameplay.iface.AddButton(btn_pl_pause);
            //GameManager.gameplay.userInputHandler.AddButnHandler(btn_pl_pause);
            //AddLabel();

            Upgrade(); // подт€нуть параметры корабл€ из параметров игрока
        }
        ~PlayerShip()
        {
        }
        private void PrepareWeapon()
        {
            /*if (GameManager.player.last_used_gun != null)
                gun = GameManager.player.last_used_gun;
            else
                gun = new WeaponCannon(this); // new WeaponBurstCannon(this);*/

            switch(GameManager.player.last_used_gun)
            {
                case Weapon.WeaponType.CANNON:
                    {
                        gun = new WeaponCannon(this);
                        break;
                    }
                case Weapon.WeaponType.AUTOCANNON:
                    {
                        gun = new WeaponAutoCannon(this);
                        break;
                    }
                case Weapon.WeaponType.MINIGUN:
                    {
                        gun = new WeaponMinigun(this);
                        break;
                    }
                case Weapon.WeaponType.LASER:
                    {
                        gun = new WeaponLaser(this);
                        break;
                    }
                default:
                    {
                        gun = new WeaponCannon(this);
                        break;
                    }
            }
        }
        override public void Activity(float time)
        {
            // move
            move.Move(time);

            ActionTime += time;
            if (ActionTime > gun.GunFireRate) // пора стрел€ть   // if (ActionTime > GameManager.player.GunFireRate)
            {
                ActionTime = 0;
                shoot.Shoot(time);
                //System.Diagnostics.Debug.WriteLine("=== Shoot: " + rounds);
            }
        }
        private void AddLabel()
        {
            label = new CCLabel("", "Arial", 20, CCLabelFormat.SystemFont);
            label.Color = CCColor3B.Gray;
            label.AnchorPoint = CCPoint.AnchorMiddle;
            label.PositionY = 50;
            AddChild(label);
        }
        public void Upgrade()
        {
            gun.GunFireRate = GameManager.player.GunFireRate;
            //Schedule(shoot.Shoot, interval: GameManager.player.GunFireRate);
        }
        public override bool Collision(GameObject obj)
        {
            if (!obj.playerSide)    // на свои снар€ды не реагируем
                Hit(obj);
            return true;
        }
        public void HpUp(int hpup)
        {
            Hp += hpup;
            if (Hp > GameManager.player.MaxHp)
                Hp = GameManager.player.MaxHp;
            //GameManager.gameplay.gameEventDirector.AddEvent((int)GameEventDirector.EventsID.HP_UPD);
        }
        void Hit(GameObject obj)
        {
            System.Diagnostics.Debug.WriteLine("=== Player hit!: " + obj.Damage + " xp, by " + obj.objectType);
            if (GodMode)
                return;
            Hp -= obj.Damage;
            //GameManager.gameplay.gameEventDirector.AddEvent((int)GameEventDirector.EventsID.HP_UPD);

            AddMyshield();

            if (Hp <= 0)
            {
                CCAudioEngine.SharedEngine.PlayEffect("explode1");
                needToDelete = true;
            }
        }
        async void AddMyshield(int delay = 2000) // временна€ неубиваемость
        {
            //System.Diagnostics.Debug.WriteLine("Add shield");
            // Sprite
            CCSprite shield_sprite_op = GameManager.sLoader.Load("shield.png");
            shield_sprite_op.Opacity = 40;
            shield_sprite_op.Scale = 1.2f;
            //shield_sprite_op.ContentSize = new CCSize(110,110);
            shield_sprite_op.AnchorPoint = CCPoint.AnchorMiddle;
            AddChild(shield_sprite_op);

            CCSprite shield_sprite = GameManager.sLoader.Load("shield.png");
            shield_sprite.Opacity = 80;
            shield_sprite.AnchorPoint = CCPoint.AnchorMiddle;
            AddChild(shield_sprite);
            GodMode = true;

            await Task.Delay(delay);
            RemoveChild(shield_sprite);
            RemoveChild(shield_sprite_op);
            GodMode = false;
        }
    }
}