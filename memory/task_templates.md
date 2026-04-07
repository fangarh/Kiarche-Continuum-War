# Task Templates

Этот файл задает безопасный модульный стандарт для задач.
Здесь нет одного жесткого шаблона на все случаи. Вместо этого используется общее ядро и 4 режима:

- `Lite`
- `Standard`
- `Exploration`
- `Handoff`

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
verification:
```

### Поля ядра

- `task_id`: уникальный идентификатор задачи.
- `context_version`: версия текущего состояния задачи.
- `task_type`: `lite`, `standard`, `exploration`, `handoff`.
- `goal`: какой результат нужен.
- `verification`: как понять, что задача выполнена.

## Lite

Для простых локальных задач в одном домене.

Использовать, когда:

- затронут один домен;
- изменение небольшое;
- проверка очевидна;
- делегация, скорее всего, не нужна.

Шаблон:

```text
task_id:
context_version: v1
task_type: lite
goal:
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
context_scope:
  owned:
    - ...
  excluded:
    - ...
  owners:
    - Agent ...
support_agents:
  - Agent F
  - Agent G
context_snapshot:
  facts:
    - ...
  constraints:
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
- `risks`.

`support_agents` использовать, если задаче нужны:

- `Agent F` для git/release hygiene;
- `Agent G` для deploy/VPS rollout preparation.

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
handoff_contract:
  completed:
    - ...
  remaining:
    - ...
  changed_files:
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

`deployment_notes` и `git_notes` обязательны только если handoff связан с релизом, деплоем или подготовкой к передаче на VPS.

## Правила выбора шаблона

- Если задача маленькая и локальная, выбирай `Lite`.
- Если задача содержит этапы, интеграцию или несколько доменов, выбирай `Standard`.
- Если сначала нужно выяснить, что вообще происходит, выбирай `Exploration`.
- Если задача переходит между агентами или итерациями, используй `Handoff`.

## Правила перехода между режимами

- `Exploration -> Lite`, если найден локальный фикс.
- `Exploration -> Standard`, если выяснилось, что задача многошаговая.
- `Standard -> Handoff`, если работа передается другому агенту.
- `Lite -> Handoff`, если даже простая задача уходит на review или возврат.
- После ошибки на тесте повышай `context_version`.

## Антипаттерны

- Не использовать `Standard` для микроправок без реальной пользы.
- Не использовать `Lite`, если затрагивается несколько доменов.
- Не заставлять `Exploration` притворяться полноценным планом до завершения исследования.
- Не передавать задачу агенту без `Handoff`, если уже были изменения или промежуточные выводы.
