"""
Простой скрипт для тестирования системы памяти агента.
"""

import os
import sys
import time

# Добавляем путь к модулям
sys.path.append(os.path.dirname(os.path.abspath(__file__)))

from agent_memory import AgentMemory as AgentMemoryModule

def test_memory_system():
    """Тестирование системы памяти"""
    print("Начало тестирования системы памяти...")
    
    # Создаем новую сессию памяти
    memory = AgentMemoryModule()
    
    # Проверяем ID сессии
    print(f"ID сессии: {memory.session.session_id}")
    
    # Добавляем задачу
    memory.add_task_completed("Создана система памяти для агента")
    print("Задача добавлена в сводку")
    
    # Обновляем компонент проекта
    memory.update_project_component("MemorySystem", "Система сохранения контекста для AI агента")
    print("Компонент проекта обновлен")
    
    # Добавляем важный файл
    memory.add_important_file("ai-agent/agent-memory.py", "Основной модуль памяти агента")
    print("Важный файл добавлен")
    
    # Сохраняем сообщения
    memory.save_message("human", "Тестирование системы памяти")
    memory.save_message("assistant", "Система памяти работает корректно")
    print("Сообщения сохранены")
    
    # Получаем контекст для промпта
    context = memory.get_context_for_prompt()
    print("\nКонтекст для промпта:")
    print(context)
    
    # Завершаем сессию
    memory.end_current_session()
    print("\nСессия завершена")
    
    # Проверяем файлы
    print("\nПроверка созданных файлов:")
    
    # Проверяем сводку
    summary_file = os.path.join(memory.memory_dir, "summary.txt")
    if os.path.exists(summary_file):
        print(f"Файл сводки создан: {summary_file}")
        with open(summary_file, "r", encoding="utf-8") as f:
            print("Содержимое сводки:")
            print(f.read())
    
    # Проверяем контекст проекта
    context_file = os.path.join(memory.memory_dir, "project_context.json")
    if os.path.exists(context_file):
        print(f"Файл контекста проекта создан: {context_file}")
    
    # Проверяем файлы сессий
    sessions = os.listdir(memory.sessions_dir)
    print(f"Созданные файлы сессий: {sessions}")
    
    print("\nТестирование завершено успешно!")

if __name__ == "__main__":
    test_memory_system()
