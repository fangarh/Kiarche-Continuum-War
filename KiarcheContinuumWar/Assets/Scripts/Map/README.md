# Карта и RTS Камера — Руководство

## Обзор

Реализована система карт с ландшафтом, препятствиями и RTS камерой.

**Компоненты:**
- **MapManager** — управление данными карты (размер, границы, спавны)
- **Obstacle** — препятствия с автоматической регистрацией в FlowField
- **RTSCamera** — камера с WSAD панорамированием и зумом (namespace: CameraSystem)
- **GenerateTestMap** — редактор для генерации тестовой карты

---

## Карта

### MapManager

**Расположение:** `Assets/Scripts/Map/MapManager.cs`

**Функционал:**
- Хранение размера карты (по умолчанию 100x100)
- Границы карты (ограничение для камеры и юнитов)
- Спавн-поинты игроков и врагов
- Работа с высотой ландшафта

**Использование:**
```csharp
// Получить экземпляр
MapManager map = MapManager.Instance;

// Проверка границ
if (map.IsWithinBounds(position)) { ... }

// Ограничить позицию границами
Vector3 clamped = map.ClampToBounds(position);

// Получить высоту ландшафта
float height = map.GetTerrainHeight(position);

// Получить позицию на ландшафте
Vector3 onTerrain = map.GetPositionOnTerrain(x, z);

// Случайная позиция
Vector3 random = map.GetRandomPosition();

// Спавн-поинты
Vector3 playerSpawn = map.GetPlayerSpawnPoint(0);
Vector3 enemySpawn = map.GetEnemySpawnPoint(0);
```

**Параметры в инспекторе:**
```
Map Settings:
  Map Size: (100, 100)        # Размер карты
  Map Origin: (-50, 0, -50)   # Левый нижний угол

Spawn Points:
  Player Spawn Points: [...]  # Спавны игрока
  Enemy Spawn Points: [...]   # Спавны врага

References:
  Terrain: [Terrain]          # Ссылка на Terrain
```

---

### Obstacle

**Расположение:** `Assets/Scripts/Map/Obstacle.cs`

**Функционал:**
- Автоматическая регистрация в FlowFieldManager при старте
- Визуализация радиуса препятствия
- Проверка попадания точки в препятствие

**Использование:**
```csharp
// Добавить на GameObject с Collider
Obstacle obstacle = gameObject.AddComponent<Obstacle>();

// Настроить радиус
obstacle.SetObstacleRadius(3f);

// Принудительная регистрация
obstacle.RegisterObstacle();

// Проверка точки
if (obstacle.IsPointInside(point)) { ... }
```

**Параметры в инспекторе:**
```
Obstacle Settings:
  Obstacle Radius: 2.0        # Радиус препятствия
  Register On Start: true     # Авто-регистрация

Debug:
  Draw Debug Gizmos: true     # Визуализация в редакторе
```

---

### GenerateTestMap (Editor)

**Расположение:** `Assets/Scripts/Editor/GenerateTestMap.cs`

**Использование:**
1. В Unity Editor: **Tools → KCW → Generate Test Map**
2. Сохранить сцену как `TestMap.unity`
3. Открыть сцену

**Что создаётся:**
- Terrain 100x100 с холмами (до 15 единиц высоты)
- 9 препятствий (цилиндры разных размеров)
- 4 спавн-поинта (2 игрока, 2 врага)
- MapManager с настроенными параметрами

---

## RTS Камера

### RTSCamera

**Расположение:** `Assets/Scripts/CameraSystem/RTSCamera.cs`

**Функционал:**
- WSAD панорамирование
- Зум колёсиком мыши
- Ограничение границами карты
- Плавное движение (lerp)

**Управление:**
| Клавиша | Действие |
|---------|----------|
| **W / ↑** | Движение вперёд |
| **S / ↓** | Движение назад |
| **A / ←** | Движение влево |
| **D / →** | Движение вправо |
| **Колёсико** | Зум (10-50 единиц) |

**Использование:**
```csharp
// Получить экземпляр
RTSCamera camera = RTSCamera.Instance;

// Установить цель
camera.SetTarget(targetPosition);

// Мгновенное перемещение
camera.Teleport(position);

// Переместить к спавну игрока
camera.MoveToPlayerSpawn(0);

// Текущая позиция
Vector3 pos = camera.CameraPosition;
```

**Параметры в инспекторе:**
```
Camera Settings:
  Move Speed: 20              # Скорость панорамирования
  Zoom Speed: 10              # Скорость зума
  Min Zoom: 10                # Минимальная высота
  Max Zoom: 50                # Максимальная высота
  Default Zoom: 30            # Начальная высота

Rotation Settings:
  Rotation Speed: 5           # Скорость вращения
  Min Rotation X: 20          # Мин. угол наклона
  Max Rotation X: 70          # Макс. угол наклона
  Default Rotation X: 45      # Начальный угол

Bounds:
  Limit To Map Bounds: true   # Ограничение картой
  Map Bounds: (100, 100)      # Размер карты
  Map Origin: (0, 0)          # Начало карты

Smooth Follow:
  Smooth Speed: 5             # Плавность движения
  Offset: (0, 30, -10)        # Смещение камеры
```

---

## Интеграция

### Настройка сцены

1. **Создать карту:**
   - Tools → KCW → Generate Test Map
   - Сохранить сцену

2. **Добавить камеру:**
   - На Main Camera добавить компонент `RTSCamera`
   - Настроить параметры (zoom, speed)

3. **Настроить FlowFieldManager:**
   - Убедиться, что Field Width/Height = 100
   - Cell Size = 1

4. **Добавить юнитов:**
   - Спавн через UnitPoolManager
   - Или использовать GameManager

### Взаимодействие компонентов

```
MapManager (границы 100x100)
    ↓
RTSCamera (ограничение границами)
    ↓
FlowFieldManager (поле 100x100)
    ↓
Obstacle (регистрация в FlowField)
    ↓
UnitPathfinder (движение по полю + обход)
```

---

## Отладка

### Gizmos

**MapManager:**
- Жёлтые линии — границы карты
- Зелёные сферы — спавны игрока
- Красные сферы — спавны врага

**Obstacle:**
- Красный проводфрейм — радиус препятствия
- Красные точки — точки регистрации в FlowField

**RTSCamera:**
- Голубой куб — границы карты
- Жёлтая сфера — текущая позиция
- Зелёная сфера — целевая позиция

### В инспекторе

- Включить `drawDebugGizmos` в Obstacle
- Включить `limitToMapBounds` в RTSCamera
- Проверить `Map Size` в MapManager

---

## Расширение

### Добавление своей карты

1. Создать Terrain вручную или через GenerateTestMap
2. Добавить MapManager
3. Расставить Obstacle (препятствия)
4. Настроить спавн-поинты
5. Сохранить как сцену

### Редактор карт (будущее)

**Архитектура заложена:**
- Obstacle регистрируется автоматически
- MapManager хранит данные
- GenerateTestMap — основа для редактора

**В будущем:**
- EditorWindow для расстановки объектов
- Сохранение в ScriptableObject
- Загрузка сценариев кампании

---

## Производительность

### FlowField + Obstacles

- 9 препятствий = ~36 точек регистрации
- Обновление FlowField: ~5-10ms
- Динамические препятствия: полная перерегистрация

### Камера

- WSAD: 60 FPS (просто перемещение)
- Зум: 60 FPS (без эффектов)
- Ограничения: Mathf.Clamp (быстро)

---

## Известные ограничения

1. **Динамические препятствия** — перерегистрация полная (не оптимизировано)
2. **Вращение камеры** — не реализовано (только наклон)
3. **Края экрана** — панорамирование только WSAD (edge scroll в будущем)

---

## Следующие улучшения

- [ ] Вращение камеры (ПКМ + drag)
- [ ] Панорамирование краями экрана
- [ ] Миникамера (minimap)
- [ ] Сохранение/загрузка карт (ScriptableObject)
- [ ] EditorWindow для карт
