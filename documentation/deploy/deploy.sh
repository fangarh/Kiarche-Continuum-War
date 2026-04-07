#!/usr/bin/env bash
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
REPO_ROOT="$(cd "${SCRIPT_DIR}/../.." && pwd)"
ENV_FILE="${SCRIPT_DIR}/.env"

if [[ ! -f "${ENV_FILE}" ]]; then
    echo "Missing ${ENV_FILE}. Copy .env.example to .env and configure it first."
    exit 1
fi

set -a
source "${ENV_FILE}"
set +a

APP_DIR="${APP_DIR:-${REPO_ROOT}/documentation/kcwweb}"
PUBLIC_DIR="${PUBLIC_DIR:-}"
ENABLE_BACKUP="${ENABLE_BACKUP:-true}"
RELOAD_COMMAND="${RELOAD_COMMAND:-}"

if [[ -z "${PUBLIC_DIR}" ]]; then
    echo "PUBLIC_DIR is required."
    exit 1
fi

if [[ -z "${BACKUP_ROOT:-}" ]]; then
    BACKUP_ROOT="${SCRIPT_DIR}/backups"
fi

BUILD_DIR="${APP_DIR}/dist"
TIMESTAMP="$(date +%Y%m%d-%H%M%S)"
STAGING_DIR="${SCRIPT_DIR}/artifacts/current"

echo "Installing dependencies in ${APP_DIR}"
(cd "${APP_DIR}" && npm ci)

echo "Building production bundle"
(cd "${APP_DIR}" && npm run build)

if [[ ! -d "${BUILD_DIR}" ]]; then
    echo "Build output ${BUILD_DIR} not found."
    exit 1
fi

rm -rf "${STAGING_DIR}"
mkdir -p "${STAGING_DIR}"
cp -R "${BUILD_DIR}/." "${STAGING_DIR}/"

if [[ "${ENABLE_BACKUP}" == "true" && -d "${PUBLIC_DIR}" ]]; then
    mkdir -p "${BACKUP_ROOT}/${TIMESTAMP}"
    cp -R "${PUBLIC_DIR}/." "${BACKUP_ROOT}/${TIMESTAMP}/"
    echo "Backup created in ${BACKUP_ROOT}/${TIMESTAMP}"
fi

mkdir -p "${PUBLIC_DIR}"
rm -rf "${PUBLIC_DIR:?}/"*
cp -R "${STAGING_DIR}/." "${PUBLIC_DIR}/"

if [[ -n "${RELOAD_COMMAND}" ]]; then
    echo "Running reload command"
    eval "${RELOAD_COMMAND}"
fi

echo "Deploy completed successfully."
