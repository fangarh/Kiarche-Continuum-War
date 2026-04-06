# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

**Kiarche Continuum War** — научно-фантастическая RTS-стратегия с героями, разрабатываемая для Early Access. Проект включает Unity-игру и документационный веб-портал.

## Repository Structure

```
KiarcheContinuumWar/     # Unity game project (Universal Render Pipeline)
├── Assets/              # Game assets, scenes, settings
├── Packages/            # Unity package dependencies
├── ProjectSettings/     # Unity project configuration
└── KiarcheContinuumWar.sln

documentation/
├── kcwweb/              # React documentation portal (see kcwweb section below)
├── memory/              # Session memories, lore documents
├── concept/             # Concept art and videos
└── CLAUDE.md            # Detailed web app guidance (supplements this file)
```

## Unity Game Project (KiarcheContinuumWar/)

**Unity 2022.x** with Universal Render Pipeline 17.4.0.

Key packages: 2D Animation, Input System, Visual Scripting, Timeline, Multiplayer Center.

Commands are run through the Unity Editor. No CLI build commands are configured.

### Faction Names
- Architects → Синтекс
- Goa'улды → Вел'Кеты
- Асгарды → Кешари
- Ноксы → Сил'Ни
- Древние → Этерны

## Web Documentation Portal (kcwweb/)

React 19 + TypeScript 5.9 + Vite 8 + React Router 7.

Commands run from `documentation/kcwweb/`:

```bash
npm install       # Install dependencies
npm run dev       # Start dev server with HMR
npm run build     # Production build (tsc -b && vite build)
npm run preview   # Preview production build
npm run lint      # Run ESLint
```

### Architecture

**Data-driven**: All content lives in typed data files under `src/data/`. Components consume data, not hardcode it.

```
src/components/
├── layout/     # Header, Footer, Layout
└── ui/         # Button, Card, Modal, Gallery, Section

src/pages/      # Route pages with co-located CSS
src/data/       # Typed content: home.ts, lore.ts, mechanics.ts, etc.
src/types/      # TypeScript interfaces
```

### Path Aliases

| Alias | Path |
|-------|------|
| `@/` | `src/` |
| `@components/` | `src/components/` |
| `@pages/` | `src/pages/` |
| `@data/` | `src/data/` |
| `@types/` | `src/types/` |

### Key Data Files

| File | Content |
|------|---------|
| `lore.ts` | Worlds, factions, characters |
| `mechanics.ts` | Core loop, combat, progression |
| `architecture.ts` | Game project modules |
| `roadmap.ts` | Development stages |
| `devlog.ts` | Development history |
| `home.ts` | Hero section, concept art |

### Lore Source of Truth

Primary lore document: `documentation/memory/stargates_lore_discussion.md`

## Language

Respond in Russian (per `memory/user_language.md`).
