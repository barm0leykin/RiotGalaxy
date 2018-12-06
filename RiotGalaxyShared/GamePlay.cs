using System.Collections.Generic;
using CocosSharp;
using RiotGalaxy.Objects;
using RiotGalaxy.SFX;
using RiotGalaxy.Interface;
using System.Threading.Tasks;

namespace RiotGalaxy
{
    /// <summary>
    /// класс геймплея - главный цикл игры, обработка событий, ввод игрока и тд
    /// </summary>
    public class Gameplay
    {
        public bool Pause { get; set; }     // пауза
        public List<GameObject> allObjects; // список всех объектов игры
        
        public GUI iface;                   // хранилище кнопок
        public World world;                 // мир со своей координатной сеткой
        public Hive hive;                   // улей со своей координатной сеткой, в нём роятся враги
        public PlayerShip playerShip;       // наш корабель
        public LvlEventDirector lvlEventDirector;   // события уровня
        public GameEventDirector gameEventDirector; // прочие события
        int spawn_count = 0;                // счетчик родившихся врагов
        //public Route current_route;
        //CCNode t;

        private bool hasGameEnded;
        private bool StopSignalSended;

        public Gameplay()
        {
            System.Diagnostics.Debug.WriteLine("=== Gameplay() ===");            
        }
        public void Init()
        {
            System.Diagnostics.Debug.WriteLine("=== Gameplay Init() ===");
            hasGameEnded = false;
            StopSignalSended = false;
            allObjects = new List<GameObject>();
            world = new World();
            hive = new Hive();
            lvlEventDirector = new LvlEventDirector();
            gameEventDirector = new GameEventDirector();         
            iface = new GUI();

            gameEventDirector.EnemyDie += UpdateEnemyesNum;    //регистрируем обработчик события умирание вражины

            // Спавн игрока
            if (playerShip == null)
            {
                playerShip = new PlayerShip(new CCPoint(GameManager.ScGame.gameplayLayer.ContentSize.Width / 2.0f, 50));
            }
            else
            {
                playerShip.Init();
            }
            allObjects.Add(playerShip);
            GameManager.ScGame.gameplayLayer.AddChild(playerShip);

            // вкюлчаем звезды на заднем фоне
            CCPoint pos;
            pos.X = GameManager.ScGame.gameplayLayer.ContentSize.Width / 2;
            pos.Y = GameManager.ScGame.gameplayLayer.ContentSize.Height / 2;
            SpawnSfx(pos, Sfx.SfxType.GALAXY);

            Pause = false;
            hasGameEnded = false;

            Message("В атаку!");
        }
        async void Lose(int delay = 3000)
        {
            Message("Тебя убили =(");
            await Task.Delay(delay);
            hasGameEnded = true;
            ICommand cmd = new CommandLose();
            cmd.Execute();
        }
        async void Win(int delay = 3000)
        {
            Message("Победа!");
            playerShip.gun.Safe = true;
            await Task.Delay(delay);
            hasGameEnded = true;
            ICommand cmd = new CommandWin();
            cmd.Execute();
        }

        public void Activity(float time)
        {
            if (Pause)  // еще надо остановить звездопад (спецэффекты)
                return;            
            if (playerShip.Hp < 1)  //нас убили
            {
                if (!StopSignalSended)
                {
                    StopSignalSended = true;
                    Lose();
                }
            }
            if (GameManager.level.enemyRemain < 1)  //враги закончились
            {
                if (!StopSignalSended)
                {
                    StopSignalSended = true;
                    Win();
                }                
            }
            if (hasGameEnded == false)  // ГЛАВНЫЙ ЦИКЛ ИГРОВОГО ПРОЦЕССА
            {                
                for (int i = 0; i < allObjects.Count; i++) // для каждго объекта игры...
                {
                    allObjects[i].Activity(time);    // все объекты действуют

                    for (int z = 0; z < allObjects.Count; z++)  // проверка на колизии
                    {
                        if (allObjects[i] == allObjects[z]) // столкновение с самим собой не обрабатываем
                            continue;
                        if (allObjects[i].coll.CheckCollision(allObjects[z]))
                            allObjects[z].Collision(allObjects[i]); // obj1 стукает собой по obj2 :)                            

                    }
                    if (allObjects[i].needToDelete == true) // удаляем объекты
                    {
                        if (allObjects[i].objectType == GameObject.ObjType.ENEMY )
                        {
                            CommandSpawnSFX sfx = new CommandSpawnSFX(allObjects[i].Position);
                            sfx.Execute();
                            CommandSpawnRandomBonus bonus = new CommandSpawnRandomBonus(allObjects[i].Position);
                            bonus.Execute();
                            CommandStarBonus star = new CommandStarBonus(allObjects[i].Position);
                            star.Execute();
                            GameManager.gameplay.gameEventDirector.AddEvent(GameEventDirector.EventsID.ENEMY_DIE);
                            //GameManager.gameplay.gameEventDirector.AddEvent(GameEventDirector.EventsID.SCORE_UPD);// создаем событие для обновления интерфейса
                        }

                        allObjects[i].Delete();
                        allObjects.Remove(allObjects[i]);
                        i--; //фишка в том чтобы сдвинуть i на единицу назад, т.к. после удаления следующий элемент будет на том же индексе что и старый (удаленный)
                    }
                }
                lvlEventDirector.Update(time);  // обработка событий уровня 
                gameEventDirector.Update();     // обработка прочих событий (обновить интерфейс, пауза и тд)
            }// ГЛАВНЫЙ ЦИКЛ
        }//Activity
        void UpdateEnemyesNum()
        {
            GameManager.level.enemyKilled++; ///
            GameManager.level.enemyRemain--;
        }
        public void SpawnEnemy(Enemy.EnemyType enType)
        {
            if (GameManager.level.enemySpawned >= GameManager.level.total_num_enimies)
                return;
            Enemy enemy;
            spawn_count++;
            CCPoint random_pos;
            // Спавн врага
            random_pos = new CCPoint     //рандомная точка
            {
                X = CCRandom.GetRandomFloat(30, GameManager.ScGame.gameplayLayer.ContentSize.Width - 30),
                Y = GameManager.ScGame.gameplayLayer.ContentSize.Height + 10 // респавнятся за границей экрана
            };
            
            switch (enType)
            {
                case Enemy.EnemyType.SM_SCOUT:
                    {
                        enemy = new EnemySmallScout();
                        break;
                    }
                case Enemy.EnemyType.BLUE:
                    {
                        enemy = new EnemySmallBlue();
                        enemy.Position = lvlEventDirector.curEvList.current_route.GetFirst(); //current_route.GetFirst();
                        break;
                    }
                case Enemy.EnemyType.GREEN:
                    {
                        enemy = new EnemySmallGreen();
                        enemy.Position = random_pos;
                        break;
                    }
                case Enemy.EnemyType.RED:
                    {
                        enemy = new EnemySmallRed();
                        enemy.Position = random_pos;
                        break;
                    }
                default:
                    {
                        enemy = new EnemySmallBlue();
                        break;
                    }
            }
            enemy.name = enemy.objectType + "_" + enemy.enemyType.ToString() + "_" + spawn_count;     //бортовой номер врага
            //enemy.Position = current_route.GetFirst();
            allObjects.Add(enemy);
        }

        public void SpawnBonus(CCPoint pos, Bonus.BonusType bType = Bonus.BonusType.BULLET_UP)
        {
            Bonus bonus;            
            switch (bType)
            {
                case Bonus.BonusType.BULLET_UP:
                    {
                        bonus = new BonusBulletUP(pos);
                        break;
                    }
                case Bonus.BonusType.HP_UP:
                    {
                        bonus = new BonusHpUp(pos);
                        break;
                    }
                case Bonus.BonusType.NUKE_BOMB:
                    {
                        bonus = new BonusNukeBomb(pos);
                        break;
                    }
                case Bonus.BonusType.STAR:
                    {
                        bonus = new BonusStar(pos);
                        break;
                    }
                default:
                    bonus = new BonusBulletUP(pos);
                    break;
            }
            allObjects.Add(bonus);
            //GameManager.GameSc.gameplayLayer.AddChild(bonus);
        }
        public void SpawnSfx(CCPoint pos, Sfx.SfxType sfxType = Sfx.SfxType.BLAST)
        {
            Sfx sfx;
            switch (sfxType)
            {
                case Sfx.SfxType.BLAST:
                    {
                        sfx = new SfxBlast(pos);
                        break;
                    }
                case Sfx.SfxType.GALAXY:
                    {
                        sfx = new SfxGalaxy(pos);
                        break;
                    }
                default:
                    sfx = new SfxBlast(pos);
                    break;
            }
            //allObjects.Add(sfx);
            //GameManager.GameSc.gameplayLayer.AddChild(sfx);
        }
        public async void Message(string msg, int delay = 3000)
        {
            var label = new CCLabel(msg, "Arial", 50, CCLabelFormat.SystemFont)
            {
                Color = CCColor3B.Red,
                PositionX = Options.width / 2,
                PositionY = Options.height / 2,
            };
            GameManager.ScGame.hudLayer.AddChild(label);

            await Task.Delay(delay);
            label.RemoveFromParent();
        }

        public void CloseGameplay()
        {
            System.Diagnostics.Debug.WriteLine("=== CloseGameplay() ===");
            hasGameEnded = true;

            for (int i = 0; i < allObjects.Count; i++) // удаляем объекты
            {
                if (allObjects[i].name == "PlayerShip")
                    break;
                allObjects[i].Delete();
                allObjects.Remove(allObjects[i]);
                i--;
            }
            //userInputHandler = null;
            //playerShip = null;
            //RemoveAllChildren(true);
        }
    }//Class
}