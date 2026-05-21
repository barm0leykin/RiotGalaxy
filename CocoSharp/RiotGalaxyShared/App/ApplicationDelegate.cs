using System;
using System.Collections.Generic;
using Android.App;
using Android.Content.Res;
using CocosSharp;


namespace RiotGalaxy
{
    /*public class Reflector
    {
        public GameDelegate GetDelegate()
        {
            return GetDelegate a;
        }
    }*/

    public static class GameDelegate
    {
        public static Activity mainActivity;
        public static AssetManager assets { get; set; }

        public static CCGameView GameView { get; set; }
        public static List<string> contentSearchPaths = new List<string>() { "Fonts", "Sounds", "Levels" };
        public static GameManager gameManager;

        public static void LoadGame(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("=== LOAD GAME ===");
            GameView = sender as CCGameView;

            if (GameView != null)
            {
                assets = Android.App.Application.Context.Assets;
                //ContentRootDirectory = "Content";
                contentSearchPaths.Add("Levels");//////                
                CCSizeI viewSize = GameView.ViewSize;
                
                // Set world dimensions
                GameView.DesignResolution = new CCSizeI(Options.width, Options.height); // logiack resolution
                GameView.ResolutionPolicy = CCViewResolutionPolicy.ShowAll;
                
                // Determine whether to use the high or low def versions
                // of our images. Make sure the default texel to content
                // size ratio is set correctly. Of course you're free to
                // have a finer set of image resolutions e.g (ld, hd, super-hd)
                contentSearchPaths.Add("Images"); //contentSearchPaths.Add("Images/Ld");
                CCSprite.DefaultTexelToContentSizeRatio = 1.0f;

                GameView.ContentManager.SearchPaths = contentSearchPaths;

                CCLog.Logger = Console.WriteLine;

                gameManager = new GameManager(GameView);
                gameManager.Init();
                //gameManager.Activity();
            }
        }
    }
}
