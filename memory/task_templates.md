# Task Templates

Этот файл задает безопасный модульный стандарт для задач.
Здесь нет одного жесткого шаблона на все случаи. Вместо этого используется общее ядро и 4 режима:

- `Lite`
- `Standard`
- `Exploration`
- `Handoff`

Отдельно допускается session-oriented execution для лора и обсуждаемых narrative changes.

## Зачем модульный подход

Один тяжелый шаблон ломает простые задачи, исследование и быстрые фиксы.
Поэтому шаблон должен:

- быть минимальным для простых задач;
- расширяться для multi-step задач;
- не требовать ложной точности там, где сначала нужно исследование;
- отдельно поддерживать передачу задач между агентами.

## Общее ядро

Это минимум, который должен существовать почти всегда.

```text
task_id:
context_version:
task_type:
goal:
execution_profile:
test_strategy:
documentation_strategy:
verification:
```

### Поля ядра

- `task_id`: уникальный идентификатор задачи.
- `context_version`: версия текущего состояния задачи.
- `task_type`: `lite`, `standard`, `exploration`, `handoff`.
- `goal`: какой результат нужен.
- `execution_profile`: какой `model` / `reasoning_effort` используется для primary execution и какие есть overrides.
- `test_strategy`: какие unit tests обязательны, что именно покрывается, какие команды запускаются, или почему это `not_applicable`.
- `documentation_strategy`: какие docs нужно обновить, какие комментарии обязательны, какие adjacent references становятся source of truth, или почему это `not_applicable`.
- `verification`: как понять, что задача выполнена.

Для любой задачи, которая меняет код, `test_strategy` обязана требовать unit tests.
Пустое `test_strategy`, `manual only` или перенос тестов "на потом" для code-change задачи недопустимы.
Если тестового контура в затронутой зоне нет, `test_strategy` должна включать создание минимально достаточного test harness в рамках той же задачи.
Для любой задачи, которая меняет код, `documentation_strategy` обязана требовать обновление документации и required comments.
Пустое `documentation_strategy` или перенос docs/comments "на потом" для code-change задачи недопустимы.
Если у затронутой зоны нет usable local docs, `documentation_strategy` должна включать создание минимальной adjacent documentation в рамках той же задачи.

Базовые role defaults:

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

`xhigh` не использовать как дефолт. Его можно указывать только как явный override для редких задач повышенной сложности.

Дополнительные session-поля могут использоваться для narrative/lore задач:

- `session_mode`
- `checkpoint_ready`

## Lite

Для простых локальных задач в одном домене.

Использовать, когда:

- затронут один домен;
- изменение небольшое;
- проверка очевидна;
- делегация, скорее всего, не нужна.

Не использовать `Lite`, если меняется видимый пользовательский текст и нельзя доказать корректность без проверки рендера.

Шаблон:

```text
task_id:
context_version: v1
task_type: lite
goal:
execution_profile:
  primary_reasoning_effort: high
test_strategy:
  unit_tests: required | not_applicable
  scope:
    - ...
  commands:
    - ...
documentation_strategy:
  docs: required | not_applicable
  comment_scope:
    - ...
  docs_to_update:
    - ...
scope:
  owned:
    - ...
verification:
  - ...
notes:
  - ...
```

Обязательные поля:

- `task_id`
- `context_version`
- `task_type`
- `goal`
- `test_strategy`
- `documentation_strategy`
- `scope.owned`
- `verification`

## Standard

Для обычных многошаговых задач.

Использовать, когда:

- несколько этапов;
- несколько каталогов;
- возможна делегация;
- важны ограничения и риски.

Шаблон:

```text
task_id:
context_version: v1
task_type: standard
goal:
execution_profile:
  primary_reasoning_effort: high
  overrides:
    - Agent ...: ...
test_strategy:
  unit_tests: required | not_applicable
  scope:
    - ...
  commands:
    - ...
documentation_strategy:
  docs: required | not_applicable
  comment_scope:
    - ...
  docs_to_update:
    - ...
context_scope:
  owned:
    - ...
  excluded:
    - ...
  owners:
    - Agent ...
support_agents:
  - Agent J
  - Agent F
  - Agent G
plan_review_status:
plan_review_findings:
  - ...
context_snapshot:
  facts:
    - ...
  constraints:
    - ...
  documentation_refs:
    - ...
  open_questions:
    - ...
plan:
  - ...
verification:
  - ...
risks:
  - ...
```

Обязательные поля:

- всё из ядра;
- `context_scope`;
- `context_snapshot`;
- `plan`;
- `test_strategy`;
- `documentation_strategy`;
- `risks`.

Если используется `Agent J`, в `Standard` задаче желательно явно фиксировать:

- `plan_review_status`: `pending`, `approved`, `approved_with_risks`, `refine`, `skipped`;
- `plan_review_findings`: краткие замечания или основание для `skipped`.
- `execution_profile.overrides`: если support-agent работает не на дефолтном уровне для своей роли.

`support_agents` использовать, если задаче нужны:

- `Agent J` для однократной ревизии плана до начала реализации;
- `Agent F` для git/release hygiene;
- `Agent G` для deploy/VPS rollout preparation.
- `Agent I` для MCP Playwright browser verification.
- Unity MCP verification pass для Unity runtime/editor проверки после реализации.

`Agent J` должен считать план неполным, если для code-change задачи не описан `test_strategy`.
`Agent J` должен считать план неполным, если для code-change задачи не описан `documentation_strategy`.

## Exploration

Для задач, где сначала нужно понять проблему.

Использовать, когда:

- причина бага неизвестна;
- задача может оказаться сложнее формулировки;
- scope пока нельзя определить точно;
- verification пока неполная.

Шаблон:

```text
task_id:
context_version: v1
task_type: exploration
goal:
execution_profile:
  primary_reasoning_effort:
test_strategy:
  unit_tests:
    status: pending
    questions:
      - ...
documentation_strategy:
  docs:
    status: pending
    questions:
      - ...
  comments:
    status: pending
    questions:
      - ...
known_facts:
  - ...
unknowns:
  - ...
initial_scope:
  likely_areas:
    - ...
  excluded_areas:
    - ...
investigation_questions:
  - ...
exit_criteria:
  - получить подтвержденную причину
  - собрать данные для normal plan
verification:
  - ...
```

Особенность:

- `Exploration` не требует финального ownership с самого начала;
- после исследования задача должна быть переведена в `Lite` или `Standard`.
- Если exploration касается code-change задачи, она должна завершиться понятным планом unit-test покрытия до перехода в реализацию.
- Если exploration касается code-change задачи, она должна завершиться понятным планом documentation updates и comment scope до перехода в реализацию.

Если проблема проявляется только в браузере, exploration должна явно зафиксировать, что нужна MCP Playwright verification.
Если после исследования формируется план реализации, для `Exploration -> Lite/Standard` допускается одна ревизия плана через Agent J перед стартом исполнения.
Если ревизия пропущена, в следующем шаблоне нужно явно указать основание для `skipped`.
Если exploration переходит в реализацию, в следующем шаблоне нужно явно зафиксировать `primary_reasoning_effort`; для implementation/development по умолчанию это `high`.
Если exploration переходит в code-change реализацию, в следующем шаблоне нужно явно зафиксировать unit-test scope и test commands.
Если exploration переходит в code-change реализацию, в следующем шаблоне нужно явно зафиксировать docs to update и comment scope.

## Handoff

Для передачи задачи между агентами или между итерациями.

Использовать, когда:

- одна роль завершила свой участок;
- задача уходит на review;
- задача возвращается после теста;
- меняется владелец следующего этапа.

Шаблон:

```text
task_id:
context_version:
task_type: handoff
from:
to:
goal:
execution_profile:
test_strategy:
documentation_strategy:
handoff_contract:
  completed:
    - ...
  remaining:
    - ...
  changed_files:
    - ...
  test_status:
    - ...
  documentation_status:
    - ...
  verification_status:
    - ...
  risks:
    - ...
  next_expected_action:
    - ...
  deployment_notes:
    - ...
  git_notes:
    - ...
```

Обязательные поля:

- `task_id`
- `context_version`
- `from`
- `to`
- `handoff_contract`

Для code-change handoff поле `test_status` должно явно отвечать:

- какие unit tests добавлены или обновлены;
- какие test commands уже прошли;
- что еще осталось допокрыть, если handoff идет до финала.

Для code-change handoff поле `documentation_status` должно явно отвечать:

- какие docs были добавлены или обновлены;
- какие code comments были добавлены или пересмотрены;
- какие knowledge gaps еще остались, если handoff идет до финала.

`deployment_notes` и `git_notes` обязательны только если handoff связан с релизом, деплоем или подготовкой к передаче на VPS.

Для handoff после ревизии плана можно дополнительно фиксировать:

- `plan_review_status`: `approved`, `approved_with_risks`, `refine` или `skipped`;
- `plan_review_findings`: краткий список замечаний, если они были.

## Session Mode

Это не отдельный task type, а execution flag для narrative/lore задач.

Использовать, когда:

- обсуждение идет постепенно;
- изменения логически связаны;
- запуск полного пайплайна после каждой микроправки не нужен.

Минимальные поля:

```text
session_mode: true
checkpoint_ready: false
```

Правила:

- пока `checkpoint_ready: false`, можно накапливать смысловые правки в одной задаче;
- при переходе в `checkpoint_ready: true` задача должна пройти consistency/review/commit checkpoint, если это требуется контекстом;
- `session_mode` лучше всего подходит для лора, worldbuilding и markdown narrative batches.
- Для runtime-state можно использовать отдельный шаблон `system/state/tasks/LORE_SESSION_TEMPLATE.json`.

## Правила выбора шаблона

- Если задача маленькая и локальная, выбирай `Lite`.
- Если задача содержит этапы, интеграцию или несколько доменов, выбирай `Standard`.
- Если сначала нужно выяснить, что вообще происходит, выбирай `Exploration`.
- Если задача переходит между агентами или итерациями, используй `Handoff`.

Для web/UI задач verification желательно уточнять отдельно:

- `command` — build/lint/filesystem/process checks;
- `browser` — MCP Playwright route/UI/interactions verification;
- `hybrid` — сначала command, потом browser.

Для code-change задач verification не должна ограничиваться только build/lint/manual checks; passing unit tests обязательны.
Для code-change задач verification не должна ограничиваться только tests/build/lint/manual checks; updated docs и required comments обязательны.

Для Unity задач verification желательно уточнять отдельно:

- `unity-mcp` — Unity MCP console/scene/runtime/editor verification;
- `hybrid-unity` — unit tests и локальные проверки или review плюс обязательная Unity MCP verification.

Для Unity MCP findings желательно дополнительно указывать:

- `blocking` — требует возврата на доработку;
- `non-blocking` — не требует возврата, но фиксируется;
- `informational` — наблюдение без действия.

Если задача меняет:

- headings;
- labels;
- CTA text;
- markdown content;
- portal or landing copy;
- локализованные строки;

то verification не должна оставаться только `command`.

Если задача меняет код:

- `test_strategy.unit_tests` не может быть `optional`;
- `documentation_strategy.docs` не может быть `optional`;
- отсутствие тестового контура должно трактоваться как часть scope текущей задачи;
- отсутствие usable local docs должно трактоваться как часть scope текущей задачи;
- handoff без `test_status` и `documentation_status` допустим только для `not_applicable` задач без code changes.

## Правила перехода между режимами

- `Exploration -> Lite`, если найден локальный фикс.
- `Exploration -> Standard`, если выяснилось, что задача многошаговая.
- `Standard -> Handoff`, если работа передается другому агенту.
- `Lite -> Handoff`, если даже простая задача уходит на review или возврат.
- После ошибки на тесте повышай `context_version`.
- После ошибки на Unity MCP verification повышай `context_version` и возвращай задачу профильному Unity-агенту.
- После ревизии плана не запускать второй review-pass того же плана: либо доработай его один раз, либо эскалируй блокеры пользователю.
- Если план review был `skipped`, причина пропуска должна быть зафиксирована в задаче, а не подразумеваться.

## Антипаттерны

- Не использовать `Standard` для микроправок без реальной пользы.
- Не использовать `Lite`, если затрагивается несколько доменов.
- Не использовать `Lite` для изменений пользовательского текста, если не запланирована проверка отображения.
- Не заставлять `Exploration` притворяться полноценным планом до завершения исследования.
- Не передавать задачу агенту без `Handoff`, если уже были изменения или промежуточные выводы.
