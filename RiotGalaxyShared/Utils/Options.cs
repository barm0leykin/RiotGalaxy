using System;
using System.IO;
using System.Collections;
//using RiotGalaxyAndroid;

namespace RiotGalaxy
{
    public class Options
    {
        //private ArrayList fileContent;
        //private string fileText = "";


        public const int width = 1280;
        public const int height = 768;//720
        public const bool sound_on = true;
        public const int sound_vol = 50; // % громкости
        public const bool music_on = true;
        public const int music_vol = 50; // % громкости

        public bool LoadOptions()
        {
            string filename = "Content/options.txt";

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

        private void ParceLine(string line)
        {
            //line.ToLower();
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