import type { ConceptArt, HomePageData } from '../types';

// ============================================================================
// Концепт-арты
// ============================================================================

export const conceptArts: ConceptArt[] = [
  // --------------------------------------------------------------------------
  // Ки'Архе (Ki'Arhe) — Древняя распределённая цивилизация
  // --------------------------------------------------------------------------
  {
    id: 'kiarche-001',
    title: 'Ки\'Архе: Ядро поля',
    description: 'Центральный узел распределённого поля Ки\'Архе. Яркое ядро с множественными слоями свечения — центр управления структурой.',
    imageUrl: '/assets/concepts/kiarche/1774598701.png',
    category: 'characters',
    tags: ['kiarche', 'core', 'node', 'glow'],
  },
  {
    id: 'kiarche-002',
    title: 'Ки\'Архе: Множественный взгляд',
    description: 'Энергетическая сущность с множественными светящимися глазами, расположенными в ряд. Взгляд "считывает" информацию — никаких эмоций, только функция.',
    imageUrl: '/assets/concepts/kiarche/1774598888.png',
    category: 'characters',
    tags: ['kiarche', 'eyes', 'energy', 'abstract'],
  },
  {
    id: 'kiarche-003',
    title: 'Ки\'Архе: Кристаллическая оболочка',
    description: 'Структура Ки\'Архе в "собранном" состоянии. Полупрозрачная кристаллическая оболочка скрывает тёмное ядро внутри.',
    imageUrl: '/assets/concepts/kiarche/1774600367.png',
    category: 'characters',
    tags: ['kiarche', 'crystal', 'shell', 'core'],
  },
  {
    id: 'kiarche-004',
    title: 'Ки\'Архе: Распределённое поле',
    description: 'Множественные узлы разной плотности — яркие центры и тусклые края. Визуализация распределённой природы сознания.',
    imageUrl: '/assets/concepts/kiarche/1774600392.png',
    category: 'characters',
    tags: ['kiarche', 'field', 'distributed', 'nodes'],
  },
  {
    id: 'kiarche-005',
    title: 'Ки\'Архе: Призрачная форма',
    description: 'Полупрозрачная форма с размытыми краями. Тёмный туман клубится внутри, свечение изнутри — холодные фиолетовые тона.',
    imageUrl: '/assets/concepts/kiarche/1774600385.png',
    category: 'characters',
    tags: ['kiarche', 'ghost', 'transparent', 'mist'],
  },
  {
    id: 'kiarche-006',
    title: 'Ки\'Архе: Тёмное ядро',
    description: 'Структура с выраженным тёмным ядром и светящейся оболочкой. Контраст между внутренней тьмой и внешним свечением.',
    imageUrl: '/assets/concepts/kiarche/alien_entity_invisible.jpg',
    category: 'characters',
    tags: ['kiarche', 'dark', 'core', 'glow', 'invisible'],
  },
  {
    id: 'kiarche-007',
    title: 'Ки\'Архе: Туманная структура',
    description: 'Форма почти целиком растворяется в пространстве. Только ядро остаётся видимым — остальное "утекает" в среду.',
    imageUrl: '/assets/concepts/talsin/1774601173.png',
    category: 'characters',
    tags: ['kiarche', 'mist', 'fade', 'ethereal'],
  },

  // --------------------------------------------------------------------------
  // Тал'Син (Tal'Sin) — Биологические последователи Ки'Архе
  // --------------------------------------------------------------------------
  {
    id: 'talsin-001',
    title: 'Тал\'Син: Полный образ',
    description: 'Основной концепт Тал\'Син. Удлинённые пропорции, полупрозрачная кожа, резонаторы видны как светящиеся узлы. Холодная красота без эмоций.',
    imageUrl: '/assets/concepts/talsin/1774600405.png',
    category: 'characters',
    tags: ['talsin', 'full', 'body', 'resonators'],
  },
  {
    id: 'talsin-002',
    title: 'Тал\'Син: Лицо',
    description: 'Крупный план лица — ключевой референс для моделинга. Глаза заполнены цветом (без белка), крупные и удлинённые. Удлинённый череп, минимальные черты лица.',
    imageUrl: '/assets/concepts/kiarche/1774600361.png',
    category: 'characters',
    tags: ['talsin', 'face', 'eyes', 'reference'],
  },
  {
    id: 'talsin-003',
    title: 'Тал\'Син: Кристаллическая форма',
    description: 'Структура Тал\'Син в собранном состоянии. Полупрозрачная кристаллическая форма с внутренним свечением резонаторов.',
    imageUrl: '/assets/concepts/talsin/1774599765.png',
    category: 'characters',
    tags: ['talsin', 'crystal', 'form', 'resonators'],
  },

  // --------------------------------------------------------------------------
  // Молодые расы (Young Races) — Играбельные фракции
  // --------------------------------------------------------------------------
  {
    id: 'rezir-001',
    title: 'Ре\'Зиры: Последователи Вел\'Кетов',
    description: 'Гуманоид с нанитовым симбионтом. Тёмная кожа с металлическим оттенком, светящиеся линии нанитовых потоков, глаза с вертикальными зрачками.',
    imageUrl: '/assets/concepts/characters/villain.png',
    category: 'characters',
    tags: ['rezir', 'nanites', 'velketh', 'minion'],
  },
  {
    id: 'tion-001',
    title: 'Ти\'Оны: Последователи Кешари',
    description: 'Стройный гуманоид с серебристой кожей и светящимися глазами. Мозговой имплант виден как светящийся узор на висках.',
    imageUrl: '/assets/concepts/characters/ally.png',
    category: 'characters',
    tags: ['tion', 'keshari', 'telepath', 'clone'],
  },
  {
    id: 'veori-001',
    title: 'Ве\'Ори: Последователи Этернов',
    description: 'Гуманоид со светлой кожей и динамическими рисунками. Спокойное выражение лица, глаза меняют цвет при созерцании.',
    imageUrl: '/assets/concepts/characters/hero.png',
    category: 'characters',
    tags: ['veori', 'eterns', 'meditation', 'peaceful'],
  },
  {
    id: 'rodver-001',
    title: 'Родверы: Самостоятельный путь',
    description: 'Практически неотличимы от людей Земли. Плотное телосложение, адаптированные глаза, минимальные генетические изменения.',
    imageUrl: '/assets/concepts/b369d202-detail.jpg',
    category: 'characters',
    tags: ['rodver', 'human', 'adaptive', 'mvp'],
  },
];

// ============================================================================
// Данные для главной страницы
// ============================================================================

export const homePageData: HomePageData = {
  hero: {
    title: 'Kiarche Continuum War',
    tagline: 'Стройте империю. Нанимайте героев. Исследуйте галактику.',
    ctaPrimary: {
      label: 'Об игре',
      path: '/overview',
    },
    ctaSecondary: {
      label: 'Wishlist в Steam',
      path: '#',
    },
  },
  gameInfo: {
    title: 'Kiarche Continuum War',
    tagline: 'Стройте империю. Нанимайте героев. Исследуйте галактику.',
    description: `
      Классическая 4X-стратегия с элементами RPG.

      Молодые расы галактики обнаружили Portal Network — сеть древних порталов,
      оставленных цивилизацией Синтекс. Исследуйте галактику, нанимайте героев,
      развивайте технологии и определяйте судьбу цивилизаций.
    `,
    genre: ['4X Strategy', 'Turn-Based Strategy', 'RPG', 'Sci-Fi'],
    platforms: ['PC (Windows)'],
    targetAudience: 'Любители стратегий и RPG',
    features: [
      {
        id: 'feat-heroes',
        title: 'Герои с уникальными способностями',
        description: 'Нанимайте и прокачивайте героев с уникальными навыками',
      },
      {
        id: 'feat-portals',
        title: 'Portal Network',
        description: 'Исследуйте галактику через сеть древних порталов',
      },
      {
        id: 'feat-tech',
        title: 'Технологическое развитие',
        description: 'Развивайте технологии от мечей к огнестрельному оружию',
      },
      {
        id: 'feat-factions',
        title: 'Фракции и дипломатия',
        description: 'Взаимодействуйте с разными фракциями',
      },
    ],
    steamUrl: undefined,
    trailerUrl: undefined,
  },
  featuredConcepts: conceptArts.slice(0, 6),
};
