using System;
using System.IO;
using System.Collections;
using RiotGalaxy.Utils;
//using RiotGalaxyAndroid;

namespace RiotGalaxy
{
    public class Options
    {
        //private ArrayList fileContent;
        //private string fileText = "";

        public INIManager inimanager;
        private string filename;

        public static int width = 128;
        public static int height = 76;//720
        public const bool sound_on = true;
        public const int sound_vol = 50; // % громкости
        public const bool music_on = true;
        public const int music_vol = 50; // % громкости

        public Options()
        {
            //string filename = "Content/Levels/level" + level_num + ".txt";
            filename = "Content/options.txt";
            inimanager = new INIManager(filename);///

            LoadOptions();
        }
        public bool LoadOptions()
        {
            //ParceINIFile();

            //загрузка с помощью разбора текстового файла
            if (GameManager.fUtils.OpenTextFile(filename))
            {
                ArrayList file = GameManager.fUtils.GetFileContent();
                foreach (object str in file)
                {
                    ParceLine(str.ToString());
                }
                return true; 
            }
            else
                return false;
        }
        private void ParceINIFile()
        {
            string inidata;
            inidata = inimanager.GetPrivateString("main", "width");
            width = Int32.Parse(inidata);
            inidata = inimanager.GetPrivateString("main", "height");
            height = Int32.Parse(inidata);
        }
        private void ParceLine(string line)
        {
            line.ToLower();

            if (line.Contains("width="))
            {
                line = line.Substring(line.IndexOf("=") + 1);
                System.Diagnostics.Debug.WriteLine("width=" + line);
                width = Int32.Parse(line);
            }
            else if(line.Contains("height="))
            {
                line = line.Substring(line.IndexOf("=") + 1);
                System.Diagnostics.Debug.WriteLine("width=" + line);
                height = Int32.Parse(line);
            }

            if (line.Contains("player-name:"))
            {
                line = line.Substring(line.IndexOf(":") + 1);
                System.Diagnostics.Debug.WriteLine("Parce: player-name:" + line);
                GameManager.player.Name = line;
            }
            else if (line.Contains("player-maxhp:"))
            {
                line = line.Substring(line.IndexOf(":") + 1);
                System.Diagnostics.Debug.WriteLine("Parce: player-name:" + line);
                GameManager.player.MaxHp = Int32.Parse(line); 
            }
            else if (line.Contains("player-gun-rate: "))
            {
                line = line.Substring(line.IndexOf(":") + 1);
                System.Diagnostics.Debug.WriteLine("Parce: player-name:" + line);
                GameManager.player.GunFireRate = float.Parse(line);//Convert.ToSingle(line);
            }
            else if (line.Contains("player-gun-dmg:"))
            {
                line = line.Substring(line.IndexOf(":") + 1);
                System.Diagnostics.Debug.WriteLine("Parce: player-name:" + line);
                //GameManager.player.Name = line;
            }
        }//ParceLine()
    }
}