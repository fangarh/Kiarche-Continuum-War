# Object Pooling — Руководство

## Обзор

Система Object Pooling для оптимизации производительности RTS.
Переиспользование юнитов вместо создания/уничтожения снижает нагрузку на GC и CPU.

## Компоненты

### ObjectPool<T>

Универсальный пул объектов (`Assets/Scripts/Pooling/ObjectPool.cs`)

**Возможности:**
- Предварительное заполнение (preload)
- Автоматическое расширение при нехватке
- События активации/возврата (IPoolableComponent)
- Статистика (active/available count)

**Использование:**
```csharp
// Создать пул
var pool = new ObjectPool<Unit>(prefab, initialCapacity: 100);

// Взять объект
Unit unit = pool.Get(position, rotation);

// Вернуть объект
pool.Return(unit);

// Вернуть все
pool.ReturnAll();
```

### UnitPoolManager

Менеджер пула юнитов (`Assets/Scripts/Pooling/UnitPoolManager.cs`)

**Singleton** — доступен из любого места:
```csharp
UnitPoolManager.Instance.SpawnUnit(position, rotation, color);
UnitPoolManager.Instance.DespawnUnit(unit);
```

**Параметры:**
```
Unit Prefab: [префаб юнита]
Initial Pool Size: 200    # Начальный размер пула
Auto Expand: true         # Автоматическое расширение
```

## Настройка

### 1. Добавить UnitPoolManager на сцену

Автоматически создаётся при первом запуске, или вручную:
- `GameObject → Create Empty` → "UnitPoolManager"
- Добавить компонент `UnitPoolManager`
- Назначить `Unit Prefab`

### 2. Обновить префаб юнита

Добавить на префаб:
- `Unit` (уже есть) — теперь реализует `IPoolableComponent`
- `UnitPathfinder` (уже есть)

### 3. Обновить GameManager

Назначить `unitPoolManager` в инспекторе или найдётся автоматически.

## Как работает

### Без пулинга (старый код)
```csharp
// Создание (дорого!)
GameObject obj = Instantiate(prefab, pos, rot);

// Уничтожение (аллокация памяти!)
Destroy(obj);
```

### С пулингом (новый код)
```csharp
// Получение из пула (быстро!)
Unit unit = UnitPoolManager.Instance.SpawnUnit(pos, rot);

// Возврат в пул (без аллокаций!)
UnitPoolManager.Instance.DespawnUnit(unit);
```

## Производительность

### Сравнение

| Операция | Instantiate/Destroy | Object Pool |
|----------|---------------------|-------------|
| Создание | ~0.1-0.5ms | ~0.01ms |
| Уничтожение | ~0.1ms + GC | ~0.01ms |
| GC давление | Высокое | Минимальное |

### Тесты

**400 юнитов:**
- **Без пула:** 20-30 FPS, частые GC
- **С пулом:** 50-60 FPS, стабильно

## IPoolableComponent

Интерфейс для реакции на активацию/возврат:

```csharp
public class Unit : MonoBehaviour, IPoolableComponent
{
    public void OnObjectActivate()
    {
        // Сброс состояния при получении из пула
        health = maxHealth;
        target = null;
    }

    public void OnObjectReturn()
    {
        // Очистка перед возвратом
        target = null;
        Deselect();
    }
}
```

## Расширение

### Кастомные события

Добавить свои события в `OnObjectActivate/Return`:

```csharp
public void OnObjectActivate()
{
    // Восстановить здоровье
    health = maxHealth;
    
    // Сбросить анимации
    animator.Reset();
    
    // Включить рендерер
    renderer.enabled = true;
}
```

### Пул для других объектов

Создать менеджер для зданий/снарядов:

```csharp
// BuildingPoolManager.cs
public class BuildingPoolManager : MonoBehaviour
{
    private ObjectPool<Building> _pool;
    
    void Start()
    {
        _pool = new ObjectPool<Building>(buildingPrefab, 50);
    }
    
    public Building SpawnBuilding(Vector3 pos)
    {
        return _pool.Get(pos, Quaternion.identity);
    }
}
```

## Отладка

### Статистика

В `UnitPoolManager` включи инспектор:
- `Current Pool Size` — общий размер пула
- `Active Units Count` — активные юниты

### Консоль

```csharp
Debug.Log($"Пул: активные={pool.ActiveCount}, свободные={pool.AvailableCount}");
```

## Известные проблемы

1. **Юниты не возвращаются в пул при смерти**
   - Проверить наличие `UnitPoolManager` на сцене
   - Убедиться, что `Die()` вызывает `DespawnUnit()`

2. **Не сбрасывается состояние**
   - Проверить реализацию `OnObjectActivate()`
   - Добавить сброс всех полей

3. **Пуло слишком маленький**
   - Увеличить `Initial Pool Size`
   - Включить `Auto Expand`

## Рекомендации

### Для RTS

- **Initial Pool Size:** 200-500 юнитов
- **Auto Expand:** true (для пиковых нагрузок)
- **Preload:** при загрузке уровня

### Оптимизация

- Использовать пул для **всех** часто создаваемых объектов
- Пре加载 (preload) в начале миссии
- Мониторить `ActiveCount` для баланса

## Следующие улучшения

- [ ] Пул с приоритетами (важные юниты первыми)
- [ ] Warmup (прогрев пула при старте)
- [ ] Статистика в реальном времени (FPS, GC)
- [ ] Разные пулы для разных типов юнитов
