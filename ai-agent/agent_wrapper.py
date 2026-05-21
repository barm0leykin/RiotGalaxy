"""
Обертка для интеграции AI агента с системой памяти.
Может быть использован как интерфейс между LLM API и системой сохранения контекста.
"""

import os
import sys
from typing import Dict, List, Any, Optional

# Добавляем путь к модулям
sys.path.append(os.path.dirname(os.path.abspath(__file__)))

from agent_memory import AgentMemory as AgentMemoryModule
from vector_index import get_context_for_prompt, retrieve_code_context

class AgentWithMemory:
    """Класс-обертка для AI агента с сохранением контекста"""
    
    def __init__(self, memory_dir: str = None):
        """Инициализация агента с памятью"""
        # Если директория не указана, используем стандартную
        if memory_dir is None:
            memory_dir = os.path.join(os.path.dirname(__file__), "..", "ai-agent", "memory")
        
        self.memory = AgentMemoryModule(memory_dir)
        self.current_context = None
    
    def start_session(self):
        """Начинает новую сессию"""
        print(f"Начата сессия: {self.memory.session.session_id}")
        self.current_context = get_context_for_prompt()
        return self.memory.session.session_id
    
    def process_user_message(self, user_message: str) -> Dict[str, Any]:
        """
        Обрабатывает сообщение пользователя и возвращает промпт для LLM
        
        Returns:
            Dict с полями:
            - prompt: полный промпт для отправки в LLM
            - session_id: ID текущей сессии
            - context: текущий контекст проекта
        """
        # Сохраняем сообщение пользователя
        self.memory.save_message("human", user_message)
        
        # Получаем системный промт с контекстом
        system_prompt = self._get_system_prompt()
        
        # Получаем контекст кода по запросу (если нужно)
        code_context = retrieve_code_context(user_message)
        
        # Формируем полный промпт
        full_prompt = system_prompt
        
        if code_context:
            full_prompt += f"\n\nРелевантный код:\n{code_context}"
        
        full_prompt += f"\n\nЗапрос пользователя:\n{user_message}\n\nОтвет:"
        
        self.current_context = full_prompt
        
        return {
            "prompt": full_prompt,
            "session_id": self.memory.session.session_id,
            "context": self.memory.get_context_for_prompt()
        }
    
    def save_assistant_response(self, response: str):
        """Сохраняет ответ ассистента"""
        self.memory.save_message("assistant", response)
    
    def add_completed_task(self, task_description: str):
        """Добавляет выполненную задачу в сводку"""
        self.memory.add_task_completed(task_description)
    
    def _get_system_prompt(self) -> str:
        """Формирует системный промт с учетом контекста"""
        context = self.memory.get_context_for_prompt()
        
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
    
    def end_session(self):
        """Завершает текущую сессию"""
        self.memory.end_current_session()
        print(f"Сессия {self.memory.session.session_id} завершена")
    
    def get_session_info(self) -> Dict[str, Any]:
        """Возвращает информацию о текущей сессии"""
        return {
            "session_id": self.memory.session.session_id,
            "start_time": self.memory.session.start_time,
            "tasks_completed": self.memory.session.tasks_completed,
            "components": self.memory.project_context.components,
            "important_files": self.memory.project_context.important_files
        }


# Пример использования
if __name__ == "__main__":
    # Создаем агента с памятью
    agent = AgentWithMemory()
    
    # Начинаем сессию
    session_id = agent.start_session()
    print(f"Сессия начата: {session_id}")
    
    # Обрабатываем сообщение пользователя
    user_message = "Какова структура проекта и какие основные компоненты?"
    result = agent.process_user_message(user_message)
    
    print("\nСформированный промпт:")
    print(result["prompt"])
    
    # Имитация ответа от LLM
    assistant_response = """Проект RiotGalaxy - это 2D космический шутер, который мигрируется с CocosSharp на MonoGame.

Основные компоненты проекта:
1. RiotGalaxy.Core - основная игровая логика
2. RiotGalaxy.Content - ассеты (спрайты, звуки, шрифты)
3. RiotGalaxy.DesktopGL - версия для desktop
4. RiotGalaxy.Android - версия для Android

Для более подробной информации изучите файлы prd.md и tasks.md."""
    
    # Сохраняем ответ
    agent.save_assistant_response(assistant_response)
    
    # Добавляем выполненную задачу
    agent.add_completed_task("Анализ структуры проекта RiotGalaxy")
    
    # Получаем информацию о сессии
    session_info = agent.get_session_info()
    print("\nИнформация о сессии:")
    print(f"ID: {session_info['session_id']}")
    print(f"Начата: {session_info['start_time']}")
    print(f"Выполненные задачи: {session_info['tasks_completed']}")
    
    # Завершаем сессию
    agent.end_session()
