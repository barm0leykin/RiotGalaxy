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
using RiotGalaxy.Interface;
using RiotGalaxy.Objects.Weapons;
using static RiotGalaxy.GameEventDirector;

namespace RiotGalaxy
{
    public class Player
    {
        public string Name { get; set; }
        public int Level { get; set; } // Всмы уровень крутости)                  
        //public int Score { get; set; }
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
                    GameManager.gameplay.gameEventDirector.AddEvent((int)EventsID.SCORE_UPD);
            }
        }
        public int MaxHp { get; set; }        
        //       public int Hp { get; set; }
        public float GunFireRate { get; set; }
        public Weapon.WeaponType last_used_gun = Weapon.WeaponType.CANNON; 
        public Pers pers;  // персонаж для диалогов в катсценах
        
        public Player()
        {
            Name = "-= AK =-";
            pers = new Pers("player");  // player тк по этому имени загружается спрайт
            Level = 1;
            Score = 0;
            MaxHp = 50;
            //            Hp = MaxHp;
            GunFireRate = 1.8f;
            //last_used_gun = GameManager.player.last_used_gun;//

            Init();
        }
        void Init()
        {
        }
    }
}