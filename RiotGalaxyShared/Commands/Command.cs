using RiotGalaxy.Objects;
using CocosSharp;
using System.Collections.Generic;
using static RiotGalaxy.Objects.GameObject;
using RiotGalaxy.SFX;
using RiotGalaxy.Interface;

namespace RiotGalaxy
{
    public interface ICommand
    {
        void Execute();
    }

    class NoCommand : ICommand
    {
        public NoCommand() { }
        public void Execute()
        {
        }
    }

    class CommandKillAll : ICommand
    {
        public CommandKillAll()
        {
        }
        public void Execute()
        {
            System.Diagnostics.Debug.WriteLine("=== CommandKillAll ===");
            foreach (GameObject objects in GameManager.gameplay.allObjects)
            {
                if (objects.objectType == ObjType.ENEMY)
                    objects.Kill();//objects.Hit(10000);//.needToDelete = true;    // Мочим всех врагов!
            }        
        }
    }//CommandKillAll

    class CommandWin : ICommand
    {
        public CommandWin()
        {
        }
        public void Execute()
        {
            System.Diagnostics.Debug.WriteLine("=== CommandWin ===");
            GameManager.gameplay.iface.Message("=== CommandWin ===", (int)HUD.MsgType.GAME_CONSOL);

            IGameState state = new GameStateWinSplash();
            GameDelegate.gameManager.ChangeState(state);
        }
    }//CommandWin
    class CommandLose : ICommand
    {
        public CommandLose()
        {
        }
        public void Execute()
        {
            System.Diagnostics.Debug.WriteLine("=== CommandLose ===");

            IGameState state = new GameStateLoseSplash();
            GameDelegate.gameManager.ChangeState(state);
        }
    }//CommandLose
    class CommandNextLevelSplash : ICommand
    {
        public CommandNextLevelSplash()
        {
        }
        public void Execute()
        {
            System.Diagnostics.Debug.WriteLine("=== CommandNextLevelSplash ===");

            IGameState state = new GameStateNextLevelSplash();
            GameDelegate.gameManager.ChangeState(state);
        }
    }//CommandLose

    class CommandMainMenuSplash : ICommand
    {
        public CommandMainMenuSplash()
        {
        }
        public void Execute()
        {
            System.Diagnostics.Debug.WriteLine("=== CommandMainMenuSplash ===");

            IGameState state = new GameStateMainMenu();
            GameDelegate.gameManager.ChangeState(state);
        }
    }//CommandLose

    class CommandStartNextLevel : ICommand
    {
        public CommandStartNextLevel()
        {
        }
        public void Execute()
        {
            System.Diagnostics.Debug.WriteLine("=== CommandStartNextLevel ===");            

            IGameState state = new GameStateGameplay();
            GameDelegate.gameManager.ChangeState(state);
        }
    }//CommandStartNextLevel

    class CommandHpUp : ICommand
    {
        public CommandHpUp()
        {
        }
        public void Execute()
        {
            System.Diagnostics.Debug.WriteLine("=== CommandHpUp ===");
            GameManager.gameplay.iface.Message("=== CommandHpUp ===", (int)HUD.MsgType.GAME_CONSOL);
            GameManager.gameplay.playerShip.HpUp(GameManager.player.MaxHp);
            //GameManager.gameplay.gameEventDirector.AddEvent((int)GameEventDirector.EventsID.HP_UPD); //создаем событие
        }
    }//CommandHpUp

    class CommandSpawnRandomBonus : ICommand
    {
        CCPoint pos;
        public CommandSpawnRandomBonus(CCPoint objpos)
        {
            pos = objpos;
        }
        public void Execute()
        {
            System.Diagnostics.Debug.WriteLine("=== CommandSpawnRandomBonus ===");

            // Выдаем бонус
            int rnd = CCRandom.GetRandomInt(1, 100);
            if (rnd >= 1 && rnd < 5)
                GameManager.gameplay.SpawnBonus(pos, Bonus.BonusType.NUKE_BOMB);
            if (rnd >= 70 && rnd < 80)
                GameManager.gameplay.SpawnBonus(pos, Bonus.BonusType.HP_UP);
            if (rnd > 80 && rnd < 90)
                GameManager.gameplay.SpawnBonus(pos, Bonus.BonusType.BULLET_UP);
        }
    }//CommandSpawnRandomBonus

    class CommandStarBonus : ICommand
    {
        CCPoint pos;
        public CommandStarBonus(CCPoint objpos)
        {
            pos = objpos;
        }
        public void Execute()
        {
            System.Diagnostics.Debug.WriteLine("=== CommandStarBonus ===");

            // Выдаем бонус
            GameManager.gameplay.SpawnBonus(pos, Bonus.BonusType.STAR);
        }
    }//CommandStarBonus

    class CommandSpawnSFX : ICommand
    {
        CCPoint pos;
        public CommandSpawnSFX(CCPoint objpos)
        {
            pos = objpos;
        }
        public void Execute()
        {
            System.Diagnostics.Debug.WriteLine("=== CommandSpawnSFX ===");
            GameManager.gameplay.SpawnSfx(pos, Sfx.SfxType.BLAST);
        }
    }//CommandSpawnSFX


    class CommandSwitchPause : ICommand
    {
        public CommandSwitchPause()
        {
        }
        public void Execute()
        {
            System.Diagnostics.Debug.WriteLine("=== CommandPause ===");
            //GameManager.ScGame.Unschedule(GameManager.gameplay.Activity); //Pause();
            if (GameManager.gameplay.Pause == false)
                GameManager.gameplay.Pause = true;
            else
                GameManager.gameplay.Pause = false;
        }
    }//CommandSwitchPause

    class CommandPause : ICommand
    {
        public CommandPause()
        {
        }
        public void Execute()
        {
            System.Diagnostics.Debug.WriteLine("=== CommandPause ===");
            //GameManager.ScGame.Unschedule(GameManager.gameplay.Activity); //Pause();
            GameManager.gameplay.Pause = true;  // еще надо остановить звездопад (спецэффекты)
            
        }
    }//CommandPause
    class CommandResume : ICommand
    {
        public CommandResume()
        {
        }
        public void Execute()
        {
            System.Diagnostics.Debug.WriteLine("=== CommandPause ===");
            //GameManager.ScGame.Resume();
            //GameManager.ScGame.Schedule(GameManager.gameplay.Activity);
            GameManager.gameplay.Pause = false;
        }
    }//CommandResume


}