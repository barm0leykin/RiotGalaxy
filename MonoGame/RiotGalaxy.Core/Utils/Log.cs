using System;
using System.IO;

namespace RiotGalaxy.Utils
{
    /// <summary>
    /// Простой кросс-платформенный лог: пишет в Console (видно в терминале/Debug Console VSCode)
    /// и в файл riot.log рядом с приложением (его читает ассистент при отладке).
    /// На Android позже можно добавить ветку Android.Util.Log → logcat.
    /// </summary>
    public static class Log
    {
        public static bool ToFile = true;
        private static readonly string _file = Path.Combine(AppContext.BaseDirectory, "riot.log");
        private static bool _truncated;

        public static void Debug(string msg) => Write("DBG", msg);
        public static void Error(string msg) => Write("ERR", msg);

        private static void Write(string level, string msg)
        {
            string line = $"[{level}] {msg}";
            Console.WriteLine(line);

            if (!ToFile)
                return;
            try
            {
                if (!_truncated) { File.WriteAllText(_file, string.Empty); _truncated = true; } // очистка при старте
                File.AppendAllText(_file, line + Environment.NewLine);
            }
            catch { /* лог не должен ронять игру */ }
        }
    }
}
