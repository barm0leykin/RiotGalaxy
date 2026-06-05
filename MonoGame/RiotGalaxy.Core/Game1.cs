using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using RiotGalaxy.Managers;

namespace RiotGalaxy.Core
{
    /// <summary>
    /// Это главный класс игры для RiotGalaxy на MonoGame.
    /// Наследуется от Microsoft.Xna.Framework.Game
    /// </summary>
    public class Game1 : Game
    {
        private static Game1 _instance;
        public static Game1 Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new Game1();
                return _instance;
            }
        }

        private GraphicsDeviceManager _graphics;
        private GameManager _gameManager;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            // Создаем экземпляр GameManager
            _gameManager = GameManager.Instance;
            
            // Инициализируем GameManager в конструкторе
            _gameManager.Initialize(this, _graphics, Content);
        }

        protected override void Initialize()
        {
            // Дополнительная инициализация, если нужна
            
            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Загружаем контент через GameManager
            _gameManager.LoadContent();
            
            // Создаем простую текстуру для заглушек
            _gameManager.SimpleTexture = CreateSimpleTexture(Color.White);
        }

        protected override void Update(GameTime gameTime)
        {
            // Ввод игровых состояний (меню/настройки обрабатывают сами экраны)
            HandleGameplayKeys();

            // Передаем обновление в GameManager
            _gameManager.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            // Передаем отрисовку в GameManager
            _gameManager.Draw(gameTime);
            
            base.Draw(gameTime);
        }
        
        /// <summary>
        /// Обработка ввода с клавиатуры
        /// </summary>
        private void HandleGameplayKeys()
        {
            var keyboardState = Keyboard.GetState();

            // Кнопка «Назад» на Android: MainActivity.OnBackPressed (UI-поток) выставляет флаг,
            // а смену состояния делаем здесь, в игровом потоке (без гонки с циклом Update).
            _gameManager.ProcessPendingBack();

            switch (_gameManager.CurrentGameState)
            {
                case GameManager.GameState.Playing:
                    // В игре - Esc (или P) ставит на паузу
                    if ((keyboardState.IsKeyDown(Keys.Escape) && !_previousKeyboardState.IsKeyDown(Keys.Escape)) ||
                        (keyboardState.IsKeyDown(Keys.P) && !_previousKeyboardState.IsKeyDown(Keys.P)))
                    {
                        _gameManager.ChangeGameState(GameManager.GameState.Paused);
                    }
                    break;

                case GameManager.GameState.Paused:
                    // В паузе - Esc (или P) продолжает игру, Q - выход в главное меню
                    if ((keyboardState.IsKeyDown(Keys.Escape) && !_previousKeyboardState.IsKeyDown(Keys.Escape)) ||
                        (keyboardState.IsKeyDown(Keys.P) && !_previousKeyboardState.IsKeyDown(Keys.P)))
                    {
                        _gameManager.ChangeGameState(GameManager.GameState.Playing);
                    }
                    else if (keyboardState.IsKeyDown(Keys.Q) && !_previousKeyboardState.IsKeyDown(Keys.Q))
                    {
                        _gameManager.ChangeGameState(GameManager.GameState.MainMenu);
                    }
                    break;
                    
                case GameManager.GameState.GameOver:
                    // В экране Game Over - Пробел для перезапуска, ESC для выхода в меню
                    if (keyboardState.IsKeyDown(Keys.Space) && !_previousKeyboardState.IsKeyDown(Keys.Space))
                    {
                        _gameManager.ChangeGameState(GameManager.GameState.Playing);
                    }
                    else if (keyboardState.IsKeyDown(Keys.Escape) && !_previousKeyboardState.IsKeyDown(Keys.Escape))
                    {
                        _gameManager.ChangeGameState(GameManager.GameState.MainMenu);
                    }
                    break;

                case GameManager.GameState.Victory:
                    // На экране победы - Space перезапуск, Esc в меню
                    if (keyboardState.IsKeyDown(Keys.Space) && !_previousKeyboardState.IsKeyDown(Keys.Space))
                    {
                        _gameManager.ChangeGameState(GameManager.GameState.Playing);
                    }
                    else if (keyboardState.IsKeyDown(Keys.Escape) && !_previousKeyboardState.IsKeyDown(Keys.Escape))
                    {
                        _gameManager.ChangeGameState(GameManager.GameState.MainMenu);
                    }
                    break;
            }

            // Сохраняем предыдущее состояние клавиатуры
            _previousKeyboardState = keyboardState;
        }
        
        /// <summary>
        /// Создание простой текстуры (белый квадрат)
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
        
        // Храним предыдущее состояние клавиатуры для детектирования нажатий
        private KeyboardState _previousKeyboardState;
    }
}
