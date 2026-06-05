namespace RiotGalaxy.Utils
{
    /// <summary>
    /// Параметры бонусов из Content/Config/bonuses.yaml (фолбэк — дефолты в коде).
    /// </summary>
    public static class BonusConfig
    {
        public class Data
        {
            public int HpUpAmount { get; set; } = 25; // HP за BonusHpUp
            public int StarScore { get; set; } = 10;  // очки за BonusStar
        }

        public static Data Current { get; private set; } = new Data();

        public static void Load()
        {
            var data = Yaml.LoadAsset<Data>(Yaml.ConfigAsset("bonuses.yaml"));
            if (data != null)
                Current = data;
        }
    }
}
