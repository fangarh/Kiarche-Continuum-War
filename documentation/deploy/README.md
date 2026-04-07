# KCW Web VPS Deploy

Этот каталог содержит воспроизводимый deploy flow для `documentation/kcwweb/`.

## Baseline

- разработка: Windows 11
- production deploy: Ubuntu 24 VPS
- синхронизация: Git

Канонический production flow:

1. локально сделать изменения и отправить их в Git;
2. на VPS выполнить `git pull`;
3. на VPS запустить deploy-скрипт.

## Deployment Mode

Используется режим `build-on-vps`.

Что делает deploy:

1. устанавливает зависимости для `documentation/kcwweb/`;
2. выполняет production build;
3. проверяет наличие `dist/`;
4. копирует build в publish directory на VPS;
5. при необходимости выполняет reload-команду.

## Первичная настройка

1. На VPS перейди в корень репозитория.
2. Скопируй `.env.example` в `.env`.
3. Заполни как минимум `PUBLIC_DIR`.
4. При необходимости укажи `RELOAD_COMMAND`.

Пример:

```bash
cp documentation/deploy/.env.example documentation/deploy/.env
```

## Основная команда деплоя

После настройки `.env` основной запуск на Ubuntu VPS:

```bash
bash documentation/deploy/deploy.sh
```

Полный типовой flow на VPS:

```bash
git pull
bash documentation/deploy/deploy.sh
```

`deploy.ps1` оставлен как вспомогательный локальный инструмент, но не считается основным production deploy path.

## Обязательные переменные

- `PUBLIC_DIR` — каталог, из которого web server отдает сайт.

## Необязательные переменные

- `APP_DIR` — путь к `documentation/kcwweb`, если нужен override.
- `BACKUP_ROOT` — каталог для бэкапов перед публикацией.
- `ENABLE_BACKUP` — `true` или `false`.
- `RELOAD_COMMAND` — команда reload/restart web server после выкладки.
  Пример для nginx:
  `sudo systemctl reload nginx`

## Успешный результат

Deploy считается успешным, если:

1. `npm run build` завершился без ошибки;
2. `dist/` создан;
3. файлы опубликованы в `PUBLIC_DIR`;
4. reload-команда, если задана, завершилась без ошибки.

## Recovery

Если включен backup, предыдущая версия сохраняется в `BACKUP_ROOT/<timestamp>`.
Откат можно сделать вручную, скопировав содержимое нужного backup-каталога обратно в `PUBLIC_DIR`.
