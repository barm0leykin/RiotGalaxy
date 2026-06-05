using System;
using Microsoft.Xna.Framework;
using RiotGalaxy.Managers;
using RiotGalaxy.Commands;
using RiotGalaxy.Interface;

namespace RiotGalaxy.Commands
{
    /// <summary>
    /// Классы команд для управления игрой
    /// Адаптировано из CocosSharp Command.cs
    /// </summary>
    
    public class CommandKillAll : ICommand
    {
        public CommandKillAll()
        {
        }
        
        public void Execute()
        {
            var gameObjects = GameManager.Instance.GameObjects;
            
            foreach (var obj in gameObjects)
            {
                if (obj != null && obj.GetType().Name.Contains("Enemy"))
                {
                    obj.IsAlive = false;
                }
            }
            MessageLog.Add("Уничтожить всех", Microsoft.Xna.Framework.Color.Orange);
        }
    }

    public class CommandWin : ICommand
    {
        public CommandWin()
        {
        }
        
        public void Execute()
        {
            GameManager.Instance.ChangeGameState(GameManager.GameState.Victory);
        }
    }

    public class CommandLose : ICommand
    {
        public CommandLose()
        {
        }
        
        public void Execute()
        {
            GameManager.Instance.ChangeGameState(GameManager.GameState.GameOver);
        }
    }

    public class CommandMainMenu : ICommand
    {
        public CommandMainMenu()
        {
        }
        
        public void Execute()
        {
            GameManager.Instance.ChangeGameState(GameManager.GameState.MainMenu);
        }
    }

    public class CommandStartGame : ICommand
    {
        public CommandStartGame()
        {
        }
        
        public void Execute()
        {
            GameManager.Instance.ChangeGameState(GameManager.GameState.Playing);
        }
    }

    public class CommandSwitchPause : ICommand
    {
        public CommandSwitchPause()
        {
        }
        
        public void Execute()
        {
            var currentState = GameManager.Instance.CurrentGameState;
            
            if (currentState == GameManager.GameState.Playing)
            {
                GameManager.Instance.ChangeGameState(GameManager.GameState.Paused);
            }
            else if (currentState == GameManager.GameState.Paused)
            {
                GameManager.Instance.ChangeGameState(GameManager.GameState.Playing);
            }
        }
    }

    public class CommandPause : ICommand
    {
        public CommandPause()
        {
        }
        
        public void Execute()
        {
            GameManager.Instance.ChangeGameState(GameManager.GameState.Paused);
        }
    }

    public class CommandResume : ICommand
    {
        public CommandResume()
        {
        }
        
        public void Execute()
        {
            GameManager.Instance.ChangeGameState(GameManager.GameState.Playing);
        }
    }

    public class CommandHpUp : ICommand
    {
        public CommandHpUp()
        {
        }
        
        public void Execute()
        {
            var player = GameManager.Instance.Player;
            if (player != null)
            {
                player.Health = player.MaxHealth;
                MessageLog.Add("Полное лечение", Microsoft.Xna.Framework.Color.Lime);
            }
        }
    }

    public class CommandPauseWeaponMenu : ICommand
    {
        MyButton btn_cannon, btn_minigun, btn_laser;
        
        public CommandPauseWeaponMenu()
        {
        }
        
        public void Execute()
        {
            var currentState = GameManager.Instance.CurrentGameState;
            
            if (currentState == GameManager.GameState.Playing)
            {
                CreateWeaponMenu();
                GameManager.Instance.ChangeGameState(GameManager.GameState.Paused);
            }
            else if (currentState == GameManager.GameState.Paused)
            {
                DeleteWeaponMenu();
                GameManager.Instance.ChangeGameState(GameManager.GameState.Playing);
            }
        }
        
        void CreateWeaponMenu()
        {
            // рисуем меню с выбором оружия
            var player = GameManager.Instance.Player;
            if (player == null) return;
            
            Vector2 menuPos = player.Position;
            int menuwidth = 200; //4*60 + 3 промежутка по 10 пикселов
            menuPos.X -= (menuwidth / 2);
            menuPos.Y += 90;
            
            if (menuPos.X < 0)
                menuPos.X = 30; // + пол ширины спрайта тк у них точка привязки по центру
            if (menuPos.X + menuwidth + 30 > GameManager.Instance.ScreenWidth)
                menuPos.X = GameManager.Instance.ScreenWidth - menuwidth - 30;
            
            menuPos.X += 30; //пол ширины спрайта, тк у них точка привязки по центру
            
            btn_cannon = new ButtonCannon(menuPos);
            InputManager.Instance.AddButtonHandler(btn_cannon);
            
            menuPos.X += 70;
            btn_minigun = new ButtonMinigun(menuPos);
            InputManager.Instance.AddButtonHandler(btn_minigun);
            
            menuPos.X += 70;
            btn_laser = new ButtonLaser(menuPos);
            InputManager.Instance.AddButtonHandler(btn_laser);
        }
        
        void DeleteWeaponMenu()
        {
            // удаляем иконки выбора оружия
            var guiButtons = InputManager.Instance.GUIButtons;
            
            foreach (var btn in guiButtons.ToArray())
            {
                if (btn is ButtonCannon || btn is ButtonMinigun || btn is ButtonLaser)
                {
                    InputManager.Instance.RemoveButtonHandler(btn);
                }
            }
        }
    }
}