using System;
using System.IO;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace RiotGalaxy.Utils
{
    /// <summary>
    /// Общие помощники для работы с YAML-конфигами.
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

        /// <summary>Путь к конфигу рядом с приложением: &lt;base&gt;/Content/Config/&lt;file&gt;.</summary>
        public static string ConfigPath(string file) =>
            Path.Combine(AppContext.BaseDirectory, "Content", "Config", file);

        /// <summary>Десериализовать YAML-файл в объект T. Возвращает default при ошибке/отсутствии.</summary>
        public static T LoadFile<T>(string path) where T : class
        {
            try
            {
                if (!File.Exists(path))
                {
                    Console.WriteLine($"=== YAML not found: {path} ===");
                    return null;
                }
                return Deserializer.Deserialize<T>(File.ReadAllText(path));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"=== YAML load failed '{path}': {ex.Message} ===");
                return null;
            }
        }
    }
}
