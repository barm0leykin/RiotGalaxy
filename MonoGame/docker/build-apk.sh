#!/usr/bin/env bash
# Собрать Android APK внутри сборочного контейнера (см. Dockerfile.android).
# Контейнер запускается ПОД ТЕКУЩИМ пользователем (--user), поэтому артефакты
# bin/obj принадлежат тебе и чистятся без sudo. NuGet/dotnet-кэш — в docker/.home
# (на хосте, gitignored), переиспользуется между сборками.
# Результат (APK) — в RiotGalaxy.Android/bin/<Config>/net9.0-android/.
#
# Использование:
#   ./docker/build-apk.sh [Debug|Release]   (по умолчанию Debug)
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
MONOGAME_DIR="$(cd "${SCRIPT_DIR}/.." && pwd)"

CONFIG="${1:-Debug}"
IMAGE="${2:-riotgalaxy-android-build}"

HOME_CACHE="${SCRIPT_DIR}/.home"   # пользовательский HOME контейнера (кэш NuGet/dotnet)
mkdir -p "${HOME_CACHE}"

echo ">> Building APK (config: ${CONFIG})"
docker run --rm \
    --user "$(id -u):$(id -g)" \
    -e HOME=/home/build \
    -v "${HOME_CACHE}:/home/build" \
    -v "${MONOGAME_DIR}:/src" \
    -w /src \
    "${IMAGE}" \
    bash -c "dotnet build RiotGalaxy.Android/RiotGalaxy.Android.csproj -c ${CONFIG}"

echo ">> APK output:"
find "${MONOGAME_DIR}/RiotGalaxy.Android/bin/${CONFIG}" -name '*.apk' 2>/dev/null || echo "(APK not found — check build log above)"
