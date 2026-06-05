#!/usr/bin/env bash
# Собрать Docker-образ сборочного окружения Android (см. Dockerfile.android).
# Запускать из любого места — пути вычисляются от расположения скрипта.
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
MONOGAME_DIR="$(cd "${SCRIPT_DIR}/.." && pwd)"   # каталог MonoGame — контекст сборки

IMAGE="${1:-riotgalaxy-android-build}"

echo ">> Building image '${IMAGE}' (context: ${MONOGAME_DIR})"
docker build \
    -f "${SCRIPT_DIR}/Dockerfile.android" \
    -t "${IMAGE}" \
    "${MONOGAME_DIR}"

echo ">> Done. Image: ${IMAGE}"
