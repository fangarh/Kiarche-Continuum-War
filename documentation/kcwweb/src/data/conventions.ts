
// ----------------------------------------------------------------------------
// Конвенции и правила проекта Kiarche Continuum War
// ----------------------------------------------------------------------------
//
// Здесь документируются все соглашения, правила именования,
// лор-решения и прочие условности проекта.
//
// ----------------------------------------------------------------------------

export interface ConventionCategory {
  id: string;
  name: string;
  description: string;
  items: ConventionItem[];
}

export interface ConventionItem {
  id: string;
  title: string;
  status: 'active' | 'deprecated' | 'proposed';
  description: string;
  examples?: string[];
  rationale?: string;
}

export interface ConventionsData {
  categories: ConventionCategory[];
  lastUpdated: string;
}

export const conventionsData: ConventionsData = {
  lastUpdated: '2026-03-27',

  categories: [
    {
      id: 'naming-factions',
      name: 'Именование рас',
      description: 'Правила именования фракций и рас во избежание авторских прав',
      items: [
        {
          id: 'naming-factions-001',
          title: 'Запрещённые имена',
          status: 'active',
          description: 'Категорически запрещено использовать имена из вселенной Stargate SG-1 и подобных.',
          examples: [
            'Гоа\'улды → Вел\'Кеты',
            'Асгарды → Кешари',
            'Ноксы → Сил\'Ни',
            'Древние → Этерны',
            'Architects → Синтекс',
          ],
          rationale: 'Избежание авторских прав и судебных претензий.',
        },
        {
          id: 'naming-factions-002',
          title: 'Ки\'Архе и Тал\'Син',
          status: 'active',
          description: 'Ки\'Архе и Тал\'Син являются оригинальными разработками и могут использоваться без ограничений.',
          rationale: 'Эти имена созданы специально для проекта.',
        },
      ],
    },

    {
      id: 'lore-principles',
      name: 'Принципы лора',
      description: 'Фундаментальные правила построения вселенной',
      items: [
        {
          id: 'lore-001',
          title: 'Portal Network',
          status: 'active',
          description: 'Сеть порталов, связывающая миры. Создана Синтекс. Не путать с "Звёздными вратами" — это другая вселенная.',
          examples: [
            'Portal Network — наша технология',
            'Звёздные врата — чужая торговная марка',
          ],
        },
        {
          id: 'lore-002',
          title: 'Идеология Ки\'Архе',
          status: 'active',
          description: '"Превосходство непрерывности" — ценность имеет не жизнь, а непрерывность структуры и информации.',
        },
        {
          id: 'lore-003',
          title: 'Три состояния после смерти для Тал\'Син',
          status: 'active',
          description: 'Чистая интеграция, Фрагментированная запись, Эхо (аномалия).',
        },
      ],
    },

    {
      id: 'game-mechanics',
      name: 'Игровые механики',
      description: 'Ключевые игровые концепции',
      items: [
        {
          id: 'mech-001',
          title: 'Hero Units',
          status: 'active',
          description: 'Герои — специальные юниты с уникальными способностями, прокачкой и привязкой к фракции.',
        },
        {
          id: 'mech-002',
          title: 'Две кампании',
          status: 'active',
          description: 'Кампания молодых рас (классическая 4X) и кампания Тал\'Син (уникальная механика идентичности).',
        },
        {
          id: 'mech-003',
          title: 'Этапы развития Тал\'Син',
          status: 'active',
          description: 'I. Зарождение → II. Формирование резонанса → III. Сетевое общество → IV. Управляемая эволюция → V. Частичная дематериализация',
        },
      ],
    },

    {
      id: 'code-style',
      name: 'Стиль кода',
      description: 'Соглашения по написанию кода',
      items: [
        {
          id: 'code-001',
          title: 'Именование файлов',
          status: 'active',
          description: 'Файлы компонентов — PascalCase (Home.tsx). Файлы данных — camelCase (homeData.ts).',
          examples: ['Home.tsx, Lore.tsx, homeData.ts, loreData.ts'],
        },
        {
          id: 'code-002',
          title: 'Экспорт компонентов',
          status: 'active',
          description: 'Компоненты экспортируются как именованные экспорты (export function HomePage).',
        },
        {
          id: 'code-003',
          title: 'CSS модули',
          status: 'active',
          description: 'Стили компонента хранятся в одноимённом .css файле рядом с компонентом.',
          examples: ['Home.tsx + Home.css'],
        },
      ],
    },

    {
      id: 'git-workflow',
      name: 'Git workflow',
      description: 'Правила работы с Git',
      items: [
        {
          id: 'git-001',
          title: 'Сообщения коммитов',
          status: 'active',
          description: 'Использовать русский язык для коммитов. Формат: [Категория] Описание.',
          examples: [
            '[Lore] Добавлен лор для Кешари',
            '[UI] Исправлены стили кнопок',
            '[Docs] Обновлена документация',
          ],
        },
        {
          id: 'git-002',
          title: 'Ветки',
          status: 'active',
          description: 'feature/* для новых фич, fix/* для исправлений, docs/* для документации.',
        },
      ],
    },

    {
      id: 'content-rules',
      name: 'Правила контента',
      description: 'Соглашения по созданию игрового контента',
      items: [
        {
          id: 'content-001',
          title: 'Проверка авторских прав',
          status: 'active',
          description: 'Перед добавлением новых имён рас, персонажей, локаций — проверить на сходство с существующими IP.',
          rationale: 'Проект должен быть полностью оригинальным.',
        },
        {
          id: 'content-002',
          title: 'Миры',
          status: 'proposed',
          description: 'Миры должны иметь уникальные имена, не совпадающие с реальной топонимикой.',
        },
      ],
    },
  ],
};
