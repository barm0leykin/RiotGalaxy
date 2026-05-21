using System;

namespace RiotGalaxy.DesktopGL
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            using (var game = RiotGalaxy.Core.Game1.Instance)
                game.Run();
        }
    }
}
