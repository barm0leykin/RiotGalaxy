using System;
using System.IO;
using Microsoft.Xna.Framework;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace RiotGalaxy.Utils
{
    /// <summary>
    /// Общие помощники для работы с YAML-конфигами.
    ///
    /// Упакованный read-only контент (Content/Config|Levels|Routes) грузится
    /// кросс-платформенно через <see cref="TitleContainer.OpenStream"/> — работает
    /// и на DesktopGL (файлы рядом с приложением), и на Android (ассеты в APK).
    /// Пути — относительные, со слешами '/'.
    ///
    /// Пользовательские файлы для записи (settings.yaml) идут через <see cref="LoadFile"/>
    /// по абсолютному пути в writable-каталоге.
    /// </summary>
    public static class Yaml
    {
        public static readonly IDeserializer Deserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .IgnoreUnmatchedProperties()
            .Build();

        public static readonly ISerializer Serializer = new SerializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();

        /// <summary>Относительный путь к конфигу в контенте: Content/Config/&lt;file&gt;.</summary>
        public static string ConfigAsset(string file) => "Content/Config/" + file;

        /// <summary>
        /// Загрузить YAML-ассет из бандла приложения (Content/...). Кросс-платформенно.
        /// Возвращает null при ошибке/отсутствии.
        /// </summary>
        public static T LoadAsset<T>(string relativePath) where T : class
        {
            try
            {
                using var stream = TitleContainer.OpenStream(relativePath);
                using var reader = new StreamReader(stream);
                return Deserializer.Deserialize<T>(reader);
            }
            catch (FileNotFoundException)
            {
                Log.Debug($"YAML asset not found: {relativePath}");
                return null;
            }
            catch (Exception ex)
            {
                Log.Error($"YAML asset load failed '{relativePath}': {ex.Message}");
                return null;
            }
        }

        /// <summary>Есть ли ассет в бандле (проверка через попытку открытия потока).</summary>
        public static bool AssetExists(string relativePath)
        {
            try
            {
                using var stream = TitleContainer.OpenStream(relativePath);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Десериализовать YAML-файл по абсолютному пути (writable-данные, напр. settings).
        /// Возвращает null при ошибке/отсутствии.
        /// </summary>
        public static T LoadFile<T>(string path) where T : class
        {
            try
            {
                if (!File.Exists(path))
                {
                    Log.Debug($"YAML file not found: {path}");
                    return null;
                }
                return Deserializer.Deserialize<T>(File.ReadAllText(path));
            }
            catch (Exception ex)
            {
                Log.Error($"YAML load failed '{path}': {ex.Message}");
                return null;
            }
        }
    }
}
