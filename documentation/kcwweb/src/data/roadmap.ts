import type { RoadmapData } from '../types';

// ----------------------------------------------------------------------------
// Roadmap Kiarche Continuum War
// ----------------------------------------------------------------------------
//
// Реалистичный план разработки (1 разработчик + LLM для генерации контента)
//
// Текущая дата: март 2026
//
// Жанр: 4X-стратегия с элементами RPG (Hero Units)
//
// =============================================================================
// ТЕХНИЧЕСКИЙ СТЕК (УТВЕРЖДЕНО 2026-03-27)
// =============================================================================
//
// Движок:       Unity (Personal License — бесплатен до $100k/год)
// Язык:         C#
// Версия Unity:  6000 LTS (или последняя stable)
// Платформы:    Windows, Steam OS (Linux)
//
// КЛЮЧЕВЫЕ СИСТЕМЫ:
// - Pathfinding:     Flow Fields + Recast NavMesh (для 400+ юнитов)
// - Object Pooling: Обязательно для производительности
// - Spatial:         QuadTree/Octree для поиска юнитов
// - UI:              Unity UI (Canvas)
// - Сеть:            Mirror / Netcode for Entities (архитектура сразу)
//
// МУЛЬТИПЛЕР:
// - Заложить архитектуру: Command Pattern, Deterministic Systems, Tick-based
// - Реализация в DLC
//
// =============================================================================
// ВАЖНО: Приоритет разработки рас (УТВЕРЖДЕНО 2026-03-27)
// =============================================================================
//
// ИГРАБЕЛЬНЫЕ РАСЫ (в разработке):
// - Тал'Син — уникальная механика идентичности и синхронизации
// - Молодые расы — классическая 4X-кампания:
//   1. Ре'Зиры (Re'Zir) — последователи Вел'Кетов, культ, орды
//   2. Ти'Оны (Ti'On) — последователи Кешари, телепатия, клонирование
//   3. Ве'Ори (Ve'Ori) — последователи Этернов, созерцание, ненасилие
//   4. Родверы (Rodver) — самостоятельный путь, адаптивность (MVP-раса)
//
// НЕИГРАБЕЛЬНЫЕ РАСЫ (отложено до Post-Release/DLC):
// - Синтекс — создатели Portal Network
// - Ки'Архе — распределённое поле, «боги» за сценой
// - Этерны — вознесённая раса
// - Кешари — гуманоидная раса (прототип для Ти'Онов)
// - Сил'Ни — пацифистская раса
// - Вел'Кеты — антагонисты (прототип для Ре'Зиров)
//
// КАМПАНИИ:
// - "Молодые расы" — классический 4X, выбор расы
// - "Тал'Син" — механика идентичности
//
// MVP: Родверы + базовые 4X механики + Unity
// ----------------------------------------------------------------------------

export const roadmapData: RoadmapData = {
  phases: [
    // =========================================================================
    // PHASE 1: PRE-PRODUCTION (Q2-Q3 2026)
    // =========================================================================
    {
      id: 'phase-preproduction',
      name: 'Pre-Production',
      description: 'Проработка концепции, дизайн-документ, прототип',
      milestones: [
        {
          id: 'milestone-concept',
          title: 'Концепт и дизайн',
          description: 'Создание основного концепта игры, GDD',
          status: 'done',
          startDate: '2026-03-01',
          endDate: '2026-05-31',
          tasks: [
            { id: 'task-001', title: 'Утвердить жанр и сеттинг', completed: true },
            { id: 'task-002', title: 'Описать ключевые механики (Hero Units, Portal Network, Tech Tree)', completed: true },
            { id: 'task-003', title: 'Набросать базовый лор вселенной', completed: true },
            { id: 'task-004', title: 'Создать GDD — документ дизайна игры', completed: true },
            { id: 'task-005', title: 'Определить стек технологий (Unity, C#, Flow Fields, Mirror)', completed: true },
            { id: 'task-006', title: 'Создать MVP-прототип для проверки концепции', completed: false },
            { id: 'task-007', title: 'Проработать Ки\'Архе и Тал\'Син (идеология, механика идентичности)', completed: true },
            { id: 'task-008', title: 'Проработать 4 молодые расы (Ре\'Зиры, Ти\'Оны, Ве\'Ори, Родверы)', completed: true },
            { id: 'task-009', title: 'Создать веб-портал документации (kcwweb)', completed: true },
          ],
        },
        {
          id: 'milestone-prototype',
          title: 'Прототипирование на Unity',
          description: 'Создание играбельного прототипа на Unity',
          status: 'planned',
          startDate: '2026-06-01',
          endDate: '2026-08-31',
          tasks: [
            { id: 'task-007', title: 'Unity проект — настройка, Git, базовая сцена', completed: false },
            { id: 'task-008', title: 'Flow Fields pathfinding — интеграция для 400+ юнитов', completed: false },
            { id: 'task-009', title: 'Object Pooling — система пулов юнитов', completed: false },
            { id: 'task-010', title: 'Базовый юнит — движение, выбор, командование', completed: false },
            { id: 'task-011', title: 'Прототип боя — столкновение двух групп юнитов', completed: false },
            { id: 'task-012', title: 'Базовая RTS камера — как в AoE (drag selection)', completed: false },
            { id: 'task-013', title: 'HUD прототип — ресурсы, панель юнитов', completed: false },
            { id: 'task-014', title: 'Тест производительности — 400 юнитов на карте', completed: false },
          ],
        },
        {
          id: 'milestone-core',
          title: 'Ядро игры',
          description: 'Реализация основных игровых систем',
          status: 'planned',
          startDate: '2026-09-01',
          endDate: '2026-11-30',
          tasks: [
            { id: 'task-015', title: 'Движок карты — tilemap, fog of war', completed: false },
            { id: 'task-016', title: 'Система ходов — пошаговый режим', completed: false },
            { id: 'task-017', title: 'Система ресурсов — базовые ресурсы и хранилища', completed: false },
            { id: 'task-018', title: 'Система строительства — здания колоний', completed: false },
            { id: 'task-019', title: 'Система юнитов — базовые типы юнитов', completed: false },
            { id: 'task-020', title: 'AI противников — простой тактический ИИ', completed: false },
          ],
        },
      ],
    },

    // =========================================================================
    // PHASE 2: PRODUCTION — ALPHA (Q4 2026 — Q2 2027)
    // =========================================================================
    {
      id: 'phase-alpha',
      name: 'Production — Alpha',
      description: 'Разработка core-систем и базового контента',
      milestones: [
        {
          id: 'milestone-gdd-extend',
          title: 'Расширение GDD',
          description: 'Детальная проработка механик,数值, кампаний',
          status: 'planned',
          startDate: '2026-09-01',
          endDate: '2026-11-30',
          tasks: [
            { id: 'task-021', title: 'Детальные механики рас (数值 для Родверов)', completed: false },
            { id: 'task-022', title: 'Доработка Кампании Тал\'Син — механика идентичности', completed: false },
            { id: 'task-023', title: 'Добавить остальные расы в GDD (Ре\'Зиры, Ти\'Оны, Ве\'Ори)', completed: false },
            { id: 'task-024', title: 'Документировать все системы (боевая, экономика, прогрессия)', completed: false },
          ],
        },
        {
          id: 'milestone-hero-system',
          title: 'Система героев',
          description: 'Найм, способности, прокачка героев',
          status: 'planned',
          startDate: '2026-12-01',
          endDate: '2027-02-28',
          tasks: [
            { id: 'task-025', title: 'Система найма героев — пул доступных героев', completed: false },
            { id: 'task-026', title: 'Система опыта — получение XP, повышение уровней', completed: false },
            { id: 'task-027', title: 'Древо навыков героя — способности и улучшения', completed: false },
            { id: 'task-028', title: 'Инвентарь героя — снаряжение и предметы', completed: false },
            { id: 'task-029', title: 'Уникальные способности героев — пассивные и активные', completed: false },
            { id: 'task-030', title: 'Привязка героев к юнитам — лидерство в бою', completed: false },
          ],
        },
        {
          id: 'milestone-portal-network',
          title: 'Portal Network',
          description: 'Система порталов, исследование, колонизация',
          status: 'planned',
          startDate: '2027-01-01',
          endDate: '2027-03-31',
          tasks: [
            { id: 'task-031', title: 'Генерация сети порталов — процедурное размещение', completed: false },
            { id: 'task-032', title: 'Активация порталов — стоимость и условия', completed: false },
            { id: 'task-033', title: 'Телепортация — перемещение между порталами', completed: false },
            { id: 'task-034', title: 'Колонизация через порталы — основание новых колоний', completed: false },
            { id: 'task-035', title: 'Захват порталов — военные действия', completed: false },
            { id: 'task-036', title: 'Особые порталы — легендарные локации', completed: false },
          ],
        },
        {
          id: 'milestone-tech-tree',
          title: 'Древо технологий',
          description: 'Развитие от мечей к огнестрелу',
          status: 'planned',
          startDate: '2027-02-01',
          endDate: '2027-04-30',
          tasks: [
            { id: 'task-037', title: 'Ветка военных технологий — оружие и броня', completed: false },
            { id: 'task-038', title: 'Ветка производства — здания и ресурсы', completed: false },
            { id: 'task-039', title: 'Ветка исследования — скорость и эффективность', completed: false },
            { id: 'task-040', title: 'Ветка Architects — древние технологии', completed: false },
            { id: 'task-041', title: 'Визуализация древа — интерактивное UI', completed: false },
            { id: 'task-042', title: 'Предметы из технологий — юниты и здания', completed: false },
          ],
        },
      ],
    },

    // =========================================================================
    // PHASE 3: PRODUCTION — BETA (Q4 2027 — Q1 2028)
    // =========================================================================
    {
      id: 'phase-beta',
      name: 'Production — Beta',
      description: 'Создание контента, балансировка, полировка',
      milestones: [
        {
          id: 'milestone-content',
          title: 'Контент',
          description: 'Локации, фракции, сюжет',
          status: 'planned',
          startDate: '2027-05-01',
          endDate: '2027-10-31',
          tasks: [
            { id: 'task-043', title: 'Ре\'Зиры — описание, юниты, механики (наниты, орды, культ)', completed: false },
            { id: 'task-044', title: 'Ти\'Оны — описание, юниты, механики (телепатия, клонирование)', completed: false },
            { id: 'task-045', title: 'Ве\'Ори — описание, юниты, механики (созерцание, наследие)', completed: false },
            { id: 'task-046', title: 'Тал\'Син — детальная механика синхронизации и развития', completed: false },
            { id: 'task-047', title: 'Кампания "Молодые расы" — 3 акта, выбор расы', completed: false },
            { id: 'task-048', title: 'Кампания "Тал\'Син" — механика идентичности', completed: false },
            { id: 'task-049', title: 'Локации — 10+ уникальных планет/секторов', completed: false },
            { id: 'task-050', title: 'Антагонисты — Вел\'Кеты как основной враг', completed: false },
          ],
        },
        {
          id: 'milestone-balance',
          title: 'Баланс и полировка',
          description: 'Балансировка механик, оптимизация',
          status: 'planned',
          startDate: '2027-11-01',
          endDate: '2028-01-31',
          tasks: [
            { id: 'task-051', title: 'Баланс юнитов — тестирование и корректировка', completed: false },
            { id: 'task-052', title: 'Баланс героев — проверка способностей', completed: false },
            { id: 'task-053', title: 'Баланс ресурсов — экономика колоний', completed: false },
            { id: 'task-054', title: 'Оптимизация производительности — FPS и загрузка', completed: false },
            { id: 'task-055', title: 'Полировка UI — анимации, звуки, feedback', completed: false },
            { id: 'task-056', title: 'Исправление багов — приоритетные и критические', completed: false },
            { id: 'task-057', title: 'Playtesting — внутренние и внешние тесты', completed: false },
          ],
        },
      ],
    },

    // =========================================================================
    // PHASE 4: EARLY ACCESS (Q2-Q3 2028)
    // =========================================================================
    {
      id: 'phase-ea',
      name: 'Early Access',
      description: 'Запуск в Early Access, сбор обратной связи',
      milestones: [
        {
          id: 'milestone-ea-launch',
          title: 'Запуск Early Access',
          description: 'Публичный запуск в Steam',
          status: 'planned',
          startDate: '2028-02-01',
          endDate: '2028-03-31',
          tasks: [
            { id: 'task-058', title: 'Подготовка страницы в Steam', completed: false },
            { id: 'task-059', title: 'Маркетинговые материалы — трейлер, скриншоты', completed: false },
            { id: 'task-060', title: 'Система обновлений — патчноуты, ветки', completed: false },
            { id: 'task-061', title: 'Discord и сообщество — каналы обратной связи', completed: false },
            { id: 'task-062', title: 'Запуск EA — публикация в Steam', completed: false },
          ],
        },
        {
          id: 'milestone-ea-feedback',
          title: 'Сбор обратной связи',
          description: 'Итерации на основе отзывов игроков',
          status: 'planned',
          startDate: '2028-04-01',
          endDate: '2028-08-31',
          tasks: [
            { id: 'task-063', title: 'Анализ отзывов — баги, пожелания, баланс', completed: false },
            { id: 'task-064', title: 'Патч 1 — критические исправления', completed: false },
            { id: 'task-065', title: 'Патч 2 — улучшения по отзывам', completed: false },
            { id: 'task-066', title: 'Новый контент — герои, юниты, локации', completed: false },
            { id: 'task-067', title: 'Продолжение цикла — итерации и обновления', completed: false },
          ],
        },
      ],
    },

    // =========================================================================
    // PHASE 5: RELEASE (Q3-Q4 2028)
    // =========================================================================
    {
      id: 'phase-release',
      name: 'Release',
      description: 'Финальный релиз 1.0',
      milestones: [
        {
          id: 'milestone-release',
          title: 'Релиз 1.0',
          description: 'Финальная версия игры',
          status: 'planned',
          startDate: '2028-09-01',
          endDate: '2028-10-31',
          tasks: [
            { id: 'task-068', title: 'Финальная версия — feature complete', completed: false },
            { id: 'task-069', title: 'Финальный playtest — проверка всего контента', completed: false },
            { id: 'task-070', title: 'Документация — гайды, туториал', completed: false },
            { id: 'task-071', title: 'Русификация/локализация — если нужно', completed: false },
            { id: 'task-072', title: 'Релиз в Steam — полный релиз', completed: false },
            { id: 'task-073', title: 'Маркетинговая кампания — финальный push', completed: false },
          ],
        },
      ],
    },

    // =========================================================================
    // PHASE 6: POST-RELEASE (Q4 2028+)
    // =========================================================================
    {
      id: 'phase-postrelease',
      name: 'Post-Release',
      description: 'Поддержка, патчи, DLC',
      milestones: [
        {
          id: 'milestone-support',
          title: 'Поддержка',
          description: 'Патчи и исправления',
          status: 'planned',
          startDate: '2028-11-01',
          endDate: '2029-12-31',
          tasks: [
            { id: 'task-074', title: 'Патчи безопасности и стабильности', completed: false },
            { id: 'task-075', title: 'Исправление багов — по отчётам игроков', completed: false },
            { id: 'task-076', title: 'Мелкие улучшения — quality of life', completed: false },
          ],
        },
        {
          id: 'milestone-dlc',
          title: 'DLC',
          description: 'Дополнительный контент',
          status: 'planned',
          startDate: '2029-01-01',
          endDate: '2029-12-31',
          tasks: [
            { id: 'task-077', title: 'DLC 1 — мультиплеер', completed: false },
            { id: 'task-078', title: 'DLC 2 — новая фракция или кампания', completed: false },
            { id: 'task-079', title: 'DLC 3 — расширение карты/вселенной', completed: false },
          ],
        },
      ],
    },
  ],
};
