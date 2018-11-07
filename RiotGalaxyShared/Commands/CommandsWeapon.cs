using CocosSharp;
using RiotGalaxy.Interface;
using RiotGalaxy.Objects.Weapons;

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
            //GameManager.gameplay.iface.Message("=== CommandUpgradeGun ===", (int)GUI.MsgType.DEBUG);

            GameManager.gameplay.playerShip.gun.Upgrade();
        }
    }//CommandUpgradeGun

    class CommandPauseWeaponMenu : ICommand
    {
        MyButton btn_cannon, btn_minigun, btn_laser;
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
            int menuwidth = 200; //4*60 + 3 промежутка по 10 пикселов
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
            GameManager.gameplay.iface.Message("- Weapon Cannon -", (int)GUI.MsgType.GAME_CONSOL);
            GameManager.gameplay.playerShip.ChangeWeapon(Weapon.WeaponType.CANNON);
        }
    }//CommandChAutoWeaponCannon
    class CommandChWeaponMinigun : ICommand
    {
        public CommandChWeaponMinigun()
        {
        }
        public void Execute()
        {
            GameManager.gameplay.iface.Message("- Weapon Minigun -", (int)GUI.MsgType.GAME_CONSOL);
            GameManager.gameplay.playerShip.ChangeWeapon(Weapon.WeaponType.MINIGUN);
        }
    }//CommandChAutoWeaponMinigun
    class CommandChWeaponLaser : ICommand
    {
        public CommandChWeaponLaser()
        {
        }
        public void Execute()
        {
            GameManager.gameplay.iface.Message("- Weapon Laser -", (int)GUI.MsgType.GAME_CONSOL);
            GameManager.gameplay.playerShip.ChangeWeapon(Weapon.WeaponType.LASER);
        }
    }//CommandChAutoWeaponLaser
}
