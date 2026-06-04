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
├── Game1.cs                 # главный класс игры (наследник Game) — дирижёр цикла
├── Managers/                # «менеджеры» — глобальные системы (синглтоны)
│   ├── GameManager.cs       #   центральный диспетчер: состояния, список объектов, отрисовка
│   ├── InputManager.cs      #   обработка ввода (клавиатура/мышь/тач)
│   └── AudioManager.cs      #   загрузка и проигрывание звуков
├── GameObjects/             # игровые объекты (всё, что живёт на экране)
│   ├── GameObject.cs        #   базовый класс (позиция, размер, текстура, Update/Draw)
│   ├── PlayerShip.cs        #   корабль игрока
│   ├── Enemy.cs, Bullet.cs, Bonus.cs
├── Components/              # компоненты поведения (паттерн «Стратегия»)
│   ├── MovementComponent.cs #   как объект двигается
│   ├── ShootingComponent.cs #   как стреляет
│   └── CollisionComponent.cs#   как обрабатывает столкновения
├── Commands/                # паттерн «Команда» (события/действия)
├── AI/                      # ИИ врагов (паттерн «Состояние»)
├── Interface/               # UI-элементы (напр. MyButton)
└── Utils/                   # вспомогательные классы
```

Эта раскладка повторяет архитектуру оригинала на CocosSharp (см. [prd.md](../prd.md)):
сохранены паттерны **Strategy** (компоненты), **Command** (команды), **State** (ИИ и
состояния игры).

> Часть папок (`AI/`, `Utils/`) пока пустые — это заготовки под будущие этапы миграции.
> `GameObjects/Enemy.cs`, `Bonus.cs` тоже на ранней стадии. Актуальный статус — в [tasks.md](tasks.md).

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

Плюс `Game1.Update` обрабатывает глобальные клавиши (Esc — выход/в меню, Space — старт,
P — пауза) с «детектом нажатия» через `_previousKeyboardState` (чтобы реагировать на
*момент* нажатия, а не на удержание).

> 💡 Почему ты сразу видишь геймплей, а не меню: в `Game1` есть флажок
> `_autoTransition = true`, который на первом же кадре переключает состояние
> `MainMenu → Playing` (для удобства тестирования). Это временно.

---

## 6. `GameManager` — сердце игры

[GameManager.cs](RiotGalaxy.Core/Managers/GameManager.cs) — центральный диспетчер
(синглтон `GameManager.Instance`). Отвечает за:

- **Состояния игры** (паттерн State) — `enum GameState { MainMenu, Playing, Paused, GameOver, Victory }`.
  И `Update`, и `Draw` внутри делают `switch` по текущему состоянию.
- **Список игровых объектов** — `List<GameObject> GameObjects`. В режиме `Playing`
  каждый кадр он перебирает список: вызывает `Update` у каждого, проверяет столкновения,
  удаляет «мёртвые» объекты.
- **Отрисовку** — держит `SpriteBatch`, рисует фон, все объекты и HUD.
- **Экран** — `ScreenWidth=1280`, `ScreenHeight=768`.

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
├── RiotGalaxy.Content.mgcb     # рецепт сборки
├── Images/                     # спрайты (26 шт., нарезаны из старого атласа images.png)
│   ├── ship.png, enemyBlue.png, bullet.png, shield.png …
├── Backgrounds/                # фоны: background_blue (1280×768), background, SCConvoy_0
├── Sounds/                     # звуки: fire1.wav, explode1.wav
└── TestFont.spritefont         # описание шрифта (DejaVu Sans Mono)
```

> Важная тонкость этого проекта: пути в `.mgcb` отсчитываются от папки самого `.mgcb`,
> поэтому ресурсы лежат **прямо** в `RiotGalaxy.Content/` (а имя ассета для загрузки —
> это путь без расширения, напр. `"Images/ship"`).

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

Загрузка происходит в `GameManager.LoadContent`. Сейчас звуки только *загружены*;
реальные вызовы `PlayEffect` (на выстрел/взрыв) добавятся на этапах оружия и боёв.

---

## 11. Как собрать и запустить

### 11.0. Что нужно один раз

- **.NET SDK** (проект на `net6.0`, но `RollForward=Major` позволяет собирать и на новых SDK — например, на установленном 9.x).
- **`dotnet-mgcb`** — компилятор ресурсов. Ставится локально из манифеста [.config/dotnet-tools.json](.config/dotnet-tools.json):

  ```bash
  cd MonoGame
  dotnet tool restore        # один раз после клона: поставит dotnet-mgcb
  ```

  Без него сборка контента (`.xnb`) падает с `dotnet mgcb … not found`.

### 11.1. Быстрый запуск (дев)

```bash
# из корня репозитория — собирает и запускает:
./run_game.sh

# или вручную из папки MonoGame:
dotnet build RiotGalaxy.sln
dotnet run --project RiotGalaxy.DesktopGL/RiotGalaxy.DesktopGL.csproj
```

Управление сейчас: **Space** — старт, **P** — пауза, **Esc** — в меню/выход
(стрелки — движение корабля).

### 11.2. Dev (Debug) vs Release сборка

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

### 11.3. Сборка дистрибутива под разные ОС (`dotnet publish`)

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

### 11.4. Сборка в Docker (требование PRD)

Согласно [prd.md](../prd.md), сборка должна идти в контейнере, без установки пакетов в
хост. Идея: в образе есть .NET SDK, внутри выполняется `dotnet tool restore` (ставит
`dotnet-mgcb`) и затем `dotnet publish` под нужный RID. Dockerfile в проекте пока не
заведён — это часть будущего этапа CI/CD.

### 11.5. Мобильные платформы (Android / iOS) — пока не настроены

В решении сейчас **только** `Core`, `Content`, `DesktopGL`. Каталоги
`RiotGalaxy.Android/` и `RiotGalaxy.iOS/` существуют, но **пустые** — это заготовки под
будущие этапы.

Когда дойдём: мобильные сборки MonoGame требуют отдельных проектов-обёрток и .NET
workloads (`dotnet workload install android` / `ios`), плюс Android SDK / Xcode.
Платформонезависимая логика из `Core` переиспользуется как есть.

---

## 12. Частые грабли новичка (и как их обходим здесь)

| Симптом | Причина | Решение |
|---|---|---|
| `Could not find … .xnb` при запуске | имя в `Content.Load` ≠ путь в `.mgcb`, либо `RootDirectory` неверный | имя = путь без расширения; `RootDirectory="Content"` |
| Сборка контента падает: `dotnet mgcb … not found` | не установлен `dotnet-mgcb` | `dotnet tool restore` |
| Спрайт с прозрачностью рисуется с тёмной каймой | несоответствие premultiplied alpha | в `.mgcb` `PremultiplyAlpha=True` + стандартный `SpriteBatch.Begin()` |
| `InvalidOperationException` при `Draw` | рисование вне `Begin()/End()` | всё рисование — между `spriteBatch.Begin()` и `End()` |
| Движение «дёргается» при разном FPS | скорость без учёта времени | умножать на `gameTime.ElapsedGameTime.TotalSeconds` |
| Меняешь PNG, а в игре старый | не пересобрал контент | пересборка (`dotnet build`) перекомпилирует `.xnb` |

---

## 13. Куда смотреть дальше

- **Хочешь понять поток управления** → начни с [Program.cs](RiotGalaxy.DesktopGL/Program.cs)
  → [Game1.cs](RiotGalaxy.Core/Game1.cs) → [GameManager.cs](RiotGalaxy.Core/Managers/GameManager.cs).
- **Хочешь понять объект на экране** → [GameObject.cs](RiotGalaxy.Core/GameObjects/GameObject.cs)
  → [PlayerShip.cs](RiotGalaxy.Core/GameObjects/PlayerShip.cs).
- **План работ и что уже сделано** → [tasks.md](tasks.md).
- **Требования и архитектурные решения** → [prd.md](../prd.md).
- **Официальные туториалы MonoGame** → <https://docs.monogame.net/articles/tutorials.html>
