using CocosSharp;
using RiotGalaxy.Interface;
using RiotGalaxy.Utils;
using System.Diagnostics;

namespace RiotGalaxy
{
    /// <summary>
    /// Класс содержит все основные элементы игры и управляет ее состоянием
    /// </summary>
    public class GameManager
    {
        //public enum GameStateEn : int { INIT = 0, TITLE, GAME, GAMEOVER, WIN, PAUSE }
        //public static GameStateEn gameState = GameStateEn.TITLE, prevGameState = GameStateEn.TITLE;
        public static FileUtils fUtils;     // утилиты для работы с файлами
        public static Options opt;          // настройки игры
        public static IGameState state;     // состояние игры
        public static int cur_level = 1;    // с какого уровня начинаем
        public static Level level;          // текущий уровень
        public static Player player;        // игрок
        public static Gameplay gameplay;    // сам игровой процесс
        public InputHandler userInputHandler;   // обработчик юзерского ввода

        Timer timer;
        Stopwatch sw;
        public static CCNode sh;

        public static VectorMath vMath;     // здесь математика объектов
        public static SpriteLoader sLoader; // загружатель спрайтов

        public static CCGameView GameView;  // окно программы
        public static SceneMainMenu ScMainMenu;     // сцены
        public static SceneNextLevel ScNextLevel;
        public static SceneGame ScGame;
        public static SceneWin ScWin;
        public static SceneLose ScLose;

        protected float time = 0;

        //CCTextureCache cache = CCTextureCache.SharedTextureCache;

        ICommand cmd;

        public GameManager(CCGameView gameView)
        {
            GameView = gameView;
            GameView.Stats.Enabled = true; // отображает статистику в углу экрана
        }
        public void Init()
        {
            System.Diagnostics.Debug.WriteLine("=== GAME MANAGER - INIT ===");
            fUtils  = new FileUtils();
            opt     = new Options();
            player  = new Player();
            level   = new Level();
            userInputHandler = new InputHandler();

            timer   = new Timer();//виснет
            sw = Stopwatch.StartNew();
            sh = new CCNode();

            vMath   = new VectorMath();
            sLoader = new SpriteLoader();            
            //   cache.AddImage("background.jpg");            

            // звук
            //CCAudioEngine.SharedEngine.PlayBackgroundMusic(filename: "FruityFalls", loop: false);            
            CCAudioEngine.SharedEngine.EffectsVolume = 0.1f;
            CCAudioEngine.SharedEngine.BackgroundMusicVolume = 0.1f;
            CCAudioEngine.SharedEngine.StopAllEffects();

           

            cmd = new CommandMainMenuSplash();  // первым делом включаем сцену с главным меню
            cmd.Execute();

            //GameView.Scheduler.Schedule(
            //sh.Schedule(ShActivity);
            
            //Activity(time);
        }
        public void ShActivity(float time)
        {
            if(state != null)
                state.Update(time); //timer.GetSeconds()
        }
        /*public void Activity(float time)
        {
            // переход в первое состояние
            cmd = new CommandMainMenuSplash(); //CommandMainMenuSplash();  // первым делом включаем сцену с главным меню
            cmd.Execute();

            //if (state_ready == true)
            //{
            bool needExit = false;
            float ActionTime = 0;
            

            while (true)  //(!needExit && state != null)
            {
                //GameView.RenderFrame
                ActionTime += timer.GetSeconds();
                if (ActionTime > 0.01) // периодичность кадров в секунду
                {
                    ActionTime = 0;
                    /// нужно сюда запихнуть главный цикл игры
                    /// чтобы крутился метод Update текущего состояния
                    state.Update(timer.GetSeconds()); //timer.GetSeconds()
                }
            }            
        }*/
        public void ChangeState(IGameState newState)
        {            
            if (state != null)
                state.Exit();            
            state = newState;
            state.Enter();
        }
        public static void GoToScene(CCScene scene)
        {
            GameView.Director.ReplaceScene(scene);
        }
        public static void DeleteScene(CCScene scene)
        {
            System.Diagnostics.Debug.WriteLine("=== DeleteScene({0}) ===", scene.ToString());
            
            scene.RemoveAllChildren();
            scene.RemoveFromParent();
            scene.Dispose();
            //scene = null;
        }
    }//class
}//namespace