#!/usr/bin/env bash
# Установить и запустить APK на подключённом по USB телефоне, через adb из контейнера.
# USB пробрасывается в контейнер (--privileged + /dev/bus/usb). Ключ авторизации adb
# хранится в docker/.home/.android (подтвердить запрос на телефоне нужно один раз).
#
# Использование:
#   ./docker/run-on-device.sh [Debug|Release] [секунды_логов]
# По умолчанию: Debug, 12 секунд logcat после старта.
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
MONOGAME_DIR="$(cd "${SCRIPT_DIR}/.." && pwd)"

CONFIG="${1:-Debug}"
LOG_SECONDS="${2:-12}"
IMAGE="${3:-riotgalaxy-android-build}"
PKG="com.riotgalaxy.game"
ACTIVITY="${PKG}/crc64652fc972b92ab8b5.MainActivity"

HOME_CACHE="${SCRIPT_DIR}/.home"
mkdir -p "${HOME_CACHE}"

docker run --rm \
    --privileged \
    -v /dev/bus/usb:/dev/bus/usb \
    -v "${HOME_CACHE}:/home/build" \
    -e HOME=/home/build \
    -v "${MONOGAME_DIR}:/src" \
    -w /src \
    "${IMAGE}" \
    bash -c "
        set -e
        APK=RiotGalaxy.Android/bin/${CONFIG}/net9.0-android/${PKG}-Signed.apk
        adb start-server >/dev/null 2>&1; sleep 1
        echo '=== devices ==='; adb devices
        adb uninstall ${PKG} >/dev/null 2>&1 || true
        echo '=== install ==='; adb install --no-incremental -r \"\$APK\"
        adb logcat -c
        echo '=== launch ==='; adb shell am start -n ${ACTIVITY}
        sleep ${LOG_SECONDS}
        PID=\$(adb shell pidof ${PKG} | tr -d '\r')
        echo \"PID=[\$PID] (\$([ -n \"\$PID\" ] && echo ALIVE || echo EXITED))\"
        echo '=== наши логи (тег DOTNET) + ошибки ==='
        adb logcat -d 'DOTNET:V' '*:E' 2>/dev/null \
          | grep -iE '\[DBG\]|\[ERR\]|Failed to load|not found|Exception|FATAL|abort' \
          | grep -viE 'WifiSta|Wearable|whatsapp|arcsoft|GOS:|HoneySpace|conscrypt|GoogleApi|IndoorLocation|MediaProvider|NotifHistory|Fbns|AppOpService|FeatureFlags' \
          | tail -30
    "
