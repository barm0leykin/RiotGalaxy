"""
Улучшенный модуль для управления памятью агента.
Сохраняет контекст сессий, историю диалогов и сводку проекта.
"""

import json
import os
from datetime import datetime
from typing import Dict, List, Any, Optional
from dataclasses import dataclass, asdict


@dataclass
class SessionInfo:
    """Информация о текущей сессии"""
    session_id: str
    start_time: str
    end_time: Optional[str] = None
    summary: str = ""
    tasks_completed: List[str] = None
    
    def __post_init__(self):
        if self.tasks_completed is None:
            self.tasks_completed = []


@dataclass
class ProjectContext:
    """Контекст проекта"""
    project_name: str = "RiotGalaxy"
    project_type: str = "Kubernetes с ArgoCD"
    last_modified: str = ""
    components: Dict[str, str] = None
    important_files: List[str] = None
    
    def __post_init__(self):
        if self.components is None:
            self.components = {}
        if self.important_files is None:
            self.important_files = []


class AgentMemory:
    """Класс для управления памятью агента"""
    
    def __init__(self, memory_dir: str = "ai-agent/memory"):
        self.memory_dir = memory_dir
        self.sessions_dir = os.path.join(memory_dir, "sessions")
        self.session_file = os.path.join(memory_dir, "current_session.json")
        self.project_context_file = os.path.join(memory_dir, "project_context.json")
        self.summary_file = os.path.join(memory_dir, "summary.txt")
        
        # Создаем директории, если их нет
        os.makedirs(self.sessions_dir, exist_ok=True)
        
        # Загружаем или создаем контекст проекта
        self.project_context = self.load_project_context()
        
        # Начинаем новую сессию
        self.session = self.start_new_session()
    
    def load_project_context(self) -> ProjectContext:
        """Загружает контекст проекта из файла"""
        if os.path.exists(self.project_context_file):
            try:
                with open(self.project_context_file, "r", encoding="utf-8") as f:
                    data = json.load(f)
                return ProjectContext(**data)
            except Exception as e:
                print(f"Ошибка загрузки контекста проекта: {e}")
        
        return ProjectContext()
    
    def save_project_context(self):
        """Сохраняет контекст проекта в файл"""
        try:
            with open(self.project_context_file, "w", encoding="utf-8") as f:
                json.dump(asdict(self.project_context), f, ensure_ascii=False, indent=2)
        except Exception as e:
            print(f"Ошибка сохранения контекста проекта: {e}")
    
    def start_new_session(self) -> SessionInfo:
        """Начинает новую сессию"""
        session_id = datetime.now().strftime("%Y%m%d_%H%M%S")
        session = SessionInfo(
            session_id=session_id,
            start_time=datetime.now().isoformat()
        )
        
        # Сохраняем информацию о текущей сессии
        try:
            with open(self.session_file, "w", encoding="utf-8") as f:
                json.dump(asdict(session), f, ensure_ascii=False, indent=2)
        except Exception as e:
            print(f"Ошибка сохранения сессии: {e}")
        
        return session
    
    def end_current_session(self):
        """Завершает текущую сессию"""
        self.session.end_time = datetime.now().isoformat()
        
        # Сохраняем в архив сессий
        session_archive_path = os.path.join(self.sessions_dir, f"{self.session.session_id}.json")
        
        try:
            with open(session_archive_path, "w", encoding="utf-8") as f:
                json.dump(asdict(self.session), f, ensure_ascii=False, indent=2)
            
            # Обновляем контекст проекта
            self.save_project_context()
            
        except Exception as e:
            print(f"Ошибка завершения сессии: {e}")
    
    def save_message(self, role: str, content: str):
        """Сохраняет сообщение в историю"""
        message = {
            "timestamp": datetime.now().isoformat(),
            "role": role,
            "content": content
        }
        
        # Добавляем в файл истории текущей сессии
        session_history_file = os.path.join(self.sessions_dir, f"{self.session.session_id}_messages.jsonl")
        
        try:
            with open(session_history_file, "a", encoding="utf-8") as f:
                json.dump(message, f, ensure_ascii=False)
                f.write("\n")
        except Exception as e:
            print(f"Ошибка сохранения сообщения: {e}")
    
    def add_task_completed(self, task: str):
        """Добавляет выполненную задачу в текущую сессию"""
        self.session.tasks_completed.append(task)
        
        # Сохраняем в файл сессии
        try:
            with open(self.session_file, "w", encoding="utf-8") as f:
                json.dump(asdict(self.session), f, ensure_ascii=False, indent=2)
        except Exception as e:
            print(f"Ошибкаобновления сессии: {e}")
        
        # Добавляем в сводку
        self.append_to_summary(task)
    
    def append_to_summary(self, line: str):
        """Добавляет строку в сводку проекта"""
        # Используем правильную дату
        timestamp = datetime.now().strftime("%Y-%m-%d %H:%M")
        entry = f"[{timestamp}] {line}\\n"
        
        try:
            with open(self.summary_file, "a", encoding="utf-8") as f:
                f.write(entry)
        except Exception as e:
            print(f"Ошибка добавления в сводку: {e}")
    
    def get_summary(self, lines_count: int = 20) -> str:
        """Получает последние N строк из сводки"""
        if not os.path.exists(self.summary_file):
            return "Сводка пуста"
        
        try:
            with open(self.summary_file, "r", encoding="utf-8") as f:
                all_lines = f.readlines()
            
            # Возвращаем последние lines_count строк
            return "".join(all_lines[-lines_count:])
        except Exception as e:
            return f"Ошибка чтения сводки: {e}"
    
    def get_recent_messages(self, count: int = 10) -> str:
        """Получает последние сообщения из истории"""
        session_history_file = os.path.join(self.sessions_dir, f"{self.session.session_id}_messages.jsonl")
        
        if not os.path.exists(session_history_file):
            # Если файл текущей сессии пуст, пытаемся взять из последней сессии
            return self.get_messages_from_last_session(count)
        
        try:
            with open(session_history_file, "r", encoding="utf-8") as f:
                lines = f.readlines()
            
            # Берем последние count сообщений
            recent_lines = lines[-count:]
            
            messages = []
            for line in recent_lines:
                msg = json.loads(line.strip())
                messages.append(f"[{msg['timestamp']}] {msg['role']}: {msg['content']}")
            
            return "\n".join(messages)
        except Exception as e:
            return f"Ошибка чтения сообщений: {e}"
    
    def get_messages_from_last_session(self, count: int = 10) -> str:
        """Получает сообщения из последней сессии"""
        # Находим самый новый файл сессии
        session_files = [f for f in os.listdir(self.sessions_dir) if f.endswith("_messages.jsonl")]
        
        if not session_files:
            return "История сообщений отсутствует"
        
        # Сортируем по имени (т.к. имя включает timestamp)
        session_files.sort()
        last_session_file = os.path.join(self.sessions_dir, session_files[-1])
        
        try:
            with open(last_session_file, "r", encoding="utf-8") as f:
                lines = f.readlines()
            
            # Берем последние count сообщений
            recent_lines = lines[-count:]
            
            messages = []
            for line in recent_lines:
                msg = json.loads(line.strip())
                messages.append(f"[{msg['timestamp']}] {msg['role']}: {msg['content']}")
            
            return "\n".join(messages)
        except Exception as e:
            return f"Ошибка чтения сообщений из последней сессии: {e}"
    
    def update_project_component(self, component_name: str, description: str):
        """Обновляет информацию о компоненте проекта"""
        self.project_context.components[component_name] = description
        self.project_context.last_modified = datetime.now().isoformat()
        self.save_project_context()
    
    def add_important_file(self, file_path: str, description: str = ""):
        """Добавляет важный файл в контекст проекта"""
        if file_path not in self.project_context.important_files:
            self.project_context.important_files.append(file_path)
            self.project_context.last_modified = datetime.now().isoformat()
            self.save_project_context()
    
    def get_context_for_prompt(self) -> str:
        """Формирует контекст для использования в промпте"""
        summary = self.get_summary()
        recent_messages = self.get_recent_messages()
        
        context_parts = [
            f"Проект: {self.project_context.project_name}",
            f"Тип проекта: {self.project_context.project_type}",
            f"Текущая сессия: {self.session.session_id} (начата: {self.session.start_time})"
        ]
        
        if self.project_context.components:
            context_parts.append("\nКомпоненты проекта:")
            for name, desc in self.project_context.components.items():
                context_parts.append(f"- {name}: {desc}")
        
        if self.project_context.important_files:
            context_parts.append("\nВажные файлы:")
            for file_path in self.project_context.important_files:
                context_parts.append(f"- {file_path}")
        
        context_parts.append(f"\nСводка последних действий:\n{summary}")
        
        if recent_messages:
            context_parts.append(f"\nПоследние сообщения:\n{recent_messages}")
        
        return "\n".join(context_parts)


# Глобальный объект памяти
memory = AgentMemory()
