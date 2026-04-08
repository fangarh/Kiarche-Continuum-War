# MVP Прототип — Руководство

## Обзор

MVP прототип Kiarche Continuum War — минимальная версия для проверки базовых механик RTS.

### Реализованные механики

1. **Выделение юнитов**
   - Клик ЛКМ — выделение одного юнита
   - Drag (перетаскивание) — выделение группы юнитов в прямоугольной области
   - Правый клик — снятие выделения

2. **Перемещение**
   - Правый клик по земле — перемещение выделенных юнитов
   - Юниты распределяются в формации (круг)

3. **Бой**
   - Правый клик по врагу — атака
   - Юниты автоматически атакуют при приближении
   - Есть здоровье, урон, перезарядка

4. **HUD**
   - Отображение ресурсов (Материалы, Энергия, Еда, Знания)
   - Счётчик выделенных юнитов

5. **Тестовые команды**
   - **T** — создать 10 тестовых юнитов
   - **P** — тест производительности (создать до 400 юнитов)
   - **R** — показать текущие ресурсы в консоли

---

## Структура скриптов

```
Assets/Scripts/
├── CameraSystem/
│   └── RTSCamera.cs              # RTS камера (WSAD, зум)
├── Core/
│   ├── ResourceManager.cs        # Управление ресурсами
│   └── KiarcheContinuumWar.Core.Runtime.asmdef
├── Editor/
│   ├── CreateUnitPrefabEditor.cs # Tools → KCW → Create Unit Prefab
│   ├── GenerateMVPScene.cs       # Tools → KCW → Generate MVP Scene
│   └── GenerateTestMap.cs        # Tools → KCW → Generate Test Map
├── InputSystem/
│   └── RTSInput.cs               # Обработка ввода (ЛКМ, ПКМ)
├── Managers/
│   ├── GameManager.cs            # Главный менеджер, тестовая сцена
│   ├── UnitPoolManager.cs        # Менеджер пула юнитов
│   └── FlowFieldSetup.cs         # Настройка FlowFieldManager
├── Map/
│   ├── MapManager.cs             # Данные карты (размер, границы, спавны)
│   └── Obstacle.cs               # Препятствие (регистрация в FlowField)
├── Pathfinding/
│   ├── Runtime/
│   │   ├── FlowField.cs          # Поле потока (ячейки, направления)
│   │   └── KiarcheContinuumWar.Pathfinding.Runtime.asmdef
│   ├── FlowFieldManager.cs       # Генерация поля от цели (BFS)
│   └── UnitPathfinder.cs         # Движение юнита по полю
├── Pooling/
│   ├── ObjectPool.cs             # Заглушка-перенаправление на runtime-assembly
│   ├── Runtime/
│   │   ├── ObjectPool.cs         # Универсальный пул объектов
│   │   └── KiarcheContinuumWar.Pooling.Runtime.asmdef
│   └── UnitPoolManager.cs        # Менеджер пула юнитов
├── UI/
│   └── GameUI.cs                 # HUD: ресурсы, выделение
└── Units/
    ├── Unit.cs                   # Базовый класс юнита
    └── UnitController.cs         # Контроллер группы юнитов
```

---

## Как запустить

1. Открой Unity 6 (6000.4.0f1)
2. Открой сцену `Assets/Scenes/MVP_Prototype.unity`
3. Нажми Play

### Настройка сцены

На сцене уже настроены:
- **Main Camera** — изометрический вид (45°)
- **Directional Light** — освещение
- **Ground** — плоскость 50x50
- **GameManager** — главный менеджер
- **ResourceManager** — ресурсы (100/100/100/50)
- **UnitController** — контроллер юнитов
- **RTSInput** — ввод
- **Canvas** — UI с ресурсами и выделением

---

## Префабы

### UnitPrefab

Базовый юнит:
- CapsuleCollider (радиус 0.5, высота 2)
- Rigidbody (ограниченное движение)
- MeshRenderer (цилиндр)
- Скрипт `Unit`

**Параметры юнита:**
- Speed: 5
- Health: 100
- Damage: 10
- Attack Range: 2
- Attack Cooldown: 1 сек

---

## Архитектура

### Unit

```csharp
- SetTargetPosition(Vector3)  // Движение к точке
- SetTarget(Unit)             // Атака юнита
- Select() / Deselect()       // Выделение
- TakeDamage(float)           // Получить урон
```

### UnitController

```csharp
- SelectUnit(Unit)            // Выделить одного
- SelectUnitsInRect(Rect)     // Выделить в области
- IssueMoveOrder(Vector3)     // Приказ двигаться
- IssueAttackOrder(Unit)      // Приказ атаковать
- FindNearestEnemy(Vector3)   // Найти ближайшего врага
```

### ResourceManager

```csharp
- AddResource(type, amount)   // Добавить ресурс
- SpendResource(type, amount) // Потратить ресурс
- CanAfford(type, amount)     // Проверить доступность
- SetResourceRate(type, fps)  // Установить доход/сек
```

---

## Расширение

### Добавить нового юнита

1. Создай префаб из `UnitPrefab`
2. Измени параметры в инспекторе
3. Назначь в `GameManager.unitPrefab`

### Добавить ресурс

1. Добавь тип в `ResourceType` enum
2. Добавь поле в `ResourceManager`
3. Обнови `GameUI` для отображения

### Изменить управление

Правь `RTSInput.cs`:
- `HandleMouseInput()` — обработка мыши
- `HandleClick()` — клик ЛКМ
- `HandleRightClick()` — приказ ПКМ

---

## Известные проблемы

1. ~~Юниты проходят сквозь друг друга~~ — решено: добавлен separation avoidance
2. ~~Нет пути к цели~~ — решено: Flow Fields pathfinding
3. ~~Object Pooling~~ — решено: система пулов юнитов
4. ~~Одинаковые юниты~~ — решено: разные цвета для команд
5. ~~RTS камера~~ — решено: WSAD + зум
6. **Нет карты с препятствиями** — тестовая карта генерируется через Tools → KCW

---

## Flow Fields Pathfinding

**Реализовано:**
- `FlowField.cs` — сетка ячеек с направлениями и стоимостью
- `FlowFieldManager.cs` — генерация поля через BFS от цели
- `UnitPathfinder.cs` — движение юнита по полю + separation avoidance

**Как работает:**
1. При получении приказа (ПКМ) генерируется поле потока от целевой точки
2. Каждая ячейка хранит направление к цели и стоимость (расстояние)
3. Юниты двигаются по вектору поля + избегают столкновений (separation)

**Настройка:**
- Добавь `FlowFieldManager` на сцену (или создаётся автоматически)
- Добавь `UnitPathfinder` на префаб юнита вместе с `Unit`
- Настрой `fieldWidth`, `fieldHeight`, `cellSize` в FlowFieldManager

**Отладка:**
- Включи `drawDebugGizmos` в FlowFieldManager для визуализации
- Жёлтые сферы — радиус separation юнита
- Цветные точки — стоимость ячеек (зелёный = близко, красный = далеко)

---

## Следующие шаги

Согласно roadmap:

- [x] **task-008** — Flow Fields pathfinding ✅
- [x] **task-009** — Object Pooling ✅
- [x] **task-015** — HUD прототип (ресурсы, панель юнитов) ✅
- [x] **task-019** — Базовая RTS камера (орбита, зум) ✅
- [ ] **task-010** — Прототип боя (столкновение групп)
- [ ] **task-020** — Тест производительности (400 юнитов)

---

## Automated Tests

В проект добавлена базовая инфраструктура Edit Mode unit-тестов на Unity Test Framework.

Что уже есть:
- `Packages/manifest.json` теперь явно фиксирует `com.unity.test-framework`.
- `FlowField` вынесен в отдельную runtime-assembly `Assets/Scripts/Pathfinding/Runtime/`.
- `ResourceManager` вынесен в runtime-assembly `Assets/Scripts/Core/`.
- `ObjectPool` вынесен в runtime-assembly `Assets/Scripts/Pooling/Runtime/`.
- Edit Mode тесты лежат в `Assets/Tests/EditMode/Editor/FlowFieldTests.cs`.
- Edit Mode тесты на пул лежат в `Assets/Tests/EditMode/Editor/ObjectPoolEditModeTests.cs`.
- Play Mode тесты лежат в `Assets/Tests/PlayMode/FlowFieldPlayModeTests.cs`.
- Play Mode тесты на ресурсы лежат в `Assets/Tests/PlayMode/ResourceManagerPlayModeTests.cs`.
- Для batch-mode запуска используется `scripts/run-unity-editmode-tests.ps1` с параметром `-TestPlatform`.
- Для CI добавлен workflow `.github/workflows/unity-tests.yml`.

Как запускать:
1. В Editor: `Window -> General -> Test Runner -> EditMode`.
2. Из PowerShell из корня репозитория:

```powershell
.\scripts\run-unity-editmode-tests.ps1
```

Play Mode:

```powershell
.\scripts\run-unity-editmode-tests.ps1 -TestPlatform PlayMode
```

При необходимости можно передать путь к Unity Editor:

```powershell
.\scripts\run-unity-editmode-tests.ps1 -UnityPath "C:\Program Files\Unity\Hub\Editor\6000.4.0f1\Editor\Unity.exe"
```

Результаты batch-mode тестов пишутся в:
- `test-results/unity/editmode-results.xml`
- `test-results/unity/editmode.log`
- `test-results/unity/playmode-results.xml`
- `test-results/unity/playmode.log`

Требование:
- Для batch-mode запуска Unity Editor должен быть активирован на машине, иначе тесты завершатся ошибкой лицензии до компиляции/прогона.

CI:
- GitHub Actions workflow запускает `editmode` и `playmode` через `game-ci/unity-test-runner@v4`.
- Для CI нужно настроить секреты `UNITY_LICENSE`, `UNITY_EMAIL`, `UNITY_PASSWORD`, а при серийной лицензии ещё и `UNITY_SERIAL`.
