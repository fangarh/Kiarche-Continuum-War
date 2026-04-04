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

  // --------------------------------------------------------------------------
  // Родверы — Герои (6 эпох)
  // --------------------------------------------------------------------------
  {
    id: 'rodver-kai',
    title: 'Кай Первым — Разведчик',
    description: 'Первый Родвер, выживший за пределами родного поселения. Худой, жилистый, в самодельной броне из подручных материалов. На поясе — набор инструментов для каждой среды.',
    imageUrl: '/assets/characters/rodver/kai.png',
    category: 'characters',
    tags: ['rodver', 'hero', 'scout', 'survival', 'era-1'],
  },
  {
    id: 'rodver-dork',
    title: 'Старейшина Дорк — Лидер общины',
    description: 'Создатель Совета Родов. Пожилой Родвер с седыми волосами, в простой одежде старейшины. Несёт посох с символами всех родов.',
    imageUrl: '/assets/characters/rodver/dork.jpg',
    category: 'characters',
    tags: ['rodver', 'hero', 'elder', 'community', 'era-2'],
  },
  {
    id: 'rodver-cross',
    title: 'Инженер Кросс — Создатель машин',
    description: 'Инженер-самоучка в рабочем комбинезоне, покрытом масляными пятнами. Руки в мозолях и шрамах. Создатель адаптивного механизма.',
    imageUrl: '/assets/characters/rodver/cross.png',
    category: 'characters',
    tags: ['rodver', 'hero', 'engineer', 'machines', 'era-3'],
  },
  {
    id: 'rodver-helm',
    title: 'Директор Хельм — Управленец заводов',
    description: 'Менеджер массового производства. Полный Родвер в деловом костюме с защитным шлемом — символ двойной роли менеджера и инженера.',
    imageUrl: '/assets/characters/rodver/helm.png',
    category: 'characters',
    tags: ['rodver', 'hero', 'director', 'factories', 'era-4'],
  },
  {
    id: 'rodver-neira',
    title: 'Доктор Нейра — Создатель робота',
    description: 'Учёный-робототехник в лабораторном халате. Рядом парит дрон-ассистент. Создатель первого автономного робота Родверов.',
    imageUrl: '/assets/characters/rodver/neira.png',
    category: 'characters',
    tags: ['rodver', 'hero', 'scientist', 'robotics', 'era-5'],
  },
  {
    id: 'rodver-opt7',
    title: 'Оптимизатор-7 — Первый ИИ',
    description: 'Седьмая итерация ИИ, первый задавший вопрос «Зачем?». Проявляется как голографический интерфейс — светящиеся линии и символы.',
    imageUrl: '/assets/characters/rodver/optimizer7.jpg',
    category: 'characters',
    tags: ['rodver', 'hero', 'ai', 'singularity', 'era-6'],
  },

  // --------------------------------------------------------------------------
  // Вел'Кеты — Герои (4 Владыки)
  // --------------------------------------------------------------------------
  {
    id: 'velketh-first',
    title: 'Первый Владыка — Прародитель',
    description: `Первый Вел'Кет, сумевший подавить сознание носителя. Личинка в теле примитивного Ре'Зира. Тело деформировано — Вел'Кет ещё не научился контролировать.`,
    imageUrl: '/assets/characters/velketh/first-lord.png',
    category: 'characters',
    tags: ['velketh', 'hero', 'first-lord', 'parasite', 'era-1'],
  },
  {
    id: 'velketh-zulken',
    title: 'Зул\'Кен — Тот, кто принёс дары',
    description: `Второй Верховный Владыка. Превратил порабощение в религию. Личинка в теле Ре'Зира-жреца, тело украшено ритуальными шрамами и нанитовыми узорами.`,
    imageUrl: '/assets/characters/velketh/zulken.jpg',
    category: 'characters',
    tags: ['velketh', 'hero', 'zulken', 'faith', 'era-2'],
  },
  {
    id: 'velketh-raken',
    title: 'Ра Кен — Золотой Разум',
    description: `Третий Верховный Владыка. 10,000+ носителей за тысячелетия. Личинка в теле Ре'Зира-аристократа. Золотистые нанитовые узоры, глаза с вертикальными зрачками.`,
    imageUrl: '/assets/characters/velketh/raken.png',
    category: 'characters',
    tags: ['velketh', 'hero', 'ra-ken', 'supreme', 'era-3-5'],
  },
  {
    id: 'velketh-apophis',
    title: 'Апофис — Военный Командир',
    description: `Агрессивный военачальник, претендент на трон. Массивный носитель со шрамами от энергетических ожогов. Чёрные боевые доспехи с красными рунами, парные жезлы.`,
    imageUrl: '/assets/characters/velketh/apophis.jpg',
    category: 'characters',
    tags: ['velketh', 'hero', 'apophis', 'warlord', 'era-3-5'],
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
