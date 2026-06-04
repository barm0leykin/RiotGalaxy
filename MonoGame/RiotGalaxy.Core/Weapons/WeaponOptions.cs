namespace RiotGalaxy.Weapons
{
    /// <summary>
    /// Параметры оружия. Точная копия структуры WeaponOptions из CocosSharp.
    /// </summary>
    public class WeaponOptions
    {
        public int burst;            // кол-во выстрелов в очереди
        public float burstInterval;  // промежуток между выстрелами в очереди (сек)
        public float reloadSpeed;    // время перезарядки между очередями (сек)
        public float damage;         // убойная сила
        public float shellSpeed;     // скорость снаряда (пикс/сек)

        public WeaponOptions() { }

        public WeaponOptions(int burst, float burstInterval, float reloadSpeed, float damage, float shellSpeed)
        {
            this.burst = burst;
            this.burstInterval = burstInterval;
            this.reloadSpeed = reloadSpeed;
            this.damage = damage;
            this.shellSpeed = shellSpeed;
        }
    }
}
