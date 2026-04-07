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

## Operational Extensions
- `memory/agent_profiles.md` is the authoritative operational playbook for agent roles, routing, iteration flow, and task handoff usage.
- `memory/task_templates.md` is the authoritative template source for `Lite`, `Standard`, `Exploration`, and `Handoff` task formats.
- `memory/vps_deploy_workflow.md` is the authoritative deployment playbook for preparing and shipping the docs site and landing to a personal VPS.
- Additional workflow-specific files in `memory/` may extend this system, but they must be explicitly referenced here before they are treated as normative.
- If there is any conflict, `AGENTS.md` defines the top-level policy and the referenced `memory/` files define the operational detail.

## Source Of Truth
- `AGENTS.md` is the top-level policy source of truth for repository behavior.
- Referenced files in `memory/` are the source of truth for operational playbooks, templates, and extended workflows.
- Files in `agents/` define role contracts for orchestrator-style agent execution.
- Files in `system/state/` are the authoritative runtime state for active and completed task execution.
- Files in `system/logs/` store execution logs and are not the canonical task state.
- Files in `system/outputs/` store agent results and artifacts and are not the canonical task state.

## Multi-Agent Orchestration
- Planning is mandatory before implementation, even if the task initially looks small.
- Start every task with a short triage: goal, affected area, expected verification, dependency/risk check, and whether delegation is justified.
- Default behavior: keep the critical path local, delegate only bounded side tasks or clearly separable implementation slices.
- Spawn sub-agents when the task naturally splits by project area, verification track, or documentation/research stream.
- Prefer **parallel** sub-agents when work has disjoint ownership and does not block the immediate next local step.
- Prefer **sequential** sub-agents when the next step depends on the result of the previous one, such as exploration -> implementation -> verification.
- Reuse the same agent when follow-up work stays in the same context; avoid spawning duplicate agents for the same unresolved thread.

### Planning Gate
- Before coding or editing content, classify the task as `small`, `multi-step`, or `uncertain`.
- For `small` tasks: write a short local plan and proceed.
- For `multi-step` tasks: create an explicit staged plan before implementation.
- For `uncertain` tasks: investigate first, then convert findings into a concrete plan.
- If planning reveals cross-domain impact, switch from local execution to multi-agent orchestration.
- If verification is unclear, planning is incomplete and implementation should not start yet.

### Context Isolation Standard
- Every task must receive a stable `task_id` before delegation or multi-step execution.
- Every task must define a `context_scope`: owned folders/files, excluded areas, and the agent responsible for each owned area.
- Every task must start with a `context_snapshot`: goal, current facts, constraints, verification method, and open questions.
- Every handoff between agents must use a `handoff contract`: completed work, incomplete work, changed files, verification status, and remaining risks.
- Every rework cycle must increment a `context_version` so agents know they are working from updated state, not the original prompt.
- Do not pass full conversational history to workers when a scoped snapshot is sufficient.
- If two agents need the same file, stop and re-plan ownership before execution.
- Use a modular template set rather than one rigid task form: `Lite`, `Standard`, `Exploration`, and `Handoff`.

### Recommended Agent Roles
- **Unity gameplay agent**: `KiarcheContinuumWar/Assets/Scripts/Units/`, `Managers/`, `Core/`, combat logic, unit behaviors, scene gameplay wiring.
- **Unity map/pathfinding agent**: `KiarcheContinuumWar/Assets/Scripts/Map/`, `Pathfinding/`, `Pooling/`, grid, flow fields, navigation, performance-sensitive systems.
- **Unity UI/input/camera agent**: `KiarcheContinuumWar/Assets/Scripts/UI/`, `InputSystem/`, `CameraSystem/`, player interaction, HUD, camera controls.
- **Web docs agent**: `documentation/kcwweb/src/`, React/Vite pages, components, typed data, styling, markdown rendering.
- **Lore/docs agent**: `documentation/`, `ovault/`, markdown content, worldbuilding consistency, structured documentation updates.
- **Git agent**: branch hygiene, diff inspection, commit preparation, change grouping, conflict awareness, and safe repository operations.
- **Dev-Ops agent**: deployment packaging, VPS-ready scripts, environment templates, build/run instructions, and reproducible website/landing rollout flow.
- **Review/verification agent**: focused read-only pass for regressions, missing tests, risky assumptions, or build/lint validation follow-up.

### Permanent Agent Profiles
- **Agent A — Unity Gameplay**: default worker for gameplay features, units, managers, combat, and core runtime logic.
- **Agent B — Unity Systems**: worker for map generation, pathfinding, pooling, performance-sensitive systems, and simulation infrastructure.
- **Agent C — Unity UX**: worker for UI, player input, camera behavior, and in-scene interaction polish.
- **Agent D — Web Docs**: worker for `documentation/kcwweb/`, React pages/components/styles, typed content wiring, and docs-site UX.
- **Agent E — Lore And Writing**: worker for `documentation/` and `ovault/`, lore consistency, markdown restructuring, and narrative documentation.
- **Agent F — Git**: worker for git hygiene, commit staging strategy, change auditing, branch readiness, and safe release preparation.
- **Agent G — Dev-Ops**: worker for VPS deployment assets, scripts, env examples, deploy instructions, and one-command rollout preparation for site and landing.
- **Agent H — Review And Verification**: explorer or reviewer for regression checks, risk scanning, test/build/lint follow-up, and implementation review.

### Delegation Rules By Task Type
- **Unity-only feature**: use one focused Unity agent; add a second verification agent only if the change is broad or risky.
- **Web-only feature**: use one web docs agent; add a review agent for build/lint follow-up if needed.
- **Deployment preparation**: use the web docs agent for app changes and the Dev-Ops agent for packaging and VPS rollout assets.
- **Commit/release preparation**: use the Git agent after implementation when changes need clean grouping, reviewable diffs, or release-ready handoff.
- **Cross-project feature**: run Unity and web agents in parallel when codebases do not overlap, then integrate results locally.
- **Docs + implementation**: run implementation and lore/docs agents in parallel if documentation can be updated independently.
- **Large refactor**: first send an explorer/research pass, then spawn workers by disjoint file ownership, then run a final review pass.
- **Bug investigation**: start sequentially with one explorer or local analysis; only fan out after the failing subsystem is isolated.

### Platform Baseline
- Development workstation baseline: Windows 11.
- Production VPS baseline: Ubuntu 24.
- Repository synchronization baseline: Git-based workflow between local development and VPS.
- For website or landing deployment, Ubuntu VPS is the canonical execution target.
- Windows may be used for local build verification, but it is not the primary production deploy path unless explicitly requested.

### Ownership And Safety
- Every spawned worker must receive explicit file or folder ownership.
- Workers must be told they are not alone in the codebase and must not revert unrelated edits.
- Do not delegate destructive git operations.
- Do not delegate broad exploratory work without a concrete question or output.
- If the task is small, urgent, or tightly coupled, handle it locally instead of spawning.

### Iteration Policy
- Use a verification loop when work includes testing, review, or user-reported bug reproduction.
- Standard loop: `implement -> test/review -> fix -> re-test`.
- If a defect is found, send it back to the responsible domain agent for another iteration.
- If the same task reaches more than **3 iterations**, stop the autonomous loop and discuss the problem with the user before continuing.
- After the third failed iteration, summarize what was tried, what failed, and which assumptions or constraints are blocking progress.

## Commit & Pull Request Guidelines
- **Commits**: Imperative, scope-first subjects (e.g., `Add movement`, `Fix menu layout`). Avoid placeholders like `123`.
- **Branches**: `feature/*`, `fix/*`, `docs/*`.
- **PRs must include**: short description of the change, linked issue/task ID when available, screenshots/clips for scene/UI/docs-page changes, and verification method (`Unity playtest`, `npm run build`, `npm run lint`).
