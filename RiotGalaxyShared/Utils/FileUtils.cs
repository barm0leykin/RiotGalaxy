using System;
using System.Collections;
using System.IO;
//using RiotGalaxyAndroid;
//using RiotGalaxyAndroid.DLL;

namespace RiotGalaxy.Utils
{
    public class FileUtils
    {
        private ArrayList fileContent;

        public FileUtils()
        {
        }
        public ArrayList GetFileContent()
        {
            return fileContent;
        }
        public bool OpenTextFile(string filename)
        {
            fileContent = new ArrayList();
            try
            {
                System.Diagnostics.Debug.WriteLine("!!!!!!!!!====================================TRY open file " + filename + " ================================!!!!!!!!!");
                using (StreamReader sr = new StreamReader(GameDelegate.assets.Open(filename)))
                {
                    string line;
                    char separator = System.Globalization.CultureInfo.CurrentCulture.NumberFormat.CurrencyDecimalSeparator[0]; //Разделитель в зависимости от региональных настроек
                    while ((line = sr.ReadLine()) != null)
                    {
                        line = line.Replace(',', separator);//меняем знак разделителя на местный
                        line = line.Replace('.', separator);
                        fileContent.Add(line);
                        System.Diagnostics.Debug.WriteLine("line: " + line);
                        //ParceLine(line);
                    }
                    sr.Dispose();
                    //fileText = sr.ReadToEnd();
                }
            }
            catch (FileNotFoundException e)
            {
                System.Diagnostics.Debug.WriteLine("!!!!!!!!!====-------------------------------===FILE NOT FOUND===---------------------------------------====!!!!!!!!!");
                System.Diagnostics.Debug.WriteLine(e);
                System.Diagnostics.Debug.WriteLine(e.Message);
                return false;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("!!!!!!!!!====-----------------------------------===CATCH===---------------------------------------------====!!!!!!!!!");
                System.Diagnostics.Debug.WriteLine(e);
                System.Diagnostics.Debug.WriteLine(e.Message);
                return false;
            }
            return true;
        }
    }
}
