using CocosSharp;
using RiotGalaxy.Interface;

public interface IScene
{
    void CloseScene();
    void CreateTouchListener();
    void RemoveTouchListener();
}
namespace RiotGalaxy
{
    public class SceneGame : CCScene
    {
        CCLayer backgroundLayer;
        public CCLayer gameplayLayer;
        public CCLayer hudLayer;
        //CCRenderTexture render;

        private bool HudNeedUpdate = false;

        CCLabel labelLvl;
        CCLabel labelHp;
        CCLabel labelRounds;
        CCLabel labelEnemies;
        CCLabel labelRes;

        //CCScheduler sch;

        public SceneGame(CCGameView gameView) : base(gameView)
        {
            System.Diagnostics.Debug.WriteLine("=== SceneGame() ===");
            CreateBackgroundLayer();
            CreateGameLayer();
            CreateHUDLayer();

            //Schedule(GameDelegate.gameManager.Activity);
        }
        protected override void AddedToScene()
        {
            base.AddedToScene();
            //const bool useRenderTextures = true;
            //GameView.Stats.Enabled = true;
        }

        public void Init()
        {
            //CreateBackgroundLayer();
            //CreateGameLayer();
            //CreateHUDLayer();

            // регистрируем делегат в качестве обработчика событий для обновления HUD
            //GameManager.gameplay.gameEventDirector.RegisterScoreHandler(UpdateHUD);

            // а тут - через событие
            GameManager.gameplay.gameEventDirector.PointsUpd += UpdateHUD;
            GameManager.gameplay.gameEventDirector.HpUpd += UpdateHUD;

            // вешаем кнопки вверху экрана
            MyButton b_pause = new ButtonPause(new CCPoint(970, 728));
            GameDelegate.gameManager.userInputHandler.AddButnHandler(b_pause);
            hudLayer.AddChild(b_pause);

            MyButton b_win = new ButtonWin(new CCPoint(1040, 728));
            GameDelegate.gameManager.userInputHandler.AddButnHandler(b_win);
            hudLayer.AddChild(b_win);

            MyButton b_kill = new ButtonKillAll(new CCPoint(1110, 728));
            GameDelegate.gameManager.userInputHandler.AddButnHandler(b_kill);
            hudLayer.AddChild(b_kill);

            MyButton b_upgrade_gun = new ButtonUpgradeGun(new CCPoint(1180, 728));
            GameDelegate.gameManager.userInputHandler.AddButnHandler(b_upgrade_gun);
            hudLayer.AddChild(b_upgrade_gun);

            MyButton b_hp_up = new ButtonHpUP(new CCPoint(1250, 728));
            GameDelegate.gameManager.userInputHandler.AddButnHandler(b_hp_up);
            hudLayer.AddChild(b_hp_up);

            // цепляем обработчик ввода к слою
            gameplayLayer.AddEventListener(GameDelegate.gameManager.userInputHandler.touchListener);

            HudNeedUpdate = true;
            Schedule(Activity);
        }
        public void Activity(float time)
        {
            if (HudNeedUpdate)
            {
                UpdateHUD();
                HudNeedUpdate = false;
            }
            GameDelegate.gameManager.userInputHandler.HandleScGameInput();

            GameManager.gameplay.Activity(time); //// Убрать отсюда            
        }//Activity

        void CreateBackgroundLayer()
        {
            backgroundLayer = new CCLayer();
            this.AddLayer(backgroundLayer);

            var sprite = new CCSprite("background_blue.jpg")
            {
                IsAntialiased = true,
                AnchorPoint = CCPoint.AnchorLowerLeft
            };
            backgroundLayer.AddChild(sprite);
        }

        void CreateGameLayer()
        {
            // Игровой слой
            gameplayLayer = new CCLayer();
            this.AddLayer(gameplayLayer);            
        }
        void CreateHUDLayer()
        {
            hudLayer = new CCLayer();
            this.AddLayer(hudLayer);

            // Текст    
            labelRes = new CCLabel(GameView.ViewSize.Width.ToString() + " x" + GameView.ViewSize.Height.ToString(), "Arial", 30, CCLabelFormat.SystemFont)
            {
                AnchorPoint = CCPoint.AnchorLowerLeft,
                Color = CCColor3B.Red,
                PositionX = 1100,
                PositionY = 50
            };
            hudLayer.AddChild(labelRes);

            //labelEnemies = new CCLabel("Осталось врагов: ", "SCConvoy.fnt", 40, CCLabelFormat.SystemFont)
            labelEnemies = new CCLabel("Осталось врагов: ", "Arial", 40, CCLabelFormat.SystemFont)
            {
                Scale = 0.7f,
                PositionX = gameplayLayer.ContentSize.Width / 2.0f,
                PositionY = 650
            };
            hudLayer.AddChild(labelEnemies);

            labelLvl = new CCLabel("Level #: " + GameManager.level.cur_level_num + "/" + GameManager.level.totalNumberOfLevels + " " 
                + GameManager.level.levelDescription, "Arial", 40, CCLabelFormat.SystemFont)
            {
                Scale = 0.7f,
                AnchorPoint = CCPoint.AnchorLowerLeft,
                PositionX = 20,
                PositionY = 650
            };
            hudLayer.AddChild(labelLvl);


            labelRounds = new CCLabel("Rounds: ", "Arial", 30, CCLabelFormat.SystemFont)
            {
                AnchorPoint = CCPoint.AnchorLowerLeft,
                PositionX = 20,
                PositionY = 540
            };
            hudLayer.AddChild(labelRounds);

            labelHp = new CCLabel("Hp: ", "Arial", 40, CCLabelFormat.SystemFont)
            {
                //Scale = 0.7f,
                AnchorPoint = CCPoint.AnchorLowerLeft,
                Color = CCColor3B.Green,
                PositionX = 20,
                PositionY = 400
            };
            hudLayer.AddChild(labelHp);            
        }
        void UpdateHUD()
        {
            labelEnemies.Text = "Осталось врагов: " + GameManager.level.enemyRemain + "/" + GameManager.level.total_num_enimies
                + "\n" + "interval: " + GameManager.gameplay.lvlEventDirector.curEvList.spawnInterval;
            labelRounds.Text = "rounds : " + GameManager.gameplay.playerShip.rounds.ToString()
                + "\n" + "Objects: " + GameManager.gameplay.allObjects.Count.ToString();
            //labelNumObjects.Text = 
            labelHp.Text = "Hp: " + GameManager.gameplay.playerShip.Hp.ToString() + "\n" + "Points: " + GameManager.player.Score;
        }

        public void CloseScene()
        {
            GameManager.gameplay.gameEventDirector.PointsUpd -= UpdateHUD;
            GameManager.gameplay.gameEventDirector.HpUpd -= UpdateHUD;
            
            UnscheduleAll();
            RemoveAllListeners();
        }
        new public void Dispose()
        {
            CloseScene();

            gameplayLayer.RemoveEventListeners();
            gameplayLayer.RemoveAllChildren();
            /*backgroundLayer.RemoveAllChildren();
            backgroundLayer = null;
            gameplayLayer.RemoveEventListeners();
            gameplayLayer.RemoveAllChildren();
            gameplayLayer = null;            
            hudLayer.RemoveAllChildren();
            hudLayer = null;            
            RemoveAllChildren(true);
            base.Dispose();*/
        }
    }
}