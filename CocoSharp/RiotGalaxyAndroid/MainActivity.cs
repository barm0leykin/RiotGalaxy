using Android.App;
using Android.Content.PM;
using Android.OS;
using CocosSharp;
using Android.Content.Res;
using RiotGalaxy;
using System.Reflection;
using System;

namespace RiotGalaxyAndroid
{
    [Activity(Label = "RiotGalaxy",
        MainLauncher = true,
        Icon = "@drawable/icon",
        Theme = "@style/RiotGalaxy.SplashScreen", //Тема с сплэшскрином //"@android:style/Theme.Black.NoTitleBar.Fullscreen",
        AlwaysRetainTaskState = true,
        ScreenOrientation = ScreenOrientation.Landscape,
        LaunchMode = LaunchMode.SingleInstance,
        ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize | ConfigChanges.Keyboard | ConfigChanges.KeyboardHidden)]

    class MainActivity : Activity
    {
        private CCGameView gameView;

        protected override void OnCreate(Bundle bundle)
        {
            SetTheme(Resource.Style.RiotGalaxy_Fullscreen);   // Установить тему без сплэшскрина         

            base.OnCreate(bundle);
            
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);
            
            // Get our game view from the layout resource,
            // and attach the view created event to it
            gameView = (CCGameView)FindViewById(Resource.Id.GameView);
            //gameView.ViewCreated += mi.Invoke(obj);
            gameView.ViewCreated += GameDelegate.LoadGame; //LoadGame;   // ЗАГРУЗКА ИГРЫ              
        }
        public override void OnBackPressed()
        {
            CCLog.Log("We are in the BackButton Action!!!");
            if (GameManager.state.OnBackPressed() == true)
                Android.OS.Process.KillProcess(Android.OS.Process.MyPid());
        }
        protected override void OnPause()
        {
            base.OnPause();
            gameView.Paused = true;
            CCAudioEngine.SharedEngine.PauseBackgroundMusic();
        }
        protected override void OnResume()
        {
            base.OnResume();
            if (gameView != null)
            {
                gameView.Paused = false;
                if (CCAudioEngine.SharedEngine != null)
                    CCAudioEngine.SharedEngine.ResumeBackgroundMusic();
            }
        }
    }
}