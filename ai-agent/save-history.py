"""
Модуль для сохранения истории работы агента.
Интегрирован с новой системой памяти.
"""

import os
import sys
import json
from datetime import datetime

# Добавляем путь к модулю памяти
sys.path.append(os.path.dirname(os.path.abspath(__file__)))
from agent_memory import AgentMemory

# Создаем глобальный экземпляр
memory = AgentMemory()


def save_user_message(content: str):
    """Сохраняет сообщение пользователя в историю"""
    memory.save_message("human", content)
    print(f"Сообщение пользователя сохранено: {content[:50]}...")


def save_assistant_message(content: str):
    """Сохраняет ответ ассистента в историю"""
    memory.save_message("assistant", content)
    print(f"Ответ ассистента сохранен: {content[:50]}...")


def add_task_completed(task: str):
    """Добавляет выполненную задачу в сводку"""
    memory.add_task_completed(task)
    print(f"Задача добавлена в сводку: {task}")


def update_project_component(component_name: str, description: str):
    """Обновляет информацию о компоненте проекта"""
    memory.update_project_component(component_name, description)
    print(f"Компонент обновлен: {component_name}")


def add_important_file(file_path: str, description: str = ""):
    """Добавляет важный файл в контекст проекта"""
    memory.add_important_file(file_path, description)
    print(f"Добавлен важный файл: {file_path}")


def get_current_context():
    """Возвращает текущий контекст проекта"""
    return memory.get_context_for_prompt()


def print_summary(lines=10):
    """Выводит последние записи из сводки"""
    summary = memory.get_summary(lines)
    print("Последние записи из сводки:")
    print(summary)


def print_recent_messages(count=5):
    """Выводит последние сообщения из истории"""
    messages = memory.get_recent_messages(count)
    print(f"Последние {count} сообщений:")
    print(messages)


def end_session():
    """Завершает текущую сессию"""
    memory.end_current_session()
    print("Сессия завершена и сохранена")


# Пример использования
if __name__ == "__main__":
    # Добавляем тестовые данные
    save_user_message("Помоги мне создать новый компонент для аутентификации")
    
    save_assistant_message("Я помогу вам создать компонент для аутентификации. "
                          "Сначала нам нужно определить требования к компоненту...")
    
    add_task_completed("Создан компонент для авторизации JWT")
    update_project_component("AuthService", "Обеспечивает аутентификацию и авторизацию пользователей через JWT")
    add_important_file("src/services/auth.service.ts", "Сервис аутентификации")
    
    print("\nТекущий контекст:")
    print(get_current_context())
    
    print("\nСводка:")
    print_summary(5)
    
    print("\nПоследние сообщения:")
    print_recent_messages(3)
    
    end_session()
