using CocosSharp;
using RiotGalaxy.Objects;
using System.Threading.Tasks;
using RiotGalaxy;
using System;
using System.Threading;

namespace RiotGalaxy.SFX
{
    public class Sfx : CCNode
    {
        public enum SfxType : int { BLAST = 0, GALAXY }
        public SfxType sfxType;
        
        public float timeLife = 2f;

        public Sfx()
        {
            GameManager.ScGame.gameplayLayer.AddChild(this);
            //Schedule(Activity);
            //Schedule((t) => Delete(), interval: timeLife);
        }
        void Activity(float time)
        {            
        }
        public void Delete()
        {
            //System.Diagnostics.Debug.WriteLine("=== Delete() SFX === ");
            UnscheduleAll();
            GameManager.ScGame.gameplayLayer.RemoveChild(this);

            RemoveAllChildren(true);
            RemoveFromParent(true);
            RemoveAllListeners();
            //needToDelete = true;
        }
    }
}
