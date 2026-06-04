namespace RiotGalaxy.Weapons
{
    /// <summary>
    /// Параметры оружия по уровням.
    /// Значения перенесены из оригинального CocoSharp/.../Content/weapon.ini.
    /// (В оригинале грузились из файла через Options.LoadWeponParams; здесь захардкожены.
    ///  TODO: вернуть загрузку из файла на этапе системы уровней/конфигов.)
    /// Параметры WeaponOptions: burst, burstInterval, reloadSpeed, damage, shellSpeed.
    /// </summary>
    public static class WeaponConfig
    {
        public const int MaxWeaponLevel = 3;

        // [cannon]  burst 1;1;2  bInterval 0.25  reload 2;1.5;2.4  damage 10  shellSpeed 200
        public static readonly WeaponOptions[] Cannons =
        {
            new WeaponOptions(1, 0.25f, 2.0f,  10f, 200f),
            new WeaponOptions(1, 0.25f, 1.5f,  10f, 200f),
            new WeaponOptions(2, 0.25f, 2.4f,  10f, 200f),
        };

        // [minigun] burst 3;4;4  bInterval 0.1  reload 2;2;1.6  damage 4  shellSpeed 200
        public static readonly WeaponOptions[] Miniguns =
        {
            new WeaponOptions(3, 0.1f, 2.0f, 4f, 200f),
            new WeaponOptions(4, 0.1f, 2.0f, 4f, 200f),
            new WeaponOptions(4, 0.1f, 1.6f, 4f, 200f),
        };

        // [laser]   burst 1;1;2  bInterval 0.25  reload 2.65;2;1.6  damage 10  shellSpeed 400
        public static readonly WeaponOptions[] Lasers =
        {
            new WeaponOptions(1, 0.25f, 2.65f, 10f, 400f),
            new WeaponOptions(1, 0.25f, 2.0f,  10f, 400f),
            new WeaponOptions(2, 0.25f, 1.6f,  10f, 400f),
        };
    }
}
