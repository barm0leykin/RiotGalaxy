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
        
        // Обработчик ввода пользователя
        public InputManager userInputHandler;

        // Приватный конструктор для singleton
        private GameManager()
        {
            CurrentGameState = GameState.MainMenu;
            GameObjects = new List<GameObject>();
            ScreenWidth = 1280;
            ScreenHeight = 768;
            Console.WriteLine($"=== GameManager initialized with state: {CurrentGameState} ===");
            
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

            Console.WriteLine($"=== GameManager.Initialize completed. Current state: {CurrentGameState} ===");
        }

        /// <summary>
        /// Загрузка контента
        /// </summary>
        public void LoadContent()
        {
            System.Diagnostics.Debug.WriteLine("=== GameManager Loading Content ===");
            // Создаем простой шрифт программно, так как у нас нет проекта RiotGalaxy.Content
            // Это временная мера до настройки правильного контента
            CreateDefaultFont();
        }
        
        /// <summary>
        /// Создание простого шрифта программно
        /// </summary>
        private void CreateDefaultFont()
        {
            // Поскольку мы не можем загрузить шрифт из файла, создаем простую текстуру
            // для отображения базовой информации
            // В будущем здесь будет загрузка шрифта из проекта контента
            System.Diagnostics.Debug.WriteLine("Default font creation skipped - will use simple textures");
        }

        /// <summary>
        /// Основной игровой цикл - обновление состояния игры
        /// Адаптировано из GamePlay.cs (CocosSharp)
        /// </summary>
        public void Update(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Быстрая проверка завершения игры перед обновлением
            if (CheckGameEndConditions())
            {
                return; // Игра окончена, ждем обработку в GameManager.ChangeGameState
            }

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
            Console.WriteLine($"=== Changing GameState: {CurrentGameState} -> {newState} ===");
            
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
            Console.WriteLine($"=== UpdateGameplay: Processing {GameObjects.Count} objects ===");
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
                
                Console.WriteLine($"=== UpdateGameplay: Successfully processed {GameObjects.Count} objects ===");
                
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
                Console.WriteLine("=== Player defeated! Game Over ===");
                ChangeGameState(GameState.GameOver);
                return true;
            }
            
            // Проверка победы - все враги уничтожены
            if (EnemiesRemaining <= 0)
            {
                Console.WriteLine("=== All enemies defeated! Victory ===");
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
                    Console.WriteLine($"=== Marking object for removal: {GameObjects[i].GetType().Name} ===");
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
            try {
                // Рисуем заголовок
                string titleText = "RiotGalaxy";
                _spriteBatch.DrawString(_content.Load<SpriteFont>("TestFont"), 
                    titleText, 
                    new Vector2(ScreenWidth / 2 - 100, ScreenHeight / 4), 
                    Color.White);

                // Рисуем подзаголовок
                string subtitleText = "Нажмите Пробел для начала игры";
                _spriteBatch.DrawString(_content.Load<SpriteFont>("TestFont"), 
                    subtitleText, 
                    new Vector2(ScreenWidth / 2 - 150, ScreenHeight / 2), 
                    Color.Yellow);
            }
            catch
            {
                // Если шрифт не загружен, рисуем простые прямоугольники вместо текста
                DrawSimplePlaceholder(ScreenWidth / 2 - 100, ScreenHeight / 4, 200, 40, "RiotGalaxy");
                DrawSimplePlaceholder(ScreenWidth / 2 - 150, ScreenHeight / 2, 300, 30, "Press Space to Start");
            }
        }

        private void DrawGameplay(GameTime gameTime)
        {
            Console.WriteLine($"=== DrawGameplay: Drawing {GameObjects.Count} objects ===");
            
            // Рисуем все игровые объекты
            foreach (var gameObject in GameObjects)
            {
                Console.WriteLine($"=== Drawing GameObject at ({gameObject.Position.X}, {gameObject.Position.Y}) ===");
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
            try {
                _spriteBatch.DrawString(_content.Load<SpriteFont>("TestFont"), 
                    "ПАУЗА", 
                    new Vector2(ScreenWidth / 2 - 50, ScreenHeight / 2), 
                    Color.White);
            }
            catch
            {
                DrawSimplePlaceholder(ScreenWidth / 2 - 50, ScreenHeight / 2, 100, 40, "PAUSED");
            }
        }

        private void DrawGameOver(GameTime gameTime)
        {
            try {
                string gameOverText = "GAME OVER";
                _spriteBatch.DrawString(_content.Load<SpriteFont>("TestFont"), 
                    gameOverText, 
                    new Vector2(ScreenWidth / 2 - 75, ScreenHeight / 2), 
                    Color.Red);
            }
            catch
            {
                DrawSimplePlaceholder(ScreenWidth / 2 - 75, ScreenHeight / 2, 150, 40, "GAME OVER");
            }
        }

        private void DrawVictory(GameTime gameTime)
        {
            try {
                string victoryText = "ПОБЕДА!";
                _spriteBatch.DrawString(_content.Load<SpriteFont>("TestFont"), 
                    victoryText, 
                    new Vector2(ScreenWidth / 2 - 65, ScreenHeight / 2), 
                    Color.Gold);
            }
            catch
            {
                DrawSimplePlaceholder(ScreenWidth / 2 - 65, ScreenHeight / 2, 130, 40, "VICTORY");
            }
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
            Console.WriteLine("=== InitializeGameplay method called ===");
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
                
                Console.WriteLine($"=== Gameplay Initialized with {GameObjects.Count} objects ===");
                Console.WriteLine($"=== Player created at ({Player.Position.X}, {Player.Position.Y}) ===");
                Console.WriteLine($"=== Level parameters: Total Enemies: {EnemiesRemaining}, Killed: {EnemiesKilled} ===");
            }
            catch (Exception ex)
            {
Console.WriteLine($"Error initializing gameplay: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
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
            
            Console.WriteLine("=== Gameplay stats reset ===");
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
            Console.WriteLine("=== Level parameters initialized ===");
        }

        /// <summary>
        /// Создание начальных игровых объектов
        /// Позволяет сразу запустить игру с базовыми объектами
        /// </summary>
        private void SpawnInitialObjects()
        {
            // В будущем здесь будет спавн начальных врагов
            // Пока оставляем только игрока для тестирования
            
            // todo: Добавить SpawnEnemy() когда будет реализован класс Enemy
            
            Console.WriteLine("=== Initial objects spawned ===");
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
            
            Console.WriteLine("=== GameManager subscribed to player events ===");
        }
        
        /// <summary>
        /// Обработчик изменения здоровья игрока
        /// </summary>
        private void OnPlayerHealthChanged(int oldHealth, int newHealth)
        {
            Console.WriteLine($"=== GameManager: Player health changed from {oldHealth} to {newHealth} ===");
            
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
            Console.WriteLine("=== GameManager: Player died! ===");
            
            // Меняем состояние игры на GameOver
            ChangeGameState(GameState.GameOver);
        }
        
        /// <summary>
        /// Обработчик воскрешения игрока
        /// </summary>
        private void OnPlayerRespawned()
        {
            Console.WriteLine("=== GameManager: Player respawned! ===");
            
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
            
            Console.WriteLine($"=== Processing removal of {obj.GetType().Name} ===");
            
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
            Console.WriteLine("=== RemoveDeadObjectsOptimized: Already handled in ProcessGameObjects ===");
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
            
            Console.WriteLine("=== GameManager unsubscribed from player events ===");
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

        private void ProcessCollision(GameObject obj1, GameObject obj2)
        {
            // Пока пустая реализация
            // Будем реализовывать в следующих этапах
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
                    Console.WriteLine($"=== Removing {objectType} ===");
                    
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
            Console.WriteLine($"=== Enemy killed. Total killed: {EnemiesKilled}, Remaining: {EnemiesRemaining} ===");
            
            // В будущем здесь будут игровые события
            // Например: gameEventDirector.AddEvent(GameEventDirector.EventsID.ENEMY_DIE);
            Console.WriteLine($"=== Enemy death event triggered ===");
            
            // Добавляем событие в список событий для обработки в конце обновления
            GameEvents.Add(() => Console.WriteLine($"Processing enemy death..."));
        }

        private void DrawHUD()
        {
            try {
                // Рисуем здоровье игрока (упрощенная версия без текста)
                if (Player != null)
                {
                    // Отображаем здоровье с помощью цветных прямоугольников
                    DrawSimplePlaceholder(10, 10, 200, 30, "RiotGalaxy HUD");
                    
                    // Рисуем полоску здоровья с помощью прямоугольников
                    DrawSimplePlaceholder(10, 45, 200, 20, "Health");
                    
                    // Отображаем текущее здоровье как часть полоски
                    int healthWidth = (int)(200 * (float)Player.Health / Player.MaxHealth);
                    DrawHealthBar(10, 45, healthWidth, 20, Player.Health);
                }
            }
            catch
            {
                // Заглушка - просто рисуем прямоугольник
                DrawSimplePlaceholder(10, 10, 200, 30, "HUD");
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
        /// Рисование простого заполнителя вместо текста
        /// </summary>
        private void DrawSimplePlaceholder(int x, int y, int width, int height, string text)
        {
            _spriteBatch.Draw(SimpleTexture, new Rectangle(x, y, width, height), Color.White);
            // Для отладки можно использовать консольный вывод
            System.Diagnostics.Debug.WriteLine($"Drawing text placeholder: {text} at ({x},{y})");
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
            
            Console.WriteLine($"=== Processed {eventsToProcess.Count} game events");
        }

        private void TriggerEnemyDeathEvent(GameObject enemy)
        {
            Console.WriteLine($"=== Enemy death event triggered for {enemy?.GetType().Name ?? "Unknown"} ===");
            
            // Обновляем счетчики
            EnemiesKilled++;
            EnemiesRemaining = Math.Max(0, EnemiesRemaining - 1);
            Console.WriteLine($"=== Enemy killed. Total killed: {EnemiesKilled}, Remaining: {EnemiesRemaining} ===");
            
            // Добавляем событие в список событий для обработки в конце обновления
            GameEvents.Add(() => Console.WriteLine($"Processing enemy death..."));
        }

        #endregion
    }
}
