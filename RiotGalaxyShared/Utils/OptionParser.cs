using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace RiotGalaxy.Utils
{
    class OptionParser
    {
        struct Param
        {
            public string paramName;
            public string value;
        }

        ArrayList fileContent;
        ArrayList parameters;
        // test vals
        int width, height;
        float time;
        string name;
        //
        public OptionParser()
        {

        }
        public bool LoadFile(string filename)
        {
            //загрузка с помощью разбора текстового файла
            if (GameManager.fUtils.OpenTextFile(filename))
            {
                fileContent = GameManager.fUtils.GetFileContent();
                foreach (string str in fileContent)
                {
                    Parce(str.ToString());
                }
                return true;
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Не удалось загрузить настройки оружия");
                return false;
            }
        }
        public void Parce(string line)
        {
            line.ToLower();         // всё в нижний регистр
            if (line.Contains("#")) // комментарий обрезаем
            {
                line = line.Remove(line.IndexOf("#"));
                if (line.Length == 0) //если в строке кроме комментария ничего не было, то выходим
                    return;
            }
            if (!line.Contains("="))
                return;

            // делим строку на две части - название параметра и значение
            Param p;
            p.paramName = line.Substring(0, line.IndexOf("=") - 1);
            p.value     = line.Substring(line.IndexOf("=") + 1);
            parameters.Add(p);
            // теперь нужно определить тип параметра - строка, целое или с точкой
            /*foreach (char c in value)
                if (Char.IsLetter(c))
                {
                }
            else
                {
                    if(Char.IsDigit(c))
                    { }
                }*/
        }
        public int GetInt(string param)
        {
            foreach(Param p in parameters)
            {
                if(p.paramName == param)
                {
                    try
                    {
                        return Int32.Parse(p.value);
                    }
                    catch (Exception e)
                    {
                        System.Diagnostics.Debug.WriteLine("Неверный параметр int", e.Message);                        
                        return 0;
                    }
                }
            }
            System.Diagnostics.Debug.WriteLine("Не удалось найти подходящий параметр int");
            return 0;
        }
        public void GetFloat(string param)
        {
            foreach (Param p in parameters)
            {
                if (p.paramName == param)
                {
                    float.Parse(p.value);
                }
            }
        }
        public void ParceString(string param, string value)
        {
        }

    }
}
