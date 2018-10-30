namespace RiotGalaxy
{
    /// <summary>
    /// Класс реализует текущее состояние игры - например различные меню или игровой процесс
    /// методы:
    /// Enter()         - действия при входе в нужное состояние
    /// Update()        - что происходит в процессе
    /// OnBackPressed() - действия при нажатии кнопки назад на мобиле
    /// Exit()          - действия при выходе из сотояния
    /// </summary>
    public interface IGameState
    {
        //public GameState();
        void Enter();
        void Update(float time);
        bool OnBackPressed();
        void Exit();
    }

    class GameStateGameplay : IGameState
    {
        /// <summary>
        /// состояние геймплея
        /// </summary>
        public GameStateGameplay()
        { }
        public void Enter()
        {
            System.Diagnostics.Debug.WriteLine("=== GameStateGameProcess() ===");
            
            if (GameManager.level.cur_level_num > GameManager.level.totalNumberOfLevels)
            {
                //GameManager.GameOver("Победа! Все уровни пройдены"); // Всё, прошел игру
                ICommand cmd = new CommandLose();
                cmd.Execute();
                return;
            }
            if (!GameManager.level.LoadLevel(GameManager.cur_level))
            {
                //GameManager.GameOver("Error loading level");
                ICommand cmd = new CommandLose();
                cmd.Execute();
                return;
            }

            //if(GameManager.ScGame == null)
                GameManager.ScGame = new SceneGame(GameManager.GameView);
            //if(GameManager.gameplay == null)
                GameManager.gameplay = new Gameplay();
            GameManager.gameplay.Init();

            GameManager.ScGame.InitSceneObjects();
            GameManager.GoToScene(GameManager.ScGame);

        }
        public void Update(float time)
        {
            GameManager.gameplay.Activity(time);
            GameManager.ScGame.Activity(time);///!! Нужно убрать Activity из ScGame            
        }
        public void Exit()
        {
            if (GameManager.gameplay != null)
                GameManager.gameplay.CloseGameplay();
            if (GameManager.ScGame != null)
            {
                GameManager.ScGame.CloseScene();
                GameManager.ScGame.Dispose();///
                //GameManager.DeleteScene(GameManager.ScGame);
            }
        }
        public bool OnBackPressed()
        {
            ICommand cmd = new CommandNextLevelSplash();
            cmd.Execute();
            return false;
        }
    }
    class GameStateMainMenu : IGameState
    {
        public GameStateMainMenu()
        {
        }
        public void Enter()
        {
            System.Diagnostics.Debug.WriteLine("======== GameStateMainMenu() ========");

            GameManager.ScMainMenu = new SceneMainMenu(GameManager.GameView);
            GameManager.GoToScene(GameManager.ScMainMenu);

            //GameDelegate.gameManager.userInputHandler.touchListener.OnTouchesBegan = HandleTouchesBegan;
            //layer.AddEventListener(GameDelegate.gameManager.userInputHandler.touchListener);
        }
        public void Update(float time)
        {
            //GameManager.ScMainMenu.Update(time);            
            //GameManager.ScMainMenu.HandleTouchesBegan();
        }
        public void Exit()
        {
            if (GameManager.ScMainMenu != null)
            {
                GameManager.ScMainMenu.CloseScene();
                GameManager.DeleteScene(GameManager.ScMainMenu);
            }
                
        }
        public bool OnBackPressed()
        {
            return true;//выход из приложения
        }
    }

    class GameStateNextLevelSplash : IGameState
    {
        public GameStateNextLevelSplash()
        {
        }
        public void Enter()
        {
            System.Diagnostics.Debug.WriteLine("======== GameStateMainMenu() ========");
            
            GameManager.ScNextLevel = new SceneNextLevel(GameManager.GameView);
            GameManager.GoToScene(GameManager.ScNextLevel);
        }
        public void Update(float time)
        { }
        public void Exit()
        {
            if (GameManager.ScNextLevel != null)
            {
                GameManager.ScNextLevel.CloseScene();
                GameManager.DeleteScene(GameManager.ScNextLevel);
            }
        }
        public bool OnBackPressed()
        {
            ICommand cmd = new CommandMainMenuSplash();
            cmd.Execute();
            return false;
        }
    }

    class GameStateWinSplash : IGameState
    {
        public GameStateWinSplash()
        { }
        public void Enter()
        {
            GameManager.cur_level++;
            GameManager.ScWin = new SceneWin(GameManager.GameView);
            GameManager.GoToScene(GameManager.ScWin);
        }
        public void Update(float time)
        { }
        public void Exit()
        {
            if (GameManager.ScWin != null)
            {
                GameManager.ScWin.CloseScene();
                GameManager.DeleteScene(GameManager.ScWin); //for re-init scene            
            }
        }
        public bool OnBackPressed()
        {
            ICommand cmd = new CommandMainMenuSplash();
            cmd.Execute();
            return false;
        }
    }

    class GameStateLoseSplash : IGameState
    {
        string message = "";
        public GameStateLoseSplash()
        {            
        }
        public void Enter()
        {
            GameManager.ScLose = new SceneLose(GameManager.GameView, message);
            GameManager.GoToScene(GameManager.ScLose);
        }
        public void Enter(string msg)
        {
            message = msg;
            Enter();
        }
        public void Update(float time)
        { }
        public void Exit()
        {
            if (GameManager.ScLose != null)
            {
                GameManager.ScLose.CloseScene();
                GameManager.DeleteScene(GameManager.ScLose); //for re-init scene            
            }
        }
        public bool OnBackPressed()
        {
            ICommand cmd = new CommandMainMenuSplash();
            cmd.Execute();
            return false;
        }
    }
}