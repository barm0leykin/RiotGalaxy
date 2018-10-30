using CocosSharp;
using RiotGalaxy.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace RiotGalaxy.Commands
{
    class CommandUpgradeGun : ICommand
    {
        public CommandUpgradeGun()
        {
        }
        public void Execute()
        {
            System.Diagnostics.Debug.WriteLine("=== CommandUpgradeGun ===");
            GameManager.gameplay.iface.Message("=== CommandUpgradeGun ===", (int)HUD.MsgType.GAME_CONSOL);

            GameManager.player.GunFireRate -= 0.1f; // уменьшаем интервал между выстрелами
            if (GameManager.player.GunFireRate < 0.5f)
                GameManager.player.GunFireRate = 0.5f;

            GameManager.gameplay.playerShip.Upgrade();
        }
    }//CommandUpgradeGun

    class CommandPauseWeaponMenu : ICommand
    {
        MyButton btn_cannon, btn_auto_cannon, btn_minigun, btn_laser;
        public CommandPauseWeaponMenu()
        {
        }
        public void Execute()
        {
            System.Diagnostics.Debug.WriteLine("=== CommandShowWeaponMenu ===");
            if (GameManager.gameplay.Pause == false)
            {
                CreateWeaponMenu();
                GameManager.gameplay.Pause = true;
            }
            else
            {
                DeleteWeaponMenu();
                GameManager.gameplay.Pause = false;
            }

        }
        void CreateWeaponMenu()
        {
            // рисуем меню с выбором оружия
            CCPoint menuPos = GameManager.gameplay.playerShip.Position;
            int menuwidth = 270; //4*60 + 3 промежутка по 10 пикселов
            menuPos.X -= (menuwidth / 2);
            menuPos.Y += 90;
            if (menuPos.X < 0)
                menuPos.X = 30; // + пол ширины спрайта тк у них точка привязки по центру
            if (menuPos.X + menuwidth + 30 > Options.width)
                menuPos.X = Options.width - menuwidth - 30;

            menuPos.X += 30; //пол ширины спрайта, тк у них точка привязки по центру
            btn_cannon = new ButtonCannon(menuPos);
            GameManager.gameplay.iface.AddButton(btn_cannon);

            menuPos.X += 70;
            btn_auto_cannon = new ButtonAutoCannon(menuPos);
            GameManager.gameplay.iface.AddButton(btn_auto_cannon);

            menuPos.X += 70;
            btn_minigun = new ButtonMinigun(menuPos);
            GameManager.gameplay.iface.AddButton(btn_minigun);

            menuPos.X += 70;
            btn_laser = new ButtonLaser(menuPos);
            GameManager.gameplay.iface.AddButton(btn_laser);
        }
        void DeleteWeaponMenu()
        {
            // удаляем иконки выбора оружия
            MyButton btn;
            btn = GameManager.gameplay.iface.FindByName("btn_cannon");
            GameManager.gameplay.iface.DelButton(btn);
            btn = GameManager.gameplay.iface.FindByName("btn_auto_cannon");
            GameManager.gameplay.iface.DelButton(btn);
            btn = GameManager.gameplay.iface.FindByName("btn_minigun");
            GameManager.gameplay.iface.DelButton(btn);
            btn = GameManager.gameplay.iface.FindByName("btn_laser");
            GameManager.gameplay.iface.DelButton(btn);
        }
    }//CommandSwitchPause


    class CommandChWeaponCannon : ICommand
    {
        public CommandChWeaponCannon()
        {
        }
        public void Execute()
        {
            //System.Diagnostics.Debug.WriteLine("=== CommandChWeaponCannon ===");
            //GameManager.gameplay.iface.Message("- Weapon Cannon -", (int)HUD.MsgType.MESSAGE);
            GameManager.gameplay.iface.Message("- Weapon Cannon -", (int)HUD.MsgType.GAME_CONSOL);
            GameManager.gameplay.playerShip.gun = new Objects.Weapons.WeaponCannon(GameManager.gameplay.playerShip);
        }
    }//CommandChWeaponCannon
    class CommandChWeaponAutoCannon : ICommand
    {
        public CommandChWeaponAutoCannon()
        {
        }
        public void Execute()
        {
            //System.Diagnostics.Debug.WriteLine("=== CommandChAutoWeaponCannon ===");
            //GameManager.gameplay.iface.Message("- Weapon AutoCannon -", (int)HUD.MsgType.MESSAGE);
            GameManager.gameplay.iface.Message("- Weapon AutoCannon -", (int)HUD.MsgType.GAME_CONSOL);
            GameManager.gameplay.playerShip.gun = new Objects.Weapons.WeaponAutoCannon(GameManager.gameplay.playerShip);
        }
    }//CommandChAutoWeaponCannon
    class CommandChWeaponMinigun : ICommand
    {
        public CommandChWeaponMinigun()
        {
        }
        public void Execute()
        {
            //System.Diagnostics.Debug.WriteLine("=== CommandChAutoWeaponMinigun ===");
            //GameManager.gameplay.iface.Message("- Weapon Minigun -", (int)HUD.MsgType.MESSAGE);
            GameManager.gameplay.iface.Message("- Weapon Minigun -", (int)HUD.MsgType.GAME_CONSOL);
            GameManager.gameplay.playerShip.gun = new Objects.Weapons.WeaponMinigun(GameManager.gameplay.playerShip);
        }
    }//CommandChAutoWeaponMinigun
    class CommandChWeaponLaser : ICommand
    {
        public CommandChWeaponLaser()
        {
        }
        public void Execute()
        {
            //System.Diagnostics.Debug.WriteLine("=== CommandChAutoWeaponLaser ===");
            //GameManager.gameplay.iface.Message("- Weapon Laser -", (int)HUD.MsgType.MESSAGE);
            GameManager.gameplay.iface.Message("- Weapon Laser -", (int)HUD.MsgType.GAME_CONSOL);
            GameManager.gameplay.playerShip.gun = new Objects.Weapons.WeaponLaser(GameManager.gameplay.playerShip);
        }
    }//CommandChAutoWeaponLaser
}
