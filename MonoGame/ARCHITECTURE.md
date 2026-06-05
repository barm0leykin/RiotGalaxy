# RiotGalaxy на MonoGame — как всё устроено

Гайд для тех, кто впервые видит MonoGame. Объясняет структуру проекта, точку входа,
главный цикл, загрузку ресурсов и ключевые архитектурные решения. Привязан к этому
конкретному проекту, а не абстрактный.

> Документация MonoGame: <https://docs.monogame.net/> · исходники: <https://github.com/MonoGame/MonoGame>

---

## 1. Что такое MonoGame (в двух словах)

MonoGame — это open-source реализация фреймворка **XNA** от Microsoft. Он даёт
низкоуровневые кирпичики: окно, игровой цикл, отрисовку спрайтов, ввод, звук, загрузку
ресурсов. Это **не движок с редактором** (как Unity/Godot) — здесь нет визуального
редактора сцен, всю логику и сцены пишешь кодом на C#.

Базовые понятия, которые встретятся везде:

| Понятие | Что это |
|---|---|
| `Game` | Базовый класс приложения. Держит окно и главный цикл. У нас наследник — `Game1`. |
| `GraphicsDeviceManager` | Управляет видеоустройством, разрешением, полноэкранным режимом. |
| `SpriteBatch` | «Пакетная» отрисовка 2D-спрайтов. Всё рисование идёт через него. |
| `Texture2D` | Картинка в памяти видеокарты (спрайт, фон). |
| `SoundEffect` | Звуковой эффект. |
| `GameTime` | Время: сколько прошло с прошлого кадра и всего. Нужно для плавности. |
| `ContentManager` (`Content`) | Загрузчик ресурсов из скомпилированных `.xnb`. |

---

## 2. Структура решения (`RiotGalaxy.sln`)

Решение состоит из нескольких проектов. Это типичный для MonoGame расклад:
«общая логика отдельно, платформа отдельно».

```text
MonoGame/
├── RiotGalaxy.sln                  # файл решения (открывает все проекты сразу)
├── RiotGalaxy.Core/                # 📦 БИБЛИОТЕКА: вся игровая логика (платформонезависимая)
├── RiotGalaxy.Content/             # 🎨 РЕСУРСЫ: спрайты, звуки, шрифты + сборка их в .xnb
├── RiotGalaxy.DesktopGL/           # 🖥️ ПРИЛОЖЕНИЕ для ПК (Windows/Linux/macOS) — точка входа
├── RiotGalaxy.Android/             # 📱 приложение под Android (опционально)
├── RiotGalaxy.iOS/                 # 🍎 приложение под iOS (опционально)
├── tools/                          # вспомогательные скрипты (напр. split_atlas.py)
├── tasks.md                        # план миграции и статус
└── ARCHITECTURE.md                 # этот файл
```

Зачем такое разделение:

- **`RiotGalaxy.Core`** — это `.dll`-библиотека (в [csproj](RiotGalaxy.Core/RiotGalaxy.Core.csproj)
  нет `OutputType`, значит по умолчанию library). Здесь *вся* игра: классы кораблей,
  врагов, менеджеры, компоненты. Не знает, на какой платформе запускается.
- **`RiotGalaxy.DesktopGL`** — это `.exe` (в [csproj](RiotGalaxy.DesktopGL/RiotGalaxy.DesktopGL.csproj)
  стоит `OutputType=WinExe`). Он ссылается на `Core` и на скомпилированные ресурсы.
  Именно его мы запускаем. «GL» = рендеринг через OpenGL (кроссплатформенно).
- **`RiotGalaxy.Content`** — не код, а ресурсы + рецепт их сборки ([.mgcb](RiotGalaxy.Content/RiotGalaxy.Content.mgcb)).
- **Android/iOS** — отдельные «обёртки» под мобильные платформы (пока не в фокусе).

Идея: чтобы добавить новую платформу, пишешь только маленький проект-обёртку,
а `Core` переиспользуется как есть.

---

## 3. Структура `RiotGalaxy.Core`

```text
RiotGalaxy.Core/
├── Game1.cs                 # главный класс игры (наследник Game) — делегирует в GameManager
├── Managers/                # глобальные системы (синглтоны)
│   ├── GameManager.cs       #   состояния, список объектов, столкновения, уровни, отрисовка
│   ├── InputManager.cs      #   ввод (клавиатура/мышь), GUI-кнопки
│   └── AudioManager.cs      #   загрузка/проигрывание звуков
├── GameObjects/             # всё, что живёт на экране
│   ├── GameObject.cs        #   базовый класс (позиция/размер/текстура/Update/Draw)
│   ├── PlayerShip.cs        #   корабль игрока (HP, щит, оружие, очки)
│   ├── Enemy.cs + EnemySmallBlue/Green/Red/Scout.cs + EnemyBoss.cs
│   ├── Shell.cs + Bullet/Slug/Laser.cs        # снаряды
│   ├── Bonus.cs (+ BonusHpUp/BulletUp/NukeBomb/Star)
│   └── World.cs (+ Cell), Hive.cs, Route.cs   # сетка мира, формации, маршруты
├── Weapons/                 # система оружия (паттерн «Стратегия»)
│   ├── Weapon.cs            #   база + WeaponCannon/Minigun/Laser/NoWeapon
│   └── WeaponOptions.cs, WeaponConfig.cs (грузит weapons.yaml)
├── Components/              # компоненты поведения (паттерн «Стратегия»)
│   ├── MovementComponent.cs, EnemyBounceMovement.cs
│   ├── FormationMovement.cs, RouteMovement.cs
│   └── ShootingComponent.cs, CollisionComponent.cs
├── Screens/                 # экраны меню (см. §14)
│   ├── Screen.cs, ScreenSystem.cs
│   └── SplashScreen / MainMenuScreen / SettingsScreen / NextLevelScreen
├── Commands/                # паттерн «Команда» (смена оружия, kill all, next level…)
├── Interface/               # MyButton + кнопки (тестовая панель)
├── Utils/                   # Level.cs, GameOptions.cs, GameSettings.cs, Yaml.cs
└── AI/                      # (пусто — заготовка; ИИ врагов пока внутри их классов)
```

Эта раскладка повторяет архитектуру оригинала на CocosSharp (см. [prd.md](../prd.md)):
сохранены паттерны **Strategy** (компоненты/оружие), **Command** (команды), **State**
(состояния игры).

> Папка `AI/` пока пустая — заготовка под этап 10 (формации/боссы). Остальное реализовано;
> актуальный статус по этапам — в [tasks.md](tasks.md).

---

## 4. Точка входа — откуда всё стартует

Запуск идёт по цепочке:

```text
run_game.sh
  └─ dotnet run --project RiotGalaxy.DesktopGL
       └─ Program.Main()                          ← RiotGalaxy.DesktopGL/Program.cs
            └─ Game1.Instance.Run()               ← запускает игровой цикл
```

[Program.cs](RiotGalaxy.DesktopGL/Program.cs) — крошечный файл, вся его работа:

```csharp
static void Main()
{
    using (var game = RiotGalaxy.Core.Game1.Instance)
        game.Run();   // ← здесь начинается бесконечный игровой цикл
}
```

`game.Run()` — это метод базового класса `Game`. Он создаёт окно и запускает цикл,
который сам вызывает `Update()` и `Draw()` ~60 раз в секунду, пока окно не закроют.

> ⚠️ В этом проекте `Game1` и менеджеры сделаны **синглтонами** (`Game1.Instance`,
> `GameManager.Instance`). Это не «канонический» стиль MonoGame (обычно объект игры
> создают через `new`), но здесь так — наследие переноса с CocosSharp.

---

## 5. Жизненный цикл `Game` и главный цикл

Базовый класс `Game` вызывает методы в строгом порядке. В нашем
[Game1.cs](RiotGalaxy.Core/Game1.cs) переопределены ключевые из них:

```text
1. Конструктор Game1()        → создаём GraphicsDeviceManager, задаём Content.RootDirectory,
                                 создаём GameManager и вызываем его Initialize()
2. Initialize()               → разовая инициализация (после создания графики)
3. LoadContent()              → ОДИН раз: грузим все ресурсы (спрайты, звуки, фон)
   ┌──────────────────────────────────────────────────────────┐
   │ ГЛАВНЫЙ ЦИКЛ (повторяется каждый кадр, ~60 раз/сек):       │
   │ 4. Update(gameTime)  → пересчёт логики (движение, ввод…)   │
   │ 5. Draw(gameTime)    → отрисовка кадра                     │
   └──────────────────────────────────────────────────────────┘
6. UnloadContent() / выход
```

Важная идея — **разделение Update и Draw**:

- `Update(GameTime)` — меняет *состояние мира*: позиции, здоровье, таймеры. Здесь
  читается ввод. **Никакого рисования.**
- `Draw(GameTime)` — только *рисует* текущее состояние. **Ничего не меняет** в логике.

`GameTime` даёт `gameTime.ElapsedGameTime.TotalSeconds` — сколько секунд прошло с
прошлого кадра. Скорости считают как `позиция += скорость * deltaTime`, чтобы движение
было одинаковым при любом FPS.

### Как это выглядит в нашем `Game1`

`Game1` сам по себе тонкий — он **делегирует** всю работу в `GameManager`:

```csharp
protected override void LoadContent() => _gameManager.LoadContent();
protected override void Update(GameTime gameTime)  { …ввод…; _gameManager.Update(gameTime); }
protected override void Draw(GameTime gameTime)    { _gameManager.Draw(gameTime); }
```

Плюс `Game1.Update` обрабатывает клавиши **игровых** состояний (Playing/Paused/GameOver/
Victory): Esc/P — пауза и снятие, Space — рестарт на экранах конца игры. Ввод **меню**
(Splash/MainMenu/Settings/NextLevel) обрабатывают сами экраны (см. §14). «Детект нажатия»
— через `_previousKeyboardState` (реагируем на *момент* нажатия, а не удержание).

> 💡 При старте `GameManager.LoadContent` вызывает `ChangeGameState(Splash)` — игра
> начинается с заставки → меню → игра (прежний авто-переход сразу в Playing убран).

---

## 6. `GameManager` — сердце игры

[GameManager.cs](RiotGalaxy.Core/Managers/GameManager.cs) — центральный диспетчер
(синглтон `GameManager.Instance`). Отвечает за:

- **Состояния игры** (паттерн State) —
  `enum GameState { Splash, MainMenu, Settings, Playing, Paused, GameOver, Victory, NextLevel }`.
  И `Update`, и `Draw` делают `switch` по состоянию. Состояния-**меню** (Splash, MainMenu,
  Settings, NextLevel) делегируются в `ScreenSystem` (см. §14); игровые (Playing/Paused/…)
  обрабатываются в самом `GameManager`. Переходы — через `ChangeGameState`, который решает,
  что чистить/инициализировать (новая партия, пауза = сохранить, между уровнями = сохранить игрока).
- **Список игровых объектов** — `List<GameObject> GameObjects`. В `Playing` каждый кадр:
  спавн врагов по таймлайну уровня, `Update` каждого объекта, проверка столкновений
  (`ProcessCollision`), удаление «мёртвых».
- **Уровни/прогрессия** — текущий уровень и переход на следующий (см. §15).
- **Отрисовку** — держит `SpriteBatch`, рисует фон, объекты, HUD и тестовые кнопки.
- **Экран** — `ScreenWidth/Height` (из `options.yaml`, по умолчанию 1280×768).

### Как рисуется кадр (`GameManager.Draw`)

```csharp
GraphicsDevice.Clear(Color.Black);     // 1. очистить экран
_spriteBatch.Begin();                  // 2. открыть «пакет» отрисовки
   // рисуем фон во всю ширину
   if (_background != null)
       _spriteBatch.Draw(_background, new Rectangle(0,0,ScreenWidth,ScreenHeight), Color.White);
   switch (CurrentGameState) { … }     // 3. рисуем объекты/UI текущего состояния
_spriteBatch.End();                    // 4. закрыть пакет → всё уходит на видеокарту
```

**Правило:** любое рисование спрайтов обязано быть между `spriteBatch.Begin()` и `End()`.

---

## 7. Игровые объекты и компоненты

[GameObject.cs](RiotGalaxy.Core/GameObjects/GameObject.cs) — базовый класс всего, что
есть на экране. Ключевое:

- поля: `Position`, `Size`, `Scale`, `Rotation`, `Opacity`, `Texture`;
- `virtual void Update(GameTime)` — обновляет свои компоненты;
- `virtual void Draw(GameTime, SpriteBatch)` — рисует `Texture` (с центром в середине
  спрайта). Если `Texture == null` — рисует цветную заглушку.

[PlayerShip.cs](RiotGalaxy.Core/GameObjects/PlayerShip.cs) — наследник `GameObject`:
здоровье, неуязвимость (щит), оружие, и загрузка своего спрайта `Images/ship`.

**Компоненты** (папка `Components/`) — это паттерн «Стратегия»: вместо того чтобы
зашивать поведение в сам объект, оно вынесено в подключаемые компоненты:

```csharp
Movement = new PlayerMovementComponent(this, Speed);  // как двигаться
Shooting = new PlayerShootingComponent(this);         // как стрелять
Collision = new PlayerCollisionComponent(this);       // как сталкиваться
```

`GameObject.Update` просто вызывает `Movement?.Update()`, `Shooting?.Update()` и т.д.
Так одно и то же поведение можно переиспользовать у разных объектов.

---

## 8. Ресурсы: Content Pipeline (САМОЕ ВАЖНОЕ для новичка)

Это место, которое чаще всего путает новичков в MonoGame.

### Зачем нужен «пайплайн»

MonoGame **не грузит PNG/WAV напрямую** в релизе. Сначала специальная утилита
компилирует исходные файлы (`.png`, `.wav`, `.spritefont`) в бинарный формат **`.xnb`**,
оптимизированный под платформу. В игре ты грузишь уже `.xnb`.

```text
ship.png  ──[ MGCB / dotnet-mgcb ]──►  ship.xnb  ──[ Content.Load<Texture2D> ]──►  Texture2D в игре
```

### Действующие лица

1. **`.mgcb`-файл** — [RiotGalaxy.Content.mgcb](RiotGalaxy.Content/RiotGalaxy.Content.mgcb).
   Это текстовый «рецепт»: список всех ресурсов и как каждый собирать. Пример блока:

   ```text
   #begin Images/ship.png
   /importer:TextureImporter
   /processor:TextureProcessor
   /processorParam:PremultiplyAlpha=True
   /build:Images/ship.png
   ```

2. **Утилита `dotnet-mgcb`** — собственно компилятор ресурсов. Ставится как локальный
   инструмент (см. [.config/dotnet-tools.json](.config/dotnet-tools.json)). Если её нет —
   сборка контента падает. Восстановить: `dotnet tool restore`.

3. **`MonoGameContentReference`** в [DesktopGL.csproj](RiotGalaxy.DesktopGL/RiotGalaxy.DesktopGL.csproj) —
   связывает `.mgcb` с приложением. При сборке `.xnb` автоматически копируются в
   выходную папку игры (в подпапку `Content/`).

4. **`Content.RootDirectory = "Content"`** в `Game1` — говорит, что грузить ресурсы надо
   из папки `Content/` рядом с `.exe`.

### Где лежат ресурсы

```text
RiotGalaxy.Content/
├── RiotGalaxy.Content.mgcb     # рецепт сборки (.xnb): Images, Backgrounds, Sounds, шрифт
├── Images/                     # спрайты (26 шт., нарезаны из старого атласа images.png)
│   ├── ship.png, enemyBlue.png, bullet.png, shield.png …
├── Backgrounds/                # фоны: background_blue (1280×768), background, SCConvoy_0
├── Sounds/                     # звуки: fire1.wav, explode1.wav
├── TestFont.spritefont         # описание шрифта (DejaVu Sans Mono, + кириллица)
├── Config/                     # YAML-конфиги (НЕ через MGCB): weapons.yaml, options.yaml
└── Levels/                     # уровни: level1..5.yaml (НЕ через MGCB)
```

> Важная тонкость: пути в `.mgcb` отсчитываются от папки самого `.mgcb`, поэтому
> бинарные ресурсы (`Images/Sounds/Backgrounds/`шрифт) лежат **прямо** в `RiotGalaxy.Content/`,
> а имя ассета для загрузки — путь без расширения (напр. `"Images/ship"`).
>
> **Два пути доставки контента:** бинарные ассеты идут через **MGCB → `.xnb`** и грузятся
> `Content.Load<…>`. А **текстовые** конфиги/уровни (`Config/*.yaml`, `Levels/*.yaml`)
> компилировать незачем — они копируются в выход «как есть» через `<None ... CopyToOutputDirectory>`
> в [DesktopGL.csproj](RiotGalaxy.DesktopGL/RiotGalaxy.DesktopGL.csproj) и читаются из
> `AppContext.BaseDirectory/Content/...` (см. §16).

### Как грузят ресурсы в коде

```csharp
// Texture2D (спрайт/фон):
Texture = content.Load<Texture2D>("Images/ship");
_background = content.Load<Texture2D>("Backgrounds/background_blue");

// SoundEffect (звук):
_effects["fire1"] = content.Load<SoundEffect>("Sounds/fire1");

// SpriteFont (шрифт):
var font = content.Load<SpriteFont>("TestFont");
```

В проекте загрузка собрана в одном месте — `GameManager.LoadContent()` (фон + звуки
через `AudioManager`), а корабль грузит себя сам в `PlayerShip.LoadContent()`.

### 🔧 Как добавить НОВЫЙ ресурс (пошагово)

1. Положить файл в нужную папку, напр. `RiotGalaxy.Content/Images/boss.png`.
2. Добавить блок в `.mgcb` (можно руками по образцу выше; GUI-редактор MGCB не нужен,
   тем более он не работает в Linux-сборке без дисплея).
3. Пересобрать — `.xnb` соберётся и скопируется автоматически.
4. В коде: `var tex = Content.Load<Texture2D>("Images/boss");`

> Спрайты массово нарезались из старого атласа скриптом
> [tools/split_atlas.py](tools/split_atlas.py) — он по описанию `images.plist` режет
> `images.png` на отдельные PNG.

---

## 9. Ввод

[InputManager.cs](RiotGalaxy.Core/Managers/InputManager.cs) — адаптация системы ввода из
оригинала. Базовый принцип MonoGame: **состояние ввода опрашивается каждый кадр**
(не события):

```csharp
var kb = Keyboard.GetState();
if (kb.IsKeyDown(Keys.Space)) { … }
var mouse = Mouse.GetState();
var pad = GamePad.GetState(PlayerIndex.One);
```

Чтобы поймать *момент нажатия* (а не удержание), сравнивают с прошлым кадром — см.
`_previousKeyboardState` в `Game1`.

---

## 10. Аудио

[AudioManager.cs](RiotGalaxy.Core/Managers/AudioManager.cs) (синглтон) загружает звуки в
словарь и играет их по имени:

```csharp
AudioManager.Instance.PlayEffect("fire1");   // громкость 0.1, как в оригинале
```

Загрузка — в `GameManager.LoadContent`. `fire1` играет на выстреле игрока (оружие),
`explode1` — при гибели врага. Громкость `EffectsVolume` берётся из `settings.yaml`
(меню «Настройки», см. §14/§16). Музыки (BGM) в проекте нет.

---

## 11. Оружие и снаряды

Папка [Weapons/](RiotGalaxy.Core/Weapons/). Иерархия `Weapon` (паттерн «Стратегия»):
база + `WeaponCannon` / `WeaponMinigun` / `WeaponLaser` / `NoWeapon`. У игрока — `PlayerShip.Gun`.

- **`WeaponOptions`** — параметры: `burst` (выстрелов в очереди), `burstInterval`,
  `reloadSpeed` (между очередями), `damage`, `shellSpeed`. Грузятся по уровням из
  `weapons.yaml` (`WeaponConfig.Load`, есть встроенные дефолты-фолбэк).
- **`Weapon.Update/Fire`** сами ведут очередь и перезарядку (в CocosSharp был `Schedule`).
  `Aim(угол)` задаёт направление/точку появления снаряда (с учётом инверсии оси Y).
- **Уровни оружия**: `Weapon.Level`/`Upgrade()`; уровни запоминаются по типу в
  `PlayerShip._weaponLevels`, апгрейд — бонусом BulletUp или кнопкой.

Снаряды — [Shell.cs](RiotGalaxy.Core/GameObjects/Shell.cs) (база) и `Bullet` (пушка),
`Slug` (пулемёт), `Laser` (пробивает насквозь, `IsPiercing`). Летят прямо, гибнут за
экраном. `PlayerSide` = чей снаряд (ставится из `owner is PlayerShip`).

Стрельба игрока: удержание **Space** (темп держит само оружие); смена оружия — клавиши
**1/2/3** или тестовые кнопки (§17).

## 12. Враги

[Enemy.cs](RiotGalaxy.Core/GameObjects/Enemy.cs) + типы `EnemySmallBlue/Green/Red/Scout` + `EnemyBoss`
(`EnemyType { RND, SM_SCOUT, BLUE, GREEN, RED, BOSS }`). Параметры (hp/урон/скорость/интервал
стрельбы) берутся из `enemies.yaml` через `Enemy.ApplyStats(type)` (с рандомом по диапазонам), см. §16.

- **Движение** — [EnemyBounceMovement](RiotGalaxy.Core/Components/EnemyBounceMovement.cs):
  отскок от боковых границ (поле −10%) + телепорт снизу-вверх. `SetDirection(угол)`.
- **Стрельба** (базовый AI): по таймеру `ShootInterval` (~3с). Паттерны: синий/зелёный —
  вниз, красный — прицельно в игрока, скаут — не стреляет.
- Гибель: `TakeDamage`→`Die` (звук `explode1`), уменьшает `EnemiesRemaining`, роняет бонус.

**Мир и формации** ([World.cs](RiotGalaxy.Core/GameObjects/World.cs), [Hive.cs](RiotGalaxy.Core/GameObjects/Hive.cs)):
`World` — координатная сетка ячеек (16×10), центрирована на экране. `Hive` — формация-улей
(8×2) поверх ячеек: враги занимают ячейки (`TryTakeCell`) и синхронно барражируют
(весь улей качается, `Offset`). Враг входит в формацию через `Enemy.JoinFormation` —
движение сменяется на [FormationMovement](RiotGalaxy.Core/Components/FormationMovement.cs)
(летит к своей ячейке, затем держит строй). Формация задаётся в YAML-уровне флагом `formation: true`.

**Маршруты** ([Route.cs](RiotGalaxy.Core/GameObjects/Route.cs) + [RouteMovement](RiotGalaxy.Core/Components/RouteMovement.cs)):
враг летит по точкам из `Content/Routes/<name>.yaml` (координаты ячеек World). После последней
точки переключается на стратегию `after`: `bounce` (вниз с отскоком, по умолчанию),
`scatter` (случайный разлёт) или `formation` (занять ячейку улья и встать в строй).
В уровне: `{ enemy: blue, route: zmeyka1-left, after: formation }`.

**Босс** ([EnemyBoss.cs](RiotGalaxy.Core/GameObjects/EnemyBoss.cs)) — живучий крупный враг с
прицельной стрельбой (отдельного спрайта нет — увеличенный `enemyRed`).

## 13. Бонусы и столкновения (бой)

**Столкновения** (этап столкновений) делаются без физдвижка: `GameManager.ProcessGameObjects`
двойным циклом находит пересечения (AABB через `GameObject.GetBounds/Intersects`) и зовёт
`ProcessCollision(a,b)` — диспетчер пар: снаряд игрока↔враг, враг↔игрок (таран),
вражеский снаряд↔игрок, бонус↔игрок. Лазер не исчезает при попадании.

**Бонусы** — [Bonus.cs](RiotGalaxy.Core/GameObjects/Bonus.cs): `BonusHpUp` (хил),
`BonusBulletUp` (апгрейд оружия), `BonusNukeBomb` (убить всех), `BonusStar` (очки,
притягивается к игроку в радиусе магнита). Падают вниз; при подборе вызывается `Apply(player)`.
Выпадают при гибели врага (`SpawnBonusOnEnemyDeath`: всегда звезда + 30% шанс усиления).

## 14. Меню и экраны (ScreenSystem)

Папка [Screens/](RiotGalaxy.Core/Screens/). Лёгкая система экранов меню параллельно
игровым состояниям:

- **`Screen`** (база) — сам читает клавиатуру/мышь (с защитой от ложного клика на 1-м кадре),
  имеет хелпер центрированного текста.
- **`ScreenSystem`** — хранит активный экран, проксирует Update/Draw.
- Экраны: **`SplashScreen`** (текстовый логотип + таймер → меню), **`MainMenuScreen`**
  (кнопки Начать/Настройки/Выход, мышь+клавиатура), **`SettingsScreen`** (громкость, сохр.
  в `settings.yaml`), **`NextLevelScreen`** (между уровнями: номер/описание/очки).

`GameManager` для состояний `Splash/MainMenu/Settings/NextLevel` делегирует Update/Draw в
`ScreenSystem`; экран создаётся в `ChangeGameState`. Игровые состояния (Playing/Paused/…)
рисует/обновляет сам `GameManager`.

## 15. Уровни и прогрессия

[Utils/Level.cs](RiotGalaxy.Core/Utils/Level.cs) грузит `Content/Levels/level{N}.yaml`
и разворачивает события в таймлайн спавна. `GameManager` спавнит врагов по `Level.Tick(dt)`,
ведёт `EnemiesRemaining`. Когда все враги уровня заспавнены и убиты — переход на следующий
(`NextLevel` экран) или, если уровней больше нет, — `Victory`. Игрок и счёт переносятся
между уровнями. Поражение (`GameOver`) и победа показывают очки и предлагают рестарт.

Формат уровня (YAML):

```yaml
description: "First battle"
spawnInterval: 1.0          # интервал между спавнами по умолчанию
events:
  - { enemy: blue, count: 3 }            # типы: blue/green/red/scout/boss
  - { interval: 3 }                      # сменить интервал
  - { enemy: red, count: 5 }
  - { wait: 2 }                          # пауза
  - { enemy: green, count: 8, formation: true }  # спавн в формацию (улей)
  - { enemy: blue, count: 3, route: zmeyka1-left, after: formation } # вход по маршруту; after: bounce/scatter/formation
  - { enemy: boss, count: 1 }            # босс
  - parallel:                            # синхронные волны: группы спавнятся одновременно,
      - - { enemy: blue, count: 3, route: zmeyka1-left }   # основной таймлайн ждёт их завершения
      - - { enemy: blue, count: 3, route: zmeyka1-right }  # (аналог trigger/sync_cmd из оригинала)
```

## 16. Конфиги (YAML)

Через библиотеку **YamlDotNet**. Хелпер [Utils/Yaml.cs](RiotGalaxy.Core/Utils/Yaml.cs)
(camelCase, тихий фолбэк). Файлы:

| Файл | Что | Загрузчик |
|---|---|---|
| `Content/Config/weapons.yaml` | параметры оружия по уровням | `Weapons.WeaponConfig.Load()` |
| `Content/Config/enemies.yaml` | параметры врагов (hp/урон/скорость/стрельба, рандом min/max) | `Utils.EnemyConfig.Load()` |
| `Content/Config/bonuses.yaml` | параметры бонусов (хил HP, очки за звезду) | `Utils.BonusConfig.Load()` |
| `Content/Config/options.yaml` | экран + игрок (HP, скорость, время неуязвимости…) | `Utils.GameOptions.Load()` |
| `settings.yaml` (рядом с .exe) | громкость (пользовательская) | `Utils.GameSettings` |

Все три читаются в `GameManager.LoadContent`. У каждого конфига есть дефолты в коде —
игра работает и без файлов. Уровни (`Level`) — тоже YAML (§15).

> Почему YAML, а не Content Pipeline: это **текстовые** конфиги, их не нужно компилировать
> в `.xnb`. Они копируются в выход через `<None CopyToOutputDirectory>` (см. §8) и читаются
> обычным `File.ReadAllText` из `AppContext.BaseDirectory`.

## 17. Тестовая панель и сообщения

**Сообщения** — [MessageLog.cs](RiotGalaxy.Core/Managers/MessageLog.cs): короткие всплывающие
подписи над панелью кнопок («+25 HP», «Оружие: лазер», «+10 очк.»), затухают. Вызываются из
точек событий (`Bonus.Apply`, `PlayerShip.ChangeWeapon/UpgradeWeapon`, команды кнопок).

**Тестовая панель (debug).** Внизу слева во время игры — ряд кнопок ([Interface/MyButton.cs](RiotGalaxy.Core/Interface/MyButton.cs),
регистрируются в `InputManager.GuiButtons`): смена оружия (Cannon/Minigun/Laser), апгрейд,
лечение, «убить всех», «следующий уровень». Каждая кнопка несёт `ICommand` (папка
[Commands/](RiotGalaxy.Core/Commands/)). Создаются при старте партии, чистятся при выходе в меню.

---

## 18. Как собрать и запустить

### 18.0. Что нужно один раз

- **.NET SDK** (проект на `net6.0`, но `RollForward=Major` позволяет собирать и на новых SDK — например, на установленном 9.x).
- **`dotnet-mgcb`** — компилятор ресурсов. Ставится локально из манифеста [.config/dotnet-tools.json](.config/dotnet-tools.json):

  ```bash
  cd MonoGame
  dotnet tool restore        # один раз после клона: поставит dotnet-mgcb
  ```

  Без него сборка контента (`.xnb`) падает с `dotnet mgcb … not found`.

### 18.1. Быстрый запуск (дев)

```bash
# из корня репозитория — собирает и запускает:
./run_game.sh

# или вручную из папки MonoGame:
dotnet build RiotGalaxy.sln
dotnet run --project RiotGalaxy.DesktopGL/RiotGalaxy.DesktopGL.csproj
```

Управление сейчас:

- **Меню**: мышь или стрелки + Enter; Space — начать; Esc — выход.
- **В игре**: ←/→ или A/D — движение, **Space** — огонь, **1/2/3** — смена оружия,
  **Esc/P** — пауза (в паузе **Q** — выход в меню).
- **Конец игры** (победа/поражение): Space — заново, Esc — в меню.

### 18.2. Dev (Debug) vs Release сборка

`dotnet` собирает в конфигурации **Debug** по умолчанию. Флаг `-c` (`--configuration`)
переключает:

```bash
# Debug — для разработки: без оптимизаций, с отладочной информацией (.pdb), быстрее компиляция
dotnet build -c Debug

# Release — для распространения: с оптимизациями, работает быстрее
dotnet build -c Release
dotnet run -c Release --project RiotGalaxy.DesktopGL/RiotGalaxy.DesktopGL.csproj
```

Куда кладётся результат:

| Конфигурация | Путь к бинарникам |
|---|---|
| Debug | `RiotGalaxy.DesktopGL/bin/Debug/net6.0/` |
| Release | `RiotGalaxy.DesktopGL/bin/Release/net6.0/` |

В обоих случаях рядом появляется папка `Content/` со скомпилированными `.xnb`
(копируется автоматически благодаря `MonoGameContentReference`).

### 18.3. Сборка дистрибутива под разные ОС (`dotnet publish`)

`build`/`run` годятся для разработки. Чтобы получить **готовый дистрибутив** для
конкретной операционной системы, используют `dotnet publish` с указанием **RID**
(Runtime Identifier) — кода целевой платформы.

Проект `RiotGalaxy.DesktopGL` (OpenGL) кроссплатформенный — один и тот же код собирается
под все десктопные ОС, меняется только RID:

| ОС | RID |
|---|---|
| Windows 64-bit | `win-x64` |
| Linux 64-bit | `linux-x64` |
| macOS (Intel) | `osx-x64` |
| macOS (Apple Silicon) | `osx-arm64` |

```bash
# Self-contained (включает .NET runtime — на машине пользователя SDK не нужен):
dotnet publish RiotGalaxy.DesktopGL/RiotGalaxy.DesktopGL.csproj \
    -c Release -r win-x64 --self-contained true

# Под Linux:
dotnet publish RiotGalaxy.DesktopGL/RiotGalaxy.DesktopGL.csproj \
    -c Release -r linux-x64 --self-contained true

# Под macOS (Apple Silicon):
dotnet publish RiotGalaxy.DesktopGL/RiotGalaxy.DesktopGL.csproj \
    -c Release -r osx-arm64 --self-contained true
```

Результат — в `bin/Release/net6.0/<RID>/publish/` (вместе с папкой `Content/`).

Полезные флаги:

- `--self-contained true` — упаковать .NET в дистрибутив (пользователю не нужен
  установленный runtime). `false` — дистрибутив меньше, но требует .NET на машине.
- `-p:PublishSingleFile=true` — собрать в один исполняемый файл.
- `-p:PublishTrimmed=true` — отрезать неиспользуемый код (меньше размер; с MonoGame
  тестируйте — тримминг иногда удаляет нужное через рефлексию).

> ⚠️ Кросс-публикация скачивает runtime-пакеты целевой платформы с nuget при первой
> сборке — нужен интернет. Сборка под `osx-*`/`win-*` с Linux работает (это просто
> упаковка), но саму игру для финальной проверки лучше запускать на целевой ОС.

### 18.4. Сборка в Docker (требование PRD)

Согласно [prd.md](../prd.md), сборка должна идти в контейнере, без установки пакетов в
хост. Идея: в образе есть .NET SDK, внутри выполняется `dotnet tool restore` (ставит
`dotnet-mgcb`) и затем `dotnet publish` под нужный RID. Dockerfile в проекте пока не
заведён — это часть будущего этапа CI/CD.

### 18.5. Мобильные платформы (Android / iOS) — пока не настроены

В решении сейчас **только** `Core`, `Content`, `DesktopGL`. Каталоги
`RiotGalaxy.Android/` и `RiotGalaxy.iOS/` существуют, но **пустые** — это заготовки под
будущие этапы.

Когда дойдём: мобильные сборки MonoGame требуют отдельных проектов-обёрток и .NET
workloads (`dotnet workload install android` / `ios`), плюс Android SDK / Xcode.
Платформонезависимая логика из `Core` переиспользуется как есть.

---

## 19. Частые грабли новичка (и как их обходим здесь)

| Симптом | Причина | Решение |
|---|---|---|
| `Could not find … .xnb` при запуске | имя в `Content.Load` ≠ путь в `.mgcb`, либо `RootDirectory` неверный | имя = путь без расширения; `RootDirectory="Content"` |
| Сборка контента падает: `dotnet mgcb … not found` | не установлен `dotnet-mgcb` | `dotnet tool restore` |
| Спрайт с прозрачностью рисуется с тёмной каймой | несоответствие premultiplied alpha | в `.mgcb` `PremultiplyAlpha=True` + стандартный `SpriteBatch.Begin()` |
| `InvalidOperationException` при `Draw` | рисование вне `Begin()/End()` | всё рисование — между `spriteBatch.Begin()` и `End()` |
| Движение «дёргается» при разном FPS | скорость без учёта времени | умножать на `gameTime.ElapsedGameTime.TotalSeconds` |
| Меняешь PNG, а в игре старый | не пересобрал контент | пересборка (`dotnet build`) перекомпилирует `.xnb` |

---

## 20. Куда смотреть дальше

- **Хочешь понять поток управления** → начни с [Program.cs](RiotGalaxy.DesktopGL/Program.cs)
  → [Game1.cs](RiotGalaxy.Core/Game1.cs) → [GameManager.cs](RiotGalaxy.Core/Managers/GameManager.cs).
- **Хочешь понять объект на экране** → [GameObject.cs](RiotGalaxy.Core/GameObjects/GameObject.cs)
  → [PlayerShip.cs](RiotGalaxy.Core/GameObjects/PlayerShip.cs).
- **План работ и что уже сделано** → [tasks.md](tasks.md).
- **Требования и архитектурные решения** → [prd.md](../prd.md).
- **Официальные туториалы MonoGame** → <https://docs.monogame.net/articles/tutorials.html>
