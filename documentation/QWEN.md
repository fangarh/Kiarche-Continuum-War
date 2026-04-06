# QWEN.md — Контекст проекта Kiarche Continuum War

## Обзор проекта

**Kiarche Continuum War** — научно-фантастическая RTS-стратегия с героями, разрабатываемая для релиза в Early Access. Проект включает документационный веб-портал (`kcwweb/`), служащий информационным центром об игре.

### Сеттинг
Игра повествует о молодых расах, начинающих путешествовать через порталы и развиваться от железных мечей до огнестрела и заводов. Ключевые фракции включают Ки'Архе (распределённое поле сознания), Тал'Син (их последователи с механикой идентичности), а также различные молодые расы (Ре'Зиры, Ти'Оны, Ве'Ори, Родверы).

---

## Структура репозитория

```
documentation/
├── kcwweb/                 # Веб-приложение документации (React + TypeScript + Vite)
│   ├── src/
│   │   ├── components/     # UI компоненты и layout
│   │   ├── data/           # Типизированные данные проекта
│   │   ├── pages/          # Страницы приложения
│   │   ├── styles/         # Глобальные стили
│   │   ├── types/          # TypeScript типы
│   │   ├── App.tsx         # Роутинг
│   │   └── main.tsx        # Точка входа
│   ├── public/assets/      # Статические ресурсы
│   └── package.json
├── concept/                # Концепт-арты и видео
│   ├── b369d202-detail.jpg
│   └── *.mp4 (анимации)
├── memory/                 # Память сессий и лор-документы
│   ├── MEMORY.md
│   ├── user_language.md
│   ├── young_races_gdd.md
│   └── stargates_lore_discussion.md
├── CLAUDE.md               # Инструкции для Claude Code
└── LICENSE                 # Apache License 2.0
```

---

## Технологии kcwweb

| Компонент | Версия |
|-----------|--------|
| **Framework** | React 19.2.4 с React Compiler |
| **Language** | TypeScript 5.9.3 |
| **Bundler** | Vite 8.0.1 |
| **Routing** | React Router 7 |
| **Linting** | ESLint 9 |

### Дополнительные зависимости
- `react-markdown` + `remark-gfm` — рендеринг Markdown
- `babel-plugin-react-compiler` — оптимизация React

---

## Команды сборки и запуска

Все команды выполняются из директории `kcwweb/`:

```bash
# Установка зависимостей
npm install

# Запуск dev-сервера (HMR)
npm run dev

# Сборка для продакшена
npm run build        # tsc -b && vite build

# Предпросмотр сборки
npm run preview

# Линтинг
npm run lint         # eslint .
```

---

## Архитектура приложения

### Data-Driven подход
Весь контент хранится в типизированных файлах данных под `src/data/`:

| Файл | Описание |
|------|----------|
| `navigation.ts` | Навигация по сайту |
| `home.ts` | Главная страница (hero, концепт-арты) |
| `overview.ts` | Описание игры, жанр, особенности |
| `lore.ts` | Лор: миры, история, фракции, персонажи |
| `mechanics.ts` | Игровые механики: core loop, бой, прогрессия |
| `architecture.ts` | Архитектура проекта для разработчиков |
| `conventions.ts` | Конвенции разработки |
| `documentation.ts` | Руководства, AI инструкции |
| `roadmap.ts` | Roadmap разработки (2026-2029) |
| `devlog.ts` | История разработки, решения |
| `team.ts` | Команда проекта |
| `index.ts` | Агрегатор всех данных |

### Компоненты
```
src/components/
├── layout/     # Header, Footer, Layout (обёртка страниц)
└── ui/         # Button, Card, Modal, Gallery, Section
```

### Страницы
| Страница | Путь | Файл |
|----------|------|------|
| Главная | `/` | `Home.tsx` |
| Об игре | `/overview` | `Overview.tsx` |
| Лор | `/lore` | `Lore.tsx` |
| Механики | `/mechanics` | `Mechanics.tsx` |
| Архитектура | `/architecture` | `Architecture.tsx` |
| Конвенции | `/conventions` | `Conventions.tsx` |
| Документация | `/docs` | `Docs.tsx` |
| Roadmap | `/roadmap` | `Roadmap.tsx` |
| Devlog | `/devlog` | `Devlog.tsx` |
| Команда | `/team` | `Team.tsx` |

Роутинг определён в `src/App.tsx`. Все страницы экспортируются из `src/pages/index.ts`.

### Path Aliases
Настроены в `vite.config.ts` и `tsconfig.app.json`:

| Алиас | Путь |
|-------|------|
| `@/` | `src/` |
| `@components/` | `src/components/` |
| `@pages/` | `src/pages/` |
| `@data/` | `src/data/` |
| `@types/` | `src/types/` |
| `@hooks/` | `src/hooks/` |
| `@styles/` | `src/styles/` |

### TypeScript конфигурация
Проект использует solution-style tsconfig:
- `tsconfig.json` — корневой файл с references
- `tsconfig.app.json` — приложение (строгий режим, `noUnusedLocals`, `noUnusedParameters`)
- `tsconfig.node.json` — Node-файлы (vite.config.ts и др.)

---

## Ключевые файлы для контекста

### Лор и механики
- `memory/stargates_lore_discussion.md` — **основной документ с лором**: Ки'Архе, Тал'Син, механика идентичности, этапы развития
- `memory/young_races_gdd.md` — проработка 4 молодых рас (Ре'Зиры, Ти'Оны, Ве'Ори, Родверы)
- `kcwweb/src/data/lore.ts` — опубликованный лор на портале
- `kcwweb/src/data/mechanics.ts` — механики Тал'Син

### Конвенции
- `memory/user_language.md` — **всегда отвечать на русском языке**
- `CLAUDE.md` — инструкции для Claude Code

---

## Стили и дизайн

- **CSS Variables** для темизации
- **Адаптивный дизайн** (mobile-first)
- **Тёмная тема** с фиолетовыми акцентами
- Каждая страница имеет сопутствующий CSS-файл (например, `Home.tsx` + `Home.css`)

---

## Лицензия

Apache License 2.0 — см. `LICENSE`

---

## Важные заметки для работы с проектом

1. **Язык общения**: Всегда используй русский язык для ответов (см. `memory/user_language.md`).

2. **Data-driven архитектура**: Никогда не хардкодь контент в компонентах. Все данные должны быть в `src/data/`.

3. **Стили**: Каждая страница имеет свой CSS-файл. Избегай inline-стилей.

4. **Типизация**: Все данные типизированы. Проверяй `src/types/` перед добавлением новых структур.

5. **Лор**: При работе с лором сверяйся с `memory/stargates_lore_discussion.md` — это единственный источник истины.

6. **Имена фракций** (переименовано для избежания авторских прав):
   - Architects → Синтекс
   - Goa'улды → Вел'Кеты
   - Асгарды → Кешари
   - Ноксы → Сил'Ни
   - Древние → Этерны

## Qwen Added Memories
- Проект в директории documentation — это документация и презентация разрабатываемой игры Kiarche Continuum War (RTS-стратегия с геройскими и лорными слоями). Здесь также хранится todo-лист проекта.
