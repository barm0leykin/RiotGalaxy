# RiotGalaxy (MonoGame)

Минимально работающая версия игры RiotGalaxy, мигрированной с CocosSharp на MonoGame.

## Структура проекта

```
MonoGame/
├── RiotGalaxy.Core/          # Основная игровая логика
│   ├── GameObjects/          # Игровые объекты (Player, Enemy, Bullet, Bonus)
│   ├── Components/          # Компоненты (Movement, Shooting, Collision)
│   ├── AI/                   # AI системы
│   ├── Managers/             # GameManager, AudioManager, InputManager
│   └── Utils/                # Вспомогательные классы
├── RiotGalaxy.Content/       # Ассеты (спрайты, звуки, шрифты)
│   └── Content/              # Папка для ассетов
├── RiotGalaxy.DesktopGL/     # Desktop версия (Windows/Linux/macOS)
├── RiotGalaxy.Android/       # Android версия
└── RiotGalaxy.iOS/          # iOS версия (опционально)
```

## Требования для запуска

- .NET 6.0 SDK
- MonoGame.Framework.DesktopGL 3.8.1.303

## Инструкции по сборке и запуску

### Для Linux и macOS:

1. Откройте терминал в директории `RiotGalaxy/MonoGame`
2. Соберите проект:
   ```bash
   dotnet build RiotGalaxy.sln
   ```
3. Запустите игру:
   ```bash
   dotnet run --project RiotGalaxy.DesktopGL/RiotGalaxy.DesktopGL.csproj
   ```

### Для Windows:

1. Откройте командную строку в директории `RiotGalaxy/MonoGame`
2. Соберите проект:
   ```
   dotnet build RiotGalaxy.sln
   ```
3. Запустите игру:
   ```
   dotnet run --project RiotGalaxy.DesktopGL/RiotGalaxy.DesktopGL.csproj
   ```

## Что сейчас реализовано

- ✅ Базовая структура проекта
- ✅ Окно с разрешением 1280x768 как в оригинальной игре
- ✅ Отрисовка трех цветных квадратов (красный, синий, зеленый)
- ✅ Базовая архитектура компонентов (GameObject, MovementComponent, ShootingComponent, CollisionComponent)
- ✅ Синглтон-паттерн для класса Game1
- ✅ Классы игровых объектов: Player, Enemy, Bullet, Bonus
- ✅ Компоненты для движения, стрельбы и столкновений

## Известные проблемы

- Предупреждение о целевой платформе net6.0 (не критично)
- Предупреждение об отсутствии контента (ожидаемо для минимальной версии)

## Следующие шаги

- 🔄 Реализация системы управления вводом
- 🔄 Добавление реальных спрайтов и анимаций
- 🔄 Реализация полной системы столкновений
- 🔄 Создание уровня и игровой логики

## Статус проекта

✅ **Статус: Минимально работающая версия готова**

    Проект успешно компилируется и запускается. Отображает окно с тремя цветными квадратами на черном фоне. Основа для дальнейшей миграции с CocosSharp на MonoGame создана.