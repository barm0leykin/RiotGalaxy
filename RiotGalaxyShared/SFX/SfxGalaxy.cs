using CocosSharp;
using RiotGalaxy.Objects;

namespace RiotGalaxy.SFX
{
    public class SfxGalaxy : Sfx
    {
        //CCParticleGalaxy effect;
        CCParticleRain starsSmall; //, starsBig;
        public SfxGalaxy(CCPoint pos) 
        {
            sfxType = SfxType.GALAXY;
            //timeLife = 4f;
            
            starsSmall = new CCParticleRain(new CCPoint(Options.width, Options.height));
            starsSmall.Position = new CCPoint(Options.width / 2, Options.height );
            starsSmall.StartColor = new CCColor4F(CCColor4B.LightGray);
            //starsSmall.EndColor = new CCColor4F(CCColor4B.LightGray);
            starsSmall.StartSize = 2f;
            starsSmall.StartSizeVar = 1f;
            starsSmall.Speed = 30f;
            starsSmall.Gravity = new CCPoint(0f, -10);
            starsSmall.EmissionRate = 10f;
            starsSmall.Life = 50f;
            //starsSmall.

            /*starsBig = new CCParticleSnow(new CCPoint(Options.width, 20));
            starsBig.Position = new CCPoint(Options.width / 2, Options.height + 1);
            starsBig.StartColor = new CCColor4F(CCColor4B.White);
            starsBig.EndColor = new CCColor4F(CCColor4B.LightGray);
            starsBig.StartSize = 1.5f;
            //starsBig.StartSizeVar = 1f;
            starsBig.Speed = 50f;
            //starsBig.Gravity = new CCPoint(0f, -80);
            starsBig.EmissionRate = 4f;
            starsBig.Life = 50f;*/

            AddChild(starsSmall);
            //AddChild(starsBig);
        }
    }
}