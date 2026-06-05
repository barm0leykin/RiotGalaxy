using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using RiotGalaxy.Commands;
using RiotGalaxy.Interface;
using RiotGalaxy.Components;
using RiotGalaxy.GameObjects;

namespace RiotGalaxy.Managers
{
    /// <summary>
    /// Класс InputManager - обрабатывает ввод пользователя.
    /// Аналог InputHandler из CocosSharp, адаптированный для MonoGame.
    /// </summary>
    public class InputManager
    {
        // Singleton pattern
        private static InputManager _instance;
        public static InputManager Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new InputManager();
                return _instance;
            }
        }

        // Состояния touch, адаптировано из CocosSharp
        private bool isTouch = false;
        private bool isTouchBegan = false;
        private bool _touchOnUI = false; // текущее касание началось на UI-кнопке (не двигаем корабль)
        private Vector2 locationOnScreen;   // в ВИРТУАЛЬНЫХ координатах (1280x768)
        private Vector2 previousMousePosition;
        private Vector2 _lastPointerRaw;    // последняя сырая (экранная) позиция указателя/пальца
        
        // Состояния клавиатуры и мыши
        private KeyboardState _currentKeyboardState;
        private KeyboardState _previousKeyboardState;
        private MouseState _currentMouseState;
        private MouseState _previousMouseState;
        
        // GUI кнопки (аналог MyButton из CocosSharp)
        public List<MyButton> GuiButtons { get; private set; }
        
        /// <summary>
        /// Доступ к GUIButtons для совместимости с другими частями кода
        /// </summary>
        public List<MyButton> GUIButtons => GuiButtons;
        
        // Приватный конструктор для singleton
        private InputManager()
        {
            GuiButtons = new List<MyButton>();
            CreateTouchListener();
        }

        /// <summary>
        /// Создание обработчика касаний (аналог CreateTouchListener из CocosSharp)
        /// </summary>
        private void CreateTouchListener()
        {
            // В MonoGame нет прямого аналога CCEventListenerTouchAllAtOnce
            // Вместо этого мы обрабатываем состояния мыши в методе Update()
        }

        /// <summary>
        /// Обновление состояния ввода
        /// </summary>
        public void Update()
        {
            // Сохраняем предыдущие состояния
            _previousKeyboardState = _currentKeyboardState;
            _previousMouseState = _currentMouseState;
            previousMousePosition = locationOnScreen;
            
            // Получаем текущие состояния
            _currentKeyboardState = Keyboard.GetState();
            _currentMouseState = Mouse.GetState();
            
            // Обработка мыши как аналога touch для desktop
            HandleMouseAsTouch();

            // Обработка клавиатуры и других устройств ввода
            HandleKeyboardInput();
        }

        /// <summary>
        /// Обработка мыши как аналога touch-интерфейса
        /// </summary>
        private void HandleMouseAsTouch()
        {
            // Источник указателя кросс-платформенный: на Android — палец (TouchPanel),
            // на desktop — мышь. Сырые экранные координаты переводим в виртуальные.
            bool newIsTouch;
            Vector2 raw;
#if ANDROID
            var touches = TouchPanel.GetState();
            if (touches.Count > 0)
            {
                newIsTouch = true;
                raw = touches[0].Position;
                _lastPointerRaw = raw;
            }
            else
            {
                newIsTouch = false;
                raw = _lastPointerRaw; // удерживаем последнюю позицию для корректного "отпускания"
            }
#else
            newIsTouch = _currentMouseState.LeftButton == ButtonState.Pressed;
            raw = new Vector2(_currentMouseState.X, _currentMouseState.Y);
            _lastPointerRaw = raw;
#endif
            Vector2 newLocation = GameManager.Instance.ScreenToVirtual(raw);

            if (newIsTouch && !isTouch)
            {
                // Начало касания
                isTouch = true;
                isTouchBegan = true;
                locationOnScreen = newLocation;
                HandleTouchesBegan(locationOnScreen);
            }
            else if (newIsTouch && isTouch)
            {
                // Продолжительное касание
                isTouch = true;
                isTouchBegan = false;
                if (previousMousePosition != newLocation)
                {
                    // Позиция изменилась
                    locationOnScreen = newLocation;
                    HandleTouchesMoved(locationOnScreen);
                }
            }
            else if (!newIsTouch && isTouch)
            {
                // Конец касания
                isTouch = false;
                isTouchBegan = false;
                _touchOnUI = false;
                HandleTouchesEnded(locationOnScreen);
            }
        }

        /// <summary>
        /// Проверка нажатия клавиши
        /// </summary>
        public bool IsKeyPressed(Keys key)
        {
            return _currentKeyboardState.IsKeyDown(key);
        }

        /// <summary>
        /// Проверка однократного нажатия клавиши
        /// </summary>
        public bool IsKeyJustPressed(Keys key)
        {
            return _currentKeyboardState.IsKeyDown(key) && _previousKeyboardState.IsKeyUp(key);
        }

        /// <summary>
        /// Проверка столкновения с GUI кнопками
        /// Аналог HandlePressButtons из CocosSharp InputHandler
        /// </summary>
        public bool HandlePressButtons(Vector2 touchPoint)
        {
            foreach (var button in GuiButtons)
            {
                if (button == null)
                    continue;
                if (button.CheckCollision(touchPoint))
                {
                    button.Press();
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Добавление GUI кнопки
        /// Аналог AddButnHandler из CocosSharp InputHandler
        /// </summary>
        public void AddButnHandler(MyButton btn)
        {
            GuiButtons.Add(btn);
        }

        /// <summary>
        /// Удаление GUI кнопки
        /// Аналог DelBtnHandler из CocosSharp InputHandler
        /// </summary>
        public void DelBtnHandler(MyButton btn)
        {
            GuiButtons.Remove(btn);
        }

        /// <summary>
        /// Добавление GUI кнопки (новый метод для совместимости)
        /// </summary>
        public void AddButtonHandler(MyButton btn)
        {
            AddButnHandler(btn);
        }

        /// <summary>
        /// Удаление GUI кнопки (новый метод для совместимости)
        /// </summary>
        public void RemoveButtonHandler(MyButton btn)
        {
            DelBtnHandler(btn);
        }

        /// <summary>
        /// Обработка игрового ввода
        /// Аналог HandleScGameInput из CocosSharp InputHandler
        /// </summary>
        public void HandleScGameInput()
        {
            var player = GameManager.Instance.Player;
            bool hasKeyboardInput = false;
            Vector2 movementInput = Vector2.Zero;

#if ANDROID
            // На Android — автоогонь: корабль стреляет непрерывно (палец только двигает).
            // Темп/очереди/перезарядку контролирует само оружие.
            player?.Fire();
#else
            // Стрельба: удержание Пробела. Темп/очереди/перезарядку контролирует само оружие.
            if (IsKeyPressed(Keys.Space) && player != null)
            {
                player.Fire();
            }
#endif

            // Смена оружия: 1 — пушка, 2 — пулемёт, 3 — лазер
            if (player != null)
            {
                if (IsKeyJustPressed(Keys.D1)) player.ChangeWeapon(WeaponType.Cannon);
                else if (IsKeyJustPressed(Keys.D2)) player.ChangeWeapon(WeaponType.MachineGun);
                else if (IsKeyJustPressed(Keys.D3)) player.ChangeWeapon(WeaponType.Laser);
            }
            
            // Обработка клавиатурного ввода для движения
            if (IsKeyPressed(Keys.A) || IsKeyPressed(Keys.Left))
            {
                movementInput = new Vector2(-1, 0);
                hasKeyboardInput = true;
            }
            else if (IsKeyPressed(Keys.D) || IsKeyPressed(Keys.Right))
            {
                movementInput = new Vector2(1, 0);
                hasKeyboardInput = true;
            }
            
            // Приоритет клавиатуры над мышью/тачем
            if (hasKeyboardInput && player != null && player.Movement is PlayerMovementComponent playerMovement)
            {
                // Прямое управление движением через компонент
                if (movementInput.X < 0)
                    playerMovement.MoveLeft();
                else if (movementInput.X > 0)
                    playerMovement.MoveRight();
            }
            else if (isTouchBegan)   // это первое нажатие (тач/мышь)?
            {
                isTouchBegan = false;

                if (HandlePressButtons(locationOnScreen))   // касание попало по UI-кнопке?
                {
                    _touchOnUI = true; // это касание — UI, кораблём не управляем до отпускания
                }
                else
                {
                    _touchOnUI = false;
                    if (GameManager.Instance.CurrentGameState == GameManager.GameState.Paused) // мб игра на паузе?
                    {
                        CommandPauseWeaponMenu cmd = new CommandPauseWeaponMenu();
                        cmd.Execute();
                    }
                    else
                    {
                        SetPlayerMoveDirection(locationOnScreen);    // двигаем playerShip
                    }
                }
            }
            else if (isTouch)   // касание продолжается — управляем кораблём (если оно не по UI)
            {
                if (!_touchOnUI)
                    SetPlayerMoveDirection(locationOnScreen);
            }
            else // касаний экрана и клавиатуры нет, ускорение кораблю больше не предаем
            {
                // Вызываем остановку движения только если действительно нет ввода
                if (!hasKeyboardInput && !isTouch && player != null && player.Movement is PlayerMovementComponent playerMoveComponent)
                {
                    playerMoveComponent.MoveStop();
                }
            }
        }

        /// <summary>
        /// Установка направления движения игрока
        /// </summary>
        private void SetPlayerMoveDirection(Vector2 inputPosition)
        {
            var player = GameManager.Instance.Player;
            if (player != null && player.Movement is PlayerMovementComponent playerMovement)
            {
                // Используем компонент движения игрока
                playerMovement.SetMoveDirection(inputPosition);
            }
        }

        /// <summary>
        /// Остановка движения игрока
        /// </summary>
        private void StopPlayerMovement()
        {
            var player = GameManager.Instance.Player;
            if (player != null && player.Movement is PlayerMovementComponent playerMovement)
            {
                // Используем компонент движения игрока
                playerMovement.MoveStop();
            }
        }

        /// <summary>
        /// Обработка клавиатурного ввода
        /// </summary>
        private void HandleKeyboardInput()
        {
            // Обработка клавиатуры для стрельбы и других действий
            if (IsKeyJustPressed(Keys.Space))
            {
                HandleShootAction();
            }
            
            if (IsKeyJustPressed(Keys.Escape))
            {
                HandleEscapeAction();
            }
            
            if (IsKeyJustPressed(Keys.P))
            {
                HandlePauseAction();
            }
            
            if (IsKeyJustPressed(Keys.Enter))
            {
                HandleEnterAction();
            }
        }

        /// <summary>
        /// Обработка действия стрельбы
        /// </summary>
        private void HandleShootAction()
        {
            var player = GameManager.Instance.Player;
            player?.Fire();
        }

        /// <summary>
        /// Обработка действия Escape
        /// </summary>
        private void HandleEscapeAction()
        {
            var currentState = GameManager.Instance.CurrentGameState;
            switch (currentState)
            {
                case GameManager.GameState.Playing:
                    GameManager.Instance.ChangeGameState(GameManager.GameState.Paused);
                    break;
                case GameManager.GameState.Paused:
                    GameManager.Instance.ChangeGameState(GameManager.GameState.MainMenu);
                    break;
                case GameManager.GameState.MainMenu:
                    // Выйти из игры
                    break;
            }
        }

        /// <summary>
        /// Обработка действия паузы
        /// </summary>
        private void HandlePauseAction()
        {
            var currentState = GameManager.Instance.CurrentGameState;
            if (currentState == GameManager.GameState.Playing)
            {
                GameManager.Instance.ChangeGameState(GameManager.GameState.Paused);
            }
            else if (currentState == GameManager.GameState.Paused)
            {
                GameManager.Instance.ChangeGameState(GameManager.GameState.Playing);
            }
        }

        /// <summary>
        /// Обработка действия Enter
        /// </summary>
        private void HandleEnterAction()
        {
            var currentState = GameManager.Instance.CurrentGameState;
            switch (currentState)
            {
                case GameManager.GameState.MainMenu:
                    GameManager.Instance.ChangeGameState(GameManager.GameState.Playing);
                    break;
                case GameManager.GameState.GameOver:
                case GameManager.GameState.Victory:
                    GameManager.Instance.ChangeGameState(GameManager.GameState.MainMenu);
                    break;
            }
        }

        #region Методы-аналоги обработчиков touch из CocosSharp

        /// <summary>
        /// Аналог HandleTouchesBegan из CocosSharp
        /// </summary>
        private void HandleTouchesBegan(Vector2 position)
        {
            isTouch = true;
            isTouchBegan = true;
            locationOnScreen = position;
        }

        /// <summary>
        /// Аналог HandleTouchesEnded из CocosSharp
        /// </summary>
        private void HandleTouchesEnded(Vector2 position)
        {
            isTouch = false;
        }

        /// <summary>
        /// Аналог HandleTouchesCanceled из CocosSharp
        /// </summary>
        private void HandleTouchesCanceled(Vector2 position)
        {
            isTouch = false;
        }

        /// <summary>
        /// Аналог HandleTouchesMoved из CocosSharp
        /// </summary>
        private void HandleTouchesMoved(Vector2 position)
        {
            isTouch = true;
            locationOnScreen = position;
        }

        #endregion
    }
}