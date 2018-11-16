using CocosSharp;
using System.Threading.Tasks;
using RiotGalaxy.Objects.Weapons;
using RiotGalaxy.Interface;
using RiotGalaxy.Objects.ObjBehavior;
using static RiotGalaxy.Objects.Weapons.Weapon;

namespace RiotGalaxy.Objects
{
    public class PlayerShip : GameObject
    {
        public int rounds = 0;
        public bool GodMode { get; set; }
        CCLabel label;
        public new ObjBehPlayerMove move; // !!! �� ����� ObjBehPlayerMove ���������� ��� ������, ����� new
        private ButtonPlayerPause btn_pl_pause; // �� ������� �� ������� ���� �������� �� �����, ���������� ���� ������ ������
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
                // ���������� ������� � ���, ��� ����� ����������. ��� HUD � ��
                if (GameManager.gameplay != null)
                    GameManager.gameplay.gameEventDirector.AddEvent((int)GameEventDirector.EventsID.HP_UPD);
            }
        }

        public PlayerShip(CCPoint pos)
        {
            objectType = ObjType.PLAYER;
            name = "PlayerShip";
            playerSide = true;
            Position = pos;

            // ������ ��������� ��������� ��������� (������� ���������)
            move = new ObjBehPlayerMove(this);
            coll = new BehHardCollision(this);            
            shoot = new ObjBehShootUp(this);
            gun = new WeaponCannon(this);
            draw = new ObjBehDrawSprite(this);
            //PrepareWeapon();

            //HpMax = 100;
            Hp = GameManager.player.MaxHp;
            Damage = 100;   // ���� ������ ����� ������

            // Sprite
            draw.LoadGraphics("ship.png");

            // ����������� � ������� ��������� ������ �����
            btn_pl_pause = new ButtonPlayerPause(new CCPoint(0, 0), this);
            btn_pl_pause.AnchorPoint = CCPoint.AnchorMiddle;
            AddChild(btn_pl_pause);
            GameManager.gameplay.iface.AddButton(btn_pl_pause);
            //GameManager.gameplay.userInputHandler.AddButnHandler(btn_pl_pause);
            //AddLabel();
        }
        public void Init()
        {
            ActionTime = 0;
            gun.Safe = false;
            Position = new CCPoint(GameManager.ScGame.gameplayLayer.ContentSize.Width / 2.0f, 50);
        }
        override public void Activity(float time)
        {
            // move
            move.Move(time);

            ActionTime += time;
            if (ActionTime > gun.wpOptions.reloadSpeed) // ���� ��������   // if (ActionTime > GameManager.player.GunFireRate)
            {
                ActionTime = 0;
                shoot.Shoot(time);
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
        public void ChangeWeapon(WeaponType weapon)
        {
            switch(weapon)
            {
                case WeaponType.CANNON:
                    {
                        gun = new WeaponCannon(this, GameManager.player.upgrades.cannon);
                        break;
                    }
                case WeaponType.MINIGUN:
                    {
                        gun = new WeaponMinigun(this, GameManager.player.upgrades.minigun);
                        break;
                    }
                case WeaponType.LASER:
                    {
                        gun = new WeaponLaser(this, GameManager.player.upgrades.laser);
                        break;
                    }
                default:
                    break;
            }
        }
        public override bool Collision(GameObject obj)
        {
            if (!obj.playerSide)    // �� ���� ������� �� ���������
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
            Hp -= (int)obj.Damage;
            //GameManager.gameplay.gameEventDirector.AddEvent((int)GameEventDirector.EventsID.HP_UPD);

            AddMyshield();

            if (Hp <= 0)
            {
                CCAudioEngine.SharedEngine.PlayEffect("explode1");
                needToDelete = true;
            }
        }
        async void AddMyshield(int delay = 2000) // ��������� �������������
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