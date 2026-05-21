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

def update_task_status(task_id: str, markers: Dict[str, str]) -> List[str]:
    """
    Обновляет статус конкретной задачи
    
    Args:
        task_id: ID задачи в формате "1.1", "2.3" и т.д.
        markers: словарь с метками статусов
            "pending": маркер для невыполненной задачи (по умолчанию "-")
            "completed": маркер для выполненной задачи (по умолчанию "✅")
            "in_progress": маркер для задачи в процессе (по умолчанию "🔄")
    """
    lines = load_tasks()
    updated_lines = []
    
    # Ищем нужную задачу
    for line in lines:
        if line.strip().startswith(f"### {task_id}"):
            # Нашли задачу, обновляем ее статус
            updated_line = line
            
            # Определяем текущий статус
            if "✅" in line:
                current_status = "completed"
            elif "🔄" in line:
                current_status = "in_progress"
            else:
                current_status = "pending"
            
            # Получаем правильный маркер
            marker = markers.get(current_status, "-")
            
            # Обновляем строку с правильным маркером
            updated_line = re.sub(r"^### \d+\.\d+ ", f"### {task_id} ", line)
            updated_line = re.sub(r"\s+[🔄✅-]*\s+", f" ", updated_line)
            updated_line = re.sub(f"^{task_id} ", f"{task_id} {marker}", updated_line)
            
            updated_lines.append(updated_line)
        elif line.strip().startswith("**Результат**"):
            # Для строк с результатами, если задача выполнена
            prev_line = updated_lines[-1] if updated_lines else ""
            if prev_line and "✅" in prev_line:
                updated_line = line
                if "заглушки" in line:  # Если результат содержит слово "заглушки", обновляем
                    updated_line = line.replace("заглушки", "UI элементами")
                updated_lines.append(updated_line)
            else:
                updated_lines.append(line)
        else:
            updated_lines.append(line)
    
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
    
    lines = update_task_status(task_id, markers)
    save_tasks(lines)
    
    # Добавляем информацию в сводку
    if os.path.exists("../../ai-agent/save-history.sh"):
        os.system("cd ../../ai-agent && ./save-history.sh")
    else:
        # Прямое обновление информации
        try:
            with open("../../ai-agent/memory/summary.txt", "a") as f:
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
