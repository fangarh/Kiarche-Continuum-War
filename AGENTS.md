# Repository Guidelines

## Project Structure & Module Organization
`KiarcheContinuumWar/` contains the Unity 6 game project. Core gameplay code lives under `Assets/Scripts/` and is split by domain: `Units/`, `Pathfinding/`, `Map/`, `Pooling/`, `Managers/`, `UI/`, and editor automation in `Editor/`. Main scenes are in `Assets/Scenes/`, prefabs in `Assets/Prefabs/`, and project-wide settings in `ProjectSettings/`.

`documentation/` stores design notes, lore, and devlogs. `documentation/kcwweb/` is a separate React + TypeScript + Vite app for project docs, with source in `src/`, static assets in `public/`, and page data in `src/data/`.

## Build, Test, and Development Commands
Unity uses editor workflows rather than a CLI build script in this repo.

- Open `KiarcheContinuumWar/` in Unity `6000.4.0f1`.
- Run the prototype by opening `Assets/Scenes/MVP_Prototype.unity` and pressing Play.
- Use `Tools > KCW > Generate MVP Scene` or `Generate Test Map` to rebuild editor-generated content.

For the docs app, run from `documentation/kcwweb/`:

- `npm install` installs dependencies.
- `npm run dev` starts the Vite dev server.
- `npm run build` runs TypeScript build checks and produces a production bundle.
- `npm run lint` runs ESLint.
- `npm run preview` serves the built site locally.

## Coding Style & Naming Conventions
C# uses 4-space indentation, PascalCase for public types/members, camelCase for serialized private fields, and leading underscores for private runtime fields such as `_pathfinder`. Keep namespaces under `KiarcheContinuumWar.*` and place scripts in the matching feature folder.

TypeScript/React files also use 4-space indentation in practice, PascalCase component names, and colocated `.css` files such as `Home.tsx` and `Home.css`.

## Testing Guidelines
There is no automated test suite checked in yet. Validate gameplay changes in Unity by exercising `MVP_Prototype.unity` and `TestMap.unity`, including the existing debug shortcuts (`T`, `P`, `R`) documented in `Assets/Scripts/README.md`. For the docs app, treat `npm run build` and `npm run lint` as the minimum verification before opening a PR.

## Agent Instructions
For this repository, always use Context7 for Unity questions before answering from memory. This applies to Unity API usage, editor workflows, package setup, lifecycle methods, Input System, URP, and version-specific behavior.

Use this sequence:
- `resolve-library-id` with `Unity` and the user's full question
- prefer `/websites/unity_en-us` for general Unity docs, or a more specific Unity package ID when clearly relevant
- `query-docs` with the full user question, then answer from the returned documentation

## Commit & Pull Request Guidelines
Recent history mixes short English and Russian subjects, but the useful pattern is imperative, scope-first summaries such as `Add movement` or `Fix menu layout`. Avoid placeholder subjects like `123`.

PRs should include:
- a short description of the gameplay, tooling, or docs change
- linked issue or task ID when available
- screenshots or short clips for scene, UI, or docs-page changes
- note of how the change was verified (`Unity playtest`, `npm run build`, `npm run lint`)
