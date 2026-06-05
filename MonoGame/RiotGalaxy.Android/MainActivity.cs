using System;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Android.Window;
using Microsoft.Xna.Framework;
using RiotGalaxy.Core;
using RiotGalaxy.Managers;

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

            // Android 13+ (API 33): системный back приходит через OnBackInvokedDispatcher
            // (predictive back), а НЕ через OnBackPressed/OnKeyDown (проверено на Samsung A56).
            // Регистрируем свой колбэк. На старых устройствах работают OnBackPressed/OnKeyDown ниже.
            if (OperatingSystem.IsAndroidVersionAtLeast(33))
            {
                OnBackInvokedDispatcher.RegisterOnBackInvokedCallback(
                    0 /* PRIORITY_DEFAULT */,
                    new BackCallback(this));
            }
        }

        /// <summary>Общая обработка «Назад»: из игры — в меню, из меню/заставки — выход.</summary>
        private void HandleBack()
        {
            if (Game1.Instance != null && RiotGalaxy.Managers.GameManager.Instance.OnBackRequested())
                Finish(); // мы в меню/заставке → закрываем приложение
            // иначе — переход в меню (отложен в игровой поток), активность остаётся
        }

        // --- Современный путь (API 33+): predictive back ---
        private sealed class BackCallback : Java.Lang.Object, IOnBackInvokedCallback
        {
            private readonly MainActivity _activity;
            public BackCallback(MainActivity activity) => _activity = activity;
            public void OnBackInvoked() => _activity.HandleBack();
        }

        // --- Fallback для старых устройств (кнопка/жест → KEYCODE_BACK или OnBackPressed) ---
        public override void OnBackPressed()
        {
            if (Game1.Instance != null && RiotGalaxy.Managers.GameManager.Instance.OnBackRequested())
                base.OnBackPressed(); // выход
        }

        public override bool OnKeyDown(Keycode keyCode, KeyEvent e)
        {
            if (keyCode == Keycode.Back)
            {
                HandleBack();
                return true;
            }
            return base.OnKeyDown(keyCode, e);
        }
    }
}
