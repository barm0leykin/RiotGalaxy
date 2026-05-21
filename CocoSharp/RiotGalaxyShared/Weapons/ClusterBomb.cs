using CocosSharp;
using System.Collections;

namespace RiotGalaxy.Weapons
{
    class ClusterBomb : Shell
    {
        ArrayList hitTargets; //необходимо хранить список пораженных целей чтобы не бить их постоянно
        float boom_hight = 200;

        public ClusterBomb(CCPoint point) : base(point)
        {
            name += "_Laser";

            draw.LoadGraphics("cluster_bomb.png");
            Hp = 1;
            Damage = 10;

            hitTargets = new ArrayList();
        }
        override public void Activity(float time)
        {
            base.Activity(time);
            if (PositionY < boom_hight)
                Boom();
        }
        private void Boom()
        { }
    }
}
