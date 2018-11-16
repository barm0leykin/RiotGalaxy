using RiotGalaxy.Interface;
using RiotGalaxy.Objects.Weapons;
using static RiotGalaxy.GameEventDirector;

namespace RiotGalaxy
{
    public class Player
    {
        public string Name { get; set; }
        int score;
        public int Score
        {
            get
            {
                return score;
            }
            set
            {
                score = value;
                if(GameManager.gameplay != null)
                    GameManager.gameplay.gameEventDirector.AddEvent(EventsID.SCORE_UPD);
            }
        }
        public struct Upgrades // апгрэйды оружия
        {
            public int cannon;
            public int minigun;
            public int laser;
        }
        public Upgrades upgrades;
        public int MaxHp { get; set; }        
        //       public int Hp { get; set; }
        //public float GunFireRate { get; set; }
        //public Weapon.WeaponType last_used_gun = Weapon.WeaponType.CANNON; 
        public Pers pers;  // персонаж для диалогов в катсценах
        
        public Player()
        {
            Name = "-= AK =-";
            pers = new Pers("player");  // player тк по этому имени загружается спрайт
            
            upgrades.cannon = 0;
            upgrades.minigun = 0;
            upgrades.laser = 0;
            //Level = 1;
            Score = 0;
            MaxHp = 50;
            //            Hp = MaxHp;
            Init();
        }
        void Init()
        {
        }
    }
}