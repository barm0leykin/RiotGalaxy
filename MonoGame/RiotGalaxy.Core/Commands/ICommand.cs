using System;

namespace RiotGalaxy.Commands
{
    /// <summary>
    /// Интерфейс команды по паттерну Command
    /// Аналог ICommand из CocosSharp
    /// </summary>
    public interface ICommand
    {
        void Execute();
    }

    /// <summary>
    /// Пустая команда - реализация паттерна Null Object
    /// </summary>
    public class NoCommand : ICommand
    {
        public NoCommand() { }
        public void Execute()
        {
            // Ничего не делает
        }
    }
}