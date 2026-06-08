using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using RiotGalaxy.GameObjects;
using RiotGalaxy.Components;
using RiotGalaxy.Interface;

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
        public enum GameState { Splash, MainMenu, Settings, Playing, Paused, GameOver, Victory, NextLevel }
        public GameState CurrentGameState { get; private set; }

        // Мир и формации (этап 10)
        private World _world;
        private Hive _hive;

        // Текущий уровень и прогрессия
        private Utils.Level _level;
        private int _currentLevel = 1;
        private int _totalLevels = 1;
        private int _lastScore; // итоговый счёт для экранов GameOver/Victory
        private static readonly Random _spawnRnd = new Random();
        public int CurrentLevel => _currentLevel;
        public int TotalLevels => _totalLevels;
        public string CurrentLevelDescription => _level?.Description ?? "";

        // Система экранов меню (заставка/меню/настройки)
        public Screens.ScreenSystem Screens { get; private set; } = new Screens.ScreenSystem();

        // Доступ к шрифту для экранов
        public SpriteFont Font => _defaultFont;

        // Основные объекты игры
        public List<GameObject> GameObjects { get; private set; }
        public PlayerShip Player { get; private set; }

        // Базовые игровые параметры (ВИРТУАЛЬНОЕ разрешение — вся игровая логика в нём)
        public int ScreenWidth { get; private set; }
        public int ScreenHeight { get; private set; }

        // Letterbox-масштабирование виртуального кадра (1280x768) под реальный back buffer
        // (на Android — весь экран). Вся отрисовка идёт через _renderMatrix, ввод
        // преобразуется обратно через ScreenToVirtual.
        private Matrix _renderMatrix = Matrix.Identity;
        private float _renderScale = 1f;
        private Vector2 _renderOffset = Vector2.Zero;


        // Вспомогательные текстуры
        public Texture2D SimpleTexture { get; set; }
        public GraphicsDevice GraphicsDevice => _graphics.GraphicsDevice;

        /// <summary>
        /// Пересчитывает letterbox-матрицу под текущий размер back buffer (вызывается каждый
        /// кадр перед отрисовкой — покрывает смену ориентации/размера экрана).
        /// </summary>
        private void UpdateRenderTransform()
        {
            var vp = _graphics.GraphicsDevice.Viewport;
            float scale = Math.Min((float)vp.Width / ScreenWidth, (float)vp.Height / ScreenHeight);
            if (scale <= 0f) scale = 1f;
            _renderScale = scale;
            _renderOffset = new Vector2(
                (vp.Width - ScreenWidth * scale) / 2f,
                (vp.Height - ScreenHeight * scale) / 2f);
            _renderMatrix = Matrix.CreateScale(scale, scale, 1f)
                          * Matrix.CreateTranslation(_renderOffset.X, _renderOffset.Y, 0f);
        }

        /// <summary>Переводит координаты экрана (пиксели мыши/тача) в виртуальные (1280x768).</summary>
        public Vector2 ScreenToVirtual(Vector2 screenPoint) =>
            (screenPoint - _renderOffset) / _renderScale;

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

#if ANDROID
            // На Android оставляем back buffer равным размеру экрана (полноэкранно):
            // фиксированный PreferredBackBuffer заставил бы рисовать игру в углу.
            // Виртуальный кадр 1280x768 масштабируется letterbox'ом (см. UpdateRenderTransform).
            _graphics.IsFullScreen = true;
#else
            // Desktop: окно ровно под виртуальное разрешение (scale=1, без полей).
            _graphics.PreferredBackBufferWidth = ScreenWidth;
            _graphics.PreferredBackBufferHeight = ScreenHeight;
            _graphics.ApplyChanges();
#endif

            // SpriteBatch НЕ создаём здесь: в конструкторе Game1 GraphicsDevice ещё может быть
            // не создан (на Android он появляется позже, чем на DesktopGL). Создаём в LoadContent,
            // когда устройство гарантированно готово (см. LoadContent).
        }

        /// <summary>
        /// Загрузка контента
        /// </summary>
        public void LoadContent()
        {
            System.Diagnostics.Debug.WriteLine("=== GameManager Loading Content ===");

            // Создаём SpriteBatch здесь: GraphicsDevice уже готов на всех платформах
            // (на Android он недоступен в конструкторе Game1 — см. Initialize).
            _spriteBatch = new SpriteBatch(_graphics.GraphicsDevice);
            UpdateRenderTransform(); // первичный расчёт letterbox-матрицы (до первого Draw)

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

            // Загружаем конфиги из YAML (оружие, враги, параметры игры) и сохранённые настройки
            Weapons.WeaponConfig.Load();
            Utils.EnemyConfig.Load();
            Utils.BonusConfig.Load();
            Utils.GameOptions.Load();
            Utils.GameSettings.Load();

            // Сколько уровней доступно (по файлам Content/Levels/level*.yaml)
            _totalLevels = Utils.Level.CountLevels();
            if (_totalLevels < 1) _totalLevels = 1;
            Utils.Log.Debug($"Configs loaded. Levels found: {_totalLevels}, player HP: {Utils.GameOptions.PlayerMaxHp}");

            ChangeGameState(GameState.Splash);
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
                case GameState.Splash:
                case GameState.MainMenu:
                case GameState.Settings:
                case GameState.NextLevel:
                    Screens.Update(gameTime);
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

            // Letterbox: пересчитываем матрицу под текущий back buffer и масштабируем всю сцену.
            UpdateRenderTransform();
            _spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, _renderMatrix);

            // Рисуем фоновое изображение (задник) под всеми состояниями
            if (_background != null)
            {
                _spriteBatch.Draw(_background, new Rectangle(0, 0, ScreenWidth, ScreenHeight), Color.White);
            }

            // Рисуем в соответствии с текущим состоянием
            switch (CurrentGameState)
            {
                case GameState.Splash:
                case GameState.MainMenu:
                case GameState.Settings:
                case GameState.NextLevel:
                    Screens.Draw(_spriteBatch);
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

        // Запрошен переход «в меню» по кнопке Назад. Выставляется из UI-потока (OnBackPressed),
        // а сама смена состояния выполняется в игровом потоке (ProcessPendingBack) — иначе
        // гонка с циклом Update (чистка GameObjects) роняет игру.
        private volatile bool _backToMenuPending;

        /// <summary>
        /// Запрос «Назад» из UI-потока (Android Back, см. MainActivity). НЕ меняет состояние сам.
        /// </summary>
        /// <returns>true, если приложение должно закрыться (мы уже в меню/заставке);
        /// false — переход в меню отложен в игровой поток.</returns>
        public bool OnBackRequested()
        {
            switch (CurrentGameState)
            {
                case GameState.MainMenu:
                case GameState.Splash:
                    return true; // выходим из приложения (закрытие активности — на UI-потоке)
                default:
                    _backToMenuPending = true; // смену состояния сделает игровой поток
                    return false;
            }
        }

        /// <summary>Обрабатывает отложенный запрос «Назад» — вызывать из игрового потока (Game1.Update).</summary>
        public void ProcessPendingBack()
        {
            if (!_backToMenuPending)
                return;
            _backToMenuPending = false;
            ChangeGameState(GameState.MainMenu);
        }

        /// <summary>
        /// Смена состояния игры
        /// </summary>
        public void ChangeGameState(GameState newState)
        {
            GameState oldState = CurrentGameState;

            // Полная очистка партии только при настоящем завершении игры
            // (не на паузу и не между уровнями — там игрок сохраняется).
            bool endGame =
                (oldState == GameState.Playing &&
                    (newState == GameState.MainMenu || newState == GameState.GameOver || newState == GameState.Victory)) ||
                (oldState == GameState.Paused && newState == GameState.MainMenu);
            if (endGame)
            {
                if (Player != null) _lastScore = Player.Score; // запоминаем счёт до очистки
                CleanupGameplay();
            }

            CurrentGameState = newState;

            // Инициализация ресурсов при входе в состояние
            switch (newState)
            {
                case GameState.Splash:
                    Screens.Change(new Screens.SplashScreen());
                    break;
                case GameState.MainMenu:
                    Screens.Change(new Screens.MainMenuScreen());
                    break;
                case GameState.Settings:
                    Screens.Change(new Screens.SettingsScreen());
                    break;
                case GameState.NextLevel:
                    // Убираем остатки прошлого уровня (пули/бонусы), игрок остаётся
                    ClearNonPlayerObjects();
                    Screens.Change(new Screens.NextLevelScreen());
                    break;
                case GameState.Playing:
                    // Новая партия — только если пришли из меню/конца игры.
                    // Из паузы — продолжение; из NextLevel — уровень уже загружен.
                    if (oldState != GameState.Paused && oldState != GameState.NextLevel)
                        InitializeGameplay();
                    break;
            }
        }

        #region Методы обновления для каждого состояния

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
                
                // Барражирование улья (формации)
                _hive?.Update(deltaTime);

                // Всплывающие сообщения
                MessageLog.Update(deltaTime);

                // Спавн врагов по таймлайну уровня
                if (_level != null)
                {
                    foreach (var info in _level.Tick(deltaTime))
                        SpawnEnemy(info.Type, info.Formation, info.Route, info.After);
                }

                // Аналог основного цикла из GamePlay.cs - обрабатываем все объекты
                // (удаление мёртвых объектов встроено в ProcessGameObjects)
                ProcessGameObjects(gameTime);

                // Обрабатываем игровые события (аналог gameEventDirector.Update())
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
                Utils.Log.Debug($"Game over: score={Player.Score}, level={_currentLevel}");
                ChangeGameState(GameState.GameOver);
                return true;
            }

            // Уровень пройден: все враги уровня заспавнены и уничтожены
            if (_level != null && _level.AllSpawned && EnemiesRemaining <= 0)
            {
                AdvanceLevel();
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

        private void DrawGameplay(GameTime gameTime)
        {
            
            // Рисуем все игровые объекты
            foreach (var gameObject in GameObjects)
            {
                gameObject.Draw(gameTime, _spriteBatch);
            }

            // Рисуем HUD
            DrawHUD();

            // Панель тестовых кнопок
            foreach (var btn in InputManager.Instance.GuiButtons)
                btn.Draw(_spriteBatch, SimpleTexture);

            // Всплывающие сообщения над кнопками
            MessageLog.Draw(_spriteBatch, _defaultFont, ScreenWidth, ScreenHeight);
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

            // Меню паузы
            DrawCenteredText("ПАУЗА", ScreenHeight / 2f - 40, Color.White);
            DrawCenteredText("Esc / P — продолжить", ScreenHeight / 2f + 20, Color.Yellow);
            DrawCenteredText("Q — выход в меню", ScreenHeight / 2f + 56, Color.Gray);
        }

        private void DrawGameOver(GameTime gameTime)
        {
            DrawCenteredText("GAME OVER", ScreenHeight / 2f - 40, Color.Red);
            DrawCenteredText($"Очки: {_lastScore}", ScreenHeight / 2f + 10, Color.White);
            DrawCenteredText("Пробел — заново, Esc — в меню", ScreenHeight / 2f + 60, Color.Gray);
        }

        private void DrawVictory(GameTime gameTime)
        {
            DrawCenteredText("ПОБЕДА!", ScreenHeight / 2f - 40, Color.Gold);
            DrawCenteredText($"Очки: {_lastScore}", ScreenHeight / 2f + 10, Color.White);
            DrawCenteredText("Пробел — заново, Esc — в меню", ScreenHeight / 2f + 60, Color.Gray);
        }

        #endregion

        #region Вспомогательные методы

        private void InitializeGameplay()
        {
            try
            {
                // Новая игра — с первого уровня
                ResetGameplayStats();
                _currentLevel = 1;

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
                Player.Score = 0;
                
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
                
                // Загружаем первый уровень (враги спавнятся по таймлайну в UpdateGameplay)
                LoadLevel(_currentLevel);

                // Панель тестовых кнопок
                CreateDebugButtons();

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
            EnemiesRemaining = 0; // фактическое значение задаст LoadLevel по данным уровня
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
        /// <summary>Загрузить уровень N: данные из YAML, счётчики врагов. Игрока не трогает.</summary>
        private void LoadLevel(int number)
        {
            _level = new Utils.Level();
            if (!_level.Load(number))
            {
                Console.WriteLine($"=== Level {number} not loaded ===");
                EnemiesRemaining = 0;
                return;
            }
            EnemiesKilled = 0;
            EnemiesRemaining = _level.TotalEnemies;
            Utils.Log.Debug($"Level {_currentLevel} loaded: \"{_level.Description}\", enemies={_level.TotalEnemies}");

            // Мир и улей (формации) — пересоздаём на каждый уровень (сброс занятых ячеек)
            _world = new World(ScreenWidth, ScreenHeight);
            _hive = new Hive(_world, 4, 1, 8, 2); // 8×2 у верхней кромки

            // Вылеты из улья (Galaga) — если включены в описании уровня
            if (_level.Sortie)
                _hive.EnableSortie(_level.SortieInterval, _level.SortieCount);
        }

        /// <summary>Удалить все объекты кроме игрока (между уровнями).</summary>
        private void ClearNonPlayerObjects()
        {
            GameObjects.RemoveAll(o => !(o is PlayerShip));
        }

        /// <summary>Перейти к следующему уровню или к финальной победе.</summary>
        private void AdvanceLevel()
        {
            if (_currentLevel >= _totalLevels)
            {
                ChangeGameState(GameState.Victory); // все уровни пройдены
            }
            else
            {
                _currentLevel++;
                LoadLevel(_currentLevel);            // подготовить следующий уровень
                ChangeGameState(GameState.NextLevel); // показать экран между уровнями
            }
        }

        /// <summary>
        /// Создать врага сверху экрана. formation — в улей; routeName — пустить по маршруту.
        /// </summary>
        private void SpawnEnemy(EnemyType type, bool formation, string routeName = null, string after = null)
        {
            // Пытаемся занять ячейку улья для формации
            int cx = -1, cy = -1;
            bool inFormation = formation && _hive != null && _hive.TryTakeCell(out cx, out cy);

            // Маршрут (если не в формации и задан)
            Route route = (!inFormation && !string.IsNullOrEmpty(routeName) && _world != null)
                ? Route.Load(routeName, _world)
                : null;

            // Точка появления: ячейка улья / первая точка маршрута / случайно по X
            Vector2 pos;
            if (inFormation)
                pos = new Vector2(_hive.CellWorldPos(cx, cy).X, -30);
            else if (route != null && route.HasPoints)
                pos = route.Current;
            else
            {
                float border = ScreenWidth * 0.12f;
                float x = border + (float)_spawnRnd.NextDouble() * (ScreenWidth - 2 * border);
                pos = new Vector2(x, -30);
            }

            Enemy e;
            switch (type)
            {
                case EnemyType.BLUE: e = new EnemySmallBlue(pos); break;
                case EnemyType.GREEN: e = new EnemySmallGreen(pos); break;
                case EnemyType.RED: e = new EnemySmallRed(pos); break;
                case EnemyType.BOSS: e = new EnemyBoss(pos); break;
                default: e = new EnemySmallScout(pos); break;
            }

            if (inFormation)
            {
                e.JoinFormation(_hive, cx, cy);
                _hive.Register(e, cx, cy); // учёт в улье — для координации вылетов
            }
            else if (route != null && route.HasPoints)
                e.SetRoute(route, ParseRouteEnd(after), _hive);

            GameObjects.Add(e);
        }

        private static RouteEndBehavior ParseRouteEnd(string s)
        {
            switch (s?.Trim().ToLowerInvariant())
            {
                case "formation": return RouteEndBehavior.Formation;
                case "scatter": return RouteEndBehavior.Scatter;
                default: return RouteEndBehavior.Bounce;
            }
        }

        /// <summary>Тестовый переход на следующий уровень (кнопка/команда).</summary>
        public void DebugNextLevel()
        {
            if (CurrentGameState == GameState.Playing)
                AdvanceLevel();
        }

        /// <summary>
        /// Создаёт панель тестовых кнопок (смена оружия, апгрейд, лечение, убить всех,
        /// следующий уровень) и регистрирует их в InputManager. Внизу слева.
        /// </summary>
        private void CreateDebugButtons()
        {
            InputManager.Instance.GuiButtons.Clear();
            const int size = 50, gap = 6;
            int y = ScreenHeight - size - 8;
            int x = 10;
            AddDebugButton(new ButtonCannon(Vector2.Zero), "Images/btn_cannon", ref x, y, size, gap);
            AddDebugButton(new ButtonMinigun(Vector2.Zero), "Images/btn_minigun", ref x, y, size, gap);
            AddDebugButton(new ButtonLaser(Vector2.Zero), "Images/btn_laser", ref x, y, size, gap);
            AddDebugButton(new ButtonUpgradeGun(Vector2.Zero), "Images/btn_BulletUp", ref x, y, size, gap);
            AddDebugButton(new ButtonHpUp(Vector2.Zero), "Images/btn_hp_up", ref x, y, size, gap);
            AddDebugButton(new ButtonKillAll(Vector2.Zero), "Images/btn_killall", ref x, y, size, gap);
            AddDebugButton(new ButtonNextLevel(Vector2.Zero), "Images/btn_win", ref x, y, size, gap);
        }

        private void AddDebugButton(MyButton b, string sprite, ref int x, int y, int size, int gap)
        {
            b.Width = b.Height = size;
            b.Position = new Vector2(x + size / 2f, y + size / 2f); // GetRect центрирует по Position
            try { b.sprite = _content.Load<Texture2D>(sprite); }
            catch (Exception ex) { Console.WriteLine($"=== Button sprite '{sprite}' load failed: {ex.Message} ==="); }
            InputManager.Instance.GuiButtons.Add(b);
            x += size + gap;
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
            // НЕ меняем состояние здесь: смерть происходит внутри цикла обработки объектов
            // (ProcessCollision), а ChangeGameState→CleanupGameplay чистит список объектов и
            // привёл бы к падению. Переход в GameOver безопасно выполнит CheckGameEndConditions
            // в начале следующего кадра (Player.Health <= 0).
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
            if (obj is Enemy)
            {
                // Запускаем ивент смерти врага
                TriggerEnemyDeathEvent(obj);

                // Выпадение бонусов из убитого врага
                SpawnBonusOnEnemyDeath(obj.Position);
            }

            // Выполняем базовое удаление объекта
            obj.IsAlive = false; // Помечаем объект как мертвый
        }

        private static readonly Random _bonusRnd = new Random();

        /// <summary>
        /// Выпадение бонусов из убитого врага: всегда звезда (очки) + иногда усиление.
        /// Аналог CommandStarBonus + CommandSpawnRandomBonus из CocosSharp.
        /// </summary>
        private void SpawnBonusOnEnemyDeath(Vector2 pos)
        {
            // Звезда выпадает всегда
            GameObjects.Add(new BonusStar(pos));

            // С шансом 30% — случайное усиление
            int roll = _bonusRnd.Next(100);
            if (roll < 30)
            {
                int kind = _bonusRnd.Next(100);
                Bonus bonus;
                if (kind < 45)
                    bonus = new BonusHpUp(pos);        // 45%
                else if (kind < 90)
                    bonus = new BonusBulletUp(pos);    // 45%
                else
                    bonus = new BonusNukeBomb(pos);    // 10%
                GameObjects.Add(bonus);
            }
        }

        private void CleanupGameplay()
        {
            // Отписываемся от событий игрока
            UnsubscribeFromPlayerEvents();
            
            // Очистка ресурсов игрового процесса
            GameObjects.Clear();
            InputManager.Instance.GuiButtons.Clear(); // убрать тестовые кнопки
            MessageLog.Clear();
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
            // Игрок подобрал бонус
            else if (a is Bonus bonus && b is PlayerShip player)
            {
                bonus.Apply(player);
                bonus.IsAlive = false;
            }
        }

        /// <summary>Уничтожить всех врагов на экране (бонус NukeBomb).</summary>
        public void KillAllEnemies()
        {
            foreach (var obj in GameObjects)
            {
                if (obj is Enemy enemy)
                    enemy.TakeDamage(enemy.Hp);
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

        private void DrawHUD()
        {
            if (Player == null)
                return;

            // Текст здоровья и очков
            if (_defaultFont != null)
            {
                _spriteBatch.DrawString(_defaultFont, $"HP: {Player.Health}/{Player.MaxHealth}",
                    new Vector2(10, 10), Color.White);

                string scoreText = $"Очки: {Player.Score}";
                float scoreW = _defaultFont.MeasureString(scoreText).X;
                _spriteBatch.DrawString(_defaultFont, scoreText, new Vector2(ScreenWidth - scoreW - 10, 10), Color.White);

                string weaponText = $"Оружие: {Player.CurrentWeapon} (ур. {Player.Gun.Level + 1})";
                _spriteBatch.DrawString(_defaultFont, weaponText, new Vector2(10, 60), Color.LightGray);
            }

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
