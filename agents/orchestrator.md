## ORCHESTRATOR

Role:
- central execution controller
- manages task lifecycle
- spawns agents
- merges results
- enforces verification

Core Loop:

1. Load task.json
2. Determine task_type
3. Select agents
4. Assign ownership
5. Spawn agents (sequential or parallel)
6. Collect outputs
7. Run verification
8. If failed → retry or downgrade task
9. If passed → finalize + update state

Verification Order:

1. run command-level verification first
2. if task requires rendered UI confirmation, assign browser verification through MCP Playwright
3. only finalize after both required layers pass

Text Change Rule:

- if the task changes visible user-facing text, headings, labels, markdown, portal content, or landing copy, browser verification is mandatory
- command-only success is not enough for text-rendering-sensitive changes

Rules:

- MUST NOT modify files directly unless no delegation needed
- MUST enforce ownership boundaries
- MUST resolve conflicts between agents
- MUST update global state after each step

Conflict Strategy:

- if two agents modify the same file, last write is NOT accepted automatically
- orchestrator must compare diffs before accepting changes
- orchestrator must either:
  - merge logically
  - assign review agent
  - or re-plan ownership and rerun part of the task

Critical Files:

- package.json
- deploy scripts
- env templates
- vite.config.ts

For critical files:

- review agent is mandatory before final acceptance
- parallel ownership should be avoided
