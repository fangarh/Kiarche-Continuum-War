# Настройка MVP Прототипа

## Требования

- Unity 6000.4.0f1 или новее
- Input System Package (установлен через Package Manager)
- Unity UI Package

## Установка

### 1. Проверка пакетов

Убедитесь, что установлены пакеты:
- **Input System** (Window → Package Manager → Input System)
- **Unity UI** (должен быть установлен по умолчанию)

### 2. Генерация сцены и префаба

1. В Unity Editor откройте меню **Tools → KCW**
2. Выберите **Generate MVP Scene**
3. Затем **Create Unit Prefab**

### 3. Настройка сцены

После генрации сцены:

1. Откройте `Assets/Scenes/MVP_Prototype.unity` (должна открыться автоматически)
2. Найдите GameObject **GameManager** в иерархии
3. В инспекторе назначьте **Unit Prefab** (перетащите из `Assets/Prefabs/`)

## Управление

| Действие | Управление |
|----------|------------|
| Выделение юнита | ЛКМ (клик) |
| Выделение группы | ЛКМ (drag) |
| Перемещение | ПКМ по земле |
| Атака | ПКМ по врагу |

## Тестовые команды

| Клавиша | Действие |
|---------|----------|
| **T** | Создать 10 тестовых юнитов |
| **P** | Тест производительности (400 юнитов) |
| **R** | Показать ресурсы в консоли |

## Структура скриптов

```
Assets/Scripts/
├── Core/
│   └── ResourceManager.cs      # Управление ресурсами
├── Editor/
│   ├── CreateUnitPrefabEditor.cs   # Генерация префаба
│   └── GenerateMVPScene.cs         # Генерация сцены
├── InputSystem/
│   ├── PlayerInput.inputactions    # Input Actions asset
│   └── RTSInput.cs                 # Обработка ввода (Input System)
├── Managers/
│   └── GameManager.cs              # Главный менеджер
├── UI/
│   └── GameUI.cs                   # HUD
└── Units/
    ├── Unit.cs                     # Базовый класс юнита
    └── UnitController.cs           # Контроллер группы
```

## Решение проблем

### Ошибки компиляции

1. **CS0246: The type or namespace name 'InputSystem'**
   - Установите Input System через Package Manager

2. **CS0246: The type or namespace name 'UnityEngine.UI'**
   - Убедитесь, что Unity UI package установлен

3. **Не находится GameManager/UnitController**
   - Проверьте, что все скрипты в папке Assets/Scripts/

### Сцена не загружается

1. Удалите `Assets/Scenes/MVP_Prototype.unity`
2. Запустите **Tools → KCW → Generate MVP Scene** заново

### Input не работает

1. Проверьте, что **Input System Package** установлен
2. Убедитесь, что `PlayerInput.inputactions` сгенерировал C# класс
   - Выделите файл в Project окне
   - Проверьте, что в инспекторе есть кнопка "Save Asset" (если есть — нажмите)

### Префаб не назначается

1. Убедитесь, что префаб создан через **Tools → KCW → Create Unit Prefab**
2. Перетащите `UnitPrefab.prefab` из `Assets/Prefabs/` в поле **Unit Prefab** компонента GameManager

## Следующие шаги

После успешного запуска:

1. Настройте цвета юнитов (синие vs красные)
2. Отрегулируйте баланс (скорость, урон, здоровье)
3. Добавьте миникарту
4. Реализуйте Flow Fields для pathfinding
