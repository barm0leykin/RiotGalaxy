#!/usr/bin/env bash
# Открыть оболочку в сборочном контейнере с примонтированными исходниками MonoGame.
# Запуск под текущим пользователем — создаваемые файлы принадлежат тебе.
# Внутри доступны: dotnet (+workload android), java 17, Android SDK, sdkmanager, adb.
#   /src — каталог MonoGame с хоста; HOME=/home/build (кэш в docker/.home на хосте).
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
MONOGAME_DIR="$(cd "${SCRIPT_DIR}/.." && pwd)"

IMAGE="${1:-riotgalaxy-android-build}"

HOME_CACHE="${SCRIPT_DIR}/.home"
mkdir -p "${HOME_CACHE}"

docker run --rm -it \
    --user "$(id -u):$(id -g)" \
    -e HOME=/home/build \
    -v "${HOME_CACHE}:/home/build" \
    -v "${MONOGAME_DIR}:/src" \
    -w /src \
    "${IMAGE}" \
    /bin/bash
