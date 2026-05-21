"""
Интеграционный модуль для работы с памятью и векторным индексом.
Обеспечивает функциональность для сохранения контекста и поиска по коду.
"""

import os
import sys
from typing import List, Dict, Any, Optional
from datetime import datetime

# Импортируем модуль памяти
sys.path.append(os.path.dirname(os.path.abspath(__file__)))
from agent_memory import AgentMemory

# Создаем глобальный экземпляр
memory = AgentMemory()

# Импортируем модуль для работы с векторным индексом
try:
    from langchain.text_splitter import RecursiveCharacterTextSplitter
    from langchain.embeddings.openai import OpenAIEmbeddings
    from langchain.vectorstores import Chroma
    VECTOR_INDEX_AVAILABLE = True
except ImportError:
    VECTOR_INDEX_AVAILABLE = False
    print("Внимание: модули для векторного индекса не установлены. Функция поиска по коду будет недоступна.")


class CodeContextManager:
    """Менеджер контекста кода и памяти"""
    
    def __init__(self, use_vector_index: bool = True):
        self.memory = memory
        self.use_vector_index = use_vector_index and VECTOR_INDEX_AVAILABLE
        
        # Инициализация векторного индекса
        if self.use_vector_index:
            self.chroma_dir = "ai-agent/memory/code_index"
            os.makedirs(self.chroma_dir, exist_ok=True)
            
            # Пытаемся загрузить существующий индекс
            try:
                self.vectorstore = Chroma(
                    persist_directory=self.chroma_dir,
                    embedding_function=OpenAIEmbeddings()
                )
                self.index_exists = True
            except:
                self.vectorstore = None
                self.index_exists = False
    
    def index_code_folder(self, folder_path: str):
        """Индексирует папку с кодом для последующего поиска"""
        if not self.use_vector_index:
            print("Векторный индекс недоступен")
            return None
            
        if not os.path.exists(folder_path):
            print(f"Папка не существует: {folder_path}")
            return None
        
        splitter = RecursiveCharacterTextSplitter(chunk_size=1000, chunk_overlap=200)
        docs = []
        
        # Поддерживаемые расширения файлов
        supported_extensions = ('.cs', '.csproj', '.json', '.yaml', '.yml', '.md', '.txt', '.py', '.js', '.ts')
        
        for root, _, files in os.walk(folder_path):
            for fn in files:
                if fn.endswith(supported_extensions):
                    try:
                        file_path = os.path.join(root, fn)
                        with open(file_path, "r", encoding="utf-8") as f:
                            content = f.read()
                        
                        # Добавляем каждый фрагмент с метаданными
                        for chunk in splitter.split_text(content):
                            docs.append({
                                "page_content": chunk,
                                "metadata": {
                                    "source": file_path,
                                    "filename": fn
                                }
                            })
                    except Exception as e:
                        print(f"Ошибка при обработке файла {fn}: {e}")
        
        if not docs:
            print("Не найдено файлов для индексации")
            return None
        
        # Создаем или обновляем векторное хранилище
        try:
            if self.index_exists and self.vectorstore:
                # Добавляем документы в существующий индекс
                self.vectorstore.add_texts(
                    texts=[doc["page_content"] for doc in docs],
                    metadatas=[doc["metadata"] for doc in docs]
                )
            else:
                # Создаем новый индекс
                self.vectorstore = Chroma.from_texts(
                    texts=[doc["page_content"] for doc in docs],
                    embedding=OpenAIEmbeddings(),
                    persist_directory=self.chroma_dir,
                    metadatas=[doc["metadata"] for doc in docs]
                )
                self.index_exists = True
            
            self.vectorstore.persist()
            print(f"Проиндексировано {len(docs)} фрагментов из {len(docs)} файлов")
            return self.vectorstore
        except Exception as e:
            print(f"Ошибка при индексации: {e}")
            return None
    
    def retrieve_code_context(self, query: str, k: int = 3) -> str:
        """Извлекает релевантные фрагменты кода по запросу"""
        if not self.use_vector_index or not self.vectorstore:
            return ""
        
        try:
            # Поиск релевантных документов
            docs = self.vectorstore.similarity_search(query, k=k)
            
            if not docs:
                return ""
            
            # Форматируем результаты
            context_parts = []
            for doc in docs:
                source = doc.metadata.get("source", "unknown")
                filename = doc.metadata.get("filename", "unknown")
                context_parts.append(f"Файл: {source}\n```\n{doc.page_content}\n```")
            
            return "\n\n".join(context_parts)
        except Exception as e:
            print(f"Ошибка при поиске: {e}")
            return ""
    
    def save_message(self, role: str, content: str):
        """Сохраняет сообщение в историю"""
        self.memory.save_message(role, content)
    
    def add_task_completed(self, task: str):
        """Добавляет выполненную задачу в сводку"""
        self.memory.add_task_completed(task)
    
    def update_project_component(self, component_name: str, description: str):
        """Обновляет информацию о компоненте проекта"""
        self.memory.update_project_component(component_name, description)
    
    def add_important_file(self, file_path: str, description: str = ""):
        """Добавляет важный файл в контекст проекта"""
        self.memory.add_important_file(file_path, description)
    
    def get_context_for_prompt(self) -> str:
        """Формирует контекст для использования в промпте"""
        return self.memory.get_context_for_prompt()
    
    def end_session(self):
        """Завершает текущую сессию"""
        self.memory.end_current_session()


# Глобальный экземпляр менеджера контекста
context_manager = CodeContextManager()


# Удобные функции для использования
def save_user_message(content: str):
    """Сохраняет сообщение пользователя"""
    context_manager.save_message("human", content)


def save_assistant_message(content: str):
    """Сохраняет ответ ассистента"""
    context_manager.save_message("assistant", content)


def add_task_completed(task: str):
    """Добавляет выполненную задачу"""
    context_manager.add_task_completed(task)


def update_project_component(component_name: str, description: str):
    """Обновляет компонент проекта"""
    context_manager.update_project_component(component_name, description)


def add_important_file(file_path: str, description: str = ""):
    """Добавляет важный файл"""
    context_manager.add_important_file(file_path, description)


def get_context_for_prompt() -> str:
    """Получает контекст для промпта"""
    return context_manager.get_context_for_prompt()


def retrieve_code_context(query: str, k: int = 3) -> str:
    """Извлекает контекст кода"""
    return context_manager.retrieve_code_context(query, k)


def index_code_folder(folder_path: str):
    """Индексирует папку с кодом"""
    return context_manager.index_code_folder(folder_path)


def end_session():
    """Завершает сессию"""
    context_manager.end_session()


# Пример использования
if __name__ == "__main__":
    # Создаем тестовую сессию
    save_user_message("Проанализируй структуру проекта")
    
    # Добавляем задачу
    add_task_completed("Проанализирована структура проекта RiotGalaxy")
    
    # Обновляем компонент
    update_project_component("Frontend", "React-приложение для управления играми")
    
    # Добавляем важный файл
    add_important_file("package.json", "Файл с зависимостями")
    
    # Получаем контекст
    context = get_context_for_prompt()
    print("Текущий контекст:")
    print(context)
    
    # Индексируем код (если доступно)
    if VECTOR_INDEX_AVAILABLE:
        index_result = index_code_folder(".")
        if index_result:
            # Ищем релевантный код
            code_context = retrieve_code_context("deployment configuration")
            if code_context:
                print("\nНайденный контекст кода:")
                print(code_context)
    
    # Завершаем сессию
    end_session()
