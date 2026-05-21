using System;
using System.Collections.Generic;
using System.Text;
using CocosSharp;

namespace RiotGalaxy
{
    public class SpriteLoader
    {
        CCSpriteSheet sheet;
        public SpriteLoader()
        {
            sheet = new CCSpriteSheet("images.plist", "images.png");
        }
        public CCSprite Load(string filename)
        {
            //System.Diagnostics.Debug.WriteLine("---- SpriteLoader Load({filename})----------" + filename, filename);
            CCSpriteFrame frame; /*= new CCSpriteFrame();                
                frame.TextureFilename = filename;*/
            try
            {
                frame = sheet.Frames.Find(item => item.TextureFilename == filename);
            }
            catch
            {
                frame = null;
            }
            if (frame != null)
            {
                CCSprite sprite = new CCSprite(frame);
                return sprite;
            }
            else
                return null;


        }
    }
}
