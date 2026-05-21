#!/bin/bash
# Многофункциональный скрипт для управления проектом RiotGalaxy (MonoGame)

# Цвета для вывода (отключены для совместимости)
# GREEN='\033[0;32m'
# YELLOW='\033[1;33m'
# RED='\033[0;31m'
# BLUE='\033[0;34m'
# NC='\033[0m' # No Color
GREEN=""
YELLOW=""
RED=""
BLUE=""
NC=""

# Функция для вывода справки
show_help() {
    echo -e "${BLUE}RiotGalaxy (MonoGame) - Управление проектом${NC}"
    echo ""
    echo "Использование: ./RiotGalaxy.sh [команда]"
    echo ""
    echo "Команды:"
    echo "  ${GREEN}build${NC}       - Скомпилировать проект"
    echo "  ${GREEN}build --clean${NC}  - Скомпилировать проект с очисткой"
    echo "  ${GREEN}build --release${NC} - Скомпилировать проект в режиме релиза"
    echo "  ${GREEN}run${NC}         - Собрать и запустить игру (по умолчанию)"
    echo "  ${GREEN}clean${NC}        - Очистить папки сборки"
    echo "  ${GREEN}status${NC}       - Показать статус проекта"
    echo "  ${GREEN}tasks${NC}        - Показать статус задач из tasks.md"
    echo "  ${GREEN}task <id> [status]${NC} - Обновить статус задачи (pending, in_progress, completed)"
    echo "  ${GREEN}run --no-build${NC} - Запустить игру без сборки"
    echo ""
    echo "Примеры:"
    echo "./RiotGalaxy.sh           # Собрать и запустить игру"
    echo "./RiotGalaxy.sh build      # Только собрать проект"
    echo "./RiotGalaxy.sh run        # Только запустить игру"
    echo "./RiotGalaxy.sh task 1.2  # Начать работу над задачей 1.2"
    echo "./RiotGalaxy.sh task 1.2 completed  # Отметить задачу 1.2 как выполненную"
    echo ""
}

# Функция для проверки нахождения в корневой директории
# и перехода в корневую директорию проекта
go_to_root_directory() {
    # Получение пути к скрипту
    SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
    
    # Переключаемся на корневую директорию проекта
    cd "$SCRIPT_DIR"
    
    if [ ! -d "MonoGame" ]; then
        echo -e "${RED}Ошибка: директория MonoGame не найдена!${NC}"
        echo -e "${YELLOW}Возможно вы находитесь не в корневой директории проекта.${NC}"
        echo "Текущая директория: $(pwd)"
        exit 1
    fi
}

# Функция для сборки проекта
build_project() {
    local clean=$1
    local release=$2
    
    go_to_root_directory
    
    echo -e "${BLUE}Сборка проекта RiotGalaxy${NC}"
    echo "------------------------------------"
    
    cd MonoGame
    
    # Очистка если нужно
    if [ "$clean" = "true" ]; then
        echo -e "${YELLOW}Очистка папок сборки...${NC}"
        dotnet clean
    fi
    
    # Сборка
    local build_config=""
    if [ "$release" = "true" ]; then
        build_config="-c Release"
    fi
    
    echo -e "${GREEN}Выполняется команда: dotnet build $build_config RiotGalaxy.sln${NC}"
    dotnet build $build_config RiotGalaxy.sln
    
    # Проверка результата
    if [ $? -eq 0 ]; then
        echo -e "${GREEN}Сборка завершена успешно!${NC}"
        return 0
    else
        echo -e "${RED}Ошибка: сборка проекта не удалась!${NC}"
        return 1
    fi
}

# Функция для запуска игры
run_game() {
    local skip_build=$1
    
    # Получение пути к скрипту и переход в корневую директорию
    SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
    cd "$SCRIPT_DIR"
    
    # Сборка если нужно
    if [ "$skip_build" != "no-build" ]; then
        if ! build_project; then
            echo -e "${RED}Ошибка: сборка не удалась, запуск игры отменен${NC}"
            return 1
        fi
    else
        go_to_root_directory
    fi
    
    echo -e "${BLUE}Запуск игры RiotGalaxy${NC}"
    echo "-----------------------"
    
    cd MonoGame
    echo -e "${GREEN}Выполняется команда: dotnet run --project RiotGalaxy.DesktopGL/RiotGalaxy.DesktopGL.csproj${NC}"
    dotnet run --project RiotGalaxy.DesktopGL/RiotGalaxy.DesktopGL.csproj
    
    echo -e "${BLUE}Игра завершена.${NC}"
}

# Функция для очистки
clean_project() {
    go_to_root_directory
    
    echo -e "${YELLOW}Очистка проекта...${NC}"
    
    cd MonoGame
    dotnet clean
    rm -rf RiotGalaxy.Core/bin/
    rm -rf RiotGalaxy.Core/obj/
    rm -rf RiotGalaxy.DesktopGL/bin/
    rm -rf RiotGalaxy.DesktopGL/obj/
    
    echo -e "${GREEN}Очистка завершена.${NC}"
}

# Функция для показа статуса
show_status() {
    go_to_root_directory
    
    echo -e "${BLUE}Статус проекта RiotGalaxy${NC}"
    echo "-----------------------"
    
    cd MonoGame
    
    # Проверяем наличие исполняемых файлов (для Linux без расширения .exe)
    if [ -f "RiotGalaxy.DesktopGL/bin/Debug/net6.0/RiotGalaxy.DesktopGL" ]; then
        echo "✓ Исполняемый файл найден: RiotGalaxy.DesktopGL"
        echo "  Статус: ГОТОВ К ЗАПУСКУ"
    elif [ -f "RiotGalaxy.DesktopGL/bin/Debug/net6.0/RiotGalaxy.DesktopGL.exe" ]; then
        echo "✓ Исполняемый файл найден: RiotGalaxy.DesktopGL.exe"
        echo "  Статус: ГОТОВ К ЗАПУСКУ"
    else
        echo "⚠ Исполняемый файл не найден"
        echo "  Статус: ТРЕБУЕТСЯ СБОРКА"
    fi
    
    # Показываем размер директорий
    echo ""
    echo "Размер директорий:"
    echo "- RiotGalaxy.Core: $(du -sh RiotGalaxy.Core 2>/dev/null | cut -f 1)"
    echo "- RiotGalaxy.DesktopGL: $(du -sh RiotGalaxy.DesktopGL 2>/dev/null | cut -f 1)"
}

# Функция для показа статуса задач
show_tasks() {
    go_to_root_directory
    
    if [ -f "MonoGame/tasks.md" ]; then
        echo -e "${BLUE}Статус задач разработки${NC}"
        echo "---------------------------"
        
        cd MonoGame
        grep -E "^##|^### [0-9]+\." tasks.md | while read -r line; do
            if [[ $line =~ ^## ]]; then
                echo -e "${NC}${line}"
            elif [[ $line =~ \.+\) ]]; then
                if [[ $line =~ ✅ ]]; then
                    echo -e "${GREEN}${line}${NC}"
                elif [[ $line =~ 🔄 ]]; then
                    echo -e "${YELLOW}${line}${NC}"
                else
                    echo "${line}"
                fi
            else
                echo "${line}"
            fi
        done
    else
        echo -e "${YELLOW}Файл tasks.md не найден${NC}"
    fi
}

# Функция для обновления статуса задачи
update_task() {
    local task_id="$1"
    local status="$2"
    
    go_to_root_directory
    
    if [ ! -f "MonoGame/update_tasks.py" ]; then
        echo -e "${RED}Ошибка: скрипт update_tasks.py не найден!${NC}"
        return 1
    fi
    
    cd MonoGame
    python3 update_tasks.py "$task_id" "$status"
}

# Обработка аргументов командной строки
case "$1" in
    ""|"help"|"-h"|"--help")
        show_help
        ;;
    "build")
        shift
        # Проверяем флаги
        clean_flag=false
        release_flag=false
        for arg in "$@"; do
            if [ "$arg" = "--clean" ]; then
                clean_flag=true
            elif [ "$arg" = "--release" ]; then
                release_flag=true
            fi
        done
        
        build_project "$clean_flag" "$release_flag"
        ;;
    "run"|"start")
        shift
        # Проверяем флаг
        no_build_flag=""
        if [ "$1" = "--no-build" ]; then
            no_build_flag="no-build"
        fi
        
        run_game "$no_build_flag"
        ;;
    "clean")
        clean_project
        ;;
    "status")
        show_status
        ;;
    "tasks")
        show_tasks
        ;;
    "task")
        if [ -z "$2" ]; then
            echo -e "${RED}Ошибка: не указан ID задачи${NC}"
            echo -e "${YELLOW}Использование: ./RiotGalaxy.sh task <id> [status]${NC}"
            exit 1
        fi
        
        # По умолчанию статус "completed", если не указан
        status="${3:-completed}"
        update_task "$2" "$status"
        ;;
    *)
        # По умолчанию просто запускаем игру
        echo -e "${YELLOW}Неизвестная команда '$1'. Запуск игры по умолчанию...${NC}"
        echo ""
        run_game
        ;;
esac

exit 0
