"""
Главный модуль для запуска AI агента с сохранением контекста.
Использует систему памяти для отслеживания состояния проекта и истории работы.
"""

import os
import sys
from datetime import datetime

# Добавляем путь к модулям
sys.path.append(os.path.dirname(os.path.abspath(__file__)))

# Импортируем модули
from agent_memory import AgentMemory
from vector_index import (
    save_user_message,
    save_assistant_message,
    add_task_completed,
    update_project_component,
    add_important_file,
    get_context_for_prompt,
    retrieve_code_context,
    index_code_folder,
    end_session
)

# Создаем экземпляр памяти
memory = AgentMemory()

# Инициализация компонента проекта
update_project_component(
    "RiotGalaxy", 
    "Проект миграции игры с CocosSharp на MonoGame"
)

# Добавляем важные файлы
add_important_file("agent.md", "Файл настроек для LLM агента")
add_important_file("prd.md", "Требования к продукту")
add_important_file("README.md", "Описание системы памяти")

def initialize_session():
    """Инициализация новой сессии"""
    session_id = memory.session.session_id
    print(f"Начата новая сессия: {session_id}")
    
    # Индексируем проект (если нужно)
    if os.path.exists("."):
        try:
            print("Индексирование файлов проекта...")
            indexing_result = index_code_folder(".")
            if indexing_result:
                print("Файлы проекта успешно проиндексированы")
            else:
                print("Индексирование не удалось (возможно, не установлены зависимости)")
        except Exception as e:
            print(f"Ошибка индексации: {e}")
            print("Продолжаем работу без индексации")
    
    return session_id

def get_system_prompt():
    """Формирует системный промт с учетом контекста"""
    context = get_context_for_prompt()
    
    system_prompt = f"""Ты — помощник разработчика, работающий над проектом RiotGalaxy.

{context}

Руководствуйся следующими правилами:
- Всегда говори на русском
- Не выдумывай, если данных нет - скажи
- Если не знаешь ответа, скажи, что не знаешь
- Если не знаешь ответа, предложи поискать в интернете
- Руководствуйся файлами prd.md и tasks.md
- Сохраняй контекст разговора в директории ai-agent/memory/ но избегай лишних деталей

При выполнении задач:
1. Анализируй текущий контекст проекта
2. Изучай релевантные файлы кода
3. Предлагай решения с учетом требований проекта
4. Сохраняй выполненные задачи в сводку
"""
    
    return system_prompt

def process_user_input(user_input: str):
    """Обрабатывает ввод пользователя"""
    # Сохраняем сообщение пользователя
    save_user_message(user_input)
    
    # Получаем системный промт с контекстом
    system_prompt = get_system_prompt()
    
    # Получаем контекст кода по запросу (если нужно)
    code_context = retrieve_code_context(user_input)
    
    # Формируем полный промт
    full_prompt = system_prompt
    
    if code_context:
        full_prompt += f"\n\nРелевантный код:\n{code_context}"
    
    full_prompt += f"\n\nЗапрос пользователя:\n{user_input}\n\nОтвет:"
    
    return full_prompt

def save_assistant_response_func(response: str):
    """Сохраняет ответ ассистента"""
    save_assistant_message(response)

def add_completed_task(task_description: str):
    """Добавляет выполненную задачу"""
    add_task_completed(task_description)

# Пример использования
if __name__ == "__main__":
    # Инициализация сессии
    session_id = initialize_session()
    
    # Пример обработки запроса
    user_query = "Покажи структуру проекта и основные компоненты"
    prompt = process_user_input(user_query)
    
    print("Сформированный промт:")
    print(prompt)
    
    # Имитация ответа ассистента
    assistant_response = """Проект RiotGalaxy - это набор приложений развернутых в Kubernetes с помощью ArgoCD.

Основные компоненты:
1. Backend-сервисы на C#
2. Frontend-приложения
3. Kubernetes-манифесты
4. Конфигурации ArgoCD

Для более подробной информации изучите файлы prd.md и tasks.md."""
    
    # Сохранение ответа
    save_assistant_response_func(assistant_response)
    
    # Добавление выполненной задачи
    add_completed_task("Анализ структуры проекта RiotGalaxy")
    
    # Завершение сессии
    print("\nЗавершение сессии...")
    end_session()
    print("Сессия завершена")
