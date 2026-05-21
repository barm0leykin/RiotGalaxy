using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using RiotGalaxy.GameObjects;

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

        // Приватный конструктор для singleton
        private GameManager()
        {
            CurrentGameState = GameState.MainMenu;
            GameObjects = new List<GameObject>();
            ScreenWidth = 1280;
            ScreenHeight = 768;
            Console.WriteLine($"=== GameManager initialized with state: {CurrentGameState} ===");
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
            // Здесь будем загружать спрайты, звуки и т.д.
        }

        /// <summary>
        /// Основной игровой цикл - обновление состояния игры
        /// </summary>
        public void Update(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Обновляем все объекты в соответствии с текущим состоянием
            switch (CurrentGameState)
            {
                case GameState.MainMenu:
                    // UpdateMainMenu(deltaTime);
                    break;
                case GameState.Playing:
                    UpdateGameplay(gameTime);
                    break;
                case GameState.Paused:
                    // UpdatePaused(deltaTime);
                    break;
                case GameState.GameOver:
                    // UpdateGameOver(deltaTime);
                    break;
                case GameState.Victory:
                    // UpdateVictory(deltaTime);
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
            // Обновляем все игровые объекты
            foreach (var gameObject in GameObjects)
            {
                gameObject.Update(gameTime);
            }

            // Проверка столкновений (пока аналогично CocosSharp)
            CheckCollisions();

            // Удаление объектов отмеченных для удаления
            RemoveDeadObjects();
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
                // Инициализация игрового процесса
                GameObjects.Clear();
                
                // Создаем игрока
                Player = new PlayerShip(new Vector2(ScreenWidth / 2, ScreenHeight - 100));
                Player.SetGraphicsDevice(GraphicsDevice);
                GameObjects.Add(Player);

                Console.WriteLine($"=== Gameplay Initialized - Player created at ({Player.Position.X}, {Player.Position.Y}) ===");
                Console.WriteLine($"=== Total game objects: {GameObjects.Count} ===");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error initializing gameplay: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                // Заглушка - просто очищаем объекты
                GameObjects.Clear();
            }
        }

        private void CleanupGameplay()
        {
            // Очистка ресурсов игрового процесса
            GameObjects.Clear();
            Player = null;
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
            // Удаляем объекты отмеченные для удаления
            for (int i = GameObjects.Count - 1; i >= 0; i--)
            {
                if (!GameObjects[i].IsAlive)
                {
                    GameObjects.RemoveAt(i);
                }
            }
        }

        private void DrawHUD()
        {
            try {
                // Рисуем здоровье игрока (пока заглушка)
                // if (Player != null)
                // {
                //     string healthText = $"Health: {Player.Health}/{Player.MaxHealth}";
                    try {
                        // _spriteBatch.DrawString(_content.Load<SpriteFont>("TestFont"), 
                        //     healthText, 
                        //     new Vector2(10, 10), 
                        //     Color.White);
                        DrawSimplePlaceholder(10, 10, 200, 30, "RiotGalaxy HUD");
                    }
                    catch
                    {
                        // Если шрифт не загружен, просто пропускаем отрисовку текста
                    }
                // }
            }
            catch
            {
                // Заглушка - просто рисуем прямоугольник
                DrawSimplePlaceholder(10, 10, 200, 30, "HUD");
            }
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

        #endregion
    }
}
