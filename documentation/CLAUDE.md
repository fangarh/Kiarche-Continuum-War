# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

StarGatesTheGame is a sci-fi adventure RPG with a documentation web application (`kcwweb/`) that serves as an information portal for the game.

## Tech Stack

- **Framework:** React 19.2.4 with React Compiler
- **Language:** TypeScript 5.9.3
- **Bundler:** Vite 8.0.1
- **Routing:** React Router 7
- **Linting:** ESLint 9

## Common Commands

All commands run from the `sgdocs/` directory:

```bash
npm install       # Install dependencies
npm run dev       # Start dev server with HMR
npm run build     # Production build (tsc -b && vite build)
npm run preview   # Preview production build
npm run lint      # Run ESLint
```

## Architecture

### Data-Driven Architecture

All content is stored in typed data files under `src/data/`. Components consume this data rather than containing it. Data files include: `navigation.ts`, `home.ts`, `overview.ts`, `lore.ts`, `mechanics.ts`, `architecture.ts`, `documentation.ts`, `roadmap.ts`, `devlog.ts`, `team.ts`. These are aggregated in `src/data/index.ts`.

### Component Structure

```
src/components/
├── layout/     # Header, Footer, Layout (wraps all pages)
└── ui/        # Reusable: Button, Card, Modal, Gallery, Section
```

### Page Structure

Each page has a co-located CSS file (e.g., `Home.tsx` + `Home.css`). Pages are exported from `src/pages/index.ts`.

### Path Aliases

The following aliases are configured in both `vite.config.ts` and `tsconfig.app.json`:

| Alias | Path |
|-------|------|
| `@/` | `src/` |
| `@components/` | `src/components/` |
| `@pages/` | `src/pages/` |
| `@data/` | `src/data/` |
| `@types/` | `src/types/` |
| `@hooks/` | `src/hooks/` |
| `@styles/` | `src/styles/` |

### Routing

Routes are defined in `src/App.tsx` using React Router 7. The `Layout` component wraps all routes.

### TypeScript Configuration

Project uses solution-style tsconfig with references:
- `tsconfig.app.json` — for the application (strict mode, `noUnusedLocals`, `noUnusedParameters`)
- `tsconfig.node.json` — for Node files (vite.config.ts, etc.)

## Pages

| Route | Page | Description |
|-------|------|-------------|
| `/` | Home | Hero, game preview, concept art, quick links |
| `/overview` | Overview | Genre, platforms, features |
| `/lore` | Lore | Worlds, history, factions, characters |
| `/mechanics` | Mechanics | Core loop, combat, progression, economy |
| `/architecture` | Architecture | Project modules, data flows |
| `/docs` | Docs | Developer guide, specifications |
| `/roadmap` | Roadmap | Development stages, milestones |
| `/devlog` | Devlog | Development history, decisions |
| `/team` | Team | Team members, roles |
