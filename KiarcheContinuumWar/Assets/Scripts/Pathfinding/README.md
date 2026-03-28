# Flow Fields Pathfinding — Руководство по настройке

## Быстрый старт

### 1. Добавить FlowFieldManager на сцену

**Автоматически:**
- `FlowFieldManager` создаётся автоматически при первом запуске (singleton)
- Или добавь вручную: `GameObject → Create Empty` → переименуй в "FlowFieldManager" → добавь компонент `FlowFieldManager`

**Параметры:**
```
Field Width: 100        # Ширина поля (ячеек)
Field Height: 100       # Высота поля (ячеек)
Cell Size: 1.0          # Размер ячейки (единиц Unity)
Draw Debug Gizmos: false # Визуализация (включить для отладки)
```

### 2. Добавить UnitPathfinder на префаб юнита

1. Открой префаб юнита (`UnitPrefab`)
2. Добавь компонент `UnitPathfinder`
3. Настрой параметры:

```
Pathfinding Settings:
  Move Speed: 5.0
  Stopping Distance: 0.1
  Path Update Interval: 0.1

Separation (avoidance):
  Use Separation: true   # Избегание столкновений
  Separation Distance: 1.0
  Separation Strength: 2.0
```

### 3. Проверка работы

1. Запусти сцену
2. Выдели юнитов (ЛКМ или drag)
3. Кликни ПКМ по земле
4. Юниты должны двигаться к цели, огибая препятствия

## Отладка

### Визуализация поля

В `FlowFieldManager`:
- Включи `Draw Debug Gizmos`
- Запусти сцену
- В Scene view увидишь:
  - **Зелёные точки** — близкие к цели ячейки
  - **Красные точки** — далёкие ячейки
  - **Белые стрелки** — направление потока
  - **Красные точки** — препятствия

### Визулизация юнитов

В `UnitPathfinder` (при выделении):
- **Жёлтая линия** — путь к цели
- **Жёлтая сфера** — радиус separation

## Архитектура

### FlowField
- Хранит сетку ячеек (направление + стоимость)
- Конвертирует мировые координаты ↔ координаты сетки
- Методы: `GetCell()`, `SetCell()`, `GetDirection()`, `WorldToGrid()`, `GridToWorld()`

### FlowFieldManager
- Singleton, доступен из любого места: `FlowFieldManager.Instance`
- Генерирует поле через BFS от цели
- Методы: `GenerateFlowField()`, `GetDirection()`, `SetObstacle()`

### UnitPathfinder
- Компонент на юните
- Делегирует движение FlowField
- Добавляет separation avoidance
- Методы: `SetTargetPosition()`, `SetTarget()`, `Stop()`

## Производительность

### Оптимизация BFS
- Поле 100x100 = 10,000 ячеек
- Генерация: ~5-10ms на CPU
- Обновление: при каждом приказе (ПКМ)

### Рекомендации
- Уменьши `fieldWidth/Height` для маленьких карт
- Увеличь `cellSize` для меньшей детализации (быстрее)
- Кэшируй поле для статичных целей

## Расширение

### Добавить препятствия

```csharp
// В любом скрипте:
FlowFieldManager.Instance.SetObstacle(position);
```

### Обновить поле от нескольких целей

```csharp
var targets = new List<Vector3> { target1, target2, target3 };
FlowFieldManager.Instance.GenerateFlowFieldFromMultipleTargets(targets);
```

### Проверка достижимости

```csharp
bool reachable = FlowFieldManager.Instance.IsReachable(targetPosition);
```

## Известные ограничения

1. **Динамические препятствия** — не обновляются автоматически
   - Решение: вызывать `SetObstacle()` при появлении зданий/юнитов

2. **Разный размер юнитов** — все юниты используют одно поле
   - Решение: создать несколько слоёв FlowField для разных размеров

3. **Диагональное движение** — 8 направлений
   - Решение: увеличить точность поля (меньше cellSize)

## Следующие улучшения

- [ ] Динамическое обновление поля при движении
- [ ] Учёт размера юнита (large units)
- [ ] Weighted terrain (разная стоимость поверхности)
- [ ] Multi-layer flow fields (воздух, земля, вода)
