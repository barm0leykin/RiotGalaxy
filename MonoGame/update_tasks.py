#!/usr/bin/env python3
"""
Скрипт для обновления статуса задач в файле tasks.md
Используется при завершении этапов миграции
"""

import re
import sys
import os
from typing import Dict, List

def load_tasks() -> List[str]:
    """Загружает содержимое tasks.md"""
    try:
        with open("tasks.md", "r", encoding="utf-8") as f:
            return f.readlines()
    except FileNotFoundError:
        print(f"Ошибка: файл tasks.md не найден в директории {os.getcwd()}")
        return []

def save_tasks(lines: List[str]) -> None:
    """Сохраняет обновленный tasks.md"""
    try:
        with open("tasks.md", "w", encoding="utf-8") as f:
            f.writelines(lines)
        print("Файл tasks.md успешно обновлен")
    except Exception as e:
        print(f"Ошибка при сохранении tasks.md: {e}")

def update_task_status(task_id: str, status: str, markers: Dict[str, str]) -> List[str]:
    """
    Обновляет статус конкретной задачи
    
    Args:
        task_id: ID задачи в формате "1.1", "2.3" и т.д.
        status: новый статус задачи ("pending", "in_progress", "completed")
        markers: словарь с метками статусов
            "pending": маркер для невыполненной задачи (по умолчанию "-")
            "completed": маркер для выполненной задачи (по умолчанию "✅")
            "in_progress": маркер для задачи в процессе (по умолчанию "🔄")
    """
    lines = load_tasks()
    updated_lines = []
    
    # Ищем нужную задачу
    current_task_found = False
    for line in lines:
        if line.strip().startswith(f"### {task_id}"):
            # Нашли задачу, обновляем ее статус
            current_task_found = True
            
            # Сначала разбираем текущую строку
            line_stripped = line.strip()
            
            # Ищем задачу в любом формате
            if line_stripped.startswith(f"### {task_id}"):
                
                # Просто заменяем всю строку, используя стандартный формат
                # Сначала извлекаем описание - все что после ### и номера задачи
                
                # Самый простой подход - переписать полностью строку без разборки старой
                # Просто обновим только маркер
                
                # Ищем маркеры в строке и заменяем их
                has_marker = any(marker in line for marker in ["🔄", "✅", "- "])
                
                if has_marker:
                    # Удаляем существующий маркер
                    new_line = re.sub(rf"### {task_id}\s+[🔄✅-]\s+", f"### {task_id} ", line_stripped)
                else:
                    new_line = line_stripped
                
                # Добавляем новый маркер
                marker = markers.get(status, "-")
                updated_line = re.sub(rf"### {task_id}\s+", f"### {task_id} {marker} ", new_line) + "\n"
            else:
                # Если это не наша строка, оставляем без изменений
                updated_line = line
            
            updated_lines.append(updated_line)
        elif line.strip().startswith("**Результат**") and current_task_found:
            # Для строк с результатами, добавляем маркер если задача выполнена
            if status == "completed":
                updated_line = line.strip()
                # Удаляем старый маркер, если есть
                updated_line = re.sub(r"\s+[🔄✅-]*$", "", updated_line)
                # Добавляем маркер выполненной задачи
                updated_line = f"{updated_line} ✅\n"
                updated_lines.append(updated_line)
                current_task_found = False  # Сбрасываем флаг после обработки
            else:
                updated_lines.append(line)
        else:
            updated_lines.append(line)
            if current_task_found and line.strip() == "":
                current_task_found = False  # Сбрасываем флаг после пустой строки
    
    return updated_lines

def update_current_task(task_id: str, status: str = "completed") -> None:
    """
    Обновляет статус текущей задачи
    
    Args:
        task_id: ID задачи ("1.1", "2.3" и т.д.)
        status: "pending", "in_progress", "completed"
    """
    markers = {
        "pending": "-",
        "in_progress": "🔄",
        "completed": "✅"
    }
    
    lines = update_task_status(task_id, status, markers)
    save_tasks(lines)
    
    # Добавляем информацию в сводку
    if os.path.exists("../ai-agent/save-history.py"):
        os.system("cd ../ai-agent && python3 save-history.py")
    else:
        # Прямое обновление информации
        try:
            with open("../ai-agent/memory/summary.txt", "a", encoding="utf-8") as f:
                from datetime import datetime
                f.write(f"[{datetime.now().strftime('%Y-%m-%d %H:%M')}] Обновлен статус задачи {task_id} -> {status}\n")
        except Exception as e:
            print(f"Ошибка при обновлении сводки: {e}")

def main():
    """Главная функция для CLI"""
    if len(sys.argv) < 2:
        print("Использование: python update_tasks.py <task_id> [status]")
        print("Статусы: pending, in_progress, completed (по умолчанию)")
        print("Пример: python update_tasks.py 1.1")
        print("Пример: python update_tasks.py 1.2 in_progress")
        return
    
    task_id = sys.argv[1]
    status = sys.argv[2] if len(sys.argv) > 2 else "completed"
    
    if status not in ["pending", "in_progress", "completed"]:
        print(f"Неверный статус: {status}. Используйте pending, in_progress или completed")
        return
    
    update_current_task(task_id, status)

if __name__ == "__main__":
    main()
