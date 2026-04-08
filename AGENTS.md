# Repository Guidelines

## Project Structure & Module Organization
This repo contains **two projects**:

**Unity 6 Game** — `KiarcheContinuumWar/`
- Core gameplay code: `Assets/Scripts/` split by domain: `Units/`, `Pathfinding/`, `Map/`, `Pooling/`, `Managers/`, `UI/`, `CameraSystem/`, `InputSystem/`, `Core/`, and editor automation in `Editor/`.
- Scenes: `Assets/Scenes/` (main working scenes: `MVP_Prototype.unity`, `TestMap.unity`; auxiliary: `SampleScene.unity`). Prefabs: `Assets/Prefabs/`. Settings: `ProjectSettings/`.
- Runtime and test boundaries may use targeted `.asmdef` files where the project needs isolated compilation or automated tests.

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
- Run EditMode tests: `powershell -ExecutionPolicy Bypass -File scripts\run-unity-editmode-tests.ps1`
- Run PlayMode tests: `powershell -ExecutionPolicy Bypass -File scripts\run-unity-editmode-tests.ps1 -TestPlatform PlayMode`
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
Treat `npm run build` and `npm run lint` as the minimum verification before opening a PR. If a web-code task changes behavior, the task must also add or update unit tests for the touched logic.

## Coding Style & Conventions

### C# (Unity)
- 4-space indentation. No `.editorconfig` — follow existing files.
- **Naming**: PascalCase for public types/members, camelCase for serialized private fields, `_camelCase` with leading underscore for private runtime fields (e.g., `_pathfinder`).
- **Namespaces**: `KiarcheContinuumWar.*` matching feature folder.
- **Organization**: One class per file, file name matches class name. Place scripts in matching domain folder.
- **Unity patterns**: Use `[SerializeField] private` for inspector-exposed fields. Keep `[RequireComponent]` and `[DisallowMultipleComponent]` where applicable.
- **Comments**: add concise comments for non-obvious logic, invariants, lifecycle assumptions, data contracts, and editor/runtime caveats. Do not add noise comments that merely restate the next line of code.
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
- **Comments**: add concise comments for non-obvious state flow, rendering assumptions, edge cases, data transformations, and integration contracts. Do not add line-by-line narration of obvious code.
- **Error handling**: Use React error boundaries where appropriate. Log with `console.error` for unexpected states.

## Testing Guidelines
Code changes must be covered by unit tests. This is mandatory for new features, bug fixes, refactors, and behavior-changing maintenance work.

- A coding task is not complete until relevant unit tests are added or updated and executed successfully.
- Missing unit tests for changed code are a workflow failure, not an optional follow-up.
- If the touched area does not yet have a usable unit-test harness, adding the minimal harness required to test that code becomes part of the same task.
- If no meaningful unit test can be written, treat that as a blocker to escalate explicitly; do not silently ship untested code.
- Non-code tasks may use other verification appropriate to scope, but they do not relax the unit-test requirement for code changes.
- **Unity**: run the relevant EditMode and/or PlayMode unit tests, then perform Unity MCP verification before considering the task complete.
- **Web App**: run unit tests for changed code in addition to `npm run build` and `npm run lint`; use browser verification when the change affects rendered UI, text, routing, or interactions.
- **Manual checks** support automated tests, but they do not replace required unit-test coverage for code changes.

## Documentation Guidelines
Code changes must also update documentation and required code comments. This is part of the same task, not follow-up polish.

- A code task is not complete until the relevant adjacent documentation is added or updated.
- The documentation target should be the closest durable source of truth for that code: feature README, module notes, architecture docs, runbook, or other repo-local reference used during future tasks.
- If behavior, contracts, configuration, assumptions, data flow, setup steps, or debugging workflow changed, the related documentation must change in the same task.
- If the touched area has no usable local documentation, creating the minimal adjacent documentation becomes part of the task.
- New or changed non-trivial code must include concise comments for intent, invariants, edge cases, integration points, and other information that is not obvious from the code alone.
- Comments should explain `why`, constraints, or caveats. They must not devolve into noisy line-by-line paraphrases.
- Non-code tasks may be `not_applicable` for code comments, but they do not waive the documentation requirement if the task changes operational knowledge or workflows.

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
- `memory/execution_flow.md` is the canonical short-form execution checklist for task progression from triage through verification and handoff.
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
- Start every task with a short triage: goal, affected area, expected verification, required unit-test scope, documentation scope, comment expectations, dependency/risk check, and whether delegation is justified.
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
- For `multi-step` and `uncertain` tasks, run one plan-review pass before implementation. The reviewer may approve the plan or return it once for refinement.
- Plan review is single-pass only: after one review-and-refinement cycle, either proceed with the revised plan or escalate open blockers to the user.
- The plan review pass may be skipped only for repeatable, low-ambiguity workflows where ownership, verification, and task decomposition already follow an established pattern and no new cross-domain risk is introduced.
- If planning reveals cross-domain impact, switch from local execution to multi-agent orchestration.
- If verification is unclear, planning is incomplete and implementation should not start yet.
- If a code task does not yet have a concrete unit-test plan, planning is incomplete and implementation should not start yet.
- If a code task does not yet have a concrete documentation/comment plan, planning is incomplete and implementation should not start yet.

### Context Isolation Standard
- Every task must receive a stable `task_id` before delegation or multi-step execution.
- Every task must define a `context_scope`: owned folders/files, excluded areas, and the agent responsible for each owned area.
- Every task must start with a `context_snapshot`: goal, current facts, constraints, verification method, unit-test expectations, documentation references, comment expectations, and open questions.
- Every handoff between agents must use a `handoff contract`: completed work, incomplete work, changed files, verification status, and remaining risks.
- Every rework cycle must increment a `context_version` so agents know they are working from updated state, not the original prompt.
- Do not pass full conversational history to workers when a scoped snapshot is sufficient.
- If two agents need the same file, stop and re-plan ownership before execution.
- Use a modular template set rather than one rigid task form: `Lite`, `Standard`, `Exploration`, and `Handoff`.

### Model And Reasoning Policy
- For implementation and development work, the default `reasoning_effort` is `high`.
- This default applies to local implementation on the critical path and to development-focused workers such as Agent A, Agent B, Agent C, Agent D, and Agent G.
- Repo-wide role defaults:
  - Agent A: `high`
  - Agent B: `high`
  - Agent C: `high`
  - Agent D: `high`
  - Agent E: `medium`
  - Agent F: `medium`
  - Agent G: `high`
  - Agent H: `high`
  - Agent I: `medium`
  - Agent J: `medium`
- Do not use `xhigh` as a default role level.
- Use `xhigh` only as an explicit override for rare cases such as a complex architectural refactor, a stubborn cross-domain defect, or a release-blocking investigation where `high` is not sufficient.
- If a task overrides the default model or reasoning level, record that override in the task template or handoff so the choice is visible to the next agent.

### Recommended Agent Roles
- **Unity gameplay agent**: `KiarcheContinuumWar/Assets/Scripts/Units/`, `Managers/`, `Core/`, combat logic, unit behaviors, scene gameplay wiring.
- **Unity map/pathfinding agent**: `KiarcheContinuumWar/Assets/Scripts/Map/`, `Pathfinding/`, `Pooling/`, grid, flow fields, navigation, performance-sensitive systems.
- **Unity UI/input/camera agent**: `KiarcheContinuumWar/Assets/Scripts/UI/`, `InputSystem/`, `CameraSystem/`, player interaction, HUD, camera controls.
- **Web docs agent**: `documentation/kcwweb/src/`, React/Vite pages, components, typed data, styling, markdown rendering.
- **Lore/docs agent**: `documentation/`, `ovault/`, markdown content, worldbuilding consistency, structured documentation updates.
- **Plan review agent**: read-only review of the generated plan, scope, verification logic, dependency assumptions, and delegation choices before implementation starts.
- **Git agent**: branch hygiene, diff inspection, commit preparation, change grouping, conflict awareness, and safe repository operations.
- **Dev-Ops agent**: deployment packaging, VPS-ready scripts, environment templates, build/run instructions, and reproducible website/landing rollout flow.
- **Browser/Playwright agent**: browser-level verification via MCP Playwright, page navigation, UI smoke tests, route checks, and regression confirmation for the docs site or landing.
- **Review/verification agent**: focused read-only pass for regressions, missing tests, risky assumptions, or build/lint validation follow-up.

### Permanent Agent Profiles
- **Agent A — Unity Gameplay**: default worker for gameplay features, units, managers, combat, and core runtime logic.
- **Agent B — Unity Systems**: worker for map generation, pathfinding, pooling, performance-sensitive systems, and simulation infrastructure.
- **Agent C — Unity UX**: worker for UI, player input, camera behavior, and in-scene interaction polish.
- **Agent D — Web Docs**: worker for `documentation/kcwweb/`, React pages/components/styles, typed content wiring, and docs-site UX.
- **Agent E — Lore And Writing**: worker for `documentation/` and `ovault/`, lore consistency, markdown restructuring, and narrative documentation.
- **Agent J — Plan Reviewer**: reviewer for plan quality before implementation, checking task decomposition, ownership boundaries, verification completeness, and risk coverage. This review is performed at most once per task.
- **Agent F — Git**: worker for git hygiene, commit staging strategy, change auditing, branch readiness, and safe release preparation.
- **Agent G — Dev-Ops**: worker for VPS deployment assets, scripts, env examples, deploy instructions, and one-command rollout preparation for site and landing.
- **Agent H — Review And Verification**: explorer or reviewer for regression checks, risk scanning, test/build/lint follow-up, and implementation review.
- **Agent I — Browser And Playwright**: worker for MCP Playwright verification, route walkthroughs, page-level smoke tests, interaction checks, and post-deploy browser validation.

### Delegation Rules By Task Type
- **Unity-only feature**: use one focused Unity agent; the task must add or update unit tests, adjacent documentation, and required code comments for changed code, then run those tests and a Unity MCP verification pass before considering the task done. Add a second verification agent if the change is broad or risky.
- **Web-only feature**: use one web docs agent; the task must add or update unit tests, adjacent documentation, and required code comments for changed code, and may add a review agent for build/lint follow-up if needed.
- **Web UI verification**: use the Browser/Playwright agent when correctness depends on rendered routes, interactions, modal flows, navigation, or post-deploy page checks.
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

### MCP Playwright Usage
- Use MCP Playwright for browser-level verification of `documentation/kcwweb` and landing flows when command-only verification is insufficient.
- Prefer MCP Playwright for route smoke tests, UI regression confirmation, modal/dialog behavior, navigation checks, and deploy validation through a real browser.
- Prefer command-based verification for build, lint, filesystem checks, and server/process health.
- When both are needed, run command verification first and browser verification second.

### Unity MCP Usage
- Use Unity MCP for post-implementation verification of every Unity code change in `KiarcheContinuumWar/` before the task can be considered complete.
- Unity verification should include the safest applicable mix of: console log inspection, scene/state inspection, Play Mode checks when relevant, and targeted runtime/editor command execution.
- Classify Unity MCP findings as `blocking`, `non-blocking`, or `informational`.
- `blocking` findings include failed MCP commands, compile/runtime errors, broken scene/runtime behavior, missing expected objects/components, or warnings that directly indicate the task result is not reliable.
- `non-blocking` findings include known editor noise, pre-existing warnings not caused by the task, or tool-side warnings that do not invalidate the feature under test.
- `informational` findings include observations worth recording without requiring rework.
- If Unity MCP verification finds a `blocking` issue, return the task to the responsible Unity agent for rework and increment `context_version`.
- Non-blocking and informational findings should be recorded in the verification summary but must not trigger automatic rework by themselves.
- Unity-only tasks should default to a verification loop of `implement -> Unity MCP verify -> fix -> Unity MCP re-verify`.
- If the same Unity task exceeds **3 full implementation/verification iterations**, stop the autonomous loop and ask the user for help with a short summary of attempts, remaining symptoms, and current blockers.

### Text Rendering Risk
- Any change to user-facing text, labels, headings, markdown content, portal content, landing content, or localized strings must not be treated as command-only verification.
- For web-visible text changes, verification must be at least `hybrid` and include browser-level confirmation.
- If a change could introduce unreadable symbols, broken encoding, invisible characters, or rendering anomalies, the task must be upgraded out of `Lite` if needed.
- Portal or landing text changes should default to MCP Playwright verification unless the user explicitly limits scope.

### Lore Session Mode
- Lore discussion and gradual worldbuilding updates may run in a session-oriented mode instead of spawning full orchestration after every micro-change.
- In Lore Session Mode, multiple small related lore edits are grouped into one session task with incrementing `context_version`.
- Agent spawning, consistency review, and commit preparation are deferred until a semantic checkpoint is reached.
- A checkpoint is typically a completed batch such as 2-3 related sections, one faction block, one era block, or another coherent lore package.
- Full verification and commit/release handling should happen at checkpoint time, not after every sentence-level adjustment.

### Ownership And Safety
- Every spawned worker must receive explicit file or folder ownership.
- Workers must be told they are not alone in the codebase and must not revert unrelated edits.
- Do not delegate destructive git operations.
- Do not delegate broad exploratory work without a concrete question or output.
- The plan review agent is read-only and must not implement the task or rewrite the full plan from scratch unless the owner requests refinement.
- If the task is small, urgent, or tightly coupled, handle it locally instead of spawning.

### Plan Review Loop
- For `multi-step` and `uncertain` tasks, create the initial plan first, then send it to the plan review agent for a single review pass.
- The plan review agent checks decomposition quality, ownership conflicts, verification sufficiency, dependency assumptions, whether delegation is justified, and whether risks are explicit.
- A plan should be marked `refine` only when there is a material issue: missing or unclear verification, ownership conflict, hidden dependency on another domain, unjustified delegation, unsequenced critical-path work, or unresolved blocker that makes implementation unsafe.
- A plan may be marked `approve with risks` when risks remain but implementation order, ownership, and verification are still adequate.
- If the review finds material issues, return the plan once for refinement before implementation starts.
- After one refinement, do not repeat plan review. Either proceed with the revised plan or ask the user to resolve the remaining blockers.

### Iteration Policy
- Use a verification loop when work includes testing, review, or user-reported bug reproduction.
- Standard loop: `implement -> add/update unit tests/docs/comments -> run tests/review -> fix -> re-test`.
- If a defect is found, send it back to the responsible domain agent for another iteration.
- For code tasks, passing unit tests are required before the task is considered complete.
- For code tasks, updated documentation and required code comments are required before the task is considered complete.
- For Unity work, the verification step must use Unity MCP after unit tests before the task is considered complete.
- If the same task reaches more than **3 iterations**, stop the autonomous loop and ask the user for help before continuing.
- After the third failed iteration, summarize what was tried, what failed, and which assumptions or constraints are blocking progress.

### Review Role Split
- `Agent J` reviews plan quality before implementation.
- `Agent H` reviews implementation results, regressions, unit-test adequacy, documentation adequacy, comment adequacy, and verification outcomes after or alongside execution.
- Do not use `Agent H` as a substitute for the mandatory pre-implementation plan review when `Agent J` is required.

## Commit & Pull Request Guidelines
- **Commits**: Imperative, scope-first subjects (e.g., `Add movement`, `Fix menu layout`). Avoid placeholders like `123`.
- **Branches**: `feature/*`, `fix/*`, `docs/*`.
- **PRs must include**: short description of the change, linked issue/task ID when available, screenshots/clips for scene/UI/docs-page changes, verification method including the unit tests that were added or run, and the documentation that was added or updated.
