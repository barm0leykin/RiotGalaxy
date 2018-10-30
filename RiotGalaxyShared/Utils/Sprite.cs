using CocosSharp;
using RiotGalaxy.Objects;
using System;
using System.Collections.Generic;
using System.Text;

namespace RiotGalaxy.Utils
{
    class Sprite
    {
        private CCSprite sprite;
        private GameObject owner;
        public Sprite()
        {
        }
        public bool LoadSprite(GameObject obj, string filename)
        {
            owner = obj;
            if ( (sprite = GameManager.sLoader.Load(filename)) != null) //new CCSprite("ship"); //
            {
                sprite.AnchorPoint = CCPoint.AnchorMiddle;
                owner.AddChild(sprite);
                return true;
            }
            return false;
        }
        public CCSprite GetCCSprite()
        {
            return sprite;
        }
    }
}
