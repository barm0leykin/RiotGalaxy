using RiotGalaxy.GameObjects;
using RiotGalaxy.Managers;

namespace RiotGalaxy.Commands
{
    /// <summary>
    /// Команды для управления оружием (адаптировано из CocosSharp CommandsWeapon.cs).
    /// </summary>

    public class CommandUpgradeGun : ICommand
    {
        public void Execute()
        {
            GameManager.Instance.Player?.UpgradeWeapon();
        }
    }

    public class CommandChWeaponCannon : ICommand
    {
        public void Execute()
        {
            GameManager.Instance.Player?.ChangeWeapon(WeaponType.Cannon);
        }
    }

    public class CommandChWeaponMinigun : ICommand
    {
        public void Execute()
        {
            GameManager.Instance.Player?.ChangeWeapon(WeaponType.MachineGun);
        }
    }

    public class CommandChWeaponLaser : ICommand
    {
        public void Execute()
        {
            GameManager.Instance.Player?.ChangeWeapon(WeaponType.Laser);
        }
    }

    /// <summary>Тестовая команда: перейти к следующему уровню.</summary>
    public class CommandNextLevel : ICommand
    {
        public void Execute()
        {
            MessageLog.Add("Следующий уровень", Microsoft.Xna.Framework.Color.Yellow);
            GameManager.Instance.DebugNextLevel();
        }
    }
}
