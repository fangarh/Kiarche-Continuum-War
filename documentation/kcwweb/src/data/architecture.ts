import type { ArchitectureData } from '../types';

// ----------------------------------------------------------------------------
// Архитектура проекта Kiarche Continuum War
// ----------------------------------------------------------------------------
//
// Модульная архитектура игры на Unity 6000 LTS
//
// Основные системы:
// - Core — ядро игры, основные интерфейсы
// - Factions — фракции и расы (Тал'Син, Родверы, Ре'Зиры, Ти'Оны, Ве'Ори)
// - Units — юниты, герои, способности
// - Map — карта, порталы, исследование
// - Combat — боевая система
// - Economy — ресурсы, производство, торговля
// - AI — искусственный интеллект
// - UI — пользовательский интерфейс
// - Network — сетевой код (для DLC)
//
// Технический стек:
// - Движок: Unity 6000 LTS
// - Язык: C#
// - Pathfinding: Flow Fields + NavMesh
// - Сеть: Mirror / Netcode for Entities
//
// ----------------------------------------------------------------------------

export const architectureData: ArchitectureData = {
  modules: [
    // ========================================================================
    // CORE — Ядро игры
    // ========================================================================
    {
      name: 'Core',
      description: 'Базовые интерфейсы, менеджеры, утилиты. Ядро, от которого зависят все остальные модули.',
      path: 'src/Core',
      dependencies: [],
    },
    {
      name: 'GameState',
      description: 'Управление состоянием игры (меню, загрузка, игра, пауза). Машина состояний.',
      path: 'src/Core/GameState',
      dependencies: ['Core'],
    },
    {
      name: 'Events',
      description: 'Система событий (Event Bus). Асинхронная коммуникация между модулями.',
      path: 'src/Core/Events',
      dependencies: ['Core'],
    },
    {
      name: 'ObjectPool',
      description: 'Пул объектов для юнитов (500+ юнитов). Создание один раз, повторное использование.',
      path: 'src/Core/ObjectPool',
      dependencies: ['Core'],
    },

    // ========================================================================
    // FACTIONS — Фракции и расы
    // ========================================================================
    {
      name: 'Factions',
      description: 'Система фракций: Тал\'Син, Родверы, Ре\'Зиры, Ти\'Оны, Ве\'Ори, NPC-фракции.',
      path: 'src/Factions',
      dependencies: ['Core', 'Events'],
    },
    {
      name: 'Factions.TalSin',
      description: 'Механика Тал\'Син: синхронизация, интеграция, эхо, 5 этапов развития.',
      path: 'src/Factions/TalSin',
      dependencies: ['Factions', 'Units'],
    },
    {
      name: 'Factions.Rodver',
      description: 'Механика Родверов: адаптивность, адаптивное производство, родовые связи.',
      path: 'src/Factions/Rodver',
      dependencies: ['Factions', 'Economy'],
    },
    {
      name: 'Factions.Rezir',
      description: 'Механика Ре\'Зиров: нанитовый симбионт, орды, захват городов.',
      path: 'src/Factions/Rezir',
      dependencies: ['Factions', 'Units'],
    },
    {
      name: 'Factions.Tion',
      description: 'Механика Ти\'Онов: телепатия, клонирование, коллективный разум.',
      path: 'src/Factions/Tion',
      dependencies: ['Factions', 'Units'],
    },
    {
      name: 'Factions.Veori',
      description: 'Механика Ве\'Ори: созерцание, пробуждение, передача знаний.',
      path: 'src/Factions/Veori',
      dependencies: ['Factions', 'Units'],
    },

    // ========================================================================
    // UNITS — Юниты и герои
    // ========================================================================
    {
      name: 'Units',
      description: 'Базовые классы юнитов, характеристики, здоровье, перемещение. **Реализовано в MVP прототипе (базовое).**',
      path: 'src/Units',
      dependencies: ['Core', 'ObjectPool'],
    },
    {
      name: 'Units.Movement',
      description: 'Движение юнитов: перемещение к точке, формации (круг, линия). **Реализовано в MVP (базовое).**',
      path: 'src/Units/Movement',
      dependencies: ['Units'],
    },
    {
      name: 'Units.Combat',
      description: 'Бой юнитов: атака, урон, здоровье, перезарядка. **Реализовано в MVP (базовое).**',
      path: 'src/Units/Combat',
      dependencies: ['Units'],
    },
    {
      name: 'Units.Selection',
      description: 'Выделение юнитов: клик, drag selection, групповое выделение. **Реализовано в MVP.**',
      path: 'src/Units/Selection',
      dependencies: ['Units', 'Input'],
    },
    {
      name: 'Units.Heroes',
      description: 'Герои: уникальные способности, прокачка, уровни (до 60), эхо.',
      path: 'src/Units/Heroes',
      dependencies: ['Units', 'Progression'],
    },
    {
      name: 'Units.Abilities',
      description: 'Система способностей: баффы, дебаффы, активные/пассивные навыки.',
      path: 'src/Units/Abilities',
      dependencies: ['Units', 'Events'],
    },
    {
      name: 'Units.Formations',
      description: 'Боевые построения, формации, групповое поведение юнитов.',
      path: 'src/Units/Formations',
      dependencies: ['Units', 'Combat'],
    },

    // ========================================================================
    // MAP — Карта и исследование
    // ========================================================================
    {
      name: 'Map',
      description: 'Система карты: гексы/зоны, туман войны, исследование.',
      path: 'src/Map',
      dependencies: ['Core', 'Events'],
    },
    {
      name: 'Map.PortalNetwork',
      description: 'Portal Network: порталы Синтекс, резонансные якоря Ки\'Архе.',
      path: 'src/Map/PortalNetwork',
      dependencies: ['Map', 'Factions'],
    },
    {
      name: 'Map.Colonization',
      description: 'Колонизация планет: захват территорий, строительство.',
      path: 'src/Map/Colonization',
      dependencies: ['Map', 'Economy'],
    },
    {
      name: 'Map.Worlds',
      description: 'Мир: 8 планет (Терра Прайм, К\'Тана, Велур, Синт\'Ара, Разлом, Т\'Кир, Гоа\'ол, Асгард Протекторат).',
      path: 'src/Map/Worlds',
      dependencies: ['Map'],
    },

    // ========================================================================
    // PATHFINDING — Навигация
    // ========================================================================
    {
      name: 'Pathfinding',
      description: 'Flow Fields + NavMesh для 400+ юнитов. Оптимизированный поиск пути.',
      path: 'src/Pathfinding',
      dependencies: ['Core', 'Map'],
    },
    {
      name: 'Pathfinding.FlowFields',
      description: 'Flow Fields — один расчёт на зону, все юниты следуют полю.',
      path: 'src/Pathfinding/FlowFields',
      dependencies: ['Pathfinding', 'Map'],
    },
    {
      name: 'Pathfinding.NavMesh',
      description: 'NavMesh для 3D-навигации на планетах.',
      path: 'src/Pathfinding/NavMesh',
      dependencies: ['Pathfinding', 'Map'],
    },

    // ========================================================================
    // COMBAT — Боевая система
    // ========================================================================
    {
      name: 'Combat',
      description: 'Пошаговые/real-time сражения, урон, броня, криты.',
      path: 'src/Combat',
      dependencies: ['Core', 'Units', 'Map'],
    },
    {
      name: 'Combat.TurnBased',
      description: 'RTS-режим: непрерывное время, команды игрока, тайминги способностей и реакция ИИ.',
      path: 'src/Combat/TurnBased',
      dependencies: ['Combat', 'GameState'],
    },
    {
      name: 'Combat.RealTime',
      description: 'Real-time режим для массовых сражений.',
      path: 'src/Combat/RealTime',
      dependencies: ['Combat', 'Pathfinding'],
    },
    {
      name: 'Combat.Damage',
      description: 'Система урона: типы урона, броня, сопротивления, криты.',
      path: 'src/Combat/Damage',
      dependencies: ['Combat', 'Units'],
    },

    // ========================================================================
    // PROGRESSION — Прогрессия
    // ========================================================================
    {
      name: 'Progression',
      description: 'Система прогрессии: уровни, опыт, навыки.',
      path: 'src/Progression',
      dependencies: ['Core', 'Units'],
    },
    {
      name: 'Progression.TalSin',
      description: '5 этапов развития Тал\'Син: от индивидуальности к дематериализации.',
      path: 'src/Progression/TalSin',
      dependencies: ['Progression', 'Factions.TalSin'],
    },
    {
      name: 'Progression.TechTree',
      description: 'Дерево технологий: от мечей к огнестрелу и заводам.',
      path: 'src/Progression/TechTree',
      dependencies: ['Progression', 'Economy'],
    },

    // ========================================================================
    // ECONOMY — Экономика
    // ========================================================================
    {
      name: 'Economy',
      description: 'Ресурсы, валюта, производство, торговля.',
      path: 'src/Economy',
      dependencies: ['Core', 'Map'],
    },
    {
      name: 'Economy.Resources',
      description: 'Ресурсы: Резонанс ⟁, Знание Ξ, Непрерывность ∮, материалы.',
      path: 'src/Economy/Resources',
      dependencies: ['Economy'],
    },
    {
      name: 'Economy.Production',
      description: 'Производство: здания, фабрики, адаптивное производство (Родверы).',
      path: 'src/Economy/Production',
      dependencies: ['Economy', 'Map.Colonization'],
    },
    {
      name: 'Economy.Trade',
      description: 'Торговля между фракциями, маршруты, цены.',
      path: 'src/Economy/Trade',
      dependencies: ['Economy', 'Factions'],
    },

    // ========================================================================
    // AI — Искусственный интеллект
    // ========================================================================
    {
      name: 'AI',
      description: 'ИИ для одиночной игры: стратегия, тактика, принятие решений.',
      path: 'src/AI',
      dependencies: ['Core', 'Combat', 'Economy'],
    },
    {
      name: 'AI.Strategy',
      description: 'Стратегический ИИ: развитие базы, tech tiers, логистика и контроль карты.',
      path: 'src/AI/Strategy',
      dependencies: ['AI', 'Factions'],
    },
    {
      name: 'AI.Tactics',
      description: 'Тактический ИИ: поведение в бою, построения, приоритеты целей.',
      path: 'src/AI/Tactics',
      dependencies: ['AI', 'Combat'],
    },

    // ========================================================================
    // INPUT — Ввод
    // ========================================================================
    {
      name: 'Input',
      description: 'Система ввода: мышь, клавиатура, Input System. **Реализовано в MVP.**',
      path: 'src/InputSystem',
      dependencies: ['Core'],
    },
    {
      name: 'Input.RTS',
      description: 'RTS управление: выделение, перемещение, атака. **Реализовано в MVP.**',
      path: 'src/InputSystem/RTS',
      dependencies: ['Input', 'Units'],
    },

    // ========================================================================
    // UI — Пользовательский интерфейс
    // ========================================================================
    {
      name: 'UI',
      description: 'UI система: Canvas, панели, HUD, меню. **Реализовано в MVP.**',
      path: 'src/UI',
      dependencies: ['Core', 'Events'],
    },
    {
      name: 'UI.HUD',
      description: 'HUD в игре: ресурсы, миникарта, информация о юнитах. **Реализовано в MVP (ресурсы, выделение).**',
      path: 'src/UI/HUD',
      dependencies: ['UI', 'Units', 'Economy'],
    },
    {
      name: 'UI.Menus',
      description: 'Меню: главное, настройки, загрузка, сохранение.',
      path: 'src/UI/Menus',
      dependencies: ['UI', 'GameState'],
    },
    {
      name: 'UI.Diplomacy',
      description: 'Экран дипломатии: переговоры, договоры, отношения.',
      path: 'src/UI/Diplomacy',
      dependencies: ['UI', 'Factions'],
    },
    {
      name: 'UI.TechTree',
      description: 'Экран дерева технологий.',
      path: 'src/UI/TechTree',
      dependencies: ['UI', 'Progression.TechTree'],
    },

    // ========================================================================
    // NETWORK — Сетевой код (DLC)
    // ========================================================================
    {
      name: 'Network',
      description: 'Сетевой код для мультиплеера. Реализация в DLC.',
      path: 'src/Network',
      dependencies: ['Core', 'Events'],
    },
    {
      name: 'Network.Commands',
      description: 'Command Pattern для действий юнитов. Детерминизм.',
      path: 'src/Network/Commands',
      dependencies: ['Network', 'Units'],
    },
    {
      name: 'Network.Sync',
      description: 'Синхронизация состояния, tick-based simulation.',
      path: 'src/Network/Sync',
      dependencies: ['Network', 'GameState'],
    },

    // ========================================================================
    // DATA — Данные и контент
    // ========================================================================
    {
      name: 'Data',
      description: 'Данные игры: ScriptableObject, JSON, конфигурации.',
      path: 'src/Data',
      dependencies: ['Core'],
    },
    {
      name: 'Data.Lore',
      description: 'Лор: миры, история, фракции, персонажи.',
      path: 'src/Data/Lore',
      dependencies: ['Data'],
    },
    {
      name: 'Data.Localization',
      description: 'Локализация: тексты, переводы, шрифты.',
      path: 'src/Data/Localization',
      dependencies: ['Data', 'UI'],
    },
  ],

  // ---------------------------------------------------------------------------
  // ПОТОКИ ДАННЫХ
  // ---------------------------------------------------------------------------
  dataFlows: [
    {
      name: 'Input → Command → Action',
      description: 'Ввод игрока преобразуется в команду, которая выполняется юнитом.',
      from: 'UI.Input',
      to: 'Units.Action',
      type: 'sync',
    },
    {
      name: 'Event Bus',
      description: 'Асинхронная рассылка событий между модулями (Decoupled architecture).',
      from: 'Any Module',
      to: 'Core.Events',
      type: 'async',
    },
    {
      name: 'GameState Flow',
      description: 'Переключение состояний: Menu → Loading → Game → Pause → Game.',
      from: 'GameState.Machine',
      to: 'All Modules',
      type: 'event',
    },
    {
      name: 'Resource Flow',
      description: 'Добыча ресурсов → Склад → Производство → Юниты/Здания.',
      from: 'Economy.Resources',
      to: 'Economy.Production',
      type: 'sync',
    },
    {
      name: 'Damage Flow',
      description: 'Атака → Расчёт урона → Применение к здоровью → Смерть/Выживание.',
      from: 'Combat.Damage',
      to: 'Units.Health',
      type: 'sync',
    },
    {
      name: 'Progression Flow',
      description: 'Опыт → Уровень → Навыки/Характеристики.',
      from: 'Units.XP',
      to: 'Progression.Level',
      type: 'sync',
    },
    {
      name: 'TalSin Sync Flow',
      description: 'Синхронизация Тал\'Син: юниты объединяют сознание → бонусы.',
      from: 'Factions.TalSin.Units',
      to: 'Factions.TalSin.Network',
      type: 'async',
    },
    {
      name: 'Network Command Flow (DLC)',
      description: 'Команда → Сериализация → Отправка → Десериализация → Выполнение.',
      from: 'Network.Commands',
      to: 'Network.Sync',
      type: 'async',
    },
  ],

  // ---------------------------------------------------------------------------
  // ДИАГРАММЫ (PLACEHOLDERS)
  // ---------------------------------------------------------------------------
  diagramPlaceholders: [
    {
      id: 'diagram-architecture',
      title: 'Общая архитектура системы',
      description: 'Диаграмма всех модулей и зависимостей между ними.',
      type: 'component',
    },
    {
      id: 'diagram-gamestate',
      title: 'Машина состояний игры',
      description: 'Переходы между состояниями: Menu, Loading, Playing, Paused, GameOver.',
      type: 'flowchart',
    },
    {
      id: 'diagram-unit-lifecycle',
      title: 'Жизненный цикл юнита',
      description: 'Спавн → Инициализация → Действия → Бой → Смерть → Интеграция (Тал\'Син).',
      type: 'sequence',
    },
    {
      id: 'diagram-combat-flow',
      title: 'Боевой поток',
      description: 'Начало боя → Развёртывание → Ходы → Конец боя → Награды.',
      type: 'flowchart',
    },
    {
      id: 'diagram-talsin-sync',
      title: 'Синхронизация Тал\'Син',
      description: 'Как юниты Тал\'Син объединяют сознание и получают бонусы.',
      type: 'sequence',
    },
    {
      id: 'diagram-pathfinding',
      title: 'Flow Fields навигация',
      description: 'Расчёт поля потока → Следование юнитов → Обход препятствий.',
      type: 'flowchart',
    },
    {
      id: 'diagram-economy',
      title: 'Экономический цикл',
      description: 'Добыча → Производство → Торговля → Развитие.',
      type: 'flowchart',
    },
    {
      id: 'diagram-network-sync',
      title: 'Синхронизация сети (DLC)',
      description: 'Command Pattern + Tick-based simulation для мультиплеера.',
      type: 'sequence',
    },
  ],
};
