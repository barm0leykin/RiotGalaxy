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
            var player = GameManager.Instance.Player;
            if (player != null)
            {
                // В будущем здесь будет обновление оружия
                // player.gun.Upgrade();
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
            var player = GameManager.Instance.Player;
            if (player != null)
            {
                // В будущем здесь будет смена оружия
                // player.ChangeWeapon(Weapon.WeaponType.CANNON);
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
            var player = GameManager.Instance.Player;
            if (player != null)
            {
                // В будущем здесь будет смена оружия
                // player.ChangeWeapon(Weapon.WeaponType.MINIGUN);
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
            var player = GameManager.Instance.Player;
            if (player != null)
            {
                // В будущем здесь будет смена оружия
                // player.ChangeWeapon(Weapon.WeaponType.LASER);
            }
        }
    }
}