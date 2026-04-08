# Execution Flow

Этот файл фиксирует краткий канонический порядок выполнения задач.
Он не заменяет `AGENTS.md`, `memory/agent_profiles.md` и `memory/task_templates.md`,
а служит компактной памяткой для повседневного исполнения.

## Канонический порядок

1. Сделать triage задачи: цель, затронутые зоны, способ проверки, обязательный unit-test scope, documentation scope, comment expectations, риски, нужна ли делегация.
2. Классифицировать задачу как `small`, `multi-step` или `uncertain`.
3. Назначить `task_id`, задать `context_scope`, стартовый `context_version` и `execution_profile`.
4. Для `uncertain` задач сначала провести исследование, потом собрать нормальный план.
5. Для `multi-step` и `uncertain` задач подготовить явный план с ownership, verification, unit-test strategy, documentation strategy и рисками.
6. Пропустить план через `Agent J` один раз, если это не repeatable low-ambiguity задача с очевидными границами и стандартной проверкой.
7. Если `Agent J` вернул `refine`, доработать план один раз и дальше либо идти в реализацию, либо эскалировать блокеры пользователю.
8. Выполнить реализацию через профильного агента или локально, не нарушая ownership.
9. Для любой code-change задачи добавить или обновить unit tests, adjacent documentation и required code comments в том же цикле работы.
10. Запустить unit tests по затронутому коду; если тестового контура нет, сначала создать минимально достаточный контур.
11. Проверить, что документация и комментарии покрывают новые контракты, инварианты, flow и эксплуатационные детали.
12. После unit tests и documentation pass запустить verification по типу задачи.
13. Для Unity-задач verification обязательно включает Unity MCP после unit tests.
14. Findings из Unity MCP классифицировать как `blocking`, `non-blocking`, `informational`.
15. Если finding `blocking`, вернуть задачу профильному Unity-агенту, повысить `context_version` и повторить цикл проверки.
16. Если задача дошла до более чем 3 итераций, остановить автономный цикл и запросить помощь пользователя с кратким summary.
17. Если работа передается между ролями или итерациями, оформить `Handoff` с `handoff_contract`, включая test status и documentation status.
18. Финализировать задачу только после успешных unit tests, обновленной документации, required comments и подтвержденной verification, а не после завершения кодинга.

## Примечания

- `Agent J` проверяет качество плана до реализации.
- `Agent H` проверяет реализацию, регрессии, достаточность unit tests, достаточность документации, достаточность комментариев и verification outcome после или во время выполнения.
- Для implementation/development задач default `reasoning_effort` равен `high`, если задача явно не фиксирует другой уровень.
- Role defaults: `A/B/C/D/G/H = high`, `E/F/I/J = medium`.
- `xhigh` использовать только как явный override, а не как стандартный уровень роли.
- Для code-change задач отсутствие unit tests считается blocking gap в workflow.
- Для code-change задач отсутствие обновленной документации или required comments считается blocking gap в workflow.
- Для Unity-задач только `blocking` findings должны автоматически возвращать задачу на доработку.
- Если plan review пропущен, основание для `skipped` должно быть зафиксировано явно.
