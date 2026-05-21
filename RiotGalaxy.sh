#!/bin/bash
# Многофункциональный скрипт для управления проектом RiotGalaxy (MonoGame)

# Цвета для вывода
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
RED='\033[0;31m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

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
check_root_directory() {
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
    
    check_root_directory
    
    echo -e "${BLUE}Сборка проекта RiotGalaxy${NC}"
    echo "------------------------------------"
    
    cd RiotGalaxy/MonoGame
    
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
    
    # Сборка если нужно
    if [ "$skip_build" != "no-build" ] && build_project; then
        return 1
    fi
    
    check_root_directory
    
    echo -e "${BLUE}Запуск игры RiotGalaxy${NC}"
    echo "-----------------------"
    
    cd RiotGalaxy/MonoGame
    echo -e "${GREEN}Выполняется команда: dotnet run --project RiotGalaxy.DesktopGL/RiotGalaxy.DesktopGL.csproj${NC}"
    dotnet run --project RiotGalaxy.DesktopGL/RiotGalaxy.DesktopGL.csproj
    
    echo -e "${BLUE}Игра завершена.${NC}"
}

# Функция для очистки
clean_project() {
    check_root_directory
    
    echo -e "${YELLOW}Очистка проекта...${NC}"
    
    cd RiotGalaxy/MonoGame
    dotnet clean
    rm -rf RiotGalaxy.Core/bin/
    rm -rf RiotGalaxy.Core/obj/
    rm -rf RiotGalaxy.DesktopGL/bin/
    rm -rf RiotGalaxy.DesktopGL/obj/
    
    echo -e "${GREEN}Очистка завершена.${NC}"
}

# Функция для показа статуса
show_status() {
    check_root_directory
    
    echo -e "${BLUE}Статус проекта RiotGalaxy${NC}"
    echo "-----------------------"
    
    cd RiotGalaxy/MonoGame
    
    # Проверяем наличие исполняемых файлов
    if [ -f "RiotGalaxy.DesktopGL/bin/Debug/net6.0/RiotGalaxy.DesktopGL.exe" ]; then
        echo -e "${GREEN}✓ Исполняемый файл найден: RiotGalaxy.DesktopGL.exe${NC}"
        echo -e "${GREEN}  Статус: ГОТОВ К ЗАПУСКУ${NC}"
    else
        echo -e "${YELLOW}⚠ Исполняемый файл не найден${NC}"
        echo -e "${YELLOW}  Статус: ТРЕБУЕТСЯ СБОРКА${NC}"
    fi
    
    # Показываем размер директорий
    echo ""
    echo "Размер директорий:"
    echo "- RiotGalaxy.Core: $(du -sh RiotGalaxy.Core 2>/dev/null | cut -f 1)"
    echo "- RiotGalaxy.DesktopGL: $(du -sh RiotGalaxy.DesktopGL 2>/dev/null | cut -f 1)"
}

# Функция для показа статуса задач
show_tasks() {
    if [ -f "tasks.md" ]; then
        echo -e "${BLUE}Статус задач разработки${NC}"
        echo "---------------------------"
        
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
    
    if [ ! -f "RiotGalaxy/MonoGame/update_tasks.py" ]; then
        echo -e "${RED}Ошибка: скрипт update_tasks.py не найден!${NC}"
        return 1
    fi
    
    cd RiotGalaxy/MonoGame
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
        local clean_flag=false
        local release_flag=false
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
        local no_build_flag=false
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
        local status="${2:-completed}"
        update_task "$1" "$status"
        ;;
    *)
        # По умолчанию просто запускаем игру
        echo -e "${YELLOW}Неизвестная команда '$1'. Запуск игры по умолчанию...${NC}"
        echo ""
        run_game
        ;;
esac

exit 0
