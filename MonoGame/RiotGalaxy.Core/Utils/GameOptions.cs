namespace RiotGalaxy.Utils
{
    /// <summary>
    /// Основные настройки игры из Content/Config/options.yaml (аналог options.ini).
    /// Значения по умолчанию совпадают с оригиналом — игра работает и без файла.
    /// </summary>
    public static class GameOptions
    {
        public static int ScreenWidth = 1280;
        public static int ScreenHeight = 768;

        public static string PlayerName = "Nuke SkyRocker";
        public static int PlayerMaxHp = 50;
        public static float PlayerMaxSpeed = 450f;
        public static float PlayerAcceleration = 1500f;
        public static float PlayerBrakeSpeed = 800f;
        public static float PlayerInvulnTime = 2.0f; // секунды неуязвимости после попадания

        public static void Load()
        {
            var data = Yaml.LoadAsset<OptionsYaml>(Yaml.ConfigAsset("options.yaml"));
            if (data == null)
                return;

            if (data.Screen != null)
            {
                if (data.Screen.Width > 0) ScreenWidth = data.Screen.Width;
                if (data.Screen.Height > 0) ScreenHeight = data.Screen.Height;
            }
            if (data.Player != null)
            {
                if (!string.IsNullOrWhiteSpace(data.Player.Name)) PlayerName = data.Player.Name;
                if (data.Player.MaxHp > 0) PlayerMaxHp = data.Player.MaxHp;
                if (data.Player.MaxSpeed > 0) PlayerMaxSpeed = data.Player.MaxSpeed;
                if (data.Player.Acceleration > 0) PlayerAcceleration = data.Player.Acceleration;
                if (data.Player.BrakeSpeed > 0) PlayerBrakeSpeed = data.Player.BrakeSpeed;
                if (data.Player.InvulnTime > 0) PlayerInvulnTime = data.Player.InvulnTime;
            }
        }

        // POCO под структуру options.yaml (имена в YAML — camelCase)
        private class OptionsYaml
        {
            public ScreenYaml Screen { get; set; }
            public PlayerYaml Player { get; set; }
        }
        private class ScreenYaml
        {
            public int Width { get; set; }
            public int Height { get; set; }
        }
        private class PlayerYaml
        {
            public string Name { get; set; }
            public int MaxHp { get; set; }
            public float MaxSpeed { get; set; }
            public float Acceleration { get; set; }
            public float BrakeSpeed { get; set; }
            public float InvulnTime { get; set; }
        }
    }
}
