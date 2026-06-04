using System;
using System.Globalization;
using System.IO;
using Microsoft.Xna.Framework;
using RiotGalaxy.Managers;

namespace RiotGalaxy.Utils
{
    /// <summary>
    /// Сохранение/загрузка пользовательских настроек в файл settings.ini рядом с приложением.
    /// Пока хранит громкость эффектов.
    /// </summary>
    public static class GameSettings
    {
        private const string Key = "effects_volume=";
        private static string FilePath => Path.Combine(AppContext.BaseDirectory, "settings.ini");

        public static void Load()
        {
            try
            {
                if (!File.Exists(FilePath))
                    return;
                foreach (var raw in File.ReadAllLines(FilePath))
                {
                    string line = raw.Trim();
                    if (line.StartsWith(Key))
                    {
                        string val = line.Substring(Key.Length);
                        if (float.TryParse(val, NumberStyles.Float, CultureInfo.InvariantCulture, out float v))
                            AudioManager.Instance.EffectsVolume = MathHelper.Clamp(v, 0f, 1f);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"=== Settings load failed: {ex.Message} ===");
            }
        }

        public static void Save()
        {
            try
            {
                float v = AudioManager.Instance.EffectsVolume;
                File.WriteAllText(FilePath, Key + v.ToString(CultureInfo.InvariantCulture) + Environment.NewLine);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"=== Settings save failed: {ex.Message} ===");
            }
        }
    }
}
