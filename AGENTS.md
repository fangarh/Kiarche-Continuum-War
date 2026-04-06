# Repository Guidelines

## Project Structure & Module Organization
This repo contains **two projects**:

**Unity 6 Game** — `KiarcheContinuumWar/`
- Core gameplay code: `Assets/Scripts/` split by domain: `Units/`, `Pathfinding/`, `Map/`, `Pooling/`, `Managers/`, `UI/`, `CameraSystem/`, `InputSystem/`, `Core/`, and editor automation in `Editor/`.
- Scenes: `Assets/Scenes/` (main working scenes: `MVP_Prototype.unity`, `TestMap.unity`; auxiliary: `SampleScene.unity`). Prefabs: `Assets/Prefabs/`. Settings: `ProjectSettings/`.
- No `.asmdef` files — relies on default Unity assembly compilation.

**React Docs Web App** — `documentation/kcwweb/`
- React 19 + TypeScript 5.9 + Vite 8 + React Router 7 SPA.
- Source: `src/` with `components/`, `pages/`, `data/`, `types/`, `styles/`. Path aliases also reserve `src/hooks/`, but that directory may be absent until needed.
- Data-driven: all content lives in typed `src/data/` files, never hardcoded in components.
- Path aliases: `@/`, `@components/`, `@pages/`, `@data/`, `@types/`, `@hooks/`, `@styles/`.
- Markdown docs rendering uses `react-markdown` + `remark-gfm`; keep markdown content/config data-driven rather than embedding large static HTML in components.

## Build, Test, and Development Commands

### Unity (editor workflows)
- Open `KiarcheContinuumWar/` in Unity `6000.4.0f1`.
- Play `Assets/Scenes/MVP_Prototype.unity` to test.
- Debug shortcuts: `T` (toggle), `P` (pathfinding), `R` (reset) — see `Assets/Scripts/README.md`.
- Editor tools: `Tools > KCW > Create Unit Prefab`, `Generate MVP Scene`, `Generate Test Map`.

### Docs Web App (run from `documentation/kcwweb/`)
```bash
npm install          # Install dependencies
npm run dev          # Start Vite dev server (HMR)
npm run build        # TypeScript check + production bundle (tsc -b && vite build)
npm run lint         # Run ESLint (eslint .)
npm run preview      # Serve production build locally
```
Treat `npm run build` and `npm run lint` as the minimum verification before opening a PR. There is no automated test suite — `npm run build` serves as the type-check gate.

## Coding Style & Conventions

### C# (Unity)
- 4-space indentation. No `.editorconfig` — follow existing files.
- **Naming**: PascalCase for public types/members, camelCase for serialized private fields, `_camelCase` with leading underscore for private runtime fields (e.g., `_pathfinder`).
- **Namespaces**: `KiarcheContinuumWar.*` matching feature folder.
- **Organization**: One class per file, file name matches class name. Place scripts in matching domain folder.
- **Unity patterns**: Use `[SerializeField] private` for inspector-exposed fields. Keep `[RequireComponent]` and `[DisallowMultipleComponent]` where applicable.
- **Error handling**: Use `Debug.LogError` / `Debug.LogWarning` for runtime issues. Guard null references early with `if (x == null) return`.

### TypeScript/React
- 4-space indentation. Strict TypeScript: `noUnusedLocals`, `noUnusedParameters`, `erasableSyntaxOnly`, `noFallthroughCasesInSwitch`.
- **Naming**: PascalCase for components and types (`HomePage`, `UnitData`), camelCase for variables/functions, `camelCase.ts` for data files.
- **Components**: Named exports (`export function HomePage`). Each page has a co-located `.css` file.
- **Imports**: Use path aliases (`@components/`, `@data/`, etc.) — never relative `../../` chains across directories.
- **Aliases**: Keep `vite.config.ts` and `tsconfig.app.json` path aliases synchronized when adding, renaming, or removing aliases/directories.
- **CSS**: Co-located `.css` files per page/component. Use CSS variables for theming (dark theme, purple accents, mobile-first responsive).
- **Data**: All content in `src/data/` typed via `src/types/index.ts`. Components consume data, never embed it.
- **React toolchain**: Preserve the existing React Compiler setup (`@vitejs/plugin-react` plus `@rolldown/plugin-babel` with `reactCompilerPreset`) unless the task explicitly requires changing build tooling.
- **Error handling**: Use React error boundaries where appropriate. Log with `console.error` for unexpected states.

## Testing Guidelines
No automated test suite exists. Validate changes manually:
- **Unity**: Playtest in `MVP_Prototype.unity` and `TestMap.unity`. Use debug shortcuts.
- **Web App**: Run `npm run build` (type checks) and `npm run lint` before committing. Visually verify pages via `npm run dev`.

## Agent Instructions
- **Always use Context7 for Unity questions** before answering from memory. This applies to Unity API usage, editor workflows, package setup, lifecycle methods, Input System, URP, and version-specific behavior.
  1. `resolve-library-id` with `Unity` and the user's full question
  2. Prefer `/websites/unity_en-us` for general docs, or a specific Unity package ID
  3. `query-docs` with the full question, then answer from returned documentation
- **Also use Context7 for web stack/library questions** in this repo: React, Vite, React Router, TypeScript, ESLint, `react-markdown`, and related tooling.
- **Language**: Respond in Russian (per `documentation/memory/user_language.md`).

## Commit & Pull Request Guidelines
- **Commits**: Imperative, scope-first subjects (e.g., `Add movement`, `Fix menu layout`). Avoid placeholders like `123`.
- **Branches**: `feature/*`, `fix/*`, `docs/*`.
- **PRs must include**: short description of the change, linked issue/task ID when available, screenshots/clips for scene/UI/docs-page changes, and verification method (`Unity playtest`, `npm run build`, `npm run lint`).
