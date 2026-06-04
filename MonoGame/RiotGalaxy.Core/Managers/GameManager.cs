using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using RiotGalaxy.GameObjects;
using RiotGalaxy.Components;

namespace RiotGalaxy.Managers
{
    /// <summary>
    /// Класс GameManager - центральный диспетчер игры.
    /// Аналог GameManager из CocosSharp для MonoGame.
    /// </summary>
    public class GameManager
    {
        // Singleton pattern
        private static GameManager _instance;
        public static GameManager Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new GameManager();
                return _instance;
            }
        }

        // Ссылки на основные компоненты игры
        private Game _game;
        private GraphicsDeviceManager _graphics;
        private ContentManager _content;
        private SpriteBatch _spriteBatch;
        private SpriteFont _defaultFont;
        
        // Игровые события (аналог GamePlay.cs)
        public List<Action> GameEvents { get; private set; } = new List<Action>();
        
        // Счетчики для отслеживания статистики
        public int EnemiesKilled { get; private set; }
        public int EnemiesRemaining { get; private set; }

        // Базовые игровые состояния
        public enum GameState { MainMenu, Playing, Paused, GameOver, Victory }
        public GameState CurrentGameState { get; private set; }

        // Основные объекты игры
        public List<GameObject> GameObjects { get; private set; }
        public PlayerShip Player { get; private set; }

        // Базовые игровые параметры
        public int ScreenWidth { get; private set; }
        public int ScreenHeight { get; private set; }
        
        // Вспомогательные текстуры
        public Texture2D SimpleTexture { get; set; }
        public GraphicsDevice GraphicsDevice => _graphics.GraphicsDevice;

        // Доступ к загрузчику контента (нужен игровым объектам для загрузки спрайтов)
        public ContentManager Content => _content;

        // Фоновое изображение (задник)
        private Texture2D _background;
        
        // Обработчик ввода пользователя
        public InputManager userInputHandler;

        // Приватный конструктор для singleton
        private GameManager()
        {
            CurrentGameState = GameState.MainMenu;
            GameObjects = new List<GameObject>();
            ScreenWidth = 1280;
            ScreenHeight = 768;
            
            // Инициализируем статистические счетчики
            EnemiesKilled = 0;
            EnemiesRemaining = 0;
            
            // Инициализируем обработчик ввода
            userInputHandler = InputManager.Instance;
        }

        /// <summary>
        /// Инициализация GameManager
        /// </summary>
        public void Initialize(Game game, GraphicsDeviceManager graphics, ContentManager content)
        {
            _game = game;
            _graphics = graphics;
            _content = content;

            // Устанавливаем разрешение экрана
            _graphics.PreferredBackBufferWidth = ScreenWidth;
            _graphics.PreferredBackBufferHeight = ScreenHeight;
            _graphics.ApplyChanges();

            // Создаем SpriteBatch для отрисовки
            _spriteBatch = new SpriteBatch(_graphics.GraphicsDevice);

        }

        /// <summary>
        /// Загрузка контента
        /// </summary>
        public void LoadContent()
        {
            System.Diagnostics.Debug.WriteLine("=== GameManager Loading Content ===");

            // Загружаем фоновое изображение (1280x768, точно под разрешение игры)
            try
            {
                _background = _content.Load<Texture2D>("Backgrounds/background_blue");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"=== Failed to load background: {ex.Message} ===");
            }

            // Загружаем звуковые эффекты (fire1, explode1)
            AudioManager.Instance.LoadContent(_content);

            // Загружаем шрифт для текста (меню, HUD)
            try
            {
                _defaultFont = _content.Load<SpriteFont>("TestFont");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"=== Failed to load font 'TestFont': {ex.Message} ===");
            }
        }

        /// <summary>
        /// Рисование текста по центру по горизонтали на заданной высоте.
        /// </summary>
        private void DrawCenteredText(string text, float y, Color color)
        {
            if (_defaultFont == null)
                return;
            Vector2 size = _defaultFont.MeasureString(text);
            _spriteBatch.DrawString(_defaultFont, text, new Vector2(ScreenWidth / 2f - size.X / 2f, y), color);
        }

        /// <summary>
        /// Основной игровой цикл - обновление состояния игры
        /// Адаптировано из GamePlay.cs (CocosSharp)
        /// </summary>
        public void Update(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Условия завершения игры проверяются только в состоянии Playing
            // (внутри UpdateGameplay). Иначе в Victory/GameOver EnemiesRemaining==0
            // снова форсил бы Victory и блокировал выход.

            // Обновляем все объекты в соответствии с текущим состоянием
            switch (CurrentGameState)
            {
                case GameState.MainMenu:
                    UpdateMainMenu(deltaTime);
                    break;
                case GameState.Playing:
                    UpdateGameplay(gameTime);
                    break;
                case GameState.Paused:
                    UpdatePaused(deltaTime);
                    break;
                case GameState.GameOver:
                    UpdateGameOver(deltaTime);
                    break;
                case GameState.Victory:
                    UpdateVictory(deltaTime);
                    break;
            }
        }

        /// <summary>
        /// Отрисовка игры
        /// </summary>
        public void Draw(GameTime gameTime)
        {
            _graphics.GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin();

            // Рисуем фоновое изображение (задник) под всеми состояниями
            if (_background != null)
            {
                _spriteBatch.Draw(_background, new Rectangle(0, 0, ScreenWidth, ScreenHeight), Color.White);
            }

            // Рисуем в соответствии с текущим состоянием
            switch (CurrentGameState)
            {
                case GameState.MainMenu:
                    DrawMainMenu(gameTime);
                    break;
                case GameState.Playing:
                    DrawGameplay(gameTime);
                    break;
                case GameState.Paused:
                    DrawPaused(gameTime);
                    break;
                case GameState.GameOver:
                    DrawGameOver(gameTime);
                    break;
                case GameState.Victory:
                    DrawVictory(gameTime);
                    break;
            }

            _spriteBatch.End();
        }

        /// <summary>
        /// Смена состояния игры
        /// </summary>
        public void ChangeGameState(GameState newState)
        {
            
            // Очистка ресурсов при выходе из состояния
            switch (CurrentGameState)
            {
                case GameState.Playing:
                    CleanupGameplay();
                    break;
            }

            CurrentGameState = newState;

            // Инициализация ресурсов при входе в состояние
            switch (newState)
            {
                case GameState.MainMenu:
                    InitializeMainMenu();
                    break;
                case GameState.Playing:
                    InitializeGameplay();
                    break;
            }
        }

        #region Методы обновления для каждого состояния

        private void UpdateMainMenu(float deltaTime)
        {
            // Логика обновления главного меню (заглушка)
            // Будем реализовывать на следующих этапах
        }

        private void UpdatePaused(float deltaTime)
        {
            // Логика обновления паузы (заглушка)
            // Будем реализовывать на следующих этапах
        }

        private void UpdateGameOver(float deltaTime)
        {
            // Логика обновления экрана поражения (заглушка)
            // Будем реализовывать на следующих этапах
        }

        private void UpdateVictory(float deltaTime)
        {
            // Логика обновления экрана победы (заглушка)
            // Будем реализовывать на следующих этапах
        }

        private void UpdateGameplay(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            
            // Проверка условий завершения игры (аналог GamePlay.cs)
            if (!CheckGameEndConditions())
            {
                // Обрабатываем пользовательский ввод (аналог SceneGame.Activity)
                if (userInputHandler != null)
                {
                    userInputHandler.Update();
                    userInputHandler.HandleScGameInput();
                }
                
                // Аналог основного цикла из GamePlay.cs - обрабатываем все объекты
                ProcessGameObjects(gameTime);
                
                // Удаляем объекты помеченные для удаления (аналог GamePlay.cs)
                RemoveDeadObjectsOptimized();
                
                
                // Обрабатываем игровые события (аналог GamePlay.cs lvlEventDirector.Update(time); gameEventDirector.Update())
                ProcessGameEvents();
            }
        }

        /// <summary>
        /// Проверка условий завершения игры (победа или поражение)
        /// Аналог проверок в GamePlay.cs строка 94-109
        /// </summary>
        private bool CheckGameEndConditions()
        {
            // Проверка поражения - игрок уничтожен
            if (Player != null && Player.Health <= 0)
            {
                ChangeGameState(GameState.GameOver);
                return true;
            }
            
            // Проверка победы - все враги уничтожены
            if (EnemiesRemaining <= 0)
            {
                ChangeGameState(GameState.Victory);
                return true;
            }
            
            return false;
        }

        /// <summary>
        /// Оптимизированная обработка игровых объектов
        /// Аналог основного цикла из GamePlay.cs строка 112-145
        /// </summary>
        private void ProcessGameObjects(GameTime gameTime)
        {
            // Создаем список для объектов, которые нужно удалить в этой итерации
            var objectsToRemove = new List<int>();
            
            for (int i = 0; i < GameObjects.Count; i++)
            {
                if (GameObjects[i] == null)
                    continue;
                
                // Обновляем состояние объекта
                GameObjects[i].Update(gameTime);
                
                // Проверка столкновений с другими объектами
                for (int z = 0; z < GameObjects.Count; z++)
                {
                    if (i == z) continue; // Пропускаем столкновение с самим собой
                    
                    if (GameObjects[i].Intersects(GameObjects[z]))
                    {
                        ProcessCollision(GameObjects[i], GameObjects[z]);
                    }
                }
                
                // Проверяем, нужно ли удалить объект
                if (!GameObjects[i].IsAlive)
                {
                    objectsToRemove.Add(i);
                }
            }
            
            // Сортируем индексы в обратном порядке для безопасного удаления
            objectsToRemove.Sort((a, b) => b.CompareTo(a));
            
            // Удаляем объекты от большего индекса к меньшему
            foreach (var index in objectsToRemove)
            {
                if (index >= 0 && index < GameObjects.Count && GameObjects[index] != null)
                {
                    ProcessObjectRemoval(GameObjects[index]);
                    GameObjects.RemoveAt(index);
                }
            }
        }

        #endregion

        #region Методы отрисовки для каждого состояния

        private void DrawMainMenu(GameTime gameTime)
        {
            DrawCenteredText("RiotGalaxy", ScreenHeight / 4f, Color.White);
            DrawCenteredText("Нажмите Пробел для начала игры", ScreenHeight / 2f, Color.Yellow);
        }

        private void DrawGameplay(GameTime gameTime)
        {
            
            // Рисуем все игровые объекты
            foreach (var gameObject in GameObjects)
            {
                gameObject.Draw(gameTime, _spriteBatch);
            }

            // Рисуем HUD
            DrawHUD();
        }

        private void DrawPaused(GameTime gameTime)
        {
            // Сначала рисуем игру под паузой
            DrawGameplay(gameTime);
            
            // Затем накладываем полупрозрачный фон
            // Если текстура не загружена, создаем простую
            if (SimpleTexture == null)
                SimpleTexture = CreateSimpleTexture(Color.White);
                
            _spriteBatch.Draw(SimpleTexture,
                new Rectangle(0, 0, ScreenWidth, ScreenHeight),
                new Color(0, 0, 0, 150));

            // Рисуем текст паузы
            DrawCenteredText("ПАУЗА", ScreenHeight / 2f, Color.White);
        }

        private void DrawGameOver(GameTime gameTime)
        {
            DrawCenteredText("GAME OVER", ScreenHeight / 2f, Color.Red);
        }

        private void DrawVictory(GameTime gameTime)
        {
            DrawCenteredText("ПОБЕДА!", ScreenHeight / 2f, Color.Gold);
        }

        #endregion

        #region Вспомогательные методы

        private void InitializeMainMenu()
        {
            // Инициализация главного меню (заглушка)
            // Будем реализовывать на следующих этапах
        }

private void InitializeGameplay()
        {
            try
            {
                // Сбрас статистики (аналог начала уровня)
                ResetGameplayStats();
                
                // Инициализация игрового процесса
                GameObjects.Clear();
                GameEvents.Clear();
                
                // Базовые параметры уровня (аналог GamePlay.cs Init)
                InitializeLevelParameters();
                
                // Создаем игрока (аналог GamePlay.cs строка 49-58)
                Player = new PlayerShip(new Vector2(ScreenWidth / 2, ScreenHeight - 100));
                Player.SetGraphicsDevice(GraphicsDevice);
                Player.LoadContent(_content); // Загружаем реальный спрайт корабля "Images/ship"
                Player.Health = Player.MaxHealth; // Сбрасываем здоровье игрока до максимума
                
                // Устанавливаем границы движения для компонента движения игрока
                if (Player.Movement is PlayerMovementComponent playerMovement)
                {
                    playerMovement.SetBounds(0, ScreenWidth, 0, ScreenHeight);
                }
                
                // Подписываемся на события игрока
                SubscribeToPlayerEvents();
                
                GameObjects.Add(Player);
                
                // Регистрируем обработчики событий (аналог GamePlay.cs строка 47)
                SetupGameplayEvents();
                
                // Добавляем начальные игровые объекты
                SpawnInitialObjects();
                
            }
            catch (Exception ex)
            {
Console.WriteLine($"Error initializing gameplay: {ex.Message}");
            }
        }

        /// <summary>
        /// Сброс статистики игрового процесса
        /// Аналог сброса параметров в GamePlay.cs Init()
        /// </summary>
        private void ResetGameplayStats()
        {
            EnemiesKilled = 0;
            EnemiesRemaining = 10; // Базовое количество врагов для первого уровня
            
        }

        /// <summary>
        /// Инициализация параметров уровня
        /// Аналог инициализации мир и улья в GamePlay.cs строка 41-42
        /// </summary>
        private void InitializeLevelParameters()
        {
            // todo: Добавить World и Hive когда они будут реализованы
            // world = new World();
            // hive = new Hive();
            
            // Базовые параметры уровня
        }

        /// <summary>
        /// Создание начальных игровых объектов
        /// Позволяет сразу запустить игру с базовыми объектами
        /// </summary>
        private void SpawnInitialObjects()
        {
            // Стартовая волна врагов разных типов (появляются сверху)
            float w = ScreenWidth;
            var enemies = new List<GameObject>
            {
                new EnemySmallBlue(new Vector2(w * 0.20f, 60)),
                new EnemySmallBlue(new Vector2(w * 0.80f, 60)),
                new EnemySmallGreen(new Vector2(w * 0.35f, 20)),
                new EnemySmallGreen(new Vector2(w * 0.65f, 20)),
                new EnemySmallRed(new Vector2(w * 0.50f, 100)),
                new EnemySmallScout(new Vector2(w * 0.45f, -20)),
                new EnemySmallScout(new Vector2(w * 0.55f, -40)),
            };
            foreach (var e in enemies)
                GameObjects.Add(e);

            EnemiesRemaining = enemies.Count;
        }
        
/// <summary>
        /// Настройка игровых событий (аналог GamePlay.cs)
        /// </summary>
        private void SetupGameplayEvents()
        {
            // Очищаем предыдущие события
            GameEvents.Clear();
            
            // В будущем здесь будут регистрироваться основные игровые события
            // Например: событие смерти врага, достижение目标和 т.п.
        }
        
        /// <summary>
        /// Подписка на события игрока
        /// </summary>
        private void SubscribeToPlayerEvents()
        {
            if (Player == null) return;
            
            // Подписываемся на события здоровья игрока
            Player.HealthChanged += OnPlayerHealthChanged;
            Player.PlayerDied += OnPlayerDied;
            Player.PlayerRespawned += OnPlayerRespawned;
            
        }
        
        /// <summary>
        /// Обработчик изменения здоровья игрока
        /// </summary>
        private void OnPlayerHealthChanged(int oldHealth, int newHealth)
        {
            
            // Здесь можно добавить дополнительную логику:
            // - Обновление HUD
            // - Звуковые эффекты
            // - Визуальная обратная связь
        }
        
        /// <summary>
        /// Обработчик смерти игрока
        /// </summary>
        private void OnPlayerDied()
        {
            
            // Меняем состояние игры на GameOver
            ChangeGameState(GameState.GameOver);
        }
        
        /// <summary>
        /// Обработчик воскрешения игрока
        /// </summary>
        private void OnPlayerRespawned()
        {
            
            // Здесь можно добавить дополнительную логику при воскрешении
            // Например: сброс бонусов, перезапуск уровня и т.д.
        }

        /// <summary>
        /// Обработка удаления объекта (аналог GamePlay.cs строка 126-141)
        /// Выполняет дополнительные действия при удалении врагов
        /// </summary>
        private void ProcessObjectRemoval(GameObject obj)
        {
            if (obj == null) return;
            
            
            // Для врагов выполняем дополнительные действия (аналог GamePlay.cs)
            if (obj.GetType().Name.Contains("Enemy"))
            {
                // Запускаем ивент смерти врага
                TriggerEnemyDeathEvent(obj);
                
                // todo: Добавить спавн бонусов (аналог CommandSpawnRandomBonus)
                // todo: Добавить визуальные эффекты (аналог CommandSpawnSFX)
                // todo: Добавитьstars (аналог CommandStarBonus)
            }
            
            // Выполняем базовое удаление объекта
            obj.IsAlive = false; // Помечаем объект как мертвый
        }

        /// <summary>
        /// Оптимизированное удаление мертвых объектов
        /// Использует отложенное удаление для повышения производительности
        /// </summary>
        private void RemoveDeadObjectsOptimized()
        {
            // Этот метод теперь интегрирован в ProcessGameObjects
            // для более эффективной обработки
        }

        private void CleanupGameplay()
        {
            // Отписываемся от событий игрока
            UnsubscribeFromPlayerEvents();
            
            // Очистка ресурсов игрового процесса
            GameObjects.Clear();
            Player = null;
        }
        
        /// <summary>
        /// Отписка от событий игрока
        /// </summary>
        private void UnsubscribeFromPlayerEvents()
        {
            if (Player == null) return;
            
            Player.HealthChanged -= OnPlayerHealthChanged;
            Player.PlayerDied -= OnPlayerDied;
            Player.PlayerRespawned -= OnPlayerRespawned;
            
        }

        private void CheckCollisions()
        {
            // Аналог двойного цикла из GamePlay.cs
            for (int i = 0; i < GameObjects.Count; i++)
            {
                for (int j = 0; j < GameObjects.Count; j++)
                {
                    // Пропускаем столкновение с самим собой
                    if (i == j) continue;
                    
                    // Проверяем столкновение
                    if (GameObjects[i].Intersects(GameObjects[j]))
                    {
                        // Обрабатываем столкновение
                        ProcessCollision(GameObjects[i], GameObjects[j]);
                    }
                }
            }
        }

        /// <summary>
        /// Обработка столкновения двойной пары объектов (аналог GameObject.Collision из CocosSharp).
        /// Вызывается из ProcessGameObjects для каждой пересекающейся пары.
        /// </summary>
        private void ProcessCollision(GameObject a, GameObject b)
        {
            // Мёртвые объекты больше не наносят и не получают урон
            if (!a.IsAlive || !b.IsAlive)
                return;

            // Снаряд игрока попал во врага
            if (a is Shell shell && b is Enemy enemy && shell.PlayerSide)
            {
                ShellHitsEnemy(shell, enemy);
            }
            // Враг столкнулся с кораблём игрока (таран)
            else if (a is Enemy en && b is PlayerShip ship)
            {
                EnemyHitsPlayer(en, ship);
            }
            // Вражеский снаряд попал в игрока
            else if (a is Shell sh && b is PlayerShip ps && !sh.PlayerSide)
            {
                ShellHitsPlayer(sh, ps);
            }
        }

        private void ShellHitsEnemy(Shell shell, Enemy enemy)
        {
            enemy.TakeDamage(shell.Damage);
            if (!shell.IsPiercing)
                shell.IsAlive = false; // обычный снаряд исчезает; лазер летит насквозь
        }

        private void EnemyHitsPlayer(Enemy enemy, PlayerShip player)
        {
            player.TakeDamage(enemy.Damage);
            enemy.TakeDamage(enemy.Hp); // враг уничтожается при таране
        }

        private void ShellHitsPlayer(Shell shell, PlayerShip player)
        {
            player.TakeDamage(shell.Damage);
            shell.IsAlive = false;
        }

        private void RemoveDeadObjects()
        {
            // Удаляем объекты отмеченные для удаления (аналог GamePlay.cs)
            for (int i = GameObjects.Count - 1; i >= 0; i--)
            {
                if (!GameObjects[i].IsAlive)
                {
                    // Специальная обработка для разных типов объектов
                    var objectType = GameObjects[i].GetType().Name;
                    
                    // Для врагов и бонусов выполняем дополнительные действия (аналог GamePlay.cs)
                    if (objectType.Contains("Enemy"))
                    {
                        // Игровое событие о гибели врага
                        TriggerEnemyDeathEvent(GameObjects[i]);
                    }
                    
                    // Удаляем объект из списка
                    GameObjects.RemoveAt(i);
                }
            }
        }
        
/// <summary>
        /// Обработка смерти врага (аналог событий в GamePlay.cs)
        /// </summary>
        private void TriggerEnemyDeath(GameObject enemy)
        {
            // Обновляем счетчики
            EnemiesKilled++;
            
            // В будущем здесь будут игровые события
            // Например: gameEventDirector.AddEvent(GameEventDirector.EventsID.ENEMY_DIE);
            
            // Добавляем событие в список событий для обработки в конце обновления
        }

        private void DrawHUD()
        {
            if (Player == null)
                return;

            // Текст здоровья
            if (_defaultFont != null)
                _spriteBatch.DrawString(_defaultFont, $"HP: {Player.Health}/{Player.MaxHealth}",
                    new Vector2(10, 10), Color.White);

            // Полоска здоровья: тёмный фон + цветная заполненная часть
            if (SimpleTexture != null)
            {
                _spriteBatch.Draw(SimpleTexture, new Rectangle(10, 38, 200, 16), new Color(40, 40, 40, 180));
                int healthWidth = (int)(200 * (float)Player.Health / Player.MaxHealth);
                DrawHealthBar(10, 38, healthWidth, 16, Player.Health);
            }
        }
        
        /// <summary>
        /// Рисование полоски здоровья
        /// </summary>
        private void DrawHealthBar(int x, int y, int width, int height, int health)
        {
            Color barColor = Color.Green;
            if (health < 30) barColor = Color.Red;
            else if (health < 60) barColor = Color.Yellow;
            
            _spriteBatch.Draw(SimpleTexture, new Rectangle(x, y, width, height), barColor);
        }
        
        /// <summary>
        /// Создание простой текстуры
        /// </summary>
        private Texture2D CreateSimpleTexture(Color color)
        {
            Texture2D texture = new Texture2D(GraphicsDevice, 64, 64);
            Color[] data = new Color[64 * 64];
            
            for (int i = 0; i < data.Length; ++i)
                data[i] = color;
                
            texture.SetData(data);
            return texture;
        }
        
        /// <summary>
        /// Обработка игровых событий (аналог gameEventDirector.Update() из GamePlay.cs)
        /// </summary>
        private void ProcessGameEvents()
        {
            // Обрабатываем события в безопасном цикле, чтобы избежать problemas с изменением списка во время итерации
            var eventsToProcess = new List<Action>(GameEvents);
            
            // Копируем события и очищаем основной список
            foreach (var gameEvent in GameEvents)
            {
                eventsToProcess.Add(gameEvent);
            }
            
            // Очищаем основной список
            GameEvents.Clear();
            
            // Обрабатываем скопированные события
            foreach (var gameEvent in eventsToProcess)
            {
                gameEvent?.Invoke();
            }
            
        }

        private void TriggerEnemyDeathEvent(GameObject enemy)
        {
            
            // Обновляем счетчики
            EnemiesKilled++;
            EnemiesRemaining = Math.Max(0, EnemiesRemaining - 1);
            
            // Добавляем событие в список событий для обработки в конце обновления
        }

        #endregion
    }
}
