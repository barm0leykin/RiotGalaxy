using System.Collections.Generic;
using RiotGalaxy.Utils;

namespace RiotGalaxy.Weapons
{
    /// <summary>
    /// Параметры оружия по уровням. Грузятся из Content/Config/weapons.yaml (Load()).
    /// Значения по умолчанию (фолбэк) совпадают с оригинальным weapon.ini —
    /// игра работает и без файла. Поля WeaponOptions: burst, burstInterval, reloadSpeed, damage, shellSpeed.
    /// </summary>
    public static class WeaponConfig
    {
        public static int MaxWeaponLevel = 3;

        /// <summary>Магнит корабля — притяжение звёзд-очков (оборудование, из weapons.yaml).</summary>
        public class MagnetOptions
        {
            public float Radius { get; set; } = 250f;    // радиус действия, px
            public float PullSpeed { get; set; } = 300f;  // скорость притяжения звезды
            public float TurnSpeed { get; set; } = 360f;  // скорость доворота к игроку, град/сек
        }
        public static MagnetOptions Magnet = new MagnetOptions();

        // [cannon]  burst 1;1;2  bInterval 0.25  reload 2;1.5;2.4  damage 10  shellSpeed 200
        public static WeaponOptions[] Cannons =
        {
            new WeaponOptions(1, 0.25f, 2.0f,  10f, 200f),
            new WeaponOptions(1, 0.25f, 1.5f,  10f, 200f),
            new WeaponOptions(2, 0.25f, 2.4f,  10f, 200f),
        };

        // [minigun] burst 3;4;4  bInterval 0.1  reload 2;2;1.6  damage 4  shellSpeed 200
        public static WeaponOptions[] Miniguns =
        {
            new WeaponOptions(3, 0.1f, 2.0f, 4f, 200f),
            new WeaponOptions(4, 0.1f, 2.0f, 4f, 200f),
            new WeaponOptions(4, 0.1f, 1.6f, 4f, 200f),
        };

        // [laser]   burst 1;1;2  bInterval 0.25  reload 2.65;2;1.6  damage 10  shellSpeed 400
        public static WeaponOptions[] Lasers =
        {
            new WeaponOptions(1, 0.25f, 2.65f, 10f, 400f),
            new WeaponOptions(1, 0.25f, 2.0f,  10f, 400f),
            new WeaponOptions(2, 0.25f, 1.6f,  10f, 400f),
        };

        /// <summary>Загрузка параметров оружия из weapons.yaml (при ошибке остаются дефолты).</summary>
        public static void Load()
        {
            var data = Yaml.LoadAsset<WeaponsYaml>(Yaml.ConfigAsset("weapons.yaml"));
            if (data == null)
                return;

            if (data.MaxWeaponLevel > 0)
                MaxWeaponLevel = data.MaxWeaponLevel;

            var cannon = Convert(data.Cannon);
            var minigun = Convert(data.Minigun);
            var laser = Convert(data.Laser);
            if (cannon != null) Cannons = cannon;
            if (minigun != null) Miniguns = minigun;
            if (laser != null) Lasers = laser;

            if (data.Magnet != null)
            {
                if (data.Magnet.Radius > 0) Magnet.Radius = data.Magnet.Radius;
                if (data.Magnet.PullSpeed > 0) Magnet.PullSpeed = data.Magnet.PullSpeed;
                if (data.Magnet.TurnSpeed > 0) Magnet.TurnSpeed = data.Magnet.TurnSpeed;
            }
        }

        private static WeaponOptions[] Convert(List<WeaponLevelYaml> levels)
        {
            if (levels == null || levels.Count == 0)
                return null;
            var arr = new WeaponOptions[levels.Count];
            for (int i = 0; i < levels.Count; i++)
            {
                var l = levels[i];
                arr[i] = new WeaponOptions(l.Burst, l.BurstInterval, l.ReloadSpeed, l.Damage, l.ShellSpeed);
            }
            return arr;
        }

        // POCO под структуру weapons.yaml
        private class WeaponsYaml
        {
            public int MaxWeaponLevel { get; set; }
            public List<WeaponLevelYaml> Cannon { get; set; }
            public List<WeaponLevelYaml> Minigun { get; set; }
            public List<WeaponLevelYaml> Laser { get; set; }
            public MagnetYaml Magnet { get; set; }
        }
        private class MagnetYaml
        {
            public float Radius { get; set; }
            public float PullSpeed { get; set; }
            public float TurnSpeed { get; set; }
        }
        private class WeaponLevelYaml
        {
            public int Burst { get; set; }
            public float BurstInterval { get; set; }
            public float ReloadSpeed { get; set; }
            public float Damage { get; set; }
            public float ShellSpeed { get; set; }
        }
    }
}
