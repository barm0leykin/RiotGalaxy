using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Microsoft.Xna.Framework;
using RiotGalaxy.Core;

namespace RiotGalaxy
{
    /// <summary>
    /// Точка входа Android-версии. Аналог Program.Main у DesktopGL:
    /// создаёт Game1, получает его View и запускает игровой цикл.
    /// Ландшафтная ориентация (игра горизонтальная).
    /// </summary>
    [Activity(
        Label = "RiotGalaxy",
        MainLauncher = true,
        AlwaysRetainTaskState = true,
        LaunchMode = LaunchMode.SingleInstance,
        ScreenOrientation = ScreenOrientation.SensorLandscape,
        ConfigurationChanges = ConfigChanges.Orientation
            | ConfigChanges.Keyboard
            | ConfigChanges.KeyboardHidden
            | ConfigChanges.ScreenSize)]
    public class MainActivity : AndroidGameActivity
    {
        private Game1 _game;
        private View _view;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            _game = Game1.Instance;
            _view = _game.Services.GetService(typeof(View)) as View;

            SetContentView(_view);
            _game.Run();
        }
    }
}
