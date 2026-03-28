# RTS MVP — Прогресс разработки

**Статус:** ✅ БАЗОВОЕ УПРАВЛЕНИЕ РАБОТАЕТ  
**Дата:** 2026-03-28  
**Unity версия:** 6000.4.0f1 (Unity 6)  
**Input System:** Input Manager (Old)

---

## ✅ Реализованные механики

### 1. Выделение юнитов

#### Клик ЛКМ
- Выделение одного юнита
- Предыдущее выделение автоматически сбрасывается
- Визуализация: жёлтый проводфрейм-круг (Gizmos)

#### Drag ЛКМ (рамка)
- Выделение нескольких юнитов в прямоугольной области
- Минимальное расстояние для начала drag: 10 пикселей
- Все предыдущие выделения сбрасываются

#### Снятие выделения
- Клик по земле снимает выделение со всех юнитов
- Юниты останавливаются при снятии выделения

### 2. Приказы (ПКМ)

#### Перемещение
- Клик ПКМ по земле — приказ двигаться к точке
- Выделенные юниты распределяются в формации (круг)
- Юниты, не входящие в выделение, останавливаются

#### Атака
- Клик ПКМ по вражескому юниту — приказ атаковать
- Все выделенные юниты атакуют цель
- Автоматический поиск ближайшего врага при атаке позиции

### 3. Визуализация

#### Выделение
- `OnDrawGizmos()` рисует жёлтый круг вокруг выделенных юнитов
- Радиус: 0.6 единиц
- Цвет: `Color.yellow`

#### Приказы
- `OnDrawGizmosSelected()` (в инспекторе) — зелёный круг
- Линии к цели (в редакторе)

---

## 🏗️ Архитектура системы

### Компоненты

| Скрипт | Ответственность |
|--------|-----------------|
| `RTSInput.cs` | Обработка ввода (ЛКМ, ПКМ), выделение рамкой |
| `UnitController.cs` | Управление группой юнитов, приказы |
| `Unit.cs` | Состояние юнита (здоровье, выделение, делегирование) |
| `UnitPathfinder.cs` | Pathfinding, движение к цели |

### Поток данных

```
Input (RTSInput)
    ↓
HandleClick / HandleRightClick
    ↓
UnitController (SelectUnit / IssueMoveOrder)
    ↓
Unit (Select / Deselect / SetTargetPosition)
    ↓
UnitPathfinder (SetTargetPosition / Move)
```

### Ключевые исправления (2026-03-28)

1. **Сброс `_currentMousePosition` при нажатии**  
   Проблема: мышь дрожала, drag начинался случайно  
   Решение: сбрасывать `_currentMousePosition = Input.mousePosition` в `GetMouseButtonDown(0)`

2. **Остановка юнитов при снятии выделения**  
   Проблема: старые юниты продолжали двигаться к старой цели  
   Решение: `Unit.Deselect()` вызывает `pathfinder.Stop()`

3. **Визуализация выделения в рантайме**  
   Проблема: `OnDrawGizmosSelected()` работает только в инспекторе  
   Решение: добавить `OnDrawGizmos()` с проверкой `IsSelected`

4. **Проверка границ экрана для SelectUnitsInRect**  
   Проблема: юниты за пределами экрана неправильно проверялись  
   Решение: проверять `screenPoint.x/y` в пределах `Screen.width/height`

---

## 📋 Сценарии использования

### Сценарий 1: Выделение одного юнита
```
1. Клик ЛКМ по юниту
2. Юнит выделяется (жёлтый круг)
3. Клик ЛКМ по другому юниту
4. Первый снимается с выделения, второй выделяется
```

### Сценарий 2: Выделение рамкой
```
1. Зажать ЛКМ, тянуть рамку
2. Отпустить ЛКМ
3. Все юниты в рамке выделяются
4. Старое выделение сбрасывается
```

### Сценарий 3: Приказ о перемещении
```
1. Выделить юнитов (клик или рамка)
2. Клик ПКМ по земле
3. Выделенные юниты двигаются к точке
4. Невыделенные остаются на месте
```

### Сценарий 4: Смена цели
```
1. Выделить юнитов, отдать приказ двигаться
2. Клик ЛКМ по новому юниту (старые снимаются с выделения и останавливаются)
3. Клик ПКМ — двигается только новый юнит
```

---

## 🔧 Технические детали

### RTSInput.cs
```csharp
// Обновление _currentMousePosition только после нажатия
if (Input.GetMouseButtonDown(0))
{
    _startMousePosition = Input.mousePosition;
    _currentMousePosition = Input.mousePosition; // СБРОС!
    _isDragging = false;
}

if (Input.GetMouseButton(0))
{
    _currentMousePosition = Input.mousePosition; // Обновление при удержании
    float dragDistance = Vector2.Distance(...);
    if (dragDistance > minDragDistance)
        _isDragging = true;
}
```

### Unit.cs
```csharp
public void Deselect()
{
    IsSelected = false;
    
    // Остановить юнита при снятии выделения
    var pathfinder = GetComponent<UnitPathfinder>();
    if (pathfinder != null)
        pathfinder.Stop();
    
    OnSelectionChanged?.Invoke(this, false);
}

private void OnDrawGizmos()
{
    if (IsSelected)
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 0.6f);
    }
}
```

### UnitController.cs
```csharp
public void SelectUnitsInRect(Rect rect, Camera camera)
{
    DeselectAll(); // Сброс старого выделения
    
    foreach (var unit in _allUnits)
    {
        if (!unit.IsAlive) continue;
        
        Vector3 screenPoint = camera.WorldToScreenPoint(unit.transform.position);
        
        // Проверка: юнит перед камерой и в пределах экрана
        if (screenPoint.z < 0) continue;
        if (screenPoint.x < 0 || screenPoint.x > Screen.width ||
            screenPoint.y < 0 || screenPoint.y > Screen.height)
            continue;
        
        // Инверсия Y для GUI
        float invertedY = Screen.height - screenPoint.y;
        
        if (screenPoint.x >= rect.xMin && screenPoint.x <= rect.xMax &&
            invertedY >= rect.yMin && invertedY <= rect.yMax)
        {
            AddToSelection(unit);
        }
    }
}
```

---

## 📊 Статистика MVP

| Компонент | Статус | Файлов |
|-----------|--------|--------|
| Input System | ✅ Работает | RTSInput.cs, PlayerInput.cs |
| Unit Controller | ✅ Работает | UnitController.cs |
| Unit Base | ✅ Работает | Unit.cs |
| Pathfinding | ✅ Работает | UnitPathfinder.cs, FlowFieldManager.cs |
| UI | ⚠️ Базовый | GameUI.cs |
| Managers | ✅ Работает | GameManager.cs, ResourceManager.cs, UnitPoolManager.cs |

---

## 🎯 Следующие шаги

### Приоритет 1: Полировка MVP
- [ ] Добавить индикатор атаки (линия к цели)
- [ ] Добавить звуки выделения/приказа
- [ ] Оптимизировать отрисовку Gizmos (отключать в релизе)

### Приоритет 2: Геймплей
- [ ] Добавить здоровье и урон
- [ ] Добавить систему боя (атака/защита)
- [ ] Добавить ресурсы (дерево, камень, еда)

### Приоритет 3: Контент
- [ ] Заменить тестовые кубы на префабы юнитов
- [ ] Добавить разные типы юнитов (воин, лучник, кавалерия)
- [ ] Добавить препятствия и укрытия

---

## 📝 Заметки для разработчиков

1. **Всегда вызывать `DeselectAll()` перед новым выделением**  
   Это гарантирует, что старые юниты не останутся выделенными.

2. **Останавливать юнитов при снятии выделения**  
   Иначе они продолжат выполнять предыдущий приказ.

3. **Проверять `screenPoint.z > 0`**  
   Юниты за камерой имеют некорректные экранные координаты.

4. **Использовать `OnDrawGizmos()` для рантайм-отладки**  
   `OnDrawGizmosSelected()` работает только в инспекторе.

---

## 🔗 Связанные документы

- `stargates_lore_discussion.md` — лор вселенной
- `young_races_gdd.md` — дизайн молодых рас
- `../kcwweb/src/data/mechanics.ts` — игровые механики на сайте
