using System;
using Microsoft.Xna.Framework;
using RiotGalaxy.Commands;
using RiotGalaxy.Managers;

namespace RiotGalaxy.Commands
{
    /// <summary>
    /// Команды для управления оружием
    /// Адаптировано из CocosSharp CommandsWeapon.cs
    /// </summary>
    
    public class CommandUpgradeGun : ICommand
    {
        public CommandUpgradeGun()
        {
        }
        
        public void Execute()
        {
            Console.WriteLine("=== CommandUpgradeGun ===");
            var player = GameManager.Instance.Player;
            if (player != null)
            {
                // В будущем здесь будет обновление оружия
                // player.gun.Upgrade();
                Console.WriteLine("Weapon升级功能待实现");
            }
        }
    }

    public class CommandChWeaponCannon : ICommand
    {
        public CommandChWeaponCannon()
        {
        }
        
        public void Execute()
        {
            Console.WriteLine("=== CommandChWeaponCannon ===");
            var player = GameManager.Instance.Player;
            if (player != null)
            {
                // В будущем здесь будет смена оружия
                // player.ChangeWeapon(Weapon.WeaponType.CANNON);
                Console.WriteLine("武器切换到加农炮功能待实现");
            }
        }
    }

    public class CommandChWeaponMinigun : ICommand
    {
        public CommandChWeaponMinigun()
        {
        }
        
        public void Execute()
        {
            Console.WriteLine("=== CommandChWeaponMinigun ===");
            var player = GameManager.Instance.Player;
            if (player != null)
            {
                // В будущем здесь будет смена оружия
                // player.ChangeWeapon(Weapon.WeaponType.MINIGUN);
                Console.WriteLine("武器切换到机枪功能待实现");
            }
        }
    }

    public class CommandChWeaponLaser : ICommand
    {
        public CommandChWeaponLaser()
        {
        }
        
        public void Execute()
        {
            Console.WriteLine("=== CommandChWeaponLaser ===");
            var player = GameManager.Instance.Player;
            if (player != null)
            {
                // В будущем здесь будет смена оружия
                // player.ChangeWeapon(Weapon.WeaponType.LASER);
                Console.WriteLine("武器切换到激光功能待实现");
            }
        }
    }
}