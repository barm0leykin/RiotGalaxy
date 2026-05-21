using System;
using System.IO;
using System.Collections;
using RiotGalaxy.Utils;
using System.Collections.Generic;
//using RiotGalaxyAndroid;

namespace RiotGalaxy
{
    
    public class Options
    {
        public struct PlShipOptions
        {
            public float maxspeed;
            public float acceleration;
            public float brakingSpeed;
        }

        public struct Weapons
        {            
            public WeaponOptions[] cannons;
            public WeaponOptions[] miniguns;
            public WeaponOptions[] lasers;
        }
        public static Weapons weapons;
        WeaponOptions[] currentWO; // дл€ парсинга
        ArrayList fileContent;
        private string opt_filename;
        private string weaposn_filename;

        public static int maxWeaponLevel = 3;
        

        public static PlShipOptions plShipOptions;
        public static int width = 1280;
        public static int height = 762;//720
        public const bool sound_on = true;
        public const int sound_vol = 50; // % громкости
        public const bool music_on = true;
        public const int music_vol = 50; // % громкости

        public Options()
        {
            //инициализаци€ хранилища настроек оружи€
            weapons = new Weapons
            {
                cannons = new WeaponOptions[maxWeaponLevel],
                miniguns = new WeaponOptions[maxWeaponLevel],
                lasers = new WeaponOptions[maxWeaponLevel]
            };
            for (int i = 0; i < maxWeaponLevel; i++)
            {
                weapons.cannons[i] = new WeaponOptions();
                weapons.miniguns[i] = new WeaponOptions();
                weapons.lasers[i] = new WeaponOptions();
            }
            currentWO = weapons.cannons;////ну например

            opt_filename = "Content/options.ini";
            weaposn_filename = "Content/weapon.ini";
            LoadOptions();
            LoadWeponParams();
        }
        public bool LoadOptions()
        {
            //загрузка с помощью разбора текстового файла
            if (GameManager.fUtils.OpenTextFile(opt_filename))
            {
                fileContent = GameManager.fUtils.GetFileContent();
                foreach (object str in fileContent)
                {
                    ParceOptFile(str.ToString());
                }
                return true;
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Ќе удалось загрузить настройки");
                return false;
            }
        }
        public bool LoadWeponParams()
        {
            //загрузка с помощью разбора текстового файла
            if (GameManager.fUtils.OpenTextFile(weaposn_filename))
            {
                fileContent = GameManager.fUtils.GetFileContent();
                foreach (object str in fileContent)
                {
                    ParceWeponFile(str.ToString());
                }
                return true;
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Ќе удалось загрузить настройки оружи€");
                return false;
            }
        }

        private void ParceWeponFile(string line)
        {
            if (line.Contains("#")) // комментарий обрезаем
            {
                line = line.Remove(line.IndexOf("#"));
                if (line.Length == 0) //если в строке кроме комментари€ ничего не было, то выходим
                    return;
            }
            //
            switch (line)//можно так
            {
                case "[cannon]":
                    {
                        currentWO = weapons.cannons;
                        break;
                    }
                case "[minigun]":
                    {
                        currentWO = weapons.miniguns;
                        break;
                    }
                case "[laser]":
                    {
                        currentWO = weapons.lasers;
                        break;
                    }
                default:
                    break;
            }
            /*if (line.Contains("[minigun]"))
            {
                currentWO = weapons.minigun;
            }
            if (line.Contains("[laser]"))
            {
                currentWO = weapons.laser;
            }*/

            if (line.Contains("burst"))
            {
                line = line.Substring(line.IndexOf("=") + 1);   // берем правую половину строки
                string[] mass = line.Split(';');
                for (int i=0; i < maxWeaponLevel; i++)
                {                    
                    currentWO[i].burst = Int32.Parse(mass[i]);
                }
            }
            else if (line.Contains("bInterval"))
            {
                line = line.Substring(line.IndexOf("=") + 1);   // берем правую половину строки
                string[] mass = line.Split(';');
                for (int i = 0; i < maxWeaponLevel; i++)
                {
                    currentWO[i].burstInterval = float.Parse(mass[i]);
                }
            }
            else if (line.Contains("reloadSpeed"))
            {
                line = line.Substring(line.IndexOf("=") + 1);   // берем правую половину строки
                string[] mass = line.Split(';');
                for (int i = 0; i < maxWeaponLevel; i++)
                {
                    currentWO[i].reloadSpeed = float.Parse(mass[i]);
                }
            }
            else if (line.Contains("damage"))
            {
                line = line.Substring(line.IndexOf("=") + 1);   // берем правую половину строки
                string[] mass = line.Split(';');
                for (int i = 0; i < maxWeaponLevel; i++)
                {
                    currentWO[i].damage = float.Parse(mass[i]);
                }
            }
            else if (line.Contains("shellSpeed"))
            {
                line = line.Substring(line.IndexOf("=") + 1);   // берем правую половину строки
                string[] mass = line.Split(';');
                for (int i = 0; i < maxWeaponLevel; i++)
                {
                    currentWO[i].shellSpeed = float.Parse(mass[i]);
                }
            }
        }
        private void ParceOptFile(string line)
        {
            line.ToLower();

            if (line.Contains("width"))
            {
                line = line.Substring(line.IndexOf("=") + 1);
                System.Diagnostics.Debug.WriteLine("width: " + line);
                width = Int32.Parse(line);
            }
            else if(line.Contains("height"))
            {
                line = line.Substring(line.IndexOf("=") + 1);
                System.Diagnostics.Debug.WriteLine("width: " + line);
                height = Int32.Parse(line);
            }

            if (line.Contains("player-name"))
            {
                line = line.Substring(line.IndexOf("=") + 1);
                System.Diagnostics.Debug.WriteLine("Parce: player-name=" + line);
                GameManager.player.Name = line;
            }
            else if (line.Contains("player-maxhp"))
            {
                line = line.Substring(line.IndexOf("=") + 1);
                System.Diagnostics.Debug.WriteLine("Parce: player-name=" + line);
                GameManager.player.MaxHp = Int32.Parse(line); 
            }
            else if (line.Contains("player-maxspeed"))
            {
                line = line.Substring(line.IndexOf("=") + 1);
                System.Diagnostics.Debug.WriteLine("Parce: player-maxspeed=" + line);                
                plShipOptions.maxspeed = Int32.Parse(line);
            }
            else if (line.Contains("player-acc"))
            {
                line = line.Substring(line.IndexOf("=") + 1);
                System.Diagnostics.Debug.WriteLine("Parce: player-acc=" + line);
                plShipOptions.acceleration = Int32.Parse(line);
            }
            else if (line.Contains("player-brakespeed"))
            {
                line = line.Substring(line.IndexOf("=") + 1);
                System.Diagnostics.Debug.WriteLine("Parce: player-brakespeed=" + line);
                plShipOptions.brakingSpeed = Int32.Parse(line);
            }

        }//ParceLine()
    }
}