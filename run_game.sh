#!/bin/bash
# Скрипт для запуска игры RiotGalaxy для MonoGame

echo "Запуск игры RiotGalaxy (MonoGame)..."
echo "--------------------------------------"

# Проверяем наличие директории MonoGame
if [ ! -d "MonoGame" ]; then
    echo "Ошибка: директория RiotGalaxy/MonoGame не найдена!"
    echo "Возможно вы находитесь не в корневой директории проекта."
    exit 1
else
    echo "Найдена директория проекта: $(pwd)/MonoGame"
fi

# Переходим в директорию MonoGame
cd MonoGame

# Проверяем наличие файла решения
if [ ! -f "RiotGalaxy.sln" ]; then
    echo "Ошибка: файл решения RiotGalaxy.sln не найден!"
    exit 1
fi

# Сборка проекта
echo "Сборка проекта..."
dotnet build RiotGalaxy.sln

# Проверяем результат сборки
if [ $? -ne 0 ]; then
    echo "Ошибка: сборка проекта не удалась!"
    exit 1
fi

echo "Сборка завершена успешно!"

# Запуск игры
echo "Запуск игры..."
dotnet run --project RiotGalaxy.DesktopGL/RiotGalaxy.DesktopGL.csproj

# Уведомление о завершении
echo "Игра завершена."
echo "--------------------------------------"
echo "Спасибо за игру RiotGalaxy!"

exit 0
