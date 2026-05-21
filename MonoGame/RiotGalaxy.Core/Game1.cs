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

        private bool _autoTransition = true; // Автоматический переход в Playing

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || 
                Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                // Выход только из главного меню
                if (_gameManager.CurrentGameState == GameManager.GameState.MainMenu)
                {
                    Exit();
                }
                else
                {
                    // В остальных случаях возвращаем в главное меню
                    _gameManager.ChangeGameState(GameManager.GameState.MainMenu);
                }
            }

            // Автоматический переход в состояние Playing для тестирования
            if (_autoTransition && _gameManager.CurrentGameState == GameManager.GameState.MainMenu)
            {
                _gameManager.ChangeGameState(GameManager.GameState.Playing);
                _autoTransition = false; // Только один раз
            }

            // Обработка клавиатуры для управления игрой
            HandleKeyboardInput();
            
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
        private void HandleKeyboardInput()
        {
            var keyboardState = Keyboard.GetState();
            
            switch (_gameManager.CurrentGameState)
            {
                case GameManager.GameState.MainMenu:
                    // В главном меню - клавиша Пробел для старта игры
                    if (keyboardState.IsKeyDown(Keys.Space) && !_previousKeyboardState.IsKeyDown(Keys.Space))
                    {
                        _gameManager.ChangeGameState(GameManager.GameState.Playing);
                    }
                    break;
                    
                case GameManager.GameState.Playing:
                    // В игре - клавиша P для паузы
                    if (keyboardState.IsKeyDown(Keys.P) && !_previousKeyboardState.IsKeyDown(Keys.P))
                    {
                        _gameManager.ChangeGameState(GameManager.GameState.Paused);
                    }
                    break;
                    
                case GameManager.GameState.Paused:
                    // В паузе - клавиша P для продолжения, ESC для выхода в меню
                    if (keyboardState.IsKeyDown(Keys.P) && !_previousKeyboardState.IsKeyDown(Keys.P))
                    {
                        _gameManager.ChangeGameState(GameManager.GameState.Playing);
                    }
                    else if (keyboardState.IsKeyDown(Keys.Escape) && !_previousKeyboardState.IsKeyDown(Keys.Escape))
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
