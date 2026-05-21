using CocosSharp;
using RiotGalaxy.Objects;

namespace RiotGalaxy.SFX
{
    public class SfxBlast : Sfx
    {
        CCParticleExplosion explosion;
        
        public SfxBlast(CCPoint pos, int size = 250)
        {
            sfxType = SfxType.BLAST;
            timeLife = 1.5f;

            explosion = new CCParticleExplosion(pos, CCEmitterMode.Gravity);

            explosion.Position = pos;
            explosion.AnchorPoint = CCPoint.AnchorMiddle;
            explosion.StartColor = new CCColor4F(CCColor3B.Yellow);
            explosion.EndColor = new CCColor4F(CCColor3B.Black);
            explosion.AutoRemoveOnFinish = true;

            explosion.TotalParticles = 50;
            explosion.Life = timeLife;
            explosion.StartSize = 1f;
            explosion.EndSize = 2;
            explosion.StartRadius = 1;
            explosion.EndRadius = size;
            AddChild(explosion);

            ScheduleOnce((t) => Delete(), timeLife); // После завершения нужно удалиться
        }
    }
}